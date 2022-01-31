using HarmonyLib;
using SolastaModApi;
using SolastaModApi.Diagnostics;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace SolastaTesting;

public static class Main
{
    [Conditional("DEBUG")]
    internal static void Log(string msg) => Logger.Log(msg);

    internal static void Error(Exception ex) => Logger?.Error(ex.ToString());
    internal static void Error(string msg) => Logger?.Error(msg);
    internal static UnityModManager.ModEntry.ModLogger Logger { get; private set; }

    internal static bool Load(UnityModManager.ModEntry modEntry)
    {
        try
        {
            Logger = modEntry.Logger;

            ModBeforeDBReady();

            modEntry.OnGUI = OnGUI;

            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        catch (Exception ex)
        {
            Error(ex);
            throw;
        }
        return true;
    }

    internal static void OnGUI(UnityModManager.ModEntry modEntry)
    {
    }

    // ENTRY POINT IF YOU NEED SERVICE LOCATORS ACCESS
    internal static void ModBeforeDBReady()
    {
        Log(nameof(ModBeforeDBReady));
    }

    // ENTRY POINT IF YOU NEED SAFE DATABASE ACCESS
    internal static void ModAfterDBReady()
    {
    }
}
