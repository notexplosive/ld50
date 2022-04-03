using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;

namespace LD50.Renderer
{
    public class AscendAndFadeOutText : BaseComponent
    {
        private readonly BoundedTextRenderer textRenderer;
        private readonly Color startingColor;
        private readonly float totalLifetime;
        private float remainingLifetime;

        public AscendAndFadeOutText(Actor actor) : base(actor)
        {
            this.textRenderer = RequireComponent<BoundedTextRenderer>();
            this.startingColor = this.textRenderer.TextColor;
            this.totalLifetime = 1.5f;
            this.remainingLifetime = this.totalLifetime;
        }

        public override void Update(float dt)
        {
            this.remainingLifetime -= dt;
            this.textRenderer.TextColor = this.startingColor.WithMultipliedOpacity(this.remainingLifetime / this.totalLifetime);

            if (this.remainingLifetime < 0)
            {
                this.actor.Destroy();
            }

            transform.Position += new Vector2(0,-dt) * 60;
        }
    }
}
