using LD50.Gameplay;
using LD50.Renderer;
using Machina.Components;
using Machina.Data;
using Machina.Data.Layout;
using Machina.Data.TextRendering;
using Machina.Engine;
using Machina.Engine.Assets;
using Machina.Engine.Cartridges;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace LD50
{
    internal class Ld50Cartridge : GameCartridge
    {
        public Ld50Cartridge() : base(new Point(900/2, 1600/2), ResizeBehavior.KeepAspectRatio)
        {
        }

        public override void OnGameLoad(GameSpecification specification, MachinaRuntime runtime)
        {
            SceneLayers.BackgroundColor = Color.Black;
            var game = SceneLayers.AddNewScene();

            var windowSize = new Point(900 / 2, 1600 / 2);

            var deviceLayout = LayoutNode.VerticalParent("device", LayoutSize.Pixels(windowSize),
                LayoutStyle.Empty,
                LayoutNode.Leaf("screen", LayoutSize.StretchedBoth())
                );

            var screenRectangle = deviceLayout.Bake().GetNode("screen").Rectangle;

            var screenActor = game.AddActor("Screen", screenRectangle.Location.ToVector2());

            new BoundingRect(screenActor, screenRectangle.Size);
            new Hoverable(screenActor);
            new BoundedCanvas(screenActor);
            var sceneRenderer =  new SceneRenderer(screenActor);
            
            var background = sceneRenderer.PrimaryScene.AddActor("background");
            background.transform.Depth = Depth.Max;
            new BoundingRect(background, screenRectangle.Size);
            new BoundingRectFill(background, new Color(21, 32, 43));

            var timelineActor = sceneRenderer.PrimaryScene.AddActor("Posts");

            var scrollButton = new ScrollButton(screenActor);
            var posts = new PostTimelineRenderer(timelineActor, screenRectangle.Width);

            scrollButton.ScrolledToNextPage += posts.AddNewPosts;
            scrollButton.ScrolledToNextPage += posts.AnimateScroll;
        }

        public override void PrepareDynamicAssets(AssetLoader loader, MachinaRuntime runtime)
        {
        }
    }
}
