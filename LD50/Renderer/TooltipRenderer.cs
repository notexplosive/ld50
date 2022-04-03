using System;
using System.Linq;
using LD50.Data;
using Machina.Components;
using Machina.Data;
using Machina.Data.TextRendering;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD50.Renderer
{
    public class TooltipRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly Tuple<Hoverable, ISpell>[] hoverableSpellTuples;
        private ISpell hoveredSpell;
        private float timer;

        public TooltipRenderer(Actor actor, Tuple<Hoverable, ISpell>[] hoverableSpellTuples) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.hoverableSpellTuples = hoverableSpellTuples;
        }

        public override void Update(float dt)
        {
            this.timer -= dt;

            foreach (var tuple in this.hoverableSpellTuples)
            {
                if (tuple.Item1.IsHovered)
                {
                    this.hoveredSpell = tuple.Item2;
                    this.timer = 0.1f;
                }
            }

            if (this.timer < 0)
            {
                this.hoveredSpell = null;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var ninepatch = MachinaClient.Assets.GetMachinaAsset<NinepatchSheet>("tooltip-patch");

            var bounds = this.boundingRect.Rect;
            bounds.Inflate(-20, -20);

            if (this.hoveredSpell != null)
            {
                var boundedText = new BoundedText(bounds.Size, Alignment.TopLeft, Overflow.Ignore,
                    new FormattedText(this.hoveredSpell.GenerateDescription().ToArray()));

                foreach (var item in boundedText.GetRenderedText())
                {
                    item.Draw(spriteBatch, bounds.Location, 0f, transform.Depth - 200);
                }

                var tooltipBounds = new Rectangle(bounds.Location, boundedText.UsedSize);
                tooltipBounds.Inflate(20, 20);
                
                ninepatch.DrawFullNinepatch(spriteBatch,
                    tooltipBounds,
                    NinepatchSheet.GenerationDirection.Inner, transform.Depth - 100);
            }
        }
    }
}
