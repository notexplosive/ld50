using FluentAssertions;
using LD50.Data;
using Xunit;

namespace LD50Tests
{
    public class HealOverTimeTests
    {
        [Fact]
        public void heal_over_time_works_non_incrementally_big_numbers()
        {
            var status = new PartyMemberStatus(new BaseStats(100, 100), new Buffs(), 50);
            status.Buffs.AddBuff(new HealOverTimeBuff(3, 3));
            
            
            var statusAfterOneSecond = status.GetNext(1f, 0, 0);
            var statusAfterTwoSeconds = status.GetNext(2f, 0, 0);
            var statusAfterThreeSeconds = status.GetNext(3f, 0, 0);

            statusAfterOneSecond.Health.Should().Be(53);
            statusAfterTwoSeconds.Health.Should().Be(56);
            statusAfterThreeSeconds.Health.Should().Be(59);
        }
        
        [Fact]
        public void heal_over_time_works_non_incrementally_small_numbers()
        {
            var status = new PartyMemberStatus(new BaseStats(100, 100), new Buffs(), 50);
            status.Buffs.AddBuff(new HealOverTimeBuff(3, 1));
            
            
            var halfSecond = status.GetNext(0.5f, 0, 0);
            var oneSecond = status.GetNext(1f, 0, 0);
            var oneAndAHalfSeconds = status.GetNext(1.5f, 0, 0);
            var twoSeconds = status.GetNext(2f, 0, 0);

            halfSecond.Health.Should().Be(50);
            oneSecond.Health.Should().Be(51);
            oneAndAHalfSeconds.Health.Should().Be(51);
            twoSeconds.Health.Should().Be(52);
        }
        
        [Fact]
        public void heal_over_time_works_incrementally_big_numbers()
        {
            var status = new PartyMemberStatus(new BaseStats(100, 100), new Buffs(), 50);
            status.Buffs.AddBuff(new HealOverTimeBuff(3, 3));
            
            
            var statusAfterOneSecond = status.GetNext(1f, 0, 0);
            var statusAfterTwoSeconds = statusAfterOneSecond.GetNext(1f, 0, 0);
            var statusAfterThreeSeconds = statusAfterTwoSeconds.GetNext(1f, 0, 0);

            statusAfterOneSecond.Health.Should().Be(53);
            statusAfterTwoSeconds.Health.Should().Be(56);
            statusAfterThreeSeconds.Health.Should().Be(59);
        }
        
        [Fact]
        public void heal_over_time_works_incrementally_small_numbers()
        {
            var status = new PartyMemberStatus(new BaseStats(100, 100), new Buffs(), 50);
            status.Buffs.AddBuff(new HealOverTimeBuff(3, 1));
            
            
            var halfSecond = status.GetNext(0.5f, 0, 0);
            var oneSecond = halfSecond.GetNext(0.5f, 0, 0);
            var oneAndAHalfSeconds = oneSecond.GetNext(0.5f, 0, 0);
            var twoSeconds = oneAndAHalfSeconds.GetNext(0.5f, 0, 0);

            halfSecond.Health.Should().Be(50);
            oneSecond.Health.Should().Be(51);
            oneAndAHalfSeconds.Health.Should().Be(51);
            twoSeconds.Health.Should().Be(52);
        }
    }
}
