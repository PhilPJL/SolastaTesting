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

        /// <summary>
        /// Dump public properties of BaseDefinition
        /// </summary>
        /// <param name="affinity"></param>
        internal static void DumpDefinition<TDef>(TDef definition) where TDef : BaseDefinition
        {
            Main.Log($"---- {typeof(TDef).Name} properties for '{definition.Name}'");

            foreach (var p in typeof(TDef).GetProperties().OrderBy(p => p.Name))
            {
                Main.Log($"{p.Name}: {p.GetValue(definition, null)}");
            }
        }

        internal static T Clone<T>(this T original) where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(original);
        }
    }
}
