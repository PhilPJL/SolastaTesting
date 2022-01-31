using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace SolastaTesting;

[HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameManager_BindPostDatabase
{
    internal static void Postfix()
    {
        Main.Log("Testing: GameManager_BindPostDatabase start");

        ServiceRepository.GetService<IRuntimeService>().RuntimeLoaded += (_) =>
        {
            Main.Log("Testing: RuntimeLoaded start");

            ProtectionFromEvilFix.Load();

            Main.Log("Testing: RuntimeLoaded end");
        };

        Main.Log("Testing: GameManager_BindPostDatabase end");
    }
}
