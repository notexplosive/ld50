using FluentAssertions;
using LD50.Data;
using LD50.Gameplay;
using Machina.Engine;
using Xunit;

namespace LD50Tests
{
    public class ManaTests
    {
        [Fact]
        public void mana_regenerates_incrementally_small_numbers()
        {
            var status = new PartyMemberStatus(new BaseStats(100, 100, 1), new Buffs());
            
            var halfSecond = status.GetNext(0.5f, 0, 0, 0);
            var threeFourthsSecond = halfSecond.GetNext(0.25f, 0, 0, 0);
            var oneSecond = threeFourthsSecond.GetNext(0.25f, 0, 0, 0);

            halfSecond.Mana.Should().Be(0);
            threeFourthsSecond.Mana.Should().Be(0);
            oneSecond.Mana.Should().Be(1);
        }

        [Fact]
        public void spells_cost_mana()
        {
            var spells = new ISpell[]
            {
                new WholePartySpell("Test Spell", 0f, 5, 10, EmptyBuff.Create(), 10f)
            };
            var party = new Party();
            var player = new PartyMember(new BaseStats(10, 8));

            var actor = new Actor("Test Actor", null);
            var spellCaster = new SpellCaster(actor, party, spells, player, new Cooldown(0));

            spellCaster.TryToCastSpell(0).Should().BeTrue();
            spellCaster.TryToCastSpell(0).Should().BeFalse();
        }
    }
}
