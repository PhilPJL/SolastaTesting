using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolastaTesting
{
    public static class Helpers
    {
        internal static IEnumerable<MonsterDefinition>
            GetMonstersWithFeatureDefinition<TFeatureDef>(TFeatureDef feature, bool? guiPresentation = null) where TFeatureDef : FeatureDefinition
        {
            return DatabaseRepository
                .GetDatabase<MonsterDefinition>()
                .GetAllElements()
                .Where(m => m.Features.OfType<TFeatureDef>()
                    .Where(f => !guiPresentation.HasValue || guiPresentation.Value == !f.GuiPresentation.Hidden)
                    .Any(f => f == feature));
        }

        internal static IEnumerable<ItemDefinition> GetWeapons()
        {
            var items = DatabaseRepository.GetDatabase<ItemDefinition>().GetAllElements();

            Main.Log($"Item count={items.Count()}");
            Main.Log($"Weapon count={items.Where(item => item.IsWeapon).Count()}");

            return items.Where(item => item.IsWeapon);
        }

        internal static void DumpMonstersWithFeatureDefinition<TFeatureDef>(TFeatureDef feature, bool? guiPresentation = null) where TFeatureDef : FeatureDefinition
        {
            Main.Log($"---- Monsters with affinity '{feature.Name}' and guiPresentation={guiPresentation?.ToString() ?? "(any)"}");

            var monsters = GetMonstersWithFeatureDefinition(feature, guiPresentation);

            if (!monsters.Any())
            {
                Main.Log("*** no monsters ***");
            }
            else
            {
                foreach (var m in monsters.OrderBy(m => m.Name))
                {
                    Main.Log($"{m.Name}");
                }
            }
        }

        internal static void DumpWeaponDefinition(this ItemDefinition item)
        {
            if (!item.IsWeapon)
            {
                Main.Log($"{item.Name} isn't a weapon!");
                return;
            }

            Main.Log($"{item.Name}, {string.Join(", ", item.WeaponDescription.WeaponTags)}");

            foreach (var form in item.WeaponDescription.EffectDescription.EffectForms)
            {
                var df = form.DamageForm;
                Main.Log($"EffectForm={form.FormType}, Die={df.DiceNumber}, DieType={df.DieType}, Versatile={df.Versatile}, VersatileDie={df.VersatileDieType}, DamageType={df.DamageType}");
            }
        }

        /// <summary>
        /// Dump public properties of BaseDefinition
        /// </summary>
        /// <param name="affinity"></param>
        internal static void DumpDefinition<TDef>(this TDef definition) where TDef : BaseDefinition
        {
            Main.Log($"---- {typeof(TDef).Name} properties for '{definition.Name}'");

            foreach (var p in typeof(TDef).GetProperties().OrderBy(p => p.Name))
            {
                Main.Log($"{p.Name}: {p.GetValue(definition, null)}");
            }
        }

        internal static T Clone<T>(this T original, string name, string guid = null) where T : BaseDefinition
        {
            // Shallow copy
            var clone = UnityEngine.Object.Instantiate(original);

            AccessTools.Field(typeof(T), "guid").SetValue(clone, guid ?? Guid.NewGuid().ToString("N"));
            clone.name = name;

            return clone;
        }
    }
}
