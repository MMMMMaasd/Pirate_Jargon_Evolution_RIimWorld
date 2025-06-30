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
        private static int lastGlobalJargonSpreadTick = 0;
        private const int MinGlobalTicksBetweenSpreads = 2000; // 1 game hour
        
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
                    bool compIfUseLLM = initiator.TryGetComp<CompPirateIdentity>().useLLMNextInteraction;
                    if (compIfUseLLM)
                    {
                        // 目前必然是true
                        string same_situation = SharedEventUtil.GetSharedEventDescription(initiator, recipient);
                        if (string.IsNullOrEmpty(same_situation))
                        {
                            return true;
                        }

                        initiator.TryGetComp<CompPirateIdentity>().useLLMNextInteraction = false;
                        PirateJargonDialogueManager.StartDialogue(initiator, recipient, same_situation);
                        
                        __result = false;
                        return false;
                    }
                    
                    int currentTick = Find.TickManager.TicksGame;
                    var compInitiator = initiator.TryGetComp<CompPirateIdentity>();
                    var compRecipient = recipient.TryGetComp<CompPirateIdentity>();
                    
                    const int IndividualCooldownTicks = 4000;
                    
                    if (currentTick - lastGlobalJargonSpreadTick < MinGlobalTicksBetweenSpreads ||
                        currentTick - compInitiator.lastJargonInteractionTick < IndividualCooldownTicks ||
                        currentTick - compRecipient.lastJargonInteractionTick < IndividualCooldownTicks)
                        return true;

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
                    
                    
                    SpreadJargonBetween(initiator, recipient);
                    compInitiator.lastJargonInteractionTick = currentTick;
                    compRecipient.lastJargonInteractionTick = currentTick;
                    lastGlobalJargonSpreadTick = currentTick;
                    
                    __result = false;
                    return false;
                    
                }

                return true;
                
            }
            
            static void SpreadJargonBetween(Pawn a, Pawn b)
            {
                var compA = a.TryGetComp<CompPirateIdentity>();
                var compB = b.TryGetComp<CompPirateIdentity>();

                int beforeA = compA.knownJargon.Count;
                int beforeB = compB.knownJargon.Count;

                foreach (string jargon in compA.knownJargon)
                    if (!compB.knownJargon.Contains(jargon))
                        compB.knownJargon.Add(jargon);

                foreach (string jargon in compB.knownJargon)
                    if (!compA.knownJargon.Contains(jargon))
                        compA.knownJargon.Add(jargon);

                int afterA = compA.knownJargon.Count;
                int afterB = compB.knownJargon.Count;

                if (afterA > beforeA || afterB > beforeB)
                {
                    MoteBubbleHelper.ThrowStaticText(a, "Traded slang");
                    MoteBubbleHelper.ThrowStaticText(b, "Traded slang");
                    Log.Message($"[PirateJargon] {a.Name} and {b.Name} spread pirate jargon.");
                }
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