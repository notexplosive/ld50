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
using Machina.ThirdParty;
using Microsoft.Xna.Framework;

namespace LD50
{
    public class Ld50Cartridge : GameCartridge
    {
        private Chat chat;
        private Party party;
        private NoiseBasedRNG random;

        public Ld50Cartridge() : base(new Point(1600, 900), ResizeBehavior.KeepAspectRatio)
        {
        }

        public static SpriteFontMetrics FontMetrics { private set; get; }

        public override void OnGameLoad(GameSpecification specification, MachinaRuntime runtime)
        {
            this.random = new NoiseBasedRNG((uint) Random.Seed);
            Monster.random = new NoiseBasedRNG((uint) Random.Seed);

            SceneLayers.BackgroundColor = new Color(20, 16, 19);
            Ld50Cartridge.FontMetrics = MachinaClient.Assets.GetSpriteFont("UIFont");
            var game = SceneLayers.AddNewScene();
            LoadMenuScene(game);
        }

        private void LoadMenuScene(Scene game)
        {
            game.DeleteAllActors();

            var layout = LayoutNode.HorizontalParent("screen", LayoutSize.Pixels(1600, 900), new LayoutStyle(),
                LayoutNode.StretchedSpacer(),
                LayoutNode.VerticalParent("menu-column", LayoutSize.StretchedVertically(1000),
                    new LayoutStyle(alignment: Alignment.Center),
                    LayoutNode.Leaf("title", LayoutSize.StretchedHorizontally(100)),
                    LayoutNode.Leaf("credits", LayoutSize.StretchedHorizontally(100)),
                    LayoutNode.HorizontalParent("content", LayoutSize.StretchedHorizontally(200),
                        new LayoutStyle(padding: 25),
                        LayoutNode.Leaf("healer-button", LayoutSize.StretchedBoth()),
                        LayoutNode.Leaf("damage-button", LayoutSize.StretchedBoth()),
                        LayoutNode.Leaf("tutorial-button", LayoutSize.StretchedBoth())
                    ),
                    LayoutNode.Spacer(20),
                    LayoutNode.Leaf("status", LayoutSize.StretchedHorizontally(50))
                ),
                LayoutNode.StretchedSpacer()
            );

            var actors = new LayoutActors(game, layout.Bake());

            var uiFont = MachinaClient.Assets.GetSpriteFont("UIFont");
            var titleFont = MachinaClient.Assets.GetSpriteFont("TitleFont");
            var giantFont = MachinaClient.Assets.GetSpriteFont("GiantFont");

            new BoundedTextRenderer(actors.GetActor("title"), "Support Simulator", giantFont, Color.White,
                Alignment.Center);
            new BoundedTextRenderer(actors.GetActor("credits"), "notexplosive.net", uiFont, Color.White,
                Alignment.TopCenter);

            var queueText =
                new BoundedTextRenderer(actors.GetActor("status"), "", uiFont, Color.White, Alignment.Center);
            MenuButtonBuilder.BuildQueueButton(actors.GetActor("damage-button"), PartyRole.Damage,
                () => queueText.Text = "Estimated Queue Time: 1.3 year(s)");
            MenuButtonBuilder.BuildQueueButton(actors.GetActor("healer-button"), PartyRole.Healer, () =>
            {
                queueText.Text = "Estimated Queue Time: 54 picosecond(s)";
                TransitionToGameScene(game, false);
            });
            MenuButtonBuilder.BuildQueueButton(actors.GetActor("tutorial-button"), PartyRole.Tank,
                () => { TransitionToGameScene(game, true); });
        }

        private void TransitionToGameScene(Scene game, bool tutorial)
        {
            IEnumerator<ICoroutineAction> Coroutine()
            {
                var cameraTween = new TweenChain();
                var cameraY = new TweenAccessors<float>(0);
                cameraTween.AppendFloatTween(-900, 0.5f, EaseFuncs.CubicEaseIn, cameraY);

                var ah1 = game.AddActor("adhoc");
                new AdHoc(ah1).onUpdate += (dt) =>
                {
                    cameraTween.Update(dt);
                    game.camera.UnscaledPosition = new Vector2(0, cameraY.CurrentValue);
                };
                yield return new WaitUntil(cameraTween.IsDone);
                ah1.Destroy();
                
                LoadGameScene(game, tutorial);
                
                cameraTween.Refresh();

                cameraTween.AppendCallback(() => cameraY.setter(900));
                cameraTween.AppendFloatTween(0, 0.5f, EaseFuncs.CubicEaseOut, cameraY);

                var ah2 = game.AddActor("adhoc2");
                new AdHoc(ah2).onUpdate += (dt) =>
                {
                    cameraTween.Update(dt);
                    game.camera.UnscaledPosition = new Vector2(0, cameraY.CurrentValue);
                };
                
                yield return new WaitUntil(cameraTween.IsDone);
                ah2.Destroy();
            }
            
            game.StartCoroutine(Coroutine());
        }

