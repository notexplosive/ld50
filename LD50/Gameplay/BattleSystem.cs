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
            this.logger.Log("Encounter started!");
            CurrentEncounter = encounter;
            CurrentEncounter.StartCoroutines(this.actor.scene, this.party);
        }

        public IEnumerator<ICoroutineAction> CombatLoopCoroutine()
        {
            while (true)
            {
                var encounter = new Encounter(this.logger,
                    new Monster(100, 10, 2f),
                    new Monster(100, 3, 0.1f));
                StartNewEncounter(encounter);
                yield return new WaitUntil(CurrentEncounter.IsFightOver);
                FinishEncounter();
                yield return new WaitSeconds(5);
            }
        }

        private void FinishEncounter()
        {
            this.logger.Log("Encounter ended!");
            CurrentEncounter = new Encounter(this.logger);
            EncounterEnded?.Invoke();
        }
    }
}
