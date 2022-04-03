using System.Runtime.Serialization;
using LD50.Data;
using LD50.Gameplay;
using Machina.Components;
using Machina.Data;
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
        private readonly ISpell spell;

        public SpellRenderer(Actor actor, SpellCaster spellCaster, ISpell spell) : base(actor)
        {
            this.boundingRectangle = RequireComponent<BoundingRect>();
            this.spellCaster = spellCaster;
            this.spell = spell;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            var ninepatch = MachinaClient.Assets.GetMachinaAsset<NinepatchSheet>("ui-patch");
            ninepatch.DrawFullNinepatch(spriteBatch, this.boundingRectangle.Rect, NinepatchSheet.GenerationDirection.Inner, transform.Depth);
            
            if (this.spellCaster.InProgressSpell.Spell == this.spell)
            {
                spriteBatch.DrawRectangle(this.boundingRectangle.Rect, Color.Yellow, 10f, transform.Depth - 10);
            }
            
            if (this.spellCaster.BufferedSpell.Spell == this.spell)
            {
                spriteBatch.DrawRectangle(this.boundingRectangle.Rect, Color.LightGreen, 5f, transform.Depth - 15);
            }
            
            var spellsSheet = MachinaClient.Assets.GetMachinaAsset<SpriteSheet>("spells");
            spellsSheet.DrawFrame(spriteBatch, this.spell.FrameIndex, this.boundingRectangle.Rect.Center.ToVector2(), 1f, 0f, XYBool.False, transform.Depth - 10, Color.White, true);

            Cooldown renderedCooldown;

            if (this.spellCaster.GlobalCooldown.RemainingTime() < this.spell.Cooldown.RemainingTime())
            {
                renderedCooldown = this.spell.Cooldown;
            }
            else
            {
                renderedCooldown = this.spellCaster.GlobalCooldown;
            }

            if (!renderedCooldown.IsReady())
            {
                var cooldownRect = this.boundingRectangle.Rect;
                cooldownRect = new Rectangle(cooldownRect.Location,
                    new Point(cooldownRect.Size.X, (int) (cooldownRect.Size.Y * renderedCooldown.Percent())));
                spriteBatch.FillRectangle(cooldownRect, Color.DarkBlue.WithMultipliedOpacity(0.5f), transform.Depth - 20);
            }
        }
    }
}
