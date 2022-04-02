using LD50.Gameplay;
using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace LD50.Renderer
{
    public class SpellRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRectangle;
        private readonly SpellCaster spellCaster;

        public SpellRenderer(Actor actor, SpellCaster spellCaster) : base(actor)
        {
            this.boundingRectangle = RequireComponent<BoundingRect>();
            this.spellCaster = spellCaster;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(this.boundingRectangle.Rect, Color.White, 1f, transform.Depth);
        }
    }
}
