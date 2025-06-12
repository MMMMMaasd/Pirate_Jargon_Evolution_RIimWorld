using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using HarmonyLib;
using RestSharp;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using PirateJargonEvolution.Utils; 

namespace PirateJargonEvolution
{
    public class Patch_TryInteract
    {
        [HarmonyPatch(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.TryInteractWith))]
        public static class TryInteractWith_Patch
        {
            static bool Prefix(Pawn_InteractionsTracker __instance, Pawn recipient, InteractionDef intDef, ref bool __result)
            {
                Pawn initiator = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();

                if (initiator == null || recipient == null || !PirateJargonEvolution.settings.enableMod)
                {
                    return true;
                }

                if (intDef == InteractionDefOf.Chitchat && ShouldUsePirateJargon(initiator, recipient) && !IsPawnBusy(initiator) && !IsPawnBusy(recipient))
                {
                    __result = false;
                    
                    // 当两个傻逼没死
                    if (!initiator.Dead && !recipient.Dead)
                    {
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
                        Log.Message($"FactionId: {initiator.TryGetComp<CompPirateIdentity>().pirateFactionId}");
                        Log.Message($"Position: {initiator.TryGetComp<CompPirateIdentity>().positionInFaction}");
                        Log.Message("KnownJargon:");
                        foreach (string jargon in initiator.TryGetComp<CompPirateIdentity>().knownJargon)
                        {
                            Log.Message($"{jargon}");
                        }
                        string situation = SharedEventUtil.GetSharedEventDescription(initiator, recipient);
                        PirateJargonDialogueManager.StartDialogue(initiator, recipient, situation);
                    }

                    return false;
                }

                return true;
                
            }
            
            private static bool IsPawnBusy(Pawn p)
            {
                var comp = p.TryGetComp<CompThinking>();
                return comp != null && comp.IsBusy;
            }
            
            static bool ShouldUsePirateJargon(Pawn a, Pawn b)
            {
                var compA = a.TryGetComp<CompPirateIdentity>();
                var compB = b.TryGetComp<CompPirateIdentity>();

                // 检查两个 pawn 都有 pirate 身份且在同一 pirate 派系中
                return compA != null && compB != null &&
                       !string.IsNullOrEmpty(compA.pirateFactionId) &&
                       compA.pirateFactionId == compB.pirateFactionId;
            }
            
            
        }

    }
}