using RimWorld;
using Verse;
using System.Linq;
using System.Collections.Generic;

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

            List<Pawn> allPawns = PawnsFinder.AllMapsWorldAndTemporary_AliveOrDead.ToList();
            foreach (Pawn pawn in allPawns)
            {
                if (pawn.RaceProps.Humanlike)
                {
                    var comp = pawn.TryGetComp<CompPirateIdentity>();
                    if (comp != null && comp.knownJargon.Count == 0)
                    {
                        var manager = Current.Game.GetComponent<PirateFactionManager>();
                        string factionId = pawn.Faction == Faction.OfPlayer ? "player" : pawn.Faction?.def?.defName.ToLowerInvariant() ?? "";
                        comp.pirateFactionId = factionId;

                        if (manager.pirateFactions.TryGetValue(factionId, out var factionMem))
                        {
                            comp.knownJargon = factionMem.GetJargonListInString();
                        }
                        comp.positionInFaction = "small crew";

                        Log.Message($"[PirateJargon] Initialized CompPirateIdentity for {pawn.Name} with {comp.knownJargon.Count} words.");
                    }
                }
            }
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref initialized, "initialized");
        }
    }
}