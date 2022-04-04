using System;
using System.Collections.Generic;
using Machina.Components;
using Machina.Data;
using Machina.Engine;

namespace LD50.Gameplay
{
    public class BattleSystem : BaseComponent
    {
        public event Action EncounterEnded;
        public Encounter CurrentEncounter { get; private set; }
        private readonly Party party;
        private readonly BattleLogger logger;
        private readonly Chat chat;

        public BattleSystem(Actor actor, Party party, BattleLogger logger, Chat chat) : base(actor)
        {
            this.chat = chat;
            this.logger = logger;
            this.party = party;
            CurrentEncounter = new Encounter(logger);
        }

        public void StartNewEncounter(Encounter encounter)
        {
            CurrentEncounter = encounter;
            CurrentEncounter.StartCoroutines(this.actor.scene, this.party);
        }

        public IEnumerator<ICoroutineAction> TutorialCoroutine(Scene game, Ld50Cartridge cartridge)
        {
            var maker = new MonsterMaker(1532);
            
            this.party.EnterCombat();
            yield return game.StartCoroutine(Cinematic.TutorialIntro(this.chat, this.party, this));
            
            /*
            while (true)
            {
                
                var encounter = maker.CreateEncounter(level, this.logger);
                StartNewEncounter(encounter);
                yield return new WaitUntil(CurrentEncounter.IsFightOver);
                FinishEncounter();
                level++;
                this.party.LeaveCombat();
                yield return new WaitSeconds(0.5f);
                yield return new WaitUntil(this.party.IsFullyRegenerated);
                yield return new WaitSeconds(5);
            }
            */

            yield return game.StartCoroutine(cartridge.GoBackToMainMenuAfterDelay(game));
        }
        
        public IEnumerator<ICoroutineAction> PrimaryLoopCoroutine(uint randomSeed)
        {
            int level = 0;
            var maker = new MonsterMaker(randomSeed);
            while (true)
            {
                this.party.EnterCombat();
                var encounter = maker.CreateEncounter(level, this.logger);
                StartNewEncounter(encounter);
                yield return new WaitUntil(CurrentEncounter.IsFightOver);
                FinishEncounter();
                level++;
                this.party.LeaveCombat();
                yield return new WaitSeconds(0.5f);
                yield return new WaitUntil(this.party.IsFullyRegenerated);
                yield return new WaitSeconds(5);
            }
        }
        
        public void FinishEncounter()
        {
            CurrentEncounter = new Encounter(this.logger);
            this.logger.LogVictory();
            EncounterEnded?.Invoke();
        }
    }
}
