using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PirateJargonEvolution
{
    [HarmonyPatch(typeof(TaleRecorder), nameof(TaleRecorder.RecordTale))]
    public static class Patch_TaleManager_RecordTale
    {
        static void Postfix(TaleDef def, object[] args)
        {
            // 限制只监听我们感兴趣的 Tale 类型
            if (def == TaleDefOf.KilledColonist || def == TaleDefOf.BecameLover)
            {
                List<Pawn> involvedPawns = new List<Pawn>();

                foreach (var arg in args)
                {
                    if (arg is Pawn pawn && pawn != null)
                    {
                        involvedPawns.Add(pawn);
                    }
                }

                if (involvedPawns.Count > 0)
                {
                    PirateJargonEvolutionDriver.HandleTaleCreated(def, involvedPawns);
                }
            }
        }
    }
}