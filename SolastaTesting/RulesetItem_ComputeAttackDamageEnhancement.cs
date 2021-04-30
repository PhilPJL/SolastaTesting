using HarmonyLib;
using SolastaModApi.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace SolastaTesting
{
    //[HarmonyPatch(typeof(RulesetItem), "ComputeAttackDamageEnhancement")]
    internal static class RulesetItem_ComputeAttackDamageEnhancement
    {
        internal static void Postfix(RulesetItem __instance,
            IAttributeValueProvider attributeValueProvider, List<RulesetItemProperty> ___dynamicItemProperties, ref int __result)
        {
            // __result contains currently computed ADE
            Main.Log($"ComputeAttackDamageEnhancement: ADE={__result}");

            __result += AttackEnhancementHelper.GetEnhancement(__instance, attributeValueProvider, ___dynamicItemProperties, "ADE");
        }
    }

    //[HarmonyPatch(typeof(RulesetItem), "ComputeAttackHitEnhancement")]
    internal static class RulesetItem_ComputeAttackHitEnhancement
    {
        internal static void Postfix(RulesetItem __instance,
            IAttributeValueProvider attributeValueProvider, List<RulesetItemProperty> ___dynamicItemProperties, ref int __result)
        {
            // __result contains currently computed ACM
            Main.Log($"RulesetItem_ComputeAttackHitEnhancement: AHE={__result}");

            __result += AttackEnhancementHelper.GetEnhancement(__instance, attributeValueProvider, ___dynamicItemProperties, "AHE");
        }
    }

    internal static class AttackEnhancementHelper
    {
        internal static int GetEnhancement(RulesetItem __instance,
                IAttributeValueProvider attributeValueProvider, List<RulesetItemProperty> ___dynamicItemProperties, string type)
        {
            using (new MethodLogger($"AttackEnhancementHelper:{type}"))
            {
                // TODO: is the RulesetItem for your scaling weapon?
                // if( __instance.ItemDefinition.Name != "...." ) // something like that

                var gameManager = ServiceRepository.GetService<IGameService>();

                if (gameManager != null)
                {
                    Main.Log($"AttackEnhancementHelper: Got game manager");

                    var hero = gameManager.EnumerateHeroes()?.FirstOrDefault(h => h.Guid == __instance.BearerGuid);

                    if (hero?.ClassesAndLevels != null)
                    {
                        // get bearer level for our target class (PsiWarrior)
                        var level = hero?.ClassesAndLevels
                            //.Where(kvp => kvp.Key.Name == "Fighter") // TODO
                            .Select(kvp => (int?)kvp.Value).SingleOrDefault() ?? 0;

                        Main.Log($"AttackEnhancementHelper: Hero={hero.Name}, Level={level}");

                        if (level > 0)
                        {
                            // It may possible to modify behaviour using FeatureDefinitionAttackModifier 
                            //if (___dynamicItemProperties != null)
                            //{
                            //    var enhancement = ___dynamicItemProperties
                            //        .OfType<IAttackModificationProvider>()
                            //        .Where(p => p.AttackRollModifierMethod == (RuleDefinitions.AttackModifierMethod)1000) // (your AttackModifierMethod)
                            //        .Select(p => attributeValueProvider.TryGetAttributeValue($"{type}:{level}"))
                            //        .SingleOrDefault();

                            //    Main.Log($"AttackEnhancementHelper: Enhancement={enhancement}");
                            //    return enhancement + level;
                            //}

                            // But just taking a simple approach.  Try to get the enhancement using attributeValueProvider.
                            // If the IAttributeValueProvider is your RulesetCharacterHero, then you can possibly add and register additional attributes
                            // for your hero.  Or just patch TryGetAttributeValue as below.
                            int enhancement = attributeValueProvider.TryGetAttributeValue($"{type}:{level}");
                            Main.Log($"AttackEnhancementHelper: Enhancement={enhancement}");
                            return enhancement;
                        }
                    }
                }

                return 0;
            }
        }
    }

    /// <summary>
    /// Ideally we would add/register attributes on our hero based on the class.
    /// Hardcoded as example.  This code could just as well go straight into RulesetItem.ComputeAttackHit/DamageEnhancement if hard coding.
    /// </summary>
    //[HarmonyPatch(typeof(RulesetEntity), "TryGetAttributeValue")]
    internal static class RulesetEntity_TryGetAttributeValue
    {
        internal static void Postfix(RulesetEntity __instance, string attributeName, ref int __result)
        {
            var hero = __instance as RulesetCharacterHero;

            if (hero != null)// && hero.HasClass("MyClass"))
            {
                var originalResult = __result;

                switch (attributeName)
                {
                    case "AHE:1":
                    case "ADE:1":
                        __result = 1;
                        break;
                    case "AHE:2":
                    case "ADE:2":
                        __result = 1;
                        break;
                    case "AHE:3":
                    case "ADE:3":
                        __result = 2;
                        break;
                    case "AHE:4":
                    case "ADE:4":
                        __result = 2;
                        break;
                    case "AHE:5":
                    case "ADE:5":
                        __result = 3;
                        break;
                    case "AHE:6":
                    case "ADE:6":
                        __result = 3;
                        break;
                    case "AHE:7":
                    case "ADE:7":
                        __result = 4;
                        break;
                }

                Main.Log($"Get '{attributeName}' for '{hero.Name}'.  Original value={originalResult}, new value={__result}.");
            }
        }
    }
}
