using HarmonyLib;
using SolastaModApi;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SolastaTesting
{
    //[HarmonyPatch(typeof(UserLocationPoolManager), "EnumeratePool")]
    //internal static class UserLocationPoolManager_EnumeratePool
    //{
    //    public static void Postfix()
    //    {
    //        IGamingPlatformService service = ServiceRepository.GetService<IGamingPlatformService>();
    //        if (service != null)
    //        {
    //            Main.Log("IGamingPlatformService is not null");

    //            foreach (string userContentPath in service.UserContentPaths)
    //            {
    //                Main.Log($"Location: {userContentPath}");
    //            }
    //        }
    //        else
    //        {
    //            Main.Log("IGamingPlatformService is null");
    //        }
    //    }
    //}


    //[HarmonyPatch(typeof(RulesetCharacter), "ComputeSpeed")]
    internal static class RulesetCharacter_ComputeSpeed
    {
        public static void Postfix(
            ref int __result,
            RulesetCharacter __instance,
            RuleDefinitions.MoveMode moveMode,
            int baseSpeed,
            bool baseOnly = false,
            List<FeatureDefinition> movementModifiers = null)
        {
            var mm = (movementModifiers ?? __instance.GetMovementModifiers() ?? Enumerable.Empty<FeatureDefinition>())
                .Cast<IMovementAffinityProvider>()
                //.OfType<IMovementAffinityProvider>()  // depends if we want assertion or not
                .Where(affinityProvider => affinityProvider.AppliesToAllModes || moveMode == affinityProvider.MoveMode);

            if(mm.Any(ap => ap.HeavyArmorImmunity))
            {
                mm = mm.Where(ap => (BaseDefinition)ap != DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityHeavyArmorOverload);
            }

            if(mm.Any(ap => ap.EncumbranceImmunity))
            {
                mm = mm
                    .Where(ap => (BaseDefinition)ap != DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityConditionEncumbered)
                    .Where(ap => (BaseDefinition)ap != DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityConditionHeavilyEncumbered);
            }

            Main.Log($"Name={__instance.Name}: Parameters: Mode={moveMode}, baseSpeed={baseSpeed}, baseOnly={baseOnly}, modifierCount={mm.Count()}, result={__result}");

            if (mm.Any())
            {
                // Not sure about this - only used in tutorial to limit speed of wolf - and this code doesn't do that
                var minMaxMoves = mm.Max(ap => ap.MinMaxMoves);

                var additive = mm
                    .Where(ap => !ap.SpeedAddBase)
                    .Sum(ap => ap.BaseSpeedAdditiveModifier);

                var multiplicative = mm
                    .Where(ap => !ap.SpeedAddBase)
                    .Select(ap => ap.BaseSpeedMultiplicativeModifier)
                    .Aggregate(1f, (n1, n2) => n1 * n2);

                var newBaseSpeed = mm.Where(ap => ap.ForceMinimalBaseSpeed)
                    .Select(ap => ap.MinimalBaseSpeed)
                    .Concat(Enumerable.Repeat(baseSpeed, 1))
                    .Max();

                // TODO: where ap.SpeedAddBase == true

                int calculated = Mathf.FloorToInt(multiplicative * (newBaseSpeed + additive));

                // Not sure about this - only used in tutorial to limit speed of wolf - and this code doesn't do that
                ///var retval = Mathf.Max(minMaxMoves, calculated);

                var retval = calculated;

                Main.Log($"baseOriginal={baseSpeed}, newBase={newBaseSpeed}, min-max={minMaxMoves}, additive={additive}, multiplicative={multiplicative}, calculated={calculated}, retval={retval}");

                __result = retval;
            }
            else
            {
                Main.Log($"No affinity providers. Result=baseSpeed={baseSpeed}");
            }
        }
    }
}
