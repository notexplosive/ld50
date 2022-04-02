using Machina.Engine;

namespace LD50.Data
{
    public readonly struct Buff
    {
        private readonly float cachedHealAmount;

        public Buff(float remainingDuration, int healthPerSecond, float cachedHealAmount = 0f)
        {
            RemainingDuration = remainingDuration;
            HealthPerSecond = healthPerSecond;
            this.cachedHealAmount = cachedHealAmount;
        }

        public int HealthPerSecond { get; }
        public float RemainingDuration { get; }

        public Buff GetNext(float dt)
        {
            var healAmount = GetExactHealAmount(dt);

            var newCache = healAmount - (int) healAmount;

            return new Buff(RemainingDuration - dt, HealthPerSecond, newCache);
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

        public static Buff Empty()
        {
            return new Buff();
        }

        public static Buff HealOverTime(float duration, int totalHealing)
        {
            return new Buff(duration, (int) (totalHealing / duration));
        }

        public static Buff Shield()
        {
            return new Buff();
        }
    }
}
