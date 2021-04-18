using HarmonyLib;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaTesting
{
    [HarmonyPatch(typeof(GuiCharacter), "FormatHealthLabel")]
    internal static class GuiCharacter_FormatHealthLabel
    {
        private static readonly Regex HitPointRegex = new Regex(@"^<#.{6}>(?<current_hp>\d{1,4})</color>/(?<max_hp>\d{1,4})", RegexOptions.Compiled | RegexOptions.Singleline);

        internal static void Prefix(GuiCharacter __instance, bool ___healthLabelDirty, out bool __state)
        {
            bool dirty = __instance.UpdateHealthStatus();

            // capture current state of dirty flag for use in Postfix
            __state = ___healthLabelDirty || dirty;
        }

        internal static void Postfix(GuiCharacter __instance, GuiLabel healthLabel, bool __state)
        {
            if (!__state) return;  // health wasn't dirty so healthLabel hasn't been updated

            // A monster has __instance.RulesetCharacterMonster != null and __instance.RulesetCharacter != null
            // A hero has __instance.RulesetCharacterHero != null and __instance.RulesetCharacter != null

            if (__instance.HasHitPointsKnowledge && __instance.RulesetCharacterMonster != null)
            {
                // Our heros now have enough bestiary knowledge to display the monster hit points
                // which makes picking of damaged monsters easier that it might be.

                // Mod settings
                // normal 15/28
                // hide current **/28
                // hide all **/**

                // health colours will still be in effect

                // text can be "?? / ??" (if HasHitPointsKnowledge=false), or
                // <#xxxx>current_hp</color>/max_hp

                var text = healthLabel.Text;

                Main.Log($"{__instance.FullName}, {text}");

                var match = HitPointRegex.Match(text);

                if (match.Success && (match.Groups["current_hp"].Value != match.Groups["max_hp"].Value))
                {
                    var hp = match.Groups["current_hp"].Value;
                    var hpLen = hp.Length;
                    var stars = new string('*', hpLen);

                    healthLabel.Text = text.Replace($">{hp}<", $">{stars}<");
                }
            }
        }
    }

    [HarmonyPatch(typeof(GuiCharacter), "FormatHealthLabelAdvanced")]
    internal static class GuiCharacter_FormatHealthLabelAdvanced
    {
        // TODO: where is this used?

        //public void FormatHealthLabelAdvanced(
        //GuiLabel healthLabel,
        //GuiLabel maxHealthLabel,
        //GuiTooltip healthTooltip,
        //bool inGame)
    }

    [HarmonyPatch(typeof(GuiCharacter), "FormatHealthGauge")]
    internal static class GuiCharacter_FormatHealthGauge
    {
        internal static void Prefix(GuiCharacter __instance, bool ___healthGaugeDirty, out bool __state)
        {
            bool dirty = __instance.UpdateHealthStatus();

            // capture current state of dirty flag for use in Postfix
            __state = ___healthGaugeDirty || dirty;
        }

        internal static void Postfix(GuiCharacter __instance, Image healthGauge, float parentHeight, bool __state)
        {
            if (!__state) return;  // health wasn't dirty so healthGauge hasn't been updated

            float ratio = Mathf.Clamp(__instance.CurrentHitPoints / (float)__instance.HitPoints, 0.0f, 1f);

            ratio = HealthExtensions.GetSteppedHealthRatio(ratio);

            healthGauge.rectTransform.offsetMax = new Vector2(healthGauge.rectTransform.offsetMax.x, (float)(-parentHeight * (1.0 - ratio)));
        }
    }

    static class HealthExtensions
    {
        /// <summary>
        /// Call 'HasHealthUpdated' which returns true/false but as a side effect updates the health state and dirty flags.
        /// Badly named.
        /// </summary>
        /// <param name="__instance"></param>
        /// <returns></returns>
        internal static bool UpdateHealthStatus(this GuiCharacter __instance)
        {
            // call badly named method
            var rb = typeof(GuiCharacter).GetMethod("HasHealthUpdated", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            bool retval = false;

            if (rb != null)
            {
                retval = (bool)rb.Invoke(__instance, null);
            }

            return retval;
        }

        internal static float GetSteppedHealthRatio(float ratio)
        {
            if (ratio >= 1f) return 1f;
            if (ratio >= 0.5f) return 0.75f;
            if (ratio >= 0.25f) return 0.5f;
            if (ratio > 0f) return 0.25f;
            return ratio;
        }
    }

    [HarmonyPatch(typeof(HealthGaugeGroup), "Refresh")]
    internal static class HealthGaugeGroup_Refresh
    {
        internal static void Postfix(HealthGaugeGroup __instance, RectTransform ___gaugeRect, float ___gaugeMaxWidth)
        {
            float ratio = Mathf.Clamp(__instance.GuiCharacter.CurrentHitPoints / (float)__instance.GuiCharacter.HitPoints, 0.0f, 1f);

            ratio = HealthExtensions.GetSteppedHealthRatio(ratio);

            ___gaugeRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ___gaugeMaxWidth * ratio);
        }
    }
}
