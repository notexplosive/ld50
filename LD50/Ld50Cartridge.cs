using Machina.Engine;
using Machina.Engine.Assets;
using Machina.Engine.Cartridges;
using Microsoft.Xna.Framework;

namespace LD50
{
    internal class Ld50Cartridge : GameCartridge
    {
        public Ld50Cartridge() : base(new Point(1600, 900), ResizeBehavior.KeepAspectRatio)
        {
        }

        public override void OnGameLoad(GameSpecification specification, MachinaRuntime runtime)
        {
            SceneLayers.BackgroundColor = Color.Black;
            var game = SceneLayers.AddNewScene();
        }

        public override void PrepareDynamicAssets(AssetLoader loader, MachinaRuntime runtime)
        {
        }
    }
}
