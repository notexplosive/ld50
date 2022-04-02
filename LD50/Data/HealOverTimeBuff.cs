namespace LD50.Data
{
    public readonly struct HealOverTimeBuff : IBuff
    {
        private readonly float cachedHealAmount;

        public HealOverTimeBuff(float remainingDuration, int healthPerSecond, float cachedHealAmount = 0f)
        {
            RemainingDuration = remainingDuration;
            HealthPerSecond = healthPerSecond;
            this.cachedHealAmount = cachedHealAmount;
        }

        public int HealthPerSecond { get; }
        public float RemainingDuration { get; }

        public IBuff GetNext(float dt)
        {
            var healAmount = GetExactHealAmount(dt);

            var newCache = healAmount - (int) healAmount;

            return new HealOverTimeBuff(RemainingDuration - dt, HealthPerSecond, newCache);
        }

        public int GetHealAmount(float dt)
        {
            var healAmount = GetExactHealAmount(dt);
            return (int) healAmount;
        }

        private float GetExactHealAmount(float dt)
        {
            var healAmount = HealthPerSecond * dt;

            if (RemainingDuration < dt)
            {
                healAmount = HealthPerSecond * RemainingDuration;
            }

            return healAmount + this.cachedHealAmount;
        }

        public static IBuff Create(float duration, int totalHealing)
        {
            return new HealOverTimeBuff(duration, (int) (totalHealing / duration));
        }
    }
}
