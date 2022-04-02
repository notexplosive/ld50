namespace LD50.Data
{
    public readonly struct PartyMemberStatus
    {
        private readonly int damageTaken;
        private readonly Buffs appliedBuffs;
        public BaseStats BaseStats { get; }
        public int Health => BaseStats.MaxHealth - this.damageTaken;    

        public PartyMemberStatus(BaseStats baseStats, Buffs appliedBuffs, int damageTaken = 0)
        {
            BaseStats = baseStats;
            this.damageTaken = damageTaken;
            this.appliedBuffs = appliedBuffs;
        }

        public PartyMemberStatus GetNext(float dt, int healsThisFrame, int damageThisFrame)
        {
            var buffs = this.appliedBuffs.GetNext(dt);
            healsThisFrame += buffs.GetHealingThisTick(dt);
            damageThisFrame += buffs.GetDamageThisTick(dt);
            var damageTaken = this.damageTaken - healsThisFrame + damageThisFrame;
            
            return new PartyMemberStatus(BaseStats, buffs, damageTaken);
        }
    }
}
