using Verse;
using Verse.AI;
using System;
using RimWorld;

namespace PirateJargonEvolution
{
    public class CompProperties_Thinking : CompProperties
    {
        public CompProperties_Thinking()
        {
            this.compClass = typeof(CompThinking);
        }
    }
    
    public class CompThinking: ThingComp
    {
        public bool isThinking = false;
        public bool IsThinking => isThinking;
        
        // 放置死循环，不能有三个人一起交流
        public bool IsBusy => isThinking;

        public void startThinking()
        {
            isThinking = true;
        }

        public void stopThinking()
        {
            isThinking = false;
        }

        public override void CompTick()
        {
            base.CompTick();
            if (isThinking && parent is Pawn pawn)
            {
                pawn.jobs.StopAll();
            }
        }
    
    }
}