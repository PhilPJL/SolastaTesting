using HarmonyLib;
using SolastaModApi;
using SolastaModApi.Diagnostics;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using TA.AI.Considerations;
using UnityEngine;
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
            if (GUILayout.Button("Generate random numbers"))
            {
                var timer = new Stopwatch();
                timer.Start();

                using (var f = File.Open(Path.Combine(modEntry.Path, "unityRandom.dat"), FileMode.Create))
                {
                    for (int i = 0; i < 10000000; i++)
                    {
                        // create a single int in range 0..255
                        var b = (byte)UnityEngine.Random.Range(0, 256);  // min-inclusive, max-exclusive

                        f.Write(new byte[] { b }, 0, 1);
                    }
                }

                Log($"Unity random = {timer.ElapsedMilliseconds}");
                timer.Restart();

                using (var f = File.Open(Path.Combine(modEntry.Path, "systemRandom.dat"), FileMode.Create))
                {
                    var r = new System.Random();
                    for (int i = 0; i < 10000000; i++)
                    {
                        // create a single int in range 0..255
                        var b = (byte)r.Next(0, 256);  // min-inclusive, max-exclusive

                        f.Write(new byte[] { b }, 0, 1);
                    }
                }

                Log($"System random = {timer.ElapsedMilliseconds}");
            }
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
            using (var logger = new MethodLogger(nameof(ModAfterDBReady)))
            {
                ShowNames(DatabaseHelper.CharacterRaceDefinitions.Dwarf);
                ShowNames(DatabaseHelper.CharacterRaceDefinitions.DwarfHill);
                ShowNames(DatabaseHelper.CharacterRaceDefinitions.DwarfSnow);
                ShowNames(DatabaseHelper.CharacterRaceDefinitions.Elf);
                ShowNames(DatabaseHelper.CharacterRaceDefinitions.ElfHigh);
                ShowNames(DatabaseHelper.CharacterRaceDefinitions.ElfSylvan);
                ShowNames(DatabaseHelper.CharacterRaceDefinitions.HalfElf);
                ShowNames(DatabaseHelper.CharacterRaceDefinitions.Halfling);
                ShowNames(DatabaseHelper.CharacterRaceDefinitions.HalflingIsland);
                ShowNames(DatabaseHelper.CharacterRaceDefinitions.HalflingMarsh);
                ShowNames(DatabaseHelper.CharacterRaceDefinitions.Human);

                void ShowNames(CharacterRaceDefinition race)
                {
                    Log($"Race: {race.Name} -----------------------");
                    Log("Male");
                    race.RacePresentation.MaleNameOptions.ForEach(n => Log(n));
                    Log("Female");
                    race.RacePresentation.FemaleNameOptions.ForEach(n => Log(n));
                }

                //logger.Log("Getting dbs");

                //var databases = (Dictionary<Type, object>)AccessTools
                //    .Field(typeof(DatabaseRepository), "databases")
                //    .GetValue(null);

                //var path = Path.Combine(Directory.GetCurrentDirectory(), "Dump.text");

                //logger.Log($"Folder={path}");

                //File.WriteAllText(path, string.Join(Environment.NewLine, databases.Select(kvp => kvp.Key.FullName)));

                //logger.Log("Dump complete");

                /*
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

                logger.Log("Just testing");
                */

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

                //var boots = DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinitySixLeaguesBoots;
                //boots.SetSpeedAddBase(false);
                //boots.SetBaseSpeedMultiplicativeModifier(1f);

                //var tome = DatabaseHelper.ItemDefinitions.Tome_Of_Understanding;
                //tome.SetField("isUsableDevice", true);

                //var bread = DatabaseHelper.ItemDefinitions.DwarfBread;
                //bread.SlotsWhereActive.Clear();
                //bread.SlotsWhereActive.Add("UtilitySlot");
                //bread.SlotTypes.Clear();
                //bread.SlotTypes.AddRange(new string[] { "UtilitySlot", "ContainerSlot" });

                //DatabaseRepository.GetDatabase<ItemDefinition>().ForEach(item => item.SetInDungeonEditor(true));
                //DatabaseRepository.GetDatabase<MonsterDefinition>().ForEach(item => item.SetInDungeonEditor(true));


                //var orcJav = DatabaseHelper.ItemDefinitions.Orc_Javelin;
                //var orcJavForms = orcJav.WeaponDescription.EffectDescription.EffectForms;
                //if (!orcJavForms.Any())
                //{
                //    orcJavForms.Add(new EffectForm
                //    {
                //        DamageForm = new DamageForm
                //        {
                //            DiceNumber = 1,
                //            DieType = RuleDefinitions.DieType.D8,
                //            DamageType = "DamagePiercing"
                //        }
                //    });
                //}
                //orcJav.SetCanBeStacked(true);
                //orcJav.SetStackSize(5);

                //var ogreJav = DatabaseHelper.ItemDefinitions.Ogre_Javelin;
                //var ogreJavForms = ogreJav.WeaponDescription.EffectDescription.EffectForms;
                //if (!ogreJavForms.Any())
                //{
                //    ogreJavForms.Add(new EffectForm
                //    {
                //        DamageForm = new DamageForm
                //        {
                //            DiceNumber = 1,
                //            DieType = RuleDefinitions.DieType.D10,
                //            DamageType = "DamagePiercing"
                //        }
                //    });
                //}
                //ogreJav.SetCanBeStacked(true);
                //ogreJav.SetStackSize(5);


                //DatabaseRepository.GetDatabase<ItemDefinition>().ForEach(item =>
                //{
                //    if (!item.ForceEquip) // COTF is the only item that has to be equipped
                //    {
                //        item.SetForceEquipSlot(string.Empty);
                //    }
                //});

                //DatabaseHelper.LootPackDefinitions.
                //var chest = DatabaseHelper.LootPackDefinitions.CONJURATION_TheSuperEgo_Mansion_OptionalQuest_LootQuest_Chest_01;
                //var list = chest.ItemOccurencesList;

                //foreach(var i in list)
                //{
                //    Log($"{i.ItemDefinition.Name}, {i.DiceNumber}, {i.DiceType}");
                //}

                //list.Add(Create(DatabaseHelper.ItemDefinitions.Enchanted_Longsword_Dragonblade));
                //list.Add(Create(DatabaseHelper.ItemDefinitions.MantleOfSpellResistance));
                //list.Add(Create(DatabaseHelper.ItemDefinitions.CloakOfDisplacement));
                //list.Add(Create(DatabaseHelper.ItemDefinitions.ShieldPlus3));
                //list.Add(Create(DatabaseHelper.ItemDefinitions.ScrollGreaterInvisibility));
                //list.Add(Create(DatabaseHelper.ItemDefinitions.ScrollGreaterInvisibility));
                //list.Add(Create(DatabaseHelper.ItemDefinitions.ScrollGreaterInvisibility));

                //foreach (var i in list)
                //{
                //    Log($"{i.ItemDefinition.Name}, {i.DiceNumber}, {i.DiceType}");
                //}

                //ItemOccurence Create(ItemDefinition definition)
                //{
                //    var item = new ItemOccurence
                //    {
                //        DiceNumber = 1,
                //        DiceType = RuleDefinitions.DieType.D1
                //    };

                //    item.SetItemDefinition(definition);

                //    return item;
                //}

                //try
                //{
                //    var definition = new DivineHumanFeatureDefinitionCastSpellBuilder(
                //            DatabaseHelper.FeatureDefinitionCastSpells.CastSpellElfHigh, // clone original
                //            "DivineHumanSpells", // new name
                //            "4b61816bfbd04601b07ddba281ec7d5c") // new guid
                //        .AddToDB(); // add to database

                //    // assign
                //    DatabaseHelper.CharacterRaceDefinitions.Human.FeatureUnlocks.Add(new FeatureUnlockByLevel(definition, 1));           
                //}
                //catch (Exception ex)
                //{
                //    Error(ex);
                //    throw;
                //}

            }
        }
    }
}
