using HarmonyLib;
using SolastaModApi;
using System.Collections.Generic;

namespace SolastaTesting
{
    /// <summary>
    /// Possible custom affinity.  Problem is to build FeatureDefinitionDamageAffinity.
    /// Need to create FeatureDefinitionDamageAffinity class builder in ModApi.
    /// </summary>
    internal class FeatureDefinitionDamageAffinityEx : FeatureDefinitionDamageAffinity
    {
        public DamageAffinityTypeEx DamageAffinityTypeEx { get; set; }
        public int FlatBonusAmount { get; set; }
        public double Ratio { get; set; }


        internal static FeatureDefinitionDamageAffinityEx Create(FeatureDefinitionDamageAffinity original, 
            string name, DamageAffinityTypeEx damageAffinity, int flatBonusAmount, double ratio)
        {
            var retval = new FeatureDefinitionDamageAffinityEx
            {
                DamageAffinityTypeEx = damageAffinity,
                FlatBonusAmount = flatBonusAmount,
                Ratio = ratio
            };

            // TODO - copy original to retval
            // AccessTools etc, class builder?

            // retval.Name = name;
            // retval.Guid = new guid


            return retval;
        }
    }

    /// <summary>
    /// Patch ModulateSustainedDamage
    /// </summary>
    [HarmonyPatch(typeof(FeatureDefinitionDamageAffinity), "ModulateSustainedDamage")]
    internal static class FeatureDefinitionDamageAffinity_ModulateSustainedDamage
    {
#if false
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
#endif

        // Or

        internal static void Postfix(FeatureDefinitionDamageAffinity __instance, string damageType, int damage, List<string> sourceTags, ref int __result)
        {
            // original logic has run and damage calculated will be in __result

            // is this an instance of our custom affinity?
            var fda = __instance as FeatureDefinitionDamageAffinityEx;

            if (fda != null)
            {
                // custom logic here, modify __result 
                // custom logic here, modify __result 
                switch (fda.DamageAffinityTypeEx)
                {
                    case DamageAffinityTypeEx.FlatBonus:
                        __result = damage + fda.FlatBonusAmount;
                        break;
                    case DamageAffinityTypeEx.AttributeAndProficiency:
                        __result = (int)(damage * fda.Ratio);
                        break;
                    case DamageAffinityTypeEx.Ratio:
                        __result = (int)(damage * fda.Ratio);
                        break;
                    default:
                        Main.Error($"Unknown damage affinity: {fda.DamageAffinityTypeEx}");
                        break;
                }
            }

            // or is it something we want to change anyway
            //if (__instance.Name == "")  // or __instance.Guid ==
            //{

            //}

            // else do nothing (or something if required)
        }

        internal static void Apply()
        {
            // Start with a resistance that nearly matches the one you want
            var feature = DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityForceDamageResistance;

            // Get the monsters you want - doesn't have to be this
            var monstersWithForceResistance = Helpers.GetMonstersWithFeatureDefinition(feature);

            // create your feature from the existing one (or create one from scratch if there's a class builder)
            var myFeature = FeatureDefinitionDamageAffinityEx.Create(feature, "PsionicBlastResistance", 
                DamageAffinityTypeEx.AttributeAndProficiency, 5, 0.5);

            DatabaseRepository.GetDatabase<FeatureDefinitionDamageAffinity>().Add(myFeature);

            // Add your feature to all the monsters you want
            foreach (var m in monstersWithForceResistance)
            {
                m.Features.Add(myFeature);

                // Maybe remove the original one
                // m.Features.Remove(feature);
            }
        }
    }
}
