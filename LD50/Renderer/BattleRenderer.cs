using LD50.Gameplay;
using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace LD50.Renderer
{
    public class BattleRenderer : BaseComponent
    {
        private readonly BattleSystem battleSystem;
        private readonly BoundingRect boundingRect;

        public BattleRenderer(Actor actor, BattleSystem battleSystem) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.battleSystem = battleSystem;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!this.battleSystem.CurrentEncounter.IsFightOver())
            {
                spriteBatch.DrawRectangle(this.boundingRect.Rect, Color.OrangeRed, 5f);
            }
        }
    }
}
