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
            var spellCaster = new SpellCaster(actor, party, spells, new PartyMember(new BaseStats()), new Cooldown(0));

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
            var spellCaster = new SpellCaster(actor, party, spells, new PartyMember(new BaseStats()), new Cooldown(0));

            spellCaster.TryToCastSpell(0).Should().BeTrue();
            spellCaster.TryToCastSpell(1).Should().BeTrue();
        }

        [Fact]
        public void global_cooldown_works()
        {
            var spells = new ISpell[]
            {
                new WholePartySpell("Test Spell", 0f, 0, 10, EmptyBuff.Create(), 0f),
                new WholePartySpell("Test Spell", 0f, 0, 10, EmptyBuff.Create(), 0f)
            };
            var party = new Party();

            var actor = new Actor("Test Actor", null);
            var spellCaster = new SpellCaster(actor, party, spells, new PartyMember(new BaseStats()), new Cooldown(0.5f));

            spellCaster.TryToCastSpell(0).Should().BeTrue();
            spellCaster.TryToCastSpell(1).Should().BeFalse();
        }

        [Fact]
        public void global_cooldown_fades_away()
        {
            var spells = new ISpell[]
            {
                new WholePartySpell("Test Spell", 0f, 0, 10, EmptyBuff.Create(), 0f),
                new WholePartySpell("Test Spell", 0f, 0, 10, EmptyBuff.Create(), 0f)
            };
            var party = new Party();

            var actor = new Actor("Test Actor", null);
            var spellCaster = new SpellCaster(actor, party, spells, new PartyMember(new BaseStats()), new Cooldown(0.5f));

            spellCaster.TryToCastSpell(0).Should().BeTrue();
            spellCaster.Update(1f);
            spellCaster.TryToCastSpell(1).Should().BeTrue();
        }
    }
}
