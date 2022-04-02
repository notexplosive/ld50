using LD50.Data;
using LD50.Gameplay;
using Machina.Data;
using Machina.Data.Layout;
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

            var partyMemberLayoutNames = new[]
            {
                "party-member-1",
                "party-member-2",
                "party-member-3",
                "party-member-4",
                "party-member-5"
            };

            var spellLayoutNames = new[]
            {
                "spell-1",
                "spell-2",
                "spell-3",
                "spell-4",
                "spell-5"
            };

            var layout = LayoutNode.VerticalParent("screen", LayoutSize.Pixels(new Point(1600, 900)),
                new LayoutStyle(new Point(25, 25)),
                LayoutNode.Spacer(50),
                LayoutNode.HorizontalParent("party", LayoutSize.StretchedHorizontally(250),
                    new LayoutStyle(new Point(25, 25), 15, Alignment.Center),
                    LayoutNode.Leaf(partyMemberLayoutNames[0], LayoutSize.StretchedBoth()),
                    LayoutNode.Leaf(partyMemberLayoutNames[1], LayoutSize.StretchedBoth()),
                    LayoutNode.Leaf(partyMemberLayoutNames[2], LayoutSize.StretchedBoth()),
                    LayoutNode.Leaf(partyMemberLayoutNames[3], LayoutSize.StretchedBoth()),
                    LayoutNode.Leaf(partyMemberLayoutNames[4], LayoutSize.StretchedBoth())
                ),
                LayoutNode.HorizontalParent("spells", LayoutSize.StretchedBoth(),
                    new LayoutStyle(new Point(25, 25), 15, Alignment.Center),
                    LayoutNode.Leaf(spellLayoutNames[0], LayoutSize.FixedAspectRatio(1, 1)),
                    LayoutNode.Leaf(spellLayoutNames[1], LayoutSize.FixedAspectRatio(1, 1)),
                    LayoutNode.Leaf(spellLayoutNames[2], LayoutSize.FixedAspectRatio(1, 1)),
                    LayoutNode.Leaf(spellLayoutNames[3], LayoutSize.FixedAspectRatio(1, 1)),
                    LayoutNode.Leaf(spellLayoutNames[4], LayoutSize.FixedAspectRatio(1, 1))
                ),
                LayoutNode.Spacer(50)
            );

            var layoutActors = new LayoutActors(game, layout.Bake());

            var party = new Party(
                new PartyMember(new BaseStats(100, 100)),
                new PartyMember(new BaseStats(100, 100)),
                new PartyMember(new BaseStats(100, 100)),
                new PartyMember(new BaseStats(100, 100)),
                new PartyMember(new BaseStats(100, 100)));

            var gameActor = game.AddActor("Game");
            var spellCaster = new SpellCaster(gameActor);
            new BattleSystem(gameActor, party);
            
            int partyMemberIndex = 0;
            foreach (var name in partyMemberLayoutNames)
            {
                var partyMember = party.GetMember(partyMemberIndex);
                partyMember.TakeDamage(10);
                var partyMemberRoot = layoutActors.GetActor(name);
                PartyMemberInterface.CreateFromActor(partyMemberRoot, partyMember, spellCaster);
                partyMemberIndex++;
            }

            foreach (var name in spellLayoutNames)
            {
                var spellRoot = layoutActors.GetActor(name);
                SpellInterface.CreateFromActor(spellRoot, spellCaster);
            }

            
        }

        public override void PrepareDynamicAssets(AssetLoader loader, MachinaRuntime runtime)
        {
        }
    }
}
