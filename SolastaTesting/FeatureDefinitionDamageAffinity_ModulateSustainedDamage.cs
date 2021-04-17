using HarmonyLib;
using System.Collections.Generic;

namespace SolastaTesting
{
    /// <summary>
    /// Patch ModulateSustainedDamage
    /// </summary>
    [HarmonyPatch(typeof(FeatureDefinitionDamageAffinity), "ModulateSustainedDamage")]
    internal static class FeatureDefinitionDamageAffinity_ModulateSustainedDamage
    {
        // Either

        internal static bool Prefix(FeatureDefinitionDamageAffinity __instance, string damageType, int damage, List<string> sourceTags, ref int __result)
        {
            var fda = __instance as FeatureDefinitionDamageAffinityEx;

            if (fda != null)
            {
                // custom logic here, modify __result 
                // stop original code from running 
                return false;
            }

            // else do nothing and let original code run
            return true;
        }

        // Or

        internal static void Postfix(FeatureDefinitionDamageAffinity __instance, string damageType, int damage, List<string> sourceTags, ref int __result)
        {
            // original logic has run and damage calculated will be in __result

            // is this an instance of our custom affinity?
            var fda = __instance as FeatureDefinitionDamageAffinityEx;

            if (fda != null)
            {
                // custom logic here, modify __result 
            }

            // or is it something we want to change anyway
            if(__instance.Name == "")  // or __instance.Guid ==
            {

            }

            // else do nothing (or something if required)
        }
    }
}
