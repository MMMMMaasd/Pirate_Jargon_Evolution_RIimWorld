using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
    
namespace PirateJargonEvolution
{
    public class CompProperties_MoteRepeater : CompProperties
    {
        public CompProperties_MoteRepeater()
        {
            this.compClass = typeof(CompMoteRepeater);
        }

        public float intervalTicks = 60f;
    }
    
    public class CompMoteRepeater : ThingComp
    {
        private int ticksLeft = 0;
        private bool active = false;

        public CompProperties_MoteRepeater Props => (CompProperties_MoteRepeater)this.props;

        public void StartRepeating()
        {
            active = true;
            ticksLeft = 0;
        }

        public void StopRepeating()
        {
            active = false;
        }

        public override void CompTick()
        {
            base.CompTick();

            if (!active || !(parent is Pawn pawn)) return;

            if (ticksLeft <= 0)
            {
                MoteBubbleHelper.ThrowStaticText(pawn, "...");
                ticksLeft = (int)Props.intervalTicks;
            }

            ticksLeft--;
        }
    }
    
}