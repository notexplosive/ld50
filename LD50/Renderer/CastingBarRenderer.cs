using LD50.Gameplay;
using Machina.Components;
using Machina.Data;
using Machina.Data.TextRendering;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace LD50.Renderer
{
    public class CastingBarRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly SpellCaster spellCaster;

        public CastingBarRenderer(Actor actor, SpellCaster spellCaster) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.spellCaster = spellCaster;
        }

        public override void Update(float dt)
        {
            this.spellCaster.UpdateTween(dt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(this.boundingRect.Rect, Color.Black, 1f, transform.Depth - 10);

            var fillRect = new Rectangle(this.boundingRect.Rect.Location,
                new Point((int) (this.boundingRect.Rect.Size.X * this.spellCaster.Percent),
                    this.boundingRect.Rect.Size.Y));
            
            spriteBatch.FillRectangle(fillRect, Color.LightBlue, transform.Depth - 5);

            if (this.spellCaster.InProgressSpell.Spell != null)
            {
                var fontMetrics = new SpriteFontMetrics(MachinaClient.Assets.GetSpriteFont("TitleFont"));
                var text = new BoundedText(this.boundingRect.Size, Alignment.Center, Overflow.Ignore, FormattedText.FromString(this.spellCaster.InProgressSpell.Spell.Name, fontMetrics, Color.White));

                foreach (var item in text.GetRenderedText())
                {
                    item.Draw(spriteBatch, this.boundingRect.Location, 0f, transform.Depth - 50);
                    item.DrawDropShadow(spriteBatch, Color.DarkBlue.WithMultipliedOpacity(0.5f), this.boundingRect.Location, 0f, transform.Depth - 49);
                }
                
                var durationUpToWindow = this.spellCaster.InProgressSpell.Spell.CastDuration - SpellCaster.BufferWindow;
                var percent = durationUpToWindow / this.spellCaster.InProgressSpell.Spell.CastDuration;
                
                var alongMark = fillRect.Location.ToVector2() + new Vector2(this.boundingRect.Width * percent, 0);
                spriteBatch.DrawLine(alongMark, alongMark + new Vector2(0, fillRect.Height), Color.Black, 1f,
                    transform.Depth - 10);
            }
        }
    }
}
