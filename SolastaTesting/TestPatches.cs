using HarmonyLib;
using System;
using System.Collections.Generic;

namespace SolastaTesting
{

    //[HarmonyPatch(typeof(CharacterInspectionScreen), "Show")]
    //internal static class CharacterInspectionScreen_Show
    //{
    //    public static void Prefix(CharacterInspectionScreen __instance, string filename, GuiScreen previousScreen)
    //    {
    //        RulesetEntity.DoNotAutoregister = true;

    //        ServiceRepository.AddService(new DummyRulesetEntityService());
    //    }

    //    public static void Postfix(CharacterInspectionScreen __instance, string filename, GuiScreen previousScreen)
    //    {
    //        RulesetEntity.DoNotAutoregister = false;

    //        ServiceRepository.RemoveService<DummyRulesetEntityService>();
    //    }
    //}

    //[HarmonyPatch(typeof(RulesetEntity), "SerializeAttributes")]
    internal static class RulesetEntity_SerializeAttributes
    {
        public static void Prefix(RulesetEntity __instance, IAttributesSerializer serializer,
            IVersionProvider versionProvider)
        {
            string name;

            try
            {
                name = __instance.Name;
            }
            catch (NullReferenceException)
            {
                name = "exception";
            }

            Main.Log($"RE_SA: Name={name}, Guid={__instance.Guid}, Att cnt={__instance.Attributes?.Count ?? 0}");
        }
    }

    internal class DummyRulesetEntityService : IRulesetEntityService
    {
        public bool Dirty { get; set; }

        public Dictionary<ulong, RulesetEntity> RulesetEntities => new Dictionary<ulong, RulesetEntity>();

        public ulong GenerateGuid()
        {
            return 0UL;
        }

        public void RegisterEntity(RulesetEntity rulesetEntity)
        {
        }

        public bool TryGetEntityByGuid(ulong guid, out RulesetEntity rulesetEntity)
        {
            rulesetEntity = null;
            return false;
        }

        public void UnregisterEntity(RulesetEntity rulesetEntity)
        {
        }
    }

    //[HarmonyPatch(typeof(CharacterInspectionScreen), "HandleInput")]
    internal static class CharacterInspectionScreen_HandleInput
    {
        public static bool Prefix(CharacterInspectionScreen __instance, InputCommands.Id command)
        {
            switch (command)
            {
                case InputCommands.Id.RotateCCW:
                    SaveCharacter();
                    break;
            }

            return true;

            void SaveCharacter()
            {
                Main.Log("Save the character");

                var heroCharacter = __instance.InspectedCharacter.RulesetCharacterHero;

                //heroCharacter.ConditionsByCategory.Clear();

                var name = heroCharacter.Name;
                var builtin = heroCharacter.BuiltIn;
                var guid = heroCharacter.Guid;
                Main.Log($"Is built in={builtin}, guid={guid}.");

                try
                {
                    heroCharacter.Name = "Exp" + name;
                    heroCharacter.BuiltIn = false;

                    AccessTools.Field(heroCharacter.GetType(), "guid").SetValue(heroCharacter, 0UL);

                    ServiceRepository
                        .GetService<ICharacterPoolService>()
                        .SaveCharacter(heroCharacter, true);
                }
                finally
                {
                    heroCharacter.Name = name;
                    heroCharacter.BuiltIn = builtin;
                    AccessTools.Field(heroCharacter.GetType(), "guid").SetValue(heroCharacter, guid);
                }
            }
        }
    }
}
