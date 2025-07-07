using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;


namespace PirateJargonEvolution
{
    public class Patch_SpawnSetup
    {
        [HarmonyPatch(typeof(Pawn), nameof(Pawn.SpawnSetup))]
        public static class SpawnSetup_Patch
        {
            public static void Postfix(Pawn __instance)
            {
                if (!__instance.RaceProps.Humanlike || !__instance.Spawned)
                    return;
                
                var comp = __instance.TryGetComp<CompPirateIdentity>();
                if (comp == null || !string.IsNullOrEmpty(comp.pirateFactionId))
                    return;
                
                string factionId = __instance.Faction == Faction.OfPlayer
                    ? "player"
                    : __instance.Faction?.GetUniqueLoadID() ?? "";
                

                var manager = Current.Game.GetComponent<PirateFactionManager>();
                if (manager?.pirateFactions == null || manager.pirateFactions.Count == 0)
                {
                    Log.Warning($"[PirateJargon] SpawnSetup Patch: PirateFactionManager not ready for {__instance.Name}");
                    return;
                }
                if (manager.pirateFactions.TryGetValue(factionId, out var mem))
                {
                    comp.pirateFactionId = factionId;
                    comp.knownJargon = mem.GetJargonListInString();
                    
                    // comp.positionInFaction = "small crew";
                    
                    if (string.IsNullOrEmpty(comp.positionInFaction))
                    {
                        AssignRandomFactionPosition(__instance, factionId);
                    }

                    if (!mem.Members.Contains(__instance))
                        mem.Members.Add(__instance);

                    Log.Message($"[PirateJargon] SpawnSetup Patch: Initialized {__instance.Name} in faction {mem.FactionName} with {comp.knownJargon.Count} jargons.");
                }
                else
                {
                    Log.Warning($"[PirateJargon] SpawnSetup Patch: No memory found for factionId={factionId}, pawn={__instance.Name}");
                }

            }
        }
        
        private static void AssignRandomFactionPosition(Pawn pawn, string factionId)
        {
            var manager = Current.Game.GetComponent<PirateFactionManager>();
            if (!manager.pirateFactions.TryGetValue(factionId, out var mem))
                return;

            var factionPawns = mem.Members.Where(p => p.Spawned && p.TryGetComp<CompPirateIdentity>() != null).ToList();

            // 如果只有一个人，就直接设为 captain
            if (factionPawns.Count == 1)
            {
                var comp = pawn.TryGetComp<CompPirateIdentity>();
                comp.positionInFaction = "captain";
                return;
            }

            string[] roles = { "first mate", "gunner", "deckhand", "small crew" };
            factionPawns = factionPawns.OrderBy(_ => Rand.Value).ToList();

            for (int i = 0; i < factionPawns.Count; i++)
            {
                var p = factionPawns[i];
                var comp = p.TryGetComp<CompPirateIdentity>();
                comp.positionInFaction = roles[Mathf.Min(i * roles.Length / factionPawns.Count, roles.Length - 1)];

                Log.Message($"[PirateJargon] Assigned {comp.positionInFaction} to {p.Name}, factionID: {factionId}");
            }
        }
    }
}