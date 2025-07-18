using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
namespace PirateJargonEvolution
{
    public class CompProperties_PirateIdentity: CompProperties
    {
        public CompProperties_PirateIdentity()
        {
            this.compClass = typeof(CompPirateIdentity);
        } 
    }

    public class CompPirateIdentity : ThingComp
    {
        // public int lastJargonInteractionTick = -99999;
        public int lastJargonInteractionTick = -99999;
        public string pirateFactionId = "";
        public List<string> knownJargon = new List<string>(); // 只存黑话关键词
        public string positionInFaction = "";
        public bool useLLMNextInteraction = false;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref pirateFactionId, "pirateFactionId", "", false);
            Scribe_Collections.Look(ref knownJargon, "knownJargon",  LookMode.Value);
            Scribe_Values.Look(ref positionInFaction, "positionInFaction", "", false);
            Scribe_Values.Look(ref lastJargonInteractionTick, "lastJargonInteractionTick", -9999, false);
            Scribe_Values.Look(ref useLLMNextInteraction, "useLLMNextInteraction", false);
        }
        
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            if (parent is Pawn pawn && pawn.Faction != null)
            {
                pirateFactionId = pawn.Faction == Faction.OfPlayer ? "player" : pawn.Faction.GetUniqueLoadID();
                var manager = Current.Game.GetComponent<PirateFactionManager>();
                if (manager.pirateFactions.TryGetValue(pirateFactionId, out var factionMemory))
                {
                    knownJargon = factionMemory.GetJargonListInString();
                }

                positionInFaction = "small crew";
            }
        }
    }
}