using System.Collections.Generic;
using Verse;
using RimWorld;

namespace PirateJargonEvolution
{
    public class PirateFactionMemory: IExposable
    {
        
        public class PirateTaleRecord
        {
            public TaleDef taleDef;
            public string description;
            public List<string> witnessNames;
            public int timestamp; // GenTicks.TicksGame
        }
        
        public string FactionId;
        public string FactionName;
        public string Leader;
        // public string CurrentJargon;
        public List<JargonEntry> JargonEvolutionHistory = new List<JargonEntry>();
        public List<string> Members = new List<string>();
        public List<PirateTaleRecord> PirateTaleHistory = new List<PirateTaleRecord>();
        public string JargonStyle;
        public string OriginStory;

        
        // Default Constructor
        public PirateFactionMemory()
        {
            
        }

        public PirateFactionMemory(string factionId, string factionName, string leader, string jargonStyle, string originStory)
        {
            FactionId = factionId;
            FactionName = factionName;
            Leader = leader;
            JargonStyle = jargonStyle;
            OriginStory = originStory;
            // CurrentJargon = "none";
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref FactionId, "FactionId");
            Scribe_Values.Look(ref FactionName, "FactionName");
            Scribe_Values.Look(ref Leader, "Leader");
            // Scribe_Values.Look(ref CurrentJargon, "CurrentJargon");
            Scribe_Collections.Look(ref JargonEvolutionHistory, "JargonEvolutionHistory", LookMode.Deep); // Deep copy
            Scribe_Collections.Look(ref Members, "Members", LookMode.Value);
            Scribe_Values.Look(ref JargonStyle, "JargonStyle");
            Scribe_Values.Look(ref OriginStory, "OriginStory");
        }

        public List<string> GetJargonListInString()
        {
            List<string> result = new List<string>();
            foreach (JargonEntry entry in JargonEvolutionHistory)
            {
                result.Add(entry.getJargon());
            }

            return result;
        }

        public JargonEntry GetJargonInfo(string jargon)
        {
            foreach (JargonEntry entry in JargonEvolutionHistory)
            {
                if (entry.getJargon() == jargon)
                {
                    return entry;
                }
            }

            return null;
        }
    }
}
