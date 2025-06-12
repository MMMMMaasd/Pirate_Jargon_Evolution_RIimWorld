// PirateFactionManager.cs
using System.Collections.Generic;
using RimWorld;
using Verse;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateJargonEvolution
{
    public class PirateFactionManager : GameComponent
    {
        public Dictionary<string, PirateFactionMemory> pirateFactions = new Dictionary<string, PirateFactionMemory>();

        public PirateFactionManager(Game game) { }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            if (pirateFactions.Count == 0)
            {
                GenerateInitialPirateFactions();
            }
        }

        private void GenerateInitialPirateFactions()
        {
            int nameIndex = 1;
            foreach (Faction faction in Find.FactionManager.AllFactionsListForReading)
            {
                string factionId = faction == Faction.OfPlayer ? "player" : faction.def.defName.ToLowerInvariant();

                if (!pirateFactions.ContainsKey(factionId))
                {
                    var leaderName = faction.leader?.Name?.ToStringFull ?? "Unknown";
                    string pirateName = faction == Faction.OfPlayer ? "Crimson Tide" : $"Skull Crew {nameIndex++}";

                    PirateFactionMemory mem = new PirateFactionMemory(factionId, pirateName, leaderName);

                    if (faction == Faction.OfPlayer)
                    {
                        mem.CurrentJargon = "loot";
                        mem.JargonEvolutionHistory.Add(new JargonEntry("loot", "collect reward", "First used after successful mechanoid defense."));
                    }
                    else
                    {
                        string initialJargon = "plunder";
                        mem.CurrentJargon = initialJargon;
                        mem.JargonEvolutionHistory.Add(new JargonEntry(initialJargon, "raid resources", "Began during coastal village raids."));
                    }

                    pirateFactions[factionId] = mem;
                    faction.Name = pirateName;
                    Log.Message($"[PirateJargon] Registered pirate faction: {factionId} as {pirateName}");
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref pirateFactions, "pirateFactions", LookMode.Value, LookMode.Deep);
        }

        public PirateFactionMemory GetFactionMemory(string id)
        {
            if (pirateFactions.TryGetValue(id, out var mem))
                return mem;
            return null;
        }
    }
}