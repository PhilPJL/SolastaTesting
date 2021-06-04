using HarmonyLib;

namespace SolastaTesting
{
    //[HarmonyPatch(typeof(RulesetInventory), "AutoEquipItem")]
    internal static class RulesetInventory_AutoEquipItem
    {
#if DEBUG
        private static bool LoggedMessage = false;
#endif
        public static bool Prefix(out RulesetInventorySlot selectedSlot)
        {
#if DEBUG
            if (!LoggedMessage)
            {
                Main.Log("Ignoring AutoEquipItem");
                LoggedMessage = true;
            }
#endif
            selectedSlot = null;

            // No auto equip
            return false;
        }
    }
}
