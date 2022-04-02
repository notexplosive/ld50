using Machina.Engine;

namespace LD50.Data
{
    public interface IBuff
    {
        public float RemainingDuration { get; }
        IBuff GetNext(float dt);
        int GetHealAmount(float dt);
    }
}
