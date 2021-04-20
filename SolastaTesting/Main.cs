using HarmonyLib;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityModManagerNet;

namespace SolastaTesting
{
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
            using (new MethodLogger(nameof(ModAfterDBReady)))
            {
                var longsword = Helpers.GetWeapons().Where(w => w.Name.Contains("Longsword")).FirstOrDefault();

                if (longsword != null)
                {
                    //longsword.DumpDefinition();

                    //Log($"ACM: {longsword.ComputeAttackDamageEnhancement(true, null)}");
                    //Log($"ACM: {longsword.ComputeAttackHitEnhancement(true, null)}");

                    var defender = longsword.Clone("longsword defender", "17dee48b08434bc88b1b5297eddf5bab");

                    foreach(var ef in defender.WeaponDescription.EffectDescription.EffectForms)
                    {
                        ef.DamageForm.DamageBonusTrends.Add(new RuleDefinitions.TrendInfo
                        {
                            
                        });
                    }

                    //defender.DumpDefinition();
                }
                

                //Helpers.DumpDefinition(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance);
                //Helpers.DumpMonstersWithFeatureDefinition(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance, true);
                //Helpers.DumpDefinition(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance);
                //Helpers.DumpMonstersWithFeatureDefinition(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance, true);
                //Helpers.DumpDefinition(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityContagionFleshRotForce);
                //Helpers.DumpMonstersWithFeatureDefinition(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityContagionFleshRotForce, true);

                //var frfClone = DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityContagionFleshRotForce.Clone("MyFleshRotForce");
                //Helpers.DumpDefinition(frfClone);

                //var coldResistantMonsters = Helpers.GetMonstersWithFeatureDefinition(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance, true);

                //// TODO: create new ones / modify existing ones?
                //DatabaseRepository.GetDatabase<FeatureDefinitionDamageAffinity>().Add(new FeatureDefinitionDamageAffinityEx
                //{
                //    // TODO: populate - unfortunately some/most fields are read only -- need to use reflection or Harmony to set them
                //    // Name = "PsionicBlast"
                //    // etc
                //});


                // maybe this is something like what you want
                //FeatureDefinitionDamageAffinity_ModulateSustainedDamage_ExtraProperties.Apply();

                // Or this
                //FeatureDefinitionDamageAffinity_ModulateSustainedDamage.Apply();
            }
        }
    }

}
