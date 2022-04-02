using System;
using Machina.Engine;
using Machina.Engine.Assets;
using Machina.Engine.Cartridges;
using Microsoft.Xna.Framework;

namespace LD50
{
    class Ld50Cartridge : GameCartridge
    {
        public Ld50Cartridge() : base(new Point(1600, 900), ResizeBehavior.KeepAspectRatio)
        {
        }

        public override void OnGameLoad(GameSpecification specification, MachinaRuntime runtime)
        {
        }

        public override void PrepareDynamicAssets(AssetLoader loader, MachinaRuntime runtime)
        {
        }
    }
}
