using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace PirateJargonEvolution
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    public static class Patch_FloatMenuAddLLMOption
    {
        static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            // 只对玩家控制的海盗派系成员开放
            var comp = pawn.TryGetComp<CompPirateIdentity>();
            var identity = pawn.TryGetComp<CompPirateIdentity>();
            if (comp == null || identity == null || pawn.Faction != Faction.OfPlayer) return;

            opts.Add(new FloatMenuOption("Next Interaction LLM", () =>
            {
                comp.useLLMNextInteraction = !comp.useLLMNextInteraction;
                if (comp.useLLMNextInteraction)
                {
                    Messages.Message($"{pawn.Name} will use LLM in next pirate chitchat.", MessageTypeDefOf.PositiveEvent);
                }
                else
                {
                    Messages.Message($"{pawn.Name} stop use LLM.", MessageTypeDefOf.PositiveEvent);
                }
            }));
        }
    }
}