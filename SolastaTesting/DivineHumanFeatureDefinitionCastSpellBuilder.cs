using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaTesting
{
    public class DivineHumanFeatureDefinitionCastSpellBuilder : BaseDefinitionBuilder<FeatureDefinitionCastSpell>
    {
        // Other constructors not required

        // Clone ctor
        public DivineHumanFeatureDefinitionCastSpellBuilder(FeatureDefinitionCastSpell original, string name, string guid) : base(original, name, guid)
        {
            var presentation = new GuiPresentation
            {
                Description = "Test description",
                Title = "Test title"
            };

            presentation.SetColor(UnityEngine.Color.red);

            // Configure
            Definition
                .SetStaticDCValue(10)
                .SetStaticToHitValue(4)
                .SetSpellcastingAbility("Wisdom")
                .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.Selection)
                .SetSpellCastingLevel(-1)
                .SetGuiPresentation(presentation);
        }
    }
}