        private void LoadGameScene(Scene game, bool tutorial)
        {
            game.DeleteAllActors();
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
                new SingleTargetSpell("Lesser Heal", 1.5f, 60, 30, EmptyBuff.Create(), 0f, 1),
                new SingleTargetSpell("Greater Heal", 6f, 70, 60, EmptyBuff.Create(), 0f, 2, 1, "heal_slow"),
                new SingleTargetSpell("Healing Wind", 0.5f, 80, 0, HealOverTimeBuff.Create(6f, 35), 8f, 3, 2, "heal_over_time"),
                new SingleTargetSpell("Power Word: Shield", 0.5f, 70, 0, ShieldBuff.Create(5f, 200), 15f, 4, 3, "shield"),
                new WholePartySpell("Divine Explosion", 0f, 100, 60, EmptyBuff.Create(), 40f, 5, 4, "heal_slow")
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

            var player = new PartyMember(new BaseStats(30, 500, 10), "Player", PartyRole.Healer, PartyPortrait.Healer, new SoundEffects("ouch", "hurt_1", "final_ouch"));

            if (tutorial)
            {
                var dummySoundEffects = new SoundEffects("ouch", "hurt_1", "hurt_1");
                this.party = new Party(
                    new PartyMember(
                        new BaseStats(80, 100, 0, 5), "Advisor", PartyRole.Tank, PartyPortrait.Advisor,
                        dummySoundEffects),
                    new PartyMember(new BaseStats(30, 100, 0, 0, 2), "Dummy", PartyRole.Damage, PartyPortrait.Dummy,
                        dummySoundEffects),
                    new PartyMember(new BaseStats(35, 100), "Dummier", PartyRole.Damage, PartyPortrait.Dummy,
                        dummySoundEffects),
                    new PartyMember(new BaseStats(40, 100, 0, 0, 0.5f), "Dummest", PartyRole.Damage,
                        PartyPortrait.Dummy, dummySoundEffects),
                    player
                );
            }
            else
            {
                this.party = new Party(
                    new PartyMember(new BaseStats(100, 100, 10, 5), "Terry", PartyRole.Tank, PartyPortrait.Tank,
                        new SoundEffects("uhp", "beatbox2", "startled_cow")),
                    new PartyMember(new BaseStats(35, 40, 30, 20, 2.33f), "Miriam", PartyRole.Damage,
                        PartyPortrait.Mage, new SoundEffects("belch", "fireball", "startled_cow")),
                    new PartyMember(new BaseStats(50, 20, 30, 5, 0.55f), "Rodney", PartyRole.Damage,
                        PartyPortrait.Rogue, new SoundEffects("toa", "toa", "startled_cow")),
                    new PartyMember(new BaseStats(80, 50, 30, 10, 1.1f), "Helen", PartyRole.Damage,
                        PartyPortrait.Druid, new SoundEffects("uhp", "magic_missile", "startled_cow")),
                    player
                );
            }

            this.chat = new Chat();
            var gameActor = game.AddActor("Game");

            var spellCaster =
                new SpellCaster(gameActor, this.party, spells, player, new Cooldown(1f), new SpellLogger(this.chat));
            var battleSystem = new BattleSystem(gameActor, this.party, new BattleLogger(this.chat), this.chat);

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
                partyMember.Died += member =>
                {
                    this.chat.AppendColoredString($"{member.Name} has died", Color.Gray);
                    if (CheckGameOverStatus())
                    {
                        game.StartCoroutine(GoBackToMainMenuAfterDelay(game));
                    }
                };
                var partyMemberRoot = layoutActors.GetActor(name);
                PartyMemberInterface.CreateFromActor(partyMemberRoot, partyMember, spellCaster, this.party);
                partyMemberIndex++;
            }

            var hoverableSpellTuples = new List<Tuple<Hoverable, ISpell>>();

            foreach (var spell in spells)
            {
                var spellRoot = layoutActors.GetActor(spell.Name);
                SpellInterface.CreateFromActor(spellRoot, spellCaster, spell);
                hoverableSpellTuples.Add(
                    new Tuple<Hoverable, ISpell>(layoutActors.GetActor(spell.Name).GetComponent<Hoverable>(), spell));
            }

            new TooltipRenderer(chatActor, hoverableSpellTuples.ToArray());

            if (tutorial)
            {
                game.StartCoroutine(battleSystem.TutorialCoroutine(game, this));
            }
            else
            {
                game.StartCoroutine(battleSystem.PrimaryLoopCoroutine((uint) this.random.Next()));
            }
        }

        public IEnumerator<ICoroutineAction> GoBackToMainMenuAfterDelay(Scene game)
        {
            yield return new WaitSeconds(5);
            LoadMenuScene(game);
        }

        private bool CheckGameOverStatus()
        {
            var livingMembers = this.party.AllLivingMembers().ToArray();
            if (!livingMembers.Any())
            {
                this.chat.AppendColoredString("The party has wiped. Game Over.", Color.Yellow);
                return true;
            }

            return false;
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
