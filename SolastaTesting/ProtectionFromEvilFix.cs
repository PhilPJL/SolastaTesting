using SolastaCommunityExpansion.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using System.Collections.Generic;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaTesting;

internal static class ProtectionFromEvilFix
{
    public static void Load()
    {
        //Main.Log("Testing: fixing PoEG");

        // After investigation there's no problem with Protection from Evil and Good

        //var spell = ProtectionFromEvilGood;

        //var cafImmunity = new FeatureDefinitionConditionAffinityBuilder(
        //    ConditionAffinityProtectedFromEvilFrightenedImmunity, "CEConditionAffinityProtectedFromEvilImmunity", "fc1f4985-76fa-455f-ac8e-3e0013b21bd4")
        //    .AddToDB();

        //cafImmunity.SetConditionAffinityType(RuleDefinitions.ConditionAffinityType.Immunity)
        //    .SetSavingThrowAdvantageType(RuleDefinitions.AdvantageType.None)
        //    .SetRerollAdvantageType(RuleDefinitions.AdvantageType.None)
        //    .SetRerollSaveWhenGained(false)
        //    .SetField("otherCharacterFamilyRestrictions", new List<string>());

        //ConditionProtectedFromEvil.Features.Add(cafImmunity);

        //var cafAdvantage = new FeatureDefinitionConditionAffinityBuilder(
        //    ConditionAffinityProtectedFromEvilFrightenedImmunity, "CEConditionAffinityProtectedFromEvilAdvantage", "0386a3d9-b021-4960-afde-d369f345c807")
        //    .AddToDB();

        //cafAdvantage.SetConditionAffinityType(RuleDefinitions.ConditionAffinityType.None)
        //    .SetSavingThrowAdvantageType(RuleDefinitions.AdvantageType.Advantage)
        //    .SetRerollAdvantageType(RuleDefinitions.AdvantageType.Advantage)
        //    .SetRerollSaveWhenGained(true)
        //    .SetField("otherCharacterFamilyRestrictions", new List<string>());

        //ConditionProtectedFromEvil.Features.Add(cafAdvantage);

        //ConditionAffinityProtectedFromEvilFrightenedImmunity.SetField("otherCharacterFamilyRestrictions", new List<string>());

        //ConditionProtectedFromEvil.Features.Remove(ConditionAffinityProtectedFromEvilCharmImmunity);
        //ConditionProtectedFromEvil.Features.Remove(ConditionAffinityProtectedFromEvilPossessedImmunity);
        //ConditionProtectedFromEvil.Features.Remove(ConditionAffinityProtectedFromEvilFrightenedImmunity);
    }
}
