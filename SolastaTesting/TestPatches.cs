using System;
using UnityEngine;

namespace SolastaTesting
{

    //[HarmonyPatch(typeof(CharacterInspectionScreen), "Show")]
    //internal static class CharacterInspectionScreen_Show
    //{
    //    public static void Prefix(CharacterInspectionScreen __instance, string filename, GuiScreen previousScreen)
    //    {
    //        RulesetEntity.DoNotAutoregister = true;

    //        ServiceRepository.AddService(new DummyRulesetEntityService());
    //    }

    //    public static void Postfix(CharacterInspectionScreen __instance, string filename, GuiScreen previousScreen)
    //    {
    //        RulesetEntity.DoNotAutoregister = false;

    //        ServiceRepository.RemoveService<DummyRulesetEntityService>();
    //    }
    //}

    //[HarmonyPatch(typeof(RulesetEntity), "SerializeAttributes")]
    internal static class RulesetEntity_SerializeAttributes
    {
        public static void Prefix(RulesetEntity __instance, IAttributesSerializer serializer,
            IVersionProvider versionProvider)
        {
            string name;

            try
            {
                name = __instance.Name;
            }
            catch (NullReferenceException)
            {
                name = "exception";
            }

            Main.Log($"RE_SA: Name={name}, Guid={__instance.Guid}, Att cnt={__instance.Attributes?.Count ?? 0}");
        }
    }

    //[HarmonyPatch(typeof(AttunementModal), "Load")]
    internal static class AttunementModal_Load
    {
        public static void Postfix(GameObject ___attunementSlotPrefab, RectTransform ___attunementSlotsTable)
        {
            Main.Log("Adding prefab");
            Gui.GetPrefabFromPool(___attunementSlotPrefab, ___attunementSlotsTable);
        }
    }
}
