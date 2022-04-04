using System;
using System.Collections.Generic;
using System.Linq;
using LD50.Data;
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
using Microsoft.Xna.Framework.Graphics;

namespace LD50
{
    internal class Ld50Cartridge : GameCartridge
    {
        private Chat chat;
        private Party party;

        public Ld50Cartridge() : base(new Point(1600, 900), ResizeBehavior.KeepAspectRatio)
        {
        }

        public static SpriteFontMetrics FontMetrics { private set; get; }

        public override void OnGameLoad(GameSpecification specification, MachinaRuntime runtime)
        {
            SceneLayers.BackgroundColor = new Color(20, 16, 19);
            Ld50Cartridge.FontMetrics = MachinaClient.Assets.GetSpriteFont("UIFont");

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
                new SingleTargetSpell("Lesser Heal", 1.5f, 60, 30, EmptyBuff.Create(), 0f, 1, 0),
                new SingleTargetSpell("Greater Heal", 6f, 70, 50, EmptyBuff.Create(), 0f, 2, 1),
                new SingleTargetSpell("Healing Wind", 0.5f, 80, 0, HealOverTimeBuff.Create(6f, 35), 8f, 3, 2),
                new SingleTargetSpell("Power Word: Shield", 0.5f, 70, 0, ShieldBuff.Create(5f, 50), 10f, 4, 3),
                new WholePartySpell("Divine Explosion", 0f, 50, 25, EmptyBuff.Create(), 40f, 5, 4)
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

            LayoutNode[] PartyNodes()
            {
                var result = new List<LayoutNode>();

                foreach (var partyMemberName in partyMemberLayoutNames)
                {
                    result.Add(LayoutNode.Leaf(partyMemberName, LayoutSize.StretchedBoth()));
                }

                return result.ToArray();
            }

            var layout = LayoutNode.HorizontalParent("screen", LayoutSize.Pixels(new Point(1600, 900)),
                new LayoutStyle(new Point(25, 25)),
                LayoutNode.VerticalParent("party", LayoutSize.StretchedBoth(),
                    new LayoutStyle(new Point(15), 15, Alignment.TopLeft),
                    PartyNodes()
                ),
                LayoutNode.VerticalParent("right-half", LayoutSize.StretchedBoth(),
                    new LayoutStyle(new Point(15), 15, Alignment.Center),
                    LayoutNode.Leaf("chat", LayoutSize.StretchedBoth()),
                    LayoutNode.VerticalParent("spells-zone", LayoutSize.StretchedHorizontally(240),
                        new LayoutStyle(Point.Zero, 15, Alignment.BottomCenter),
                        LayoutNode.Leaf("casting-bar", LayoutSize.StretchedBoth()),
                        LayoutNode.HorizontalParent("spell-list", LayoutSize.StretchedHorizontally(150),
                            new LayoutStyle(alignment: Alignment.BottomCenter),
                            SpellNodes())
                    )
                )
            );

            var layoutActors = new LayoutActors(game, layout.Bake());

            var player = new PartyMember(new BaseStats(30, 500, 10, 0, 1), "Player", PartyRole.Healer, PartyPortrait.Healer);

            this.party = new Party(
                new PartyMember(new BaseStats(100, 100, 0, 5, 1), "Terry", PartyRole.Tank),
                new PartyMember(new BaseStats(35, 100, 0, 10, 2), "Miriam", PartyRole.Damage, PartyPortrait.Mage),
                new PartyMember(new BaseStats(50, 100, 0, 5, 1), "Rodney", PartyRole.Damage,  PartyPortrait.Rogue),
                new PartyMember(new BaseStats(80, 100, 0, 7, 0.5f), "Helen", PartyRole.Damage, PartyPortrait.Druid),
                player
            );

            this.chat = new Chat();
            var gameActor = game.AddActor("Game");

            var spellCaster = new SpellCaster(gameActor, this.party, spells, player, new Cooldown(1f), new SpellLogger(this.chat));
            var battleSystem = new BattleSystem(gameActor, this.party, new BattleLogger(this.chat));

            battleSystem.EncounterEnded += this.party.ReviveAnyDeadPartyMembers;

            new BattleRenderer(layoutActors.GetActor("screen"), battleSystem);

            var castingBarActor = layoutActors.GetActor("casting-bar");
            new CastingBarRenderer(castingBarActor, spellCaster);

            var chatActor = layoutActors.GetActor("chat");
            new ChatRenderer(chatActor, this.chat);

            var spellZone = layoutActors.GetActor("spells-zone");
            spellZone.transform.Depth -= 10;
            new NinepatchRenderer(spellZone, MachinaClient.Assets.GetMachinaAsset<NinepatchSheet>("ui-patch"),
                NinepatchSheet.GenerationDirection.Outer);

            var partyMemberIndex = 0;
            foreach (var name in partyMemberLayoutNames)
            {
                var partyMember = this.party.GetMember(partyMemberIndex);
                partyMember.Died += CheckGameOverStatus;
                var partyMemberRoot = layoutActors.GetActor(name);
                PartyMemberInterface.CreateFromActor(partyMemberRoot, partyMember, spellCaster, this.party);
                partyMemberIndex++;
            }

            
            var hoverableSpellTuples = new List<Tuple<Hoverable, ISpell>>();
            
            foreach (var spell in spells)
            {
                var spellRoot = layoutActors.GetActor(spell.Name);
                SpellInterface.CreateFromActor(spellRoot, spellCaster, spell);
                hoverableSpellTuples.Add(new Tuple<Hoverable, ISpell>(layoutActors.GetActor(spell.Name).GetComponent<Hoverable>(), spell));
            }

            new TooltipRenderer(chatActor, hoverableSpellTuples.ToArray());

            game.StartCoroutine(battleSystem.PrimaryLoopCoroutine((uint) Random.Seed));
        }

        private void CheckGameOverStatus(PartyMember member)
        {
            this.chat.AppendColoredString($"{member.Name} has died", Color.Gray);
            var livingMembers = this.party.AllLivingMembers().ToArray();
            if (!livingMembers.Any())
            {
                this.chat.AppendColoredString("The party has wiped. Game Over.", Color.Yellow);
            }
        }

        public override void PrepareDynamicAssets(AssetLoader loader, MachinaRuntime runtime)
        {
            loader.AddMachinaAssetCallback("ui-patch", () =>
            {
                var uiPatchImage = MachinaClient.Assets.GetTexture("ui-patch-sheet");
                return new NinepatchSheet(uiPatchImage, uiPatchImage.Bounds, new Rectangle(4, 4, 32, 32),
                    runtime.Painter);
            });
            
            loader.AddMachinaAssetCallback("tooltip-patch", () =>
            {
                var uiPatchImage = MachinaClient.Assets.GetTexture("tooltip-sheet");
                return new NinepatchSheet(uiPatchImage, uiPatchImage.Bounds, new Rectangle(7, 7, 37, 37),
                    runtime.Painter);
            });
            
            loader.AddMachinaAssetCallback("chat-patch", () =>
            {
                var uiPatchImage = MachinaClient.Assets.GetTexture("chat-patch-sheet");
                return new NinepatchSheet(uiPatchImage, uiPatchImage.Bounds, new Rectangle(4, 4, 32, 32),
                    runtime.Painter);
            });

            loader.AddMachinaAssetCallback("spells", () => new GridBasedSpriteSheet("spells", new Point(149)));
            loader.AddMachinaAssetCallback("roles", () => new GridBasedSpriteSheet("roles", new Point(76)));
            loader.AddMachinaAssetCallback("portraits", () => new GridBasedSpriteSheet("party", new Point(152)));
        }
    }
}
