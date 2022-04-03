using LD50.Gameplay;
using Machina.Components;
using Machina.Data;
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
            spriteBatch.DrawRectangle(this.boundingRect.Rect, Color.White, 1f, transform.Depth - 10);

            var fillRect = new Rectangle(this.boundingRect.Rect.Location,
                new Point((int) (this.boundingRect.Rect.Size.X * this.spellCaster.Percent),
                    this.boundingRect.Rect.Size.Y));
            
            spriteBatch.FillRectangle(fillRect, Color.White, transform.Depth - 5);

            if (this.spellCaster.InProgressSpell.Spell != null)
            {
                var durationUpToWindow = this.spellCaster.InProgressSpell.Spell.CastDuration - SpellCaster.BufferWindow;
                var percent = durationUpToWindow / this.spellCaster.InProgressSpell.Spell.CastDuration;
                
                var alongMark = fillRect.Location.ToVector2() + new Vector2(this.boundingRect.Width * percent, 0);
                spriteBatch.DrawLine(alongMark, alongMark + new Vector2(0, fillRect.Height), Color.White, 1f,
                    transform.Depth - 10);
            }
        }
    }
}
