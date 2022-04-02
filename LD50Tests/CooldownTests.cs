using FluentAssertions;
using LD50.Data;
using LD50.Gameplay;
using Machina.Engine;
using Xunit;

namespace LD50Tests
{
    public class CooldownTests
    {
        [Fact]
        public void cannot_cast_while_spell_is_on_cooldown()
        {
            var spells = new ISpell[]
            {
                new WholePartySpell("Test Spell", 0f, 0, 10, EmptyBuff.Create(), 10f)
            };
            var party = new Party();
            
            var actor = new Actor("Test Actor", null);
            var spellCaster = new SpellCaster(actor, party, spells);
            
            spellCaster.TryToCastSpell(0).Should().BeTrue();
            spellCaster.TryToCastSpell(0).Should().BeFalse();
            spellCaster.Update(20);
            spellCaster.TryToCastSpell(0).Should().BeTrue();
        }

        [Fact]
        public void can_cast_other_spell_while_spell_is_on_cooldown()
        {
            var spells = new ISpell[]
            {
                new WholePartySpell("Test Spell", 0f, 0, 10, EmptyBuff.Create(), 10f),
                new WholePartySpell("Test Spell", 0f, 0, 10, EmptyBuff.Create(), 10f)
            };
            var party = new Party();
            
            var actor = new Actor("Test Actor", null);
            var spellCaster = new SpellCaster(actor, party, spells);
            
            spellCaster.TryToCastSpell(0).Should().BeTrue();
            spellCaster.TryToCastSpell(1).Should().BeTrue();
        }
    }
}
