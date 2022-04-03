namespace LD50.Data
{
    public readonly struct HealOverTimeBuff : IBuff
    {
        private readonly ValueOverTime valueOverTime;

        public HealOverTimeBuff(ValueOverTime valueOverTime = default)
        {
            this.valueOverTime = valueOverTime;
        }

        public float RemainingDuration => this.valueOverTime.RemainingDuration;

        public IBuff GetNext(float dt)
        {
            var newValueOverTime = this.valueOverTime.GetUpdated(dt);
            return new HealOverTimeBuff(newValueOverTime);
        }

        public int GetHealAmount(float dt)
        {
            var healAmount = this.valueOverTime.GetExactAmount(dt);
            return (int) healAmount;
        }

        public static IBuff Create(float duration, int totalHealing)
        {
            return new HealOverTimeBuff(new ValueOverTime(duration, (int) (totalHealing / duration)));
        }
    }
}
