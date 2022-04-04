using LD50.Gameplay;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace LD50.Renderer
{
    public class QueueButtonRenderer : BaseComponent
    {
        private readonly NinepatchSheet ninepatch;
        private readonly BoundingRect boundingRect;
        private readonly Hoverable hoverable;

        public QueueButtonRenderer(Actor actor, PartyRole role) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.hoverable = RequireComponent<Hoverable>();
            this.ninepatch = MachinaClient.Assets.GetMachinaAsset<NinepatchSheet>("ui-patch");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var renderedRect = this.boundingRect.Rect;

            if (this.hoverable.IsHovered)
            {
                var rect = renderedRect;
                rect.Inflate(10, 10);
                spriteBatch.DrawRectangle(rect, Color.White, 3f, transform.Depth);
            }
            
            this.ninepatch.DrawFullNinepatch(spriteBatch, renderedRect, NinepatchSheet.GenerationDirection.Outer,
                transform.Depth + 100);
        }
    }
}
