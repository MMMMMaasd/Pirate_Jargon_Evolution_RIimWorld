using System.Collections.Generic;
using Verse;
namespace PirateJargonEvolution
{
    public class PirateFactionMemory: IExposable
    {
        public string FactionId;
        public string FactionName;
        public string Leader;
        public string CurrentJargon;
        public List<JargonEntry> JargonEvolutionHistory = new List<JargonEntry>();
        public List<string> Members = new List<string>();

        public PirateFactionMemory()
        {
            
        }

        public PirateFactionMemory(string factionId, string factionName, string leader)
        {
            FactionId = factionId;
            FactionName = factionName;
            Leader = leader;
            CurrentJargon = "none";
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref FactionId, "FactionId");
            Scribe_Values.Look(ref FactionName, "FactionName");
            Scribe_Values.Look(ref Leader, "Leader");
            Scribe_Values.Look(ref CurrentJargon, "CurrentJargon");
            Scribe_Collections.Look(ref JargonEvolutionHistory, "JargonEvolutionHistory", LookMode.Deep); // Deep copy
            Scribe_Collections.Look(ref Members, "Members", LookMode.Deep);
        }
    }
}