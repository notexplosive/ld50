namespace LD50.Data
{
    public readonly struct ValueOverTime
    {
        private readonly float cachedAmount;
        private readonly int amountPerSecond;
        public float RemainingDuration { get; }

        public ValueOverTime(float remainingDuration, int amountPerSecond, float cachedAmount = 0)
        {
            RemainingDuration = remainingDuration;
            this.amountPerSecond = amountPerSecond;
            this.cachedAmount = cachedAmount;
        }
        
        public float GetExactAmount(float dt)
        {
            var healAmount = this.amountPerSecond * dt;

            if (RemainingDuration < dt)
            {
                healAmount = this.amountPerSecond * RemainingDuration;
            }

            return healAmount + this.cachedAmount;
        }

        public ValueOverTime GetUpdated(float dt)
        {
            var amount = GetExactAmount(dt);
            var newCache = amount - (int) amount;
            return new ValueOverTime(RemainingDuration - dt, this.amountPerSecond, newCache);
        }
    }
}
