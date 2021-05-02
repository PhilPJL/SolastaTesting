using HarmonyLib;
using SolastaModApi.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolastaTesting
{
    //[HarmonyPatch(typeof(CharacterInspectionScreen), "HandleInput")]
    internal static class CharacterInspectionScreen_HandleInput
    {
        public static bool Prefix(CharacterInspectionScreen __instance, InputCommands.Id command)
        {
            switch (command)
            {
                // Handle 'E' for 'Export'
                case InputCommands.Id.RotateCCW:
                    SaveCharacter();
                    break;
            }

            return true;

            void SaveCharacter()
            {
                var rulesetEntityService = ServiceRepository.GetService<IRulesetEntityService>();

                using (var logger = new MethodLogger("CharacterInspectionScreen_HandleInput"))
                {
                    var heroCharacter = __instance.InspectedCharacter.RulesetCharacterHero;

                    // record current name, etc..
                    var name = heroCharacter.Name;
                    var builtin = heroCharacter.BuiltIn;
                    var guid = heroCharacter.Guid;

                    // record current conditions, powers, spells and attunements
                    var conditions = heroCharacter.ConditionsByCategory.ToList();
                    var powers = heroCharacter.PowersUsedByMe.ToList();
                    var spells = heroCharacter.SpellsCastByMe.ToList();
                    var inventoryItems = new List<RulesetItem>();
                    heroCharacter.CharacterInventory.EnumerateAllItems(inventoryItems);
                    var attunedItems = inventoryItems.Select(i => new { Item = i, Name = i.AttunedToCharacter }).ToList();

                    // record item guids
                    var heroItemGuids = heroCharacter.Items.Select(i => new { Item = i, i.Guid }).ToList();
                    var inventoryItemGuids = inventoryItems.Select(i => new { Item = i, i.Guid }).ToList();

                    try
                    {
                        // TODO: update to use convention = name-nnn.sav
                        // TODO: then need UI to allow user to change name on export
                        // For now just add EX-
                        heroCharacter.Name = "EX-" + name;
                        heroCharacter.BuiltIn = false;

                        // remove active conditions (or filter out during serialization)
                        heroCharacter.ConditionsByCategory.Clear();

                        // remove spells and effects (or filter out during serialization)
                        heroCharacter.PowersUsedByMe.Clear();
                        heroCharacter.SpellsCastByMe.Clear();

                        // remove attunement, attuned items don't work well in the character inspection screen out of game
                        foreach (var item in attunedItems)
                        {
                            item.Item.AttunedToCharacter = string.Empty;
                        }

                        // clear guids
                        AccessTools.Field(heroCharacter.GetType(), "guid").SetValue(heroCharacter, 0UL);

                        foreach(var item in heroItemGuids)
                        {
                            AccessTools.Field(item.Item.GetType(), "guid").SetValue(item.Item, 0UL);
                        }

                        foreach(var item in inventoryItemGuids)
                        {
                            AccessTools.Field(item.Item.GetType(), "guid").SetValue(item.Item, 0UL);
                        }

                        ServiceRepository
                            .GetService<ICharacterPoolService>()
                            .SaveCharacter(heroCharacter, true);
                    }
                    finally
                    {
                        // TODO: check these things are really restored

                        // restore original values
                        heroCharacter.Name = name;
                        heroCharacter.BuiltIn = builtin;

                        // restore conditions
                        foreach (var kvp in conditions)
                        {
                            heroCharacter.ConditionsByCategory.Add(kvp.Key, kvp.Value);
                        }

                        // restore active spells and effects
                        heroCharacter.PowersUsedByMe.AddRange(powers);
                        heroCharacter.SpellsCastByMe.AddRange(spells);

                        // restore attunement
                        foreach (var item in attunedItems) { item.Item.AttunedToCharacter = item.Name; }

                        // restore guids
                        AccessTools.Field(heroCharacter.GetType(), "guid").SetValue(heroCharacter, guid);

                        foreach (var item in heroItemGuids)
                        {
                            AccessTools.Field(item.Item.GetType(), "guid").SetValue(item.Item, item.Guid);
                        }

                        foreach (var item in inventoryItemGuids)
                        {
                            AccessTools.Field(item.Item.GetType(), "guid").SetValue(item.Item, item.Guid);
                        }
                    }
                }
            }
        }
    }

    //[HarmonyPatch(typeof(RulesetInventory), "SerializeElements")]
    internal static class RulesetInventory_SerializeElements
    {
        static readonly object Locker = new object();

        internal static void Prefix()
        {
            lock (Locker)
            {
                var registeredService = ServiceRepository.GetService<IRulesetEntityService>();

                if (registeredService == null)
                {
                    Main.Log("Adding DummyRulesetEntityService");
                    ServiceRepository.AddService(DummyRulesetEntityService.Instance);
                }
            }
        }

        internal static void Postfix()
        {
            lock (Locker)
            {
                var registeredService = ServiceRepository.GetService<IRulesetEntityService>();

                if (registeredService != null)
                {
                    Main.Log("Removing DummyRulesetEntityService1");

                    if (registeredService is DummyRulesetEntityService)
                    {
                        Main.Log("Removing DummyRulesetEntityService2");

                        ServiceRepository.RemoveService<IRulesetEntityService>();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Required during de-serialization in the character inspection screen to prevent null-ref exceptions
    /// </summary>
    class DummyRulesetEntityService : IRulesetEntityService
    {
        public static IRulesetEntityService Instance => new DummyRulesetEntityService();

        internal DummyRulesetEntityService()
        {
            Main.Log($"Creating new DummyRulesetEntityService: {AppDomain.CurrentDomain.FriendlyName}");
        }

        public bool Dirty { get; set; }

        public Dictionary<ulong, RulesetEntity> RulesetEntities => new Dictionary<ulong, RulesetEntity>();

        public ulong GenerateGuid()
        {
            Main.Log("GenerateGuid");
            return 0;
        }

        public void RegisterEntity(RulesetEntity rulesetEntity)
        {
            try
            {
                if (rulesetEntity is RulesetItem)
                {
                    var ri = rulesetEntity as RulesetItem;
                    Main.Log($"RegisterEntity: Type= {rulesetEntity.GetType().FullName}, Name={ri?.ItemDefinition?.Name}, Guid={ri?.ItemDefinition?.GUID}");
                }
                else if (rulesetEntity is RulesetItemProperty)
                {
                    var ri = rulesetEntity as RulesetItemProperty;
                    Main.Log($"RegisterEntity: Type= {rulesetEntity.GetType().FullName}, Name={ri?.FeatureDefinition?.Name}, Guid={ri?.FeatureDefinition?.GUID}");
                }
                else
                {
                    Main.Log($"RegisterEntity: Type={rulesetEntity.GetType().FullName}, {rulesetEntity?.Name}");
                }

                RulesetEntities.Add(rulesetEntity.Guid, rulesetEntity);
            }
            catch (Exception ex)
            {
                Main.Log($"RegisterEntity: {ex}");
            }
        }

        public bool TryGetEntityByGuid(ulong guid, out RulesetEntity rulesetEntity)
        {
            Main.Log($"TryGetEntityByGuid: {guid}");
            return RulesetEntities.TryGetValue(guid, out rulesetEntity);
        }

        public void UnregisterEntity(RulesetEntity rulesetEntity)
        {
            Main.Log($"UnregisterEntity: {rulesetEntity?.Guid}");

            if (rulesetEntity != null)
            {
                RulesetEntities.Remove(rulesetEntity.Guid);
            }
        }
    }
}
