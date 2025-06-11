using System.Collections.Generic;
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
        public string pirateFactionId = "";
        public Dictionary<string, string> knownJargon = new Dictionary<string, string>();

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref pirateFactionId, "pirateFactionId", "", false);
            Scribe_Collections.Look(ref knownJargon, "knownJargon", LookMode.Value, LookMode.Value);
        }
        
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            if (parent is Pawn pawn && pawn.Faction != null)
            {
                pirateFactionId = pawn.Faction == Faction.OfPlayer ? "player" : pawn.Faction.def.defName.ToLowerInvariant();
            }
        }
    }
}