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

        public BattleSystem(Actor actor, Party party, BattleLogger logger) : base(actor)
        {
            this.logger = logger;
            this.party = party;
            CurrentEncounter = new Encounter(logger);
        }

        public void StartNewEncounter(Encounter encounter)
        {
            CurrentEncounter = encounter;
            CurrentEncounter.StartCoroutines(this.actor.scene, this.party);
        }

        public IEnumerator<ICoroutineAction> PrimaryLoopCoroutine(uint randomSeed)
        {
            int level = 0;
            var maker = new MonsterMaker(randomSeed);
            while (true)
            {
                var encounter = maker.CreateEncounter(level, this.logger);
                StartNewEncounter(encounter);
                yield return new WaitUntil(CurrentEncounter.IsFightOver);
                FinishEncounter();
                level++;
                yield return new WaitSeconds(5);
            }
        }
        
        private void FinishEncounter()
        {
            CurrentEncounter = new Encounter(this.logger);
            EncounterEnded?.Invoke();
        }
    }
}
