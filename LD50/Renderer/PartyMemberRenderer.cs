using LD50.Gameplay;
using Machina.Components;
using Machina.Data;
using Machina.Data.Layout;
using Machina.Data.TextRendering;
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
        private readonly SpellCaster spellCaster;
        private readonly Hoverable hoverable;
        private readonly NinepatchSheet ninepatch;

        public PartyMemberRenderer(Actor actor, PartyMember partyMember, SpellCaster spellCaster) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.hoverable = RequireComponent<Hoverable>();
            this.spellCaster = spellCaster;
            this.partyMember = partyMember;
            this.ninepatch = MachinaClient.Assets.GetMachinaAsset<NinepatchSheet>("ui-patch");

            var rawLayout = LayoutNode.HorizontalParent("root", LayoutSize.Pixels(this.boundingRect.Size),
                new LayoutStyle(),
                LayoutNode.Leaf("portrait", LayoutSize.Pixels(new Point(this.boundingRect.Size.Y))),
                LayoutNode.Spacer(10),
                LayoutNode.VerticalParent("bars", LayoutSize.StretchedBoth(), new LayoutStyle(padding: 4),
                    LayoutNode.Leaf("name", LayoutSize.StretchedHorizontally(44)),
                    LayoutNode.Leaf("health", LayoutSize.StretchedBoth()),
                    LayoutNode.Leaf("mana", LayoutSize.StretchedBoth()),
                    LayoutNode.Leaf("buffs", LayoutSize.StretchedBoth()),
                    LayoutNode.Spacer(5)
                ),
                LayoutNode.Leaf("role",
                    LayoutSize.Pixels(new Point(this.boundingRect.Size.Y / 2, this.boundingRect.Size.Y)))
            );

            this.layout = rawLayout.Bake();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var root = this.layout.GetNode("root", this.boundingRect.Location);
            var portraitRegion = this.layout.GetNode("portrait", this.boundingRect.Location);
            var health = this.layout.GetNode("health", this.boundingRect.Location);
            var mana = this.layout.GetNode("mana", this.boundingRect.Location);
            var buffsRegion = this.layout.GetNode("buffs", this.boundingRect.Location);
            var nameRegion = this.layout.GetNode("name", this.boundingRect.Location);
            var roleRegion = this.layout.GetNode("role", this.boundingRect.Location);
            
            this.ninepatch.DrawFullNinepatch(spriteBatch, root.Rectangle, NinepatchSheet.GenerationDirection.Outer, transform.Depth + 100);

            var rolesImage = MachinaClient.Assets.GetMachinaAsset<SpriteSheet>("roles");
            rolesImage.DrawFrame(spriteBatch, (int) this.partyMember.Role, roleRegion.Rectangle.Center.ToVector2(), 1f, 0f, XYBool.False, transform.Depth - 10, Color.White, true);
            
            var portraitImage = MachinaClient.Assets.GetMachinaAsset<SpriteSheet>("portraits");
            portraitImage.DrawFrame(spriteBatch, (int)this.partyMember.Portrait, portraitRegion.Rectangle.Center.ToVector2(), 1f, 0f, XYBool.False, transform.Depth - 10, Color.White, true);

            if (this.spellCaster.InProgressSpell.TargetPartyMember == this.partyMember)
            {
                spriteBatch.DrawRectangle(root.Rectangle, Color.Yellow, 5f, transform.Depth - 20);
            }
            
            if (this.hoverable.IsHovered)
            {
                var hoverRectangle = root.Rectangle;
                hoverRectangle.Inflate(10, 10);
                spriteBatch.DrawRectangle(hoverRectangle, Color.LightGreen, 3f, transform.Depth - 20);
            }

            var nameText = new BoundedText(nameRegion.Size, Alignment.Center, Overflow.Ignore,
                new FormattedText(
                    new FormattedTextFragment(this.partyMember.Name, Ld50Cartridge.FontMetrics, Color.White)));

            foreach (var item in nameText.GetRenderedText())
            {
                item.Draw(spriteBatch, nameRegion.PositionRelativeToRoot, 0f, transform.Depth);
            }

            var healthFill = new Rectangle(health.Rectangle.Location,
                new Point((int) (health.Rectangle.Width * this.partyMember.HealthPercent), health.Rectangle.Height));
            spriteBatch.FillRectangle(healthFill, Color.Red, transform.Depth - 5);
            spriteBatch.DrawRectangle(health.Rectangle, Color.Black, 1f, transform.Depth - 10);

            // duplicate code alert!! should have a DrawFilledBar helper method
            var manaFill = new Rectangle(mana.Rectangle.Location,
                new Point((int) (mana.Rectangle.Width * this.partyMember.ManaPercent), mana.Rectangle.Height));
            spriteBatch.FillRectangle(manaFill, Color.Blue, transform.Depth - 5);
            spriteBatch.DrawRectangle(mana.Rectangle, Color.Black, 1f, transform.Depth - 10);

            var buffIndex = 0;
            var buffSize = new Point(buffsRegion.Rectangle.Height);
            foreach (var buff in this.partyMember.Status.Buffs.AllNonEmptyBuffs())
            {
                var buffRectangle =
                    new Rectangle(buffsRegion.PositionRelativeToRoot + new Point(buffSize.X * buffIndex, 0), buffSize);
                spriteBatch.DrawRectangle(buffRectangle, Color.Green, 2f, transform.Depth - 5);
                buffIndex++;
            }
        }
    }
}
