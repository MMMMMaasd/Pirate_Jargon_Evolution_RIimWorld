using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PirateJargonEvolution
{
    [HarmonyPatch(typeof(TaleRecorder), nameof(TaleRecorder.RecordTale))]
    public static class Patch_TaleManager_RecordTale
    {
        static readonly HashSet<TaleDef> RelevantTales = new HashSet<TaleDef>
        {
            TaleDefOf.KilledColonist,
            TaleDefOf.KilledChild,
            TaleDefOf.KilledColonyAnimal,
            TaleDefOf.KilledLongRange,
            TaleDefOf.KilledMelee,
            TaleDefOf.KilledMajorThreat,
            TaleDefOf.DefeatedHostileFactionLeader,
            TaleDefOf.ExecutedPrisoner,
            TaleDefOf.Captured,
            TaleDefOf.SoldPrisoner,
            TaleDefOf.KidnappedColonist,
            TaleDefOf.ButcheredHumanlikeCorpse,
            TaleDefOf.AteRawHumanlikeMeat,
            TaleDefOf.BecameLover,
            TaleDefOf.Marriage,
            TaleDefOf.Breakup,
            TaleDefOf.SocialFight,
            TaleDefOf.CraftedArt,
            TaleDefOf.ReadBook,
            TaleDefOf.Hunted,
            TaleDefOf.TamedAnimal,
            TaleDefOf.TrainedAnimal,
            TaleDefOf.BondedWithAnimal,
            TaleDefOf.Recruited,
            TaleDefOf.DidSurgery,
            TaleDefOf.GaveBirth,
            TaleDefOf.HealedMe,
            TaleDefOf.MutatedMyArm,
            TaleDefOf.PerformedPsychicRitual,
            TaleDefOf.StudiedEntity,
            TaleDefOf.UnnaturalDarkness,
            TaleDefOf.ClosedTheVoid,
            TaleDefOf.EmbracedTheVoid,
            TaleDefOf.LandedInPod,
            TaleDefOf.CaravanAmbushedByHumanlike,
            TaleDefOf.CaravanFled,
            TaleDefOf.CaravanAmbushDefeated,
            TaleDefOf.CaravanAssaultSuccessful
        };
        static void Postfix(TaleDef def, object[] args)
        {
            if (!RelevantTales.Contains(def)) return;

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