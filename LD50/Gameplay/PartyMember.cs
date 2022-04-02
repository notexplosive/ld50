using LD50.Data;

namespace LD50.Gameplay
{
    public class PartyMember
    {
        private PartyMemberStatus status;
        private int pendingDamage;
        private int pendingHeals;

        public PartyMember(BaseStats baseStats)
        {
            this.status = new PartyMemberStatus(baseStats, new Buffs());
        }

        public float HealthPercent => (float)this.status.Health / this.status.BaseStats.MaxHealth;

        public void Update(float dt)
        {
            this.status = this.status.GetNext(dt, this.pendingHeals, this.pendingDamage);
            this.pendingDamage = 0;
            this.pendingHeals = 0;
        }

        public void TakeDamage(int damage)
        {
            this.pendingDamage += damage;
        }

        public void TakeHeal(int heal)
        {
            this.pendingHeals += heal;
        }

        public void GainBuff(Buff buff)
        {
            this.status.Buffs.AddBuff(buff);
        }
    }
}
