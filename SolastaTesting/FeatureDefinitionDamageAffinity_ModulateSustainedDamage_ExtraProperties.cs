using HarmonyLib;
using SolastaModApi;
using System.Collections.Generic;

namespace SolastaTesting
{
    /// <summary>
    /// Patch ModulateSustainedDamage
    /// </summary>
    [HarmonyPatch(typeof(FeatureDefinitionDamageAffinity), "ModulateSustainedDamage")]
    internal static class FeatureDefinitionDamageAffinity_ModulateSustainedDamage_ExtraProperties
    {
        // could have prefix or postfix.  Just postfix for simplicity of example.

        /// <summary>
        /// Store extra data in dictionary - create at app startup after enumerating DatabaseRepository<MonsterDefinition>
        /// </summary>
        internal static Dictionary<FeatureDefinitionDamageAffinity, FeatureDefinitionExtraProperties> ExtraProperties
            => new Dictionary<FeatureDefinitionDamageAffinity, FeatureDefinitionExtraProperties>();

        internal static void Postfix(FeatureDefinitionDamageAffinity __instance, string damageType, int damage, List<string> sourceTags, ref int __result)
        {
            Main.Log("FeatureDefinitionDamageAffinity_ModulateSustainedDamage_ExtraProperties: Enter");

            // original logic has run and damage calculated so far will be in __result

            // can also use __instance.Name/__instance.Guid

            if(damageType != "PsionicBlast")
            {
                return;
            }

            // look in dictionary, is there extra data for this __instance?
            if (ExtraProperties.TryGetValue(__instance, out var extraProperties))
            {
                // custom logic here, modify __result 
                switch (extraProperties.DamageAffinityTypeEx)
                {
                    case DamageAffinityTypeEx.FlatBonus:
                        __result = damage + extraProperties.FlatBonusAmount;
                        break;
                    case DamageAffinityTypeEx.AttributeAndProficiency:
                        __result = (int)(damage * extraProperties.Ratio);
                        break;
                    case DamageAffinityTypeEx.Ratio:
                        __result = (int)(damage * extraProperties.Ratio);
                        break;
                    default:
                        Main.Error($"Unknown damage affinity: {extraProperties.DamageAffinityTypeEx}");
                        break;
                }
            }
        }

        internal static void Apply()
        {
            Main.Log("Apply");

            // Start with a resistance that nearly matches the one you want
            var feature = DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance;

            // Get the monsters you want - doesn't have to be this
            var monstersWithColdResistance = Helpers.GetMonstersWithFeatureDefinition(feature);

            // create your feature
            var myFeature = feature.Clone("MyFeature2");

            // Now set name and guid so it's unique using
            // AccessTools
            // Traverse

            //DatabaseRepository.GetDatabase<FeatureDefinitionDamageAffinity>().Add(myFeature);

            // Add your feature to all the monsters you want
            foreach (var m in monstersWithColdResistance)
            {
                Main.Log($"Adding MyFeature to monster: {m.Name}.");

                m.Features.Add(myFeature);

                // Maybe remove the original one
                // m.Features.Remove(feature);
            }

            // Setup the extra data
            ExtraProperties.Add(myFeature, new FeatureDefinitionExtraProperties
            {
                DamageAffinityTypeEx = DamageAffinityTypeEx.FlatBonus,
                FlatBonusAmount = 5,
                Ratio = 1
            });
        }
    }

    internal class FeatureDefinitionExtraProperties
    {
        public DamageAffinityTypeEx DamageAffinityTypeEx { get; set; }
        public int FlatBonusAmount { get; set; }
        public double Ratio { get; set; }
    }

    internal enum DamageAffinityTypeEx
    {
        FlatBonus,
        AttributeAndProficiency,
        Ratio
    }
}
