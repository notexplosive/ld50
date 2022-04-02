using LD50.Data;
using LD50.Gameplay;
using Machina.Components;
using Machina.Data;
using Machina.Data.Layout;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace LD50.Renderer
{
    public class PartyMemberRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly BakedLayout layout;
        private readonly PartyMember partyMember;

        public PartyMemberRenderer(Actor actor, PartyMember partyMember) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.partyMember = partyMember;

            var rawLayout = LayoutNode.HorizontalParent("root", LayoutSize.Pixels(this.boundingRect.Size),
                new LayoutStyle(alignment: Alignment.Center),
                LayoutNode.OneOffParent("portrait-container", LayoutSize.StretchedVertically(80),
                    new LayoutStyle(alignment: Alignment.Center),
                    LayoutNode.Leaf("portrait", LayoutSize.FixedAspectRatio(1, 1))),
                LayoutNode.VerticalParent("bars", LayoutSize.FixedAspectRatio(2, 1), LayoutStyle.Empty,
                    LayoutNode.Leaf("health", LayoutSize.StretchedBoth()),
                    LayoutNode.Leaf("mana", LayoutSize.StretchedBoth()),
                    LayoutNode.Leaf("buffs", LayoutSize.StretchedBoth())
                    )
            );

            this.layout = rawLayout.Bake();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var root = this.layout.GetNode("root", this.boundingRect.Location);
            var portrait = this.layout.GetNode("portrait", this.boundingRect.Location);
            var health = this.layout.GetNode("health", this.boundingRect.Location);
            var mana = this.layout.GetNode("mana", this.boundingRect.Location);
            var buffsRegion = this.layout.GetNode("buffs", this.boundingRect.Location);

            var outlineColor = Color.White;

            if (this.partyMember.Status.IsDead)
            {
                outlineColor = Color.DarkRed;
            }
            
            spriteBatch.DrawRectangle(root.Rectangle, outlineColor, 1f, transform.Depth);
            spriteBatch.DrawRectangle(portrait.Rectangle, outlineColor, 1f, transform.Depth);

            var healthFill = new Rectangle(health.Rectangle.Location,
                new Point((int) (health.Rectangle.Width * this.partyMember.HealthPercent), health.Rectangle.Height));
            spriteBatch.FillRectangle(healthFill, Color.Red, transform.Depth - 5);
            spriteBatch.DrawRectangle(health.Rectangle, outlineColor, 1f, transform.Depth - 10);

            spriteBatch.DrawRectangle(mana.Rectangle, Color.Blue, 1f, transform.Depth - 5);

            int buffIndex = 0;
            var buffSize = new Point(buffsRegion.Rectangle.Height);
            foreach (var buff in this.partyMember.Status.Buffs.AllNonEmptyBuffs())
            {
                var buffRectangle = new Rectangle(buffsRegion.PositionRelativeToRoot + new Point(buffSize.X * buffIndex,0), buffSize);
                spriteBatch.DrawRectangle(buffRectangle, Color.Green, 2f, transform.Depth - 5);
                buffIndex++;
            }
        }
    }
}
