namespace LD50.Data
{
    public readonly struct ShieldBuff : IBuff
    {
        public ShieldBuff(float duration, int damageAbsorb)
        {
            RemainingDuration = duration;
            DamageAbsorb = damageAbsorb;
        }
        
        public int FrameIndex => 3;

        public int DamageAbsorb { get; }
        
        public static IBuff Create(float duration, int damageAbsorb)
        {
            return new ShieldBuff(duration, damageAbsorb);
        }

        public float RemainingDuration { get; }

        public int GetDamageAfterAbsorb(int damage)
        {
            if (DamageAbsorb > damage)
            {
                return 0;
            }
            else
            {
                return damage - DamageAbsorb;
            }
        }

        public IBuff GetBuffAfterAbsorb(int damageMitigated)
        {
            var newAbsorb = DamageAbsorb - damageMitigated;

            if (newAbsorb == 0)
            {
                return EmptyBuff.Create();
            }
            return new ShieldBuff(RemainingDuration, newAbsorb);
        }
        
        public IBuff GetNext(float dt)
        {
            return new ShieldBuff(RemainingDuration - dt, DamageAbsorb);
        }

        public int GetHealAmount(float dt)
        {
            return 0;
        }
    }
}
