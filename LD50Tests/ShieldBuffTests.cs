using FluentAssertions;
using FluentAssertions.Execution;
using LD50.Data;
using LD50.Gameplay;
using Xunit;

namespace LD50Tests
{
    public class ShieldBuffTests
    {
        [Fact]
        public void can_absorb_damage_partial_to_shield()
        {
            var partyMember = new PartyMember(new BaseStats(100, 100));
            partyMember.GainBuff(new ShieldBuff(3f, 15));
            
            partyMember.TakeDamage(5);
            partyMember.Update(0.25f);

            var buff = partyMember.GetBuffs().At(0);
            if (buff is ShieldBuff shield)
            {
                shield.RemainingDuration.Should().BeApproximately(2.75f, 0.1f);
                shield.DamageAbsorb.Should().Be(10);
            }
            else
            {
                throw new AssertionFailedException("Should have been a shield");
            }

            partyMember.Status.Health.Should().Be(100);
            
        }

        [Fact]
        public void can_break_shield_if_exact_damage_is_dealt()
        {
            var partyMember = new PartyMember(new BaseStats(100, 100));
            partyMember.GainBuff(new ShieldBuff(3f, 15));
            
            partyMember.TakeDamage(15);
            partyMember.Update(0.25f);

            partyMember.Status.Buffs.Count.Should().Be(0);
            partyMember.Status.Health.Should().Be(100);
        }

        [Fact]
        public void destroy_shield_with_additional_damage()
        {
            var partyMember = new PartyMember(new BaseStats(100, 100));
            partyMember.GainBuff(new ShieldBuff(3f, 15));
            
            partyMember.TakeDamage(20);
            partyMember.Update(0.25f);

            partyMember.Status.Buffs.Count.Should().Be(0);
            partyMember.Status.Health.Should().Be(95);
        }
    }
}