using RimWorld;
using Verse;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


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
        
        public static List<JargonEntry> ParseJargonResponse(string rawText)
        {
            var entries = new List<JargonEntry>();

            if (string.IsNullOrWhiteSpace(rawText))
            {
                Log.Warning("[PirateJargon] Empty LLM response.");
                return entries;
            }

            // 提取所有括号包裹的三元组格式：(word, meaning, origin)
            var pattern = @"\(\s*([^,]+?)\s*,\s*([^,]+?)\s*,\s*([^)]+?)\s*\)";
            var matches = Regex.Matches(rawText, pattern);

            foreach (Match match in matches)
            {
                try
                {
                    string word = match.Groups[1].Value.Trim();
                    string meaning = match.Groups[2].Value.Trim();
                    string origin = match.Groups[3].Value.Trim();

                    // Basic sanity check
                    if (word.Length > 1 && meaning.Length > 2)
                    {
                        entries.Add(new JargonEntry(word, meaning, origin));
                        Log.Message($"[PirateJargon] Parsed jargon: {word} -> {meaning}");
                    }
                }
                catch (Exception e)
                {
                    Log.Warning($"[PirateJargon] Failed to parse one entry: {e.Message}");
                }
            }

            if (entries.Count == 0)
            {
                Log.Warning($"[PirateJargon] No valid jargon entries found in:\n{rawText}");
            }

            return entries;
        }
    }
}