using HarmonyLib;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace PirateJargonEvolution
{
    [HarmonyPatch(typeof(GameComponentUtility), "StartedNewGame")]
    public static class Patch_GameComponentUtility
    {
        public static void Postfix()
        {
            Current.Game.components.Add(new PirateCompAutoInitializer(Current.Game));

            // Inject Jargon Tab after game starts
            List<Pawn> allPawns = PawnsFinder.AllMapsWorldAndTemporary_AliveOrDead.ToList();
            foreach (var pawn in allPawns)
            {
                if (pawn.def.inspectorTabs != null)
                {
                    var tabNames = pawn.def.inspectorTabs.Select(t => t.Name).ToList();
                    Log.Message($"[Raw Tabs] {pawn.Name?.ToStringShort ?? pawn.LabelCap} ({pawn.def.defName}): {string.Join(", ", tabNames)}");
                }

                if (pawn.def.inspectorTabs != null && !pawn.def.inspectorTabs.Contains(typeof(ITab_Pawn_Jargon)))
                {
                    pawn.def.inspectorTabs.Add(typeof(ITab_Pawn_Jargon));
                    pawn.def.inspectorTabsResolved = null; // Force update
                    Log.Message($"[PirateJargon] Injected ITab_Pawn_Jargon into {pawn.def.defName} during GameComponent patch");
                }
            }
        }
    }
}