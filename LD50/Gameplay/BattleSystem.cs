using System;
using System.Collections.Generic;
using Machina.Components;
using Machina.Data;
using Machina.Engine;

namespace LD50.Gameplay
{
    public class BattleSystem : BaseComponent
    {
        private readonly Chat chat;
        private readonly BattleLogger logger;
        private readonly Party party;

        public BattleSystem(Actor actor, Party party, BattleLogger logger, Chat chat) : base(actor)
        {
            this.chat = chat;
            this.logger = logger;
            this.party = party;
            CurrentEncounter = new Encounter(0, logger);
        }

        public Encounter CurrentEncounter { get; private set; }
        public event Action EncounterEnded;

        public void StartNewEncounter(Encounter encounter)
        {
            CurrentEncounter = encounter;
            CurrentEncounter.StartCoroutines(this.actor.scene, this.party, this.chat);
        }

        public IEnumerator<ICoroutineAction> TutorialCoroutine(Scene game, Ld50Cartridge cartridge)
        {
            this.party.EnterCombat();
            yield return game.StartCoroutine(Cinematic.TutorialIntro(this.chat, this.party, this));
            yield return game.StartCoroutine(cartridge.GoBackToMainMenuAfterDelay(game));
        }

        public IEnumerator<ICoroutineAction> PrimaryLoopCoroutine(uint randomSeed)
        {
            var level = 0;
            var maker = new MonsterMaker(randomSeed);
            while (true)
            {
                var encounter = maker.CreateEncounter(level, this.logger);
                yield return new WaitSeconds(5);
                Battlecry();
                yield return new WaitSeconds(1);
                this.party.EnterCombat();
                encounter.PrintStatus(maker);
                StartNewEncounter(encounter);
                yield return new WaitUntil(CurrentEncounter.IsFightOver);
                FinishEncounter();
                level++;
                this.party.LeaveCombat();
                yield return new WaitSeconds(0.5f);
                yield return new WaitUntil(this.party.IsFullyRegenerated);
            }
        }

        private void Battlecry()
        {
            var array = new[]
            {
                "Going in!", "Let's do this!", "Let's go!", "Pulling!", "Charging!", "Attacking!", "Ready?",
                "Here we go!"
            };
            var message = array[MachinaClient.RandomDirty.Next(array.Length)];
            this.chat.PartyMemberSay(this.party.GetMember(0), message);
        }

        public void FinishEncounter()
        {
            CurrentEncounter = new Encounter(0, this.logger);
            this.logger.LogVictory();
            EncounterEnded?.Invoke();
        }
    }
}
