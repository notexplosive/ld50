using System;
using System.Collections;
using System.Collections.Generic;
using LD50.Gameplay;
using LD50.Renderer;
using Machina.Data.TextRendering;
using Machina.Engine;
using Microsoft.Xna.Framework;

namespace LD50.Data
{
    public interface ISpell
    {
        public int FrameIndex { get; }
        public int Keybind { get; }
        public string Name { get; }
        public float CastDuration { get; }
        public int ManaCost { get; }
        public int HealingAmountWhenComplete { get; }
        public IBuff BuffAppliedWhenComplete { get; }
        public Cooldown Cooldown { get; }
        bool IsInstant => CastDuration == 0f;
        void Execute(PartyMember targetPartyMember, Party party);

        IEnumerable<ITextInputFragment> GenerateDescription()
        {
            var textFont = new SpriteFontMetrics(MachinaClient.Assets.GetSpriteFont("UIFont"));
            var titleFont = new SpriteFontMetrics(MachinaClient.Assets.GetSpriteFont("TitleFont"));
            
            yield return new FormattedTextFragment($"{Name}\n", titleFont, Color.White);

            if (ManaCost > 0)
            {
                yield return new FormattedTextFragment("Costs ", textFont, Color.White);
                yield return new FormattedTextFragment(ManaCost.ToString(), textFont, Color.LightBlue);
                yield return new FormattedTextFragment(" mana\n", textFont, Color.LightBlue);
            }
            else
            {
                yield return new FormattedTextFragment("Free\n", textFont, Color.LightBlue);
            }

            if (CastDuration > 0)
            {
                yield return new FormattedTextFragment("Takes ", textFont, Color.White);
                yield return new FormattedTextFragment($"{CastDuration} seconds", textFont, Color.Yellow);
                yield return new FormattedTextFragment(" to cast\n", textFont, Color.White);                
            }
            else
            {
                yield return new FormattedTextFragment("Instant\n", textFont, Color.Yellow);
            }

            if (HealingAmountWhenComplete > 0)
            {
                var target = "target";

                if (this is WholePartySpell)
                {
                    target = "whole party";
                }
                
                yield return new FormattedTextFragment($"Heals {target} for ", textFont, Color.White);
                yield return new FormattedTextFragment($"{HealingAmountWhenComplete}\n", textFont, Color.LightGreen);
            }

            if (!(BuffAppliedWhenComplete is EmptyBuff))
            {
                if (BuffAppliedWhenComplete is HealOverTimeBuff healBuff)
                {
                    var totalHealAmount =
                        healBuff.GetHealAmount(BuffAppliedWhenComplete.RemainingDuration);
                    yield return new FormattedTextFragment("Heals for ", textFont, Color.White);
                    yield return new FormattedTextFragment($"{totalHealAmount}", textFont, Color.LightGreen);
                    yield return new FormattedTextFragment(" over ", textFont, Color.White);
                }

                if (BuffAppliedWhenComplete is ShieldBuff shieldBuff)
                {
                    yield return new FormattedTextFragment("Absorbs ", textFont, Color.White);
                    yield return new FormattedTextFragment($"{shieldBuff.DamageAbsorb}", textFont, Color.LightGreen);
                    yield return new FormattedTextFragment(" damage, but dissolves after ", textFont, Color.White);
                }

                // duration
                yield return new FormattedTextFragment($"{BuffAppliedWhenComplete.RemainingDuration} seconds\n", textFont, Color.Yellow);
            }
            
            yield return new FormattedTextFragment("To cast: ", textFont, Color.White);
            yield return new FormattedTextFragment(CastInstructions(), textFont, Color.White);
            yield return new FormattedTextFragment($"e", titleFont, Color.Transparent); // invisible text for spacing
        }

        string CastInstructions();

        void OnAttempt()
        {
            AttemptedRecently = true;
        }

        void AcknowledgeAttempt()
        {
            AttemptedRecently = false;
        }

        bool AttemptedRecently { get; set; }
        string SoundEffect { get; }
    }
}
