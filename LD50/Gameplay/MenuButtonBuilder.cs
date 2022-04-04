using System;
using LD50.Renderer;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.Engine.Input;
using Microsoft.Xna.Framework;

namespace LD50.Gameplay
{
    public static class MenuButtonBuilder
    {
        public static void BuildQueueButton(Actor actor, PartyRole role, Action callback)
        {
            var size = actor.GetComponent<BoundingRect>().Size;
            new Hoverable(actor);
            new Clickable(actor).OnClick += (button) =>
            {
                if (button == MouseButton.Left)
                {
                    callback();
                }
            };
            new QueueButtonRenderer(actor, role);

            var text = "Tutorial";

            if (role == PartyRole.Damage || role == PartyRole.Healer)
            {
                var roleName = role == PartyRole.Damage ? "Damage Dealer" : "Healer";

                var iconChild = actor.transform.AddActorAsChild("Icon", new Vector2(size.X / 2f, 0));
                new SpriteRenderer(iconChild,
                    new SpriteFrame(MachinaClient.Assets.GetMachinaAsset<SpriteSheet>("roles"), (int) role));
                text = $"Queue as {roleName}";
            }
            
            var textChild = actor.transform.AddActorAsChild("Text");
            new BoundingRect(textChild, size);
            new BoundedTextRenderer(textChild, text,
                MachinaClient.Assets.GetSpriteFont("TitleFont"),
                Color.White, Alignment.Center);
        }
    }
}
