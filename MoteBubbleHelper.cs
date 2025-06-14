using RimWorld;
using UnityEngine;
using Verse;

namespace PirateJargonEvolution
{
    public static class MoteBubbleHelper
    {
        public static void ThrowSpeechBubble(Pawn pawn, string text)
        {
            if (pawn.Spawned)
                MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, text, 360f); // 
        }

        public static void ShowLoadingBubble(Pawn pawn, string loadingText = "...")
        {
            if (pawn.Spawned)
                MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, loadingText, 360f);
        }
        
        public static void ThrowStaticText(Pawn pawn, string text, float durationTicks = 10f)
        {
            var loc = pawn.DrawPos;
            var map = pawn.Map;

            IntVec3 intVec = loc.ToIntVec3();
            if (intVec.InBounds(map))
            {
                MoteText moteText = (MoteText)ThingMaker.MakeThing(ThingDefOf.Mote_Text);
                moteText.exactPosition = loc;
                moteText.text = text;
                moteText.textColor = Color.white;
                if (text == "...")
                {
                    moteText.exactPosition += new Vector3(0.4f, 0, 0);
                    moteText.Scale = 2f;
                    moteText.overrideTimeBeforeStartFadeout = -1f;
                }
                else if (durationTicks >= 0f)
                {
                    moteText.overrideTimeBeforeStartFadeout = durationTicks;
                }

                GenSpawn.Spawn(moteText, intVec, map);
            }
        }

    }
}