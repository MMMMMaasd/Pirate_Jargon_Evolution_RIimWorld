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
        
        private bool hasInsertedWaitJob = false;
        
        // 防止死循环，不能有三个人一起交流
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
            if (isThinking && parent is Pawn pawn && pawn.jobs != null)
            {
                if (pawn.CurJobDef != JobDefOf.Wait && !hasInsertedWaitJob)
                {
                    Job waitJob = JobMaker.MakeJob(JobDefOf.Wait, 1200); // 150 ticks = 2.5s
                    waitJob.expiryInterval = 1200;
                    waitJob.checkOverrideOnExpire = true;
                    waitJob.playerForced = true;
                    pawn.jobs.StartJob(waitJob, JobCondition.InterruptForced, resumeCurJobAfterwards: false);
                    hasInsertedWaitJob = true;
                }
                else
                {
                    hasInsertedWaitJob = false;
                    // pawn.jobs.StopAll();
                }
            }
        }
    
    }
}