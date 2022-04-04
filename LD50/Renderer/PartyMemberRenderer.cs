using System;
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
        private readonly Hoverable hoverable;
        private readonly BakedLayout layout;
        private readonly NinepatchSheet ninepatch;
        private readonly PartyMember partyMember;
        private readonly SpellCaster spellCaster;
        private int damageAbsorbedThisFrame;
        private int damageThisFrame;
        private int healthThisFrame;
        private float timer;
        private float shakeFrames;

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

        public override void Update(float dt)
        {
            this.healthThisFrame += this.partyMember.Status.GetHealingTakenThisFrame(dt, this.partyMember.PendingHeals);
            this.damageThisFrame += this.partyMember.Status.GetDamageTakenThisFrame(dt, this.partyMember.PendingDamage);
            this.damageAbsorbedThisFrame +=
                this.partyMember.Status.GetAbsorbedDamageThisFrame(dt, this.partyMember.PendingDamage);

            this.timer -= dt;

            if (this.timer < 0)
            {
                if (this.healthThisFrame > 0)
                {
                    SpawnTextPopup(Color.LightGreen, this.healthThisFrame.ToString());
                }

                if (this.damageThisFrame > 0)
                {
                    SpawnTextPopup(Color.IndianRed, this.damageThisFrame.ToString());
                }

                if (this.damageAbsorbedThisFrame > 0)
                {
                    SpawnTextPopup(Color.Yellow, $"({this.damageAbsorbedThisFrame})");
                }

                this.shakeFrames = Math.Min(this.damageThisFrame / 10f, 1f);
                
                this.timer = 0.25f;
                this.healthThisFrame = 0;
                this.damageThisFrame = 0;
                this.damageAbsorbedThisFrame = 0;
            }

            if (this.shakeFrames > 0)
            {
                this.shakeFrames -= dt;
            }
        }

        private void SpawnTextPopup(Color color, string value)
        {
            var portraitRegion = this.layout.GetNode("portrait", this.boundingRect.Location);
            var rect = portraitRegion.Rectangle;
            var position = rect.Location.ToVector2() + new Vector2(rect.Width / 2f, rect.Height / 2f);
            position += new Vector2(MachinaClient.RandomDirty.NextFloat() - 0.5f,
                MachinaClient.RandomDirty.NextFloat() - 0.5f) * 10;

            var numberActor = this.actor.scene.AddActor("Number", position);
            numberActor.transform.Depth = transform.Depth - 1000;
            new BoundingRect(numberActor, new Point(100, 100)).SetOffsetToCenter();
            new BoundedTextRenderer(numberActor, value, MachinaClient.Assets.GetSpriteFont("TitleFont"), color,
                Alignment.Center, Overflow.Ignore).EnableDropShadow(Color.Black);
            new AscendAndFadeOutText(numberActor);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var shakeOffset = Vector2.Zero;
            if (this.shakeFrames > 0)
            {
                shakeOffset = new Vector2(MachinaClient.RandomDirty.NextFloat() - 0.5f,
                    MachinaClient.RandomDirty.NextFloat() - 0.5f) * 10;
            }
            
            var location = (this.boundingRect.Location.ToVector2() + shakeOffset).ToPoint();
            
            var root = this.layout.GetNode("root", location);
            var portraitRegion = this.layout.GetNode("portrait", location);
            var health = this.layout.GetNode("health", location);
            var mana = this.layout.GetNode("mana", location);
            var buffsRegion = this.layout.GetNode("buffs", location);
            var nameRegion = this.layout.GetNode("name", location);
            var roleRegion = this.layout.GetNode("role", location);

            this.ninepatch.DrawFullNinepatch(spriteBatch, root.Rectangle, NinepatchSheet.GenerationDirection.Outer,
                transform.Depth + 100);

            var rolesImage = MachinaClient.Assets.GetMachinaAsset<SpriteSheet>("roles");
            rolesImage.DrawFrame(spriteBatch, (int) this.partyMember.Role, roleRegion.Rectangle.Center.ToVector2(), 1f,
                0f, XYBool.False, transform.Depth - 10, Color.White);

            var portraitImage = MachinaClient.Assets.GetMachinaAsset<SpriteSheet>("portraits");
            portraitImage.DrawFrame(spriteBatch, (int) this.partyMember.Portrait,
                portraitRegion.Rectangle.Center.ToVector2(), 1f, 0f, XYBool.False, transform.Depth - 10, Color.White);

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
            spriteBatch.FillRectangle(healthFill, new Color(115, 23, 45), transform.Depth - 5);
            spriteBatch.DrawRectangle(health.Rectangle, Color.Black, 2f, transform.Depth - 10);

            // duplicate code alert!! should have a DrawFilledBar helper method
            var manaFill = new Rectangle(mana.Rectangle.Location,
                new Point((int) (mana.Rectangle.Width * this.partyMember.ManaPercent), mana.Rectangle.Height));
            spriteBatch.FillRectangle(manaFill, new Color(40, 92, 196), transform.Depth - 5);
            spriteBatch.DrawRectangle(mana.Rectangle, Color.Black, 2f, transform.Depth - 10);

            var spellImage = MachinaClient.Assets.GetMachinaAsset<SpriteSheet>("spells");
            
            
            var buffIndex = 0;
            var buffSize = new Point(buffsRegion.Rectangle.Height);
            foreach (var buff in this.partyMember.Status.Buffs.AllNonEmptyBuffs())
            {
                var buffRectangle =
                    new Rectangle(buffsRegion.PositionRelativeToRoot + new Point(buffSize.X * buffIndex, 0), buffSize);


                var scale = (float) buffRectangle.Width / spellImage.GetSourceRectForFrame(0).Width;
                
                spellImage.DrawFrame(spriteBatch, (int) buff.FrameIndex,
                    buffRectangle.Center.ToVector2(), scale, 0f, XYBool.False, transform.Depth - 10, Color.White);
                
                
                spriteBatch.DrawRectangle(buffRectangle, Color.Green, 2f, transform.Depth - 5);
                buffIndex++;
            }
        }
    }
}
