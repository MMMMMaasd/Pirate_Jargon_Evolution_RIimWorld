using Verse;
using Verse.AI;
using System;
using RimWorld;

namespace PirateJargonEvolution
{
    public class CompThinking: ThingComp
    {
        public bool isThinking = false;
        private int thinkingTicksLeft = 0;
        

        public override void CompTick()
        {
            base.CompTick();
            if (isThinking)
            {
                if (thinkingTicksLeft > 0)
                {
                    thinkingTicksLeft--;

                    if (parent is Pawn pawn && !pawn.Dead && pawn.jobs != null)
                    {
                        pawn.jobs.StopAll();
                    }
                }
                else
                {
                    StopThinking();
                }
            }
        }
        
        public void StartThinking(int durationTicks = 500) // 约8秒 = 60 * 8
        {
            isThinking = true;
            thinkingTicksLeft = durationTicks;
            Log.Message("Start thinking");
        }

        public void StopThinking()
        {
            isThinking = false;
            thinkingTicksLeft = 0;
        }

        public bool IsThinking => isThinking;
    
    }
}