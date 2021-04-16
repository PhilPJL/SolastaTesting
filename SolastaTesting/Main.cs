using HarmonyLib;
using SolastaModApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityModManagerNet;

namespace SolastaTesting
{
    public class Main
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

        [HarmonyPatch(typeof(MainMenuScreen), "RuntimeLoaded")]
        internal static class MainMenuScreen_RuntimeLoaded_Patch
        {
            internal static void Postfix(Runtime runtime)
            {
                ModAfterDBReady();
            }
        }

        // ENTRY POINT IF YOU NEED SERVICE LOCATORS ACCESS
        internal static void ModBeforeDBReady()
        {
            Log(nameof(ModBeforeDBReady));
        }

        // ENTRY POINT IF YOU NEED SAFE DATABASE ACCESS
        internal static void ModAfterDBReady()
        {
            Log(nameof(ModAfterDBReady));

            DumpAffinityDefinition(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance);
            DumpMonstersWithAffinity(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance, true);
            DumpAffinityDefinition(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance);
            DumpMonstersWithAffinity(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance, true);
            DumpAffinityDefinition(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityContagionFleshRotForce);
            DumpMonstersWithAffinity(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityContagionFleshRotForce, true);

            // TODO: create new ones / modify existing ones?
            DatabaseRepository.GetDatabase<FeatureDefinitionDamageAffinity>().Add(new FeatureDefinitionDamageAffinityEx
            {
                // TODO: populate - unfortunately some/most fields are read only -- need to use reflection or Harmony to set them
                // Name = "PsionicBlast"
                // etc
            });
        }

        /// <summary>
        /// Show all monsters with specified affinity
        /// </summary>
        /// <param name="affinity"></param>
        private static void DumpMonstersWithAffinity<T>(T affinity, bool? guiPresentation = null) where T : FeatureDefinitionAffinity
        {
            Log($"---- Monsters with affinity '{affinity.Name}' and guiPresentation={guiPresentation?.ToString() ?? "(any)"}");

            var monsters = DatabaseRepository
                .GetDatabase<MonsterDefinition>()
                .GetAllElements()
                .Where(m => m.Features.OfType<T>()
                    .Where(f => !guiPresentation.HasValue || guiPresentation.Value == !f.GuiPresentation.Hidden)
                    .Any(f => f == affinity));

            if (!monsters.Any())
            {
                Log("*** no monsters ***");
            }
            else
            {
                foreach (var m in monsters.OrderBy(m => m.Name))
                {
                    Log($"{m.Name}");
                }
            }
        }

        /// <summary>
        /// Dump public properties of FeatureDefinitionDamageAffinity
        /// </summary>
        /// <param name="affinity"></param>
        private static void DumpAffinityDefinition(FeatureDefinitionDamageAffinity affinity)
        {
            Log($"---- Affinity properties for '{affinity.Name}'");

            foreach (var p in typeof(FeatureDefinitionDamageAffinity).GetProperties().OrderBy(p => p.Name))
            {
                Log($"{p.Name}: {p.GetValue(affinity, null)}");
            }
        }
    }

    /// <summary>
    /// Possible custom affinity
    /// </summary>
    [HarmonyPatch(typeof(FeatureDefinitionDamageAffinity), "ModulateSustainedDamage")]
    internal class FeatureDefinitionDamageAffinityEx : FeatureDefinitionDamageAffinity
    {
        // Additional properties
        // ...

        /// <summary>
        /// Patch ModulateSustainedDamage
        /// </summary>
        internal static void Postfix(FeatureDefinitionDamageAffinity __instance, string damageType, int damage, List<string> sourceTags, ref int __result)
        {
            var fda = __instance as FeatureDefinitionDamageAffinityEx;

            if (fda != null)
            {
                // original logic has run and damage calculated will be in __result

                // custom logic here, modify __result 
            }
            // else do nothing (or something if required)
        }
    }
}
