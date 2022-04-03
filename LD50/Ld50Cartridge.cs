using System.Collections.Generic;
using LD50.Data;
using LD50.Gameplay;
using LD50.Renderer;
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

            var spells = new ISpell[]
            {
                new SingleTargetSpell("Fast Heal", 1.5f, 30, 20, EmptyBuff.Create(), 0f),
                new SingleTargetSpell("Slow Heal", 6f, 50, 50, EmptyBuff.Create(), 0f),
                new SingleTargetSpell("Shield", 0.5f, 40, 0, ShieldBuff.Create(5f, 100), 10f),
                new SingleTargetSpell("Heal Over Time", 0.5f, 40, 0, HealOverTimeBuff.Create(6f, 35), 8f),
                new WholePartySpell("AoE Heal", 0f, 50, 25, EmptyBuff.Create(), 40f)
                // new SingleTargetSpell("Clear Debuff"),
                // new SingleTargetSpell("Revive")
            };

            LayoutNode[] SpellNodes()
            {
                var result = new List<LayoutNode>();
                foreach (var spell in spells)
                {
                    result.Add(LayoutNode.Leaf(spell.Name, LayoutSize.FixedAspectRatio(1, 1)));
                }

                return result.ToArray();
            }

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
                LayoutNode.Leaf("casting-bar", LayoutSize.StretchedHorizontally(25)),
                LayoutNode.HorizontalParent("spells", LayoutSize.StretchedBoth(),
                    new LayoutStyle(new Point(25, 25), 15, Alignment.Center),
                    SpellNodes()
                ),
                LayoutNode.Spacer(50)
            );

            var layoutActors = new LayoutActors(game, layout.Bake());

            var player = new PartyMember(new BaseStats(100, 100, 5), PartyRole.Healer);

            var party = new Party(
                new PartyMember(new BaseStats(100, 100), PartyRole.Tank),
                new PartyMember(new BaseStats(100, 100), PartyRole.Damage),
                new PartyMember(new BaseStats(100, 100), PartyRole.Damage),
                new PartyMember(new BaseStats(100, 100), PartyRole.Damage),
                player
            );

            var gameActor = game.AddActor("Game");

            var spellCaster = new SpellCaster(gameActor, party, spells, player, new Cooldown(1f));
            new BattleSystem(gameActor, party);

            var castingBarActor = layoutActors.GetActor("casting-bar");
            new CastingBarRenderer(castingBarActor, spellCaster);

            var partyMemberIndex = 0;
            foreach (var name in partyMemberLayoutNames)
            {
                var partyMember = party.GetMember(partyMemberIndex);
                partyMember.TakeDamage(10);
                var partyMemberRoot = layoutActors.GetActor(name);
                PartyMemberInterface.CreateFromActor(partyMemberRoot, partyMember, spellCaster);
                partyMemberIndex++;
            }

            foreach (var spell in spells)
            {
                var spellRoot = layoutActors.GetActor(spell.Name);
                SpellInterface.CreateFromActor(spellRoot, spellCaster, spell);
            }
        }

        public override void PrepareDynamicAssets(AssetLoader loader, MachinaRuntime runtime)
        {
        }
    }
}
