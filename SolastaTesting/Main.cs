using HarmonyLib;
using SolastaModApi;
using System;
using System.Diagnostics;
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

            Helpers.DumpDefinition(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance);
            Helpers.DumpMonstersWithFeatureDefinition(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance, true);
            Helpers.DumpDefinition(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance);
            Helpers.DumpMonstersWithFeatureDefinition(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance, true);
            Helpers.DumpDefinition(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityContagionFleshRotForce);
            Helpers.DumpMonstersWithFeatureDefinition(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityContagionFleshRotForce, true);

            var frfClone = DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityContagionFleshRotForce.Clone();
            Helpers.DumpDefinition(frfClone);

            //var coldResistantMonsters = Helpers.GetMonstersWithFeatureDefinition(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance, true);

            //// TODO: create new ones / modify existing ones?
            //DatabaseRepository.GetDatabase<FeatureDefinitionDamageAffinity>().Add(new FeatureDefinitionDamageAffinityEx
            //{
            //    // TODO: populate - unfortunately some/most fields are read only -- need to use reflection or Harmony to set them
            //    // Name = "PsionicBlast"
            //    // etc
            //});

            FeatureDefinitionDamageAffinity_ModulateSustainedDamage_ExtraProperties.Apply();
        }
    }
}
