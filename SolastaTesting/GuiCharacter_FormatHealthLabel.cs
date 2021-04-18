using HarmonyLib;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SolastaTesting
{
    [HarmonyPatch(typeof(GuiCharacter), "FormatHealthLabel")]
    internal static class GuiCharacter_FormatHealthLabel
    {
        private static readonly Regex HitPointRegex = new Regex(@"^<#.{6}>(?<current_hp>\d{1,4})</color>/(?<max_hp>\d{1,4})", RegexOptions.Compiled | RegexOptions.Singleline);

        internal static void Prefix(GuiCharacter __instance, GuiLabel healthLabel, bool ___healthLabelDirty, out bool __state)
        {
            bool dirty = UpdateHealthStatus(__instance);

            // capture current state of dirty flag for use in Postfix
            __state = ___healthLabelDirty || dirty;
        }

        internal static void Postfix(GuiCharacter __instance, GuiLabel healthLabel, bool __state)
        {
            if (!__state) return;  // health wasn't dirty so hasn't been updated

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

                if(match.Success)
                {
                    var hp = match.Groups["current_hp"].Value;
                    var hpLen = hp.Length;
                    var stars = new string('*', hpLen);

                    healthLabel.Text = text.Replace($">{hp}<", $">{stars}<");
                }
            }
        }

        /// <summary>
        /// Call 'HasHealthUpdated' which returns true/false but as a side effect updates the health state and dirty flag.
        /// Badly named.
        /// </summary>
        /// <param name="__instance"></param>
        /// <returns></returns>
        private static bool UpdateHealthStatus(GuiCharacter __instance)
        {
            // call badly named method
            var rb = typeof(GuiCharacter).GetMethod("HasHealthUpdated", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            bool retval = false;

            if (rb != null)
            {
                retval = (bool)rb.Invoke(__instance, null);
            }

            // Main.Log($"GuiCharacter_FormatHealthLabel: UpdateHealthStatus - retval={retval}.");

            return retval;
        }
    }
}
