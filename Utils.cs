using RimWorld;
using Verse;
using System.Linq;

namespace PirateJargonEvolution.Utils
{
    public static class SharedEventUtil
    {
        public static string GetSharedEventDescription(Pawn a, Pawn b)
        {
            //var sharedTale = Find.TaleManager.AllTalesListForReading
            //    .Where(t => t.def.usableForArt &&
            //                t.descTargets != null &&
            //                t.descTargets.Any(d => d.Thing == a) &&
            //                t.descTargets.Any(d => d.Thing == b))
            //    .OrderByDescending(t => t.date)
            //    .FirstOrDefault();

            //if (sharedTale != null)
            //{
            //    return $"You and your crewmate both experienced: {sharedTale.def.label}.";
            //}
            
            // return "You and your crewmate are standing under the dim sun, sharing a moment of calm amidst the chaos.";
            return "You and your crewmate just survived from a big black disease";
        }
        
        
        public static PirateFactionMemory GetFaction(string factionId)
        {
            foreach (var kvp in Current.Game.GetComponent<PirateFactionManager>().pirateFactions)
            {
                var mem = kvp.Value;
                if (factionId == mem.FactionId)
                {
                    return mem;
                }
            }

            return null;
        }
    }
}