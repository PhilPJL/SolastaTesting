using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace SolastaTesting
{
    //class DynamicSlashingDamageForm : DamageForm
    //{
    //    public DynamicSlashingDamageForm()
    //    {
    //        DamageType = "DynamicDamageTypeSlashing";
    //    }
    //}

    [HarmonyPatch(typeof(RulesetItem), "ComputeAttackDamageEnhancement")]
    internal static class RulesetItem_ComputeAttackDamageEnhancement
    {
        internal static void Postfix(RulesetItem __instance,
            IAttributeValueProvider attributeValueProvider, List<RulesetItemProperty> ___dynamicItemProperties,  ref int __result)
        {
            // __result contains currently computed ACM
            Main.Log($"ComputeAttackDamageEnhancement: ADE={__result}");

            __result += AttackEnhancementHelper.GetEnhancement(__instance, attributeValueProvider, ___dynamicItemProperties, "ADE");
        }
    }

    [HarmonyPatch(typeof(RulesetItem), "ComputeAttackHitEnhancement")]
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
                            if (___dynamicItemProperties != null)
                            {
                                var enhancement = ___dynamicItemProperties
                                    .OfType<IAttackModificationProvider>()
                                    .Where(p => p.AttackRollModifierMethod == (RuleDefinitions.AttackModifierMethod)1000) // modify by level
                                    .Select(p => attributeValueProvider.TryGetAttributeValue($"{type}:{level}"))
                                    .SingleOrDefault();

                                Main.Log($"AttackEnhancementHelper: Enhancement={enhancement}");

                                return enhancement + 10;
                            }
                        }
                    }
                }

                return 0;
            }
        }
    }
}
