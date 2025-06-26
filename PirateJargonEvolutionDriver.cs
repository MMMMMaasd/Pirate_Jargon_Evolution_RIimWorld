using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateJargonEvolution.Utils;
using RimWorld;
using Verse;
using UnityEngine;
namespace PirateJargonEvolution
{
    public class PirateJargonEvolutionDriver
    {
        public static async void HandleTaleCreated(TaleDef taleDef, List<Pawn> involvedPawns)
        { 
            if (taleDef == null || involvedPawns.NullOrEmpty()) return;
            
            //  List<Pawn> involvedPawns = GetAllPawnsFromTale(tale);
            // if (involvedPawns.NullOrEmpty())
            //    return;
            
            // 是否参与pawns都为同一海盗团
            Faction targetFaction = involvedPawns[0].Faction;
            if (involvedPawns.Any(p => p.Faction != targetFaction)) return;
            
            // string factionId = involvedPawns[0].Faction == Faction.OfPlayer ? "player" : involvedPawns[0].Faction.def.defName.ToLowerInvariant();
            
            string factionId = involvedPawns[0].Faction == Faction.OfPlayer ? "player" : involvedPawns[0].Faction.GetUniqueLoadID();
            
            PirateFactionManager manager = Current.Game.GetComponent<PirateFactionManager>();
            if (manager == null || !manager.pirateFactions.ContainsKey(factionId)) return;

            PirateFactionMemory factionMemory = manager.GetFactionMemory(factionId);
            if (factionMemory == null) return;
            
            string names = string.Join(", ", involvedPawns.Select(p => p.Name?.ToStringShort ?? "Unknown"));
            string situation = $"A significant event occurred: {taleDef.label} involving {names}.";
            
            var victim = involvedPawns[1];
            var killer = involvedPawns[0];
            
            if (taleDef == TaleDefOf.KilledColonist && involvedPawns.Count >= 2)
            {
                situation = $"A significant event occurred: The pirate {victim.Name.ToStringShort} was killed by {killer.Name.ToStringShort} in a brutal event.";
            }
            
            // 寻找附近的目击成员
            List<Pawn> witnesses = new List<Pawn>();
            foreach (var pawn in involvedPawns)
            {
                if (pawn.Spawned && pawn.Map != null)
                {
                    witnesses.AddRange(pawn.Map.mapPawns.AllPawnsSpawned
                        .Where(p => p != victim && p.Faction == targetFaction && p.RaceProps.Humanlike && p.Position.InHorDistOf(pawn.Position, 15f)));
                }
            }
            witnesses = witnesses.Distinct().ToList();
            if (witnesses.Count == 0) return;
            
            string witnessNames = string.Join(", ", witnesses.Select(p => p.Name?.ToStringShort ?? "Unknown"));
            factionMemory.PirateTaleHistory.Add(new PirateFactionMemory.PirateTaleRecord
            {
                taleDef = taleDef,
                description = situation,
                timestamp = GenTicks.TicksGame,
                witnessNames = witnesses.Select(p=> p.Name.ToStringShort).ToList()
            });
            
            Log.Message($"[PirateJargon] Tale triggered slang evolution: {situation}");
            Log.Message($"Witnesses: {witnessNames}");
            
            foreach (var w in witnesses)
            {
                var comp = w.TryGetComp<CompThinking>();
                if (comp != null)
                {
                    comp.startThinking();
                }
                
                var mote = w.TryGetComp<CompMoteRepeater>();
                mote.StartRepeating();
            }
            
            string prompt = OllamaPromptGenerator.GenerateJargonEvolutionPromptTwoNPC(factionMemory, situation);
            Log.Message(prompt);
            string response = await OllamaHelper.CallOllamaAsync(prompt);
            Log.Message(response);
            
            if (!string.IsNullOrEmpty(response))
            {
                var newEntries = SharedEventUtil.ParseJargonResponse(response);
                foreach (var entry in newEntries)
                {
                    if (!factionMemory.JargonEvolutionHistory.Any(e => e.getJargon() == entry.getJargon()))
                    {
                        factionMemory.JargonEvolutionHistory.Add(entry);
                        Log.Message($"[PirateJargon] New jargon added: {entry.getJargon()} - {entry.getMeaning()}");
                    }
                }
                
                
                // Add the new jargon to every member's known jargon in the faction
                foreach (var pawn in manager.pirateFactions[factionId].Members
                             .Where(p => p.Spawned && !p.Dead && p.Map != null))
                {
                    Log.Message($"member here: {pawn.Name}");
                    if (witnesses.Contains(pawn))
                    {
                        Log.Message($"Witness here: {pawn.Name}");
                        var mote = pawn.TryGetComp<CompMoteRepeater>();
                        mote.StopRepeating();
                    }
                    MoteBubbleHelper.ThrowStaticText(pawn, "Heard new slang...");
                    foreach (var entry in newEntries)
                    {
                        pawn.TryGetComp<CompPirateIdentity>().knownJargon.Add(entry.JargonWord);
                        Log.Message($"Jargon word {entry.JargonWord} added to {pawn.Name}");
                    }
                }
            }
            else
            {
                foreach (var w in witnesses)
                {
                    var mote = w.TryGetComp<CompMoteRepeater>();
                    mote.StopRepeating();
                }
            }
            
            foreach (var w in witnesses)
            {
                var comp = w.TryGetComp<CompThinking>();
                if (comp != null)
                {
                    comp.stopThinking();
                    Log.Message($"Witness {w.Name} stop thinking");
                }
            }
            
        }
        
        public static List<Pawn> GetAllPawnsFromTale(Tale tale)
        {
            List<Pawn> pawns = new List<Pawn>();
            
            foreach (var field in tale.GetType().GetFields())
            {
                if (field.FieldType == typeof(Pawn))
                {
                    var p = field.GetValue(tale) as Pawn;
                    if (p != null) pawns.Add(p);
                }
                else if (typeof(IEnumerable<Pawn>).IsAssignableFrom(field.FieldType))
                {
                    var list = field.GetValue(tale) as IEnumerable<Pawn>;
                    if (list != null) pawns.AddRange(list);
                }
            }

            return pawns.Distinct().ToList();
        }
        
    }
}