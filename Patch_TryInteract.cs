using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using HarmonyLib;
using RestSharp;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using PirateJargonEvolution.Utils;
using UnityEngine;

namespace PirateJargonEvolution
{
    public class Patch_TryInteract
    {
        private static int lastGlobalJargonDialogueTick = 0;
        private const int MinGlobalTicksBetweenDialogues = 6000; // 1 game hour
        
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

                if (intDef != InteractionDefOf.Chitchat)
                {
                    return true;
                }

                if (intDef == InteractionDefOf.Chitchat && ShouldUsePirateJargon(initiator, recipient) && !IsPawnBusy(initiator) && !IsPawnBusy(recipient))
                {
                    int currentTick = Find.TickManager.TicksGame;
                    var compInitiator = initiator.TryGetComp<CompPirateIdentity>();
                    var compRecipient = recipient.TryGetComp<CompPirateIdentity>();
                    
                    if (currentTick - lastGlobalJargonDialogueTick < MinGlobalTicksBetweenDialogues)
                    {
                        return true;
                    }

                    const int IndividualCooldownTicks = 15000;
                    if (currentTick - compInitiator.lastJargonInteractionTick < IndividualCooldownTicks || 
                        currentTick - compRecipient.lastJargonInteractionTick < IndividualCooldownTicks)
                    {
                        return true;
                    }
                    
                    // float useProbability = GetJargonUseProbability(initiator, recipient);
                    // if (Rand.Value > useProbability)
                    //{
                    //    return true;
                    // }
                    
                    if (!HasMeaningfulLearningPotential(initiator, recipient))
                    {
                        return true;
                    }
                    
                    // 目前必然是true
                    string check_situation = SharedEventUtil.GetSharedEventDescription(initiator, recipient);
                    if (string.IsNullOrEmpty(check_situation))
                    {
                        return true;
                    }
                    
                    __result = false;
                    
                    // 当两个人
                    
                    if (!initiator.Dead && !recipient.Dead)
                    {
                        compInitiator.lastJargonInteractionTick = currentTick;
                        compRecipient.lastJargonInteractionTick = currentTick;
                        lastGlobalJargonDialogueTick = currentTick;

                        Log.Message($"Total Pirate Factions: {Current.Game.GetComponent<PirateFactionManager>().pirateFactions.Count}");
                        foreach (var kvp in Current.Game.GetComponent<PirateFactionManager>().pirateFactions)
                        {
                            var mem = kvp.Value;
                            Log.Message($"== {mem.FactionName} ==\nLeader: {mem.Leader}\n");
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
                        // Important here, get the most current event that both the initiator and the recipient
                        // are witnesses in.
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
            
            private static float GetJargonUseProbability(Pawn initiator, Pawn recipient)
            {
                float baseProb = 0.25f; // 基础概率25%

                // 根据社交技能调整
                baseProb += initiator.skills.GetSkill(SkillDefOf.Social).Level * 0.01f;
                baseProb += recipient.skills.GetSkill(SkillDefOf.Social).Level * 0.01f;
                
                // 确保概率在合理范围内
                return Mathf.Clamp(baseProb, 0.1f, 0.75f);
            }
            
            private static bool HasMeaningfulLearningPotential(Pawn initiator, Pawn recipient)
            {
                var initiatorComp = initiator.TryGetComp<CompPirateIdentity>();
                var recipientComp = recipient.TryGetComp<CompPirateIdentity>();
                
                // 检查双方是否有对方不知道的术语
                foreach (string jargon in initiatorComp.knownJargon)
                {
                    if (!recipientComp.knownJargon.Contains(jargon))
                    {
                        return true;
                    }
                }
                
                foreach (string jargon in recipientComp.knownJargon)
                {
                    if (!initiatorComp.knownJargon.Contains(jargon))
                    {
                        return true;
                    }
                }
                
                return false;
            }
        }

    }
}