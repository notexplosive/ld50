using FluentAssertions;
using LD50.Data;
using Xunit;

namespace LD50Tests
{
    public class ManaRegenerationTest
    {
        [Fact]
        public void mana_regenerates_incrementally_small_numbers()
        {
            var status = new PartyMemberStatus(new BaseStats(100, 100, 1), new Buffs());
            
            var halfSecond = status.GetNext(0.5f, 0, 0);
            var threeFourthsSecond = halfSecond.GetNext(0.25f, 0, 0);
            var oneSecond = threeFourthsSecond.GetNext(0.25f, 0, 0);

            halfSecond.Mana.Should().Be(0);
            threeFourthsSecond.Mana.Should().Be(0);
            oneSecond.Mana.Should().Be(1);
        }
    }
}
