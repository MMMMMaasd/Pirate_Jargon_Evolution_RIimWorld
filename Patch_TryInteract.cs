using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using HarmonyLib;
using RestSharp;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace PirateJargonEvolution
{
    public class Patch_TryInteract
    {
        [HarmonyPatch(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.TryInteractWith))]
        public static class TryInteractWith_Patch
        {
            public static void Prefix(Pawn_InteractionsTracker __instance, Pawn recipient, InteractionDef intDef)
            {
                if (!PirateJargonEvolution.settings.enableMod) return;
                if (__instance == null || recipient == null || intDef == null) return;

                Pawn initiator = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
                if (initiator == null || recipient == null)
                {
                    Log.Message("Cannot find initiator");
                    return;
                }

                if (!initiator.Spawned || !recipient.Spawned) return;

                initiator.jobs.StopAll();
                recipient.jobs.StopAll();
                initiator.TryGetComp<CompThinking>()?.StartThinking();
                recipient.TryGetComp<CompThinking>()?.StartThinking();
                
                string placeholder = "(thinking...)";
                
                Log.Message($"Total Pirate Factions: {Current.Game.GetComponent<PirateFactionManager>().pirateFactions.Count}");
                foreach (var kvp in Current.Game.GetComponent<PirateFactionManager>().pirateFactions)
                {
                    var mem = kvp.Value;
                    Log.Message($"== {mem.FactionName} ==\nLeader: {mem.Leader}\nCurrent Jargon: {mem.CurrentJargon}");
                    foreach (var entry in mem.JargonEvolutionHistory)
                    {
                        Log.Message($"  • {entry.JargonWord} = {entry.Meaning} ({entry.OriginStory})");
                    }
                }


                if (InteractionUtility.IsGoodPositionForInteraction(initiator.Position, recipient.Position, initiator.Map))
                {
                    MoteMaker.ThrowText(initiator.DrawPos, initiator.Map, placeholder);
                }
                
            }
        }

    }
}