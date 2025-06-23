using RimWorld;
using Verse;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PirateJargonEvolution
{
    public class PirateCompAutoInitializer : GameComponent
    {
        private bool initialized = false;

        public PirateCompAutoInitializer(Game game) { }

        public override void GameComponentTick()
        {
            if (initialized) return;
            initialized = true;

            var allPawns = PawnsFinder.AllMapsWorldAndTemporary_AliveOrDead.ToList();
            var manager = Current.Game.GetComponent<PirateFactionManager>();
            
            foreach (Pawn pawn in allPawns)
            {
                if (!pawn.RaceProps.Humanlike) continue;

                var comp = pawn.TryGetComp<CompPirateIdentity>();
                if (comp == null) continue;
                
                if (!string.IsNullOrEmpty(comp.pirateFactionId)) continue;

                // string factionId = pawn.Faction == Faction.OfPlayer ? "player" : pawn.Faction?.def?.defName.ToLowerInvariant() ?? "";
                string factionId = pawn.Faction == Faction.OfPlayer
                    ? "player"
                    : pawn.Faction?.GetUniqueLoadID() ?? "";
                
                comp.pirateFactionId = factionId;

                if (manager.pirateFactions.TryGetValue(factionId, out var factionMem))
                {
                    comp.knownJargon = factionMem.GetJargonListInString();
                }

                Log.Message($"[PirateJargon] Initialized CompPirateIdentity for {pawn.Name} with {comp.knownJargon.Count} words.");
            }
            
            AssignFactionPositionsToPawns();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref initialized, "initialized");
        }

        private void AssignFactionPositionsToPawns()
        {
            var manager = Current.Game.GetComponent<PirateFactionManager>();
            var allPawns = PawnsFinder.AllMapsWorldAndTemporary_AliveOrDead
                .Where(p => p.Faction != null && p.RaceProps.Humanlike && p.TryGetComp<CompPirateIdentity>() != null)
                .ToList();

            var groupedByFaction = allPawns.GroupBy(p => p.Faction.GetUniqueLoadID());

            foreach (var group in groupedByFaction)
            {
                string factionId = group.Key;
                var pawns = group.ToList();
                
                foreach (var p in pawns)
                {
                    var c = p.TryGetComp<CompPirateIdentity>();
                    c.positionInFaction = "";
                }

                // Assign Captain
                Pawn captain = pawns
                    .OrderByDescending(p => p.skills?.GetSkill(SkillDefOf.Shooting)?.Level ?? 0)
                    .FirstOrDefault();

                if (captain != null)
                {
                    var comp = captain.TryGetComp<CompPirateIdentity>();
                    comp.positionInFaction = "captain";
                    Log.Message($"[PirateJargon] Assigned captain to {captain.Name}");
                }

                // Assign other roles
                var remaining = pawns.Where(p => p != captain).OrderBy(_ => Rand.Value).ToList();
                string[] roles = { "first mate", "gunner", "deckhand", "small crew" };

                for (int i = 0; i < remaining.Count; i++)
                {
                    var pawn = remaining[i];
                    var comp = pawn.TryGetComp<CompPirateIdentity>();
                    comp.positionInFaction = roles[Mathf.Min(i * roles.Length / remaining.Count, roles.Length - 1)];
                    Log.Message($"[PirateJargon] Assigned {comp.positionInFaction} to {pawn.Name}, factionID: {factionId}");
                }
                
                if (manager.pirateFactions.TryGetValue(factionId, out var factionMem)
                    && factionMem.FactionName == "Calico Jack Pirate Group")
                {
                    var femalePawns = pawns
                        .Where(p => p.gender == Gender.Female)
                        .OrderBy(_ => Rand.Value)
                        .Take(2)
                        .ToList();

                    if (femalePawns.Count > 0)
                    {
                        femalePawns[0].Name = new NameSingle("Anne Bonny", false);
                        femalePawns[0].TryGetComp<CompPirateIdentity>().positionInFaction = "first mate";
                        Log.Message($"[PirateJargon] Renamed {femalePawns[0].LabelShort} to Anne Bonny");
                    }

                    if (femalePawns.Count > 1)
                    {
                        femalePawns[1].Name = new NameSingle("Mary Read", false);
                        femalePawns[1].TryGetComp<CompPirateIdentity>().positionInFaction = "first mate";
                        Log.Message($"[PirateJargon] Renamed {femalePawns[1].LabelShort} to Mary Read");
                    }
                }
            }
        }
    }
}
