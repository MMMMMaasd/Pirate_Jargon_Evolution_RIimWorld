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
            
            // string names = string.Join(", ", involvedPawns.Select(p => p.Name?.ToStringShort ?? "Unknown"));
            // string situation = $"A significant event occurred: {taleDef.label} involving {names}.";
            string situation = GenerateTaleDescription(taleDef, involvedPawns);

            var victim = new Pawn();
            var killer = new Pawn();
            
            if (taleDef.defName == "KilledColonist")
            {
                victim = involvedPawns[1];
                killer = involvedPawns[0];
            }
            // var victim = involvedPawns[1];
            // var killer = involvedPawns[0];
            
            // if (taleDef == TaleDefOf.KilledColonist && involvedPawns.Count >= 2)
            // {
            //  situation = $"A significant event occurred: The pirate {victim.Name.ToStringShort} was killed by {killer.Name.ToStringShort} in a brutal event.";
            // }
            
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
                
                foreach (var w in witnesses)
                {
                    var mote = w.TryGetComp<CompMoteRepeater>();
                    mote.StopRepeating();
                    MoteBubbleHelper.ThrowStaticText(w, "Heard new slang...");
                    foreach (var entry in newEntries)
                    {
                        w.TryGetComp<CompPirateIdentity>().knownJargon.Add(entry.JargonWord);
                        Log.Message($"Jargon word {entry.JargonWord} added to {w.Name}");
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
                }
            }
            
        }
        
        private static string GenerateTaleDescription(TaleDef taleDef, List<Pawn> pawns)
        {
            switch (taleDef.defName)
            {
                case "KilledChild":
                    if (pawns.Count >= 2)
                        return $"{pawns[0].Name.ToStringShort} killed child {pawns[1].Name.ToStringShort}.";
                    break;
                
                case "KilledColonist":
                    if (pawns.Count >= 2)
                        return $"{pawns[0].Name.ToStringShort} killed colonist {pawns[1].Name.ToStringShort}.";
                    break;
                
                case "KilledColonyAnimal":
                    if (pawns.Count >= 2)
                        return $"{pawns[0].Name.ToStringShort} killed animal {pawns[1].Name.ToStringShort}.";
                    break;
                
                case "KilledLongRange":
                    if (pawns.Count >= 2)
                        return $"{pawns[0].Name.ToStringShort} sniped {pawns[1].Name.ToStringShort} from long range.";
                    break;
                
                case "KilledMelee":
                    if (pawns.Count >= 2)
                        return $"{pawns[0].Name.ToStringShort} killed {pawns[1].Name.ToStringShort} with melee.";
                    break;
                
                case "KilledMajorThreat":
                    if (pawns.Count >= 2)
                        return $"{pawns[0].Name.ToStringShort} took down a major threat: {pawns[1].Name.ToStringShort}.";
                    break;
                
                case "DefeatedHostileFactionLeader":
                    if (pawns.Count >= 2)
                        return $"{pawns[0].Name.ToStringShort} defeated a hostile faction leader: {pawns[1].Name.ToStringShort}.";

                    break;

                case "ExecutedPrisoner":
                    if (pawns.Count >= 1)
                        return $"Prisoner {pawns[0].Name.ToStringShort} was executed.";
                    break;

                case "Captured":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} was captured.";
                    break;

                case "SoldPrisoner":
                    if (pawns.Count >= 1)
                        return $"Prisoner {pawns[0].Name.ToStringShort} was sold.";
                    break;

                case "KidnappedColonist":
                    if (pawns.Count >= 1)
                        return $"Colonist {pawns[0].Name.ToStringShort} was kidnapped.";
                    break;

                case "ButcheredHumanlikeCorpse":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} butchered a human corpse.";
                    break;

                case "AteRawHumanlikeMeat":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} ate raw human meat.";
                    break;

                case "BecameLover":
                    if (pawns.Count >= 2)
                        return $"{pawns[0].Name.ToStringShort} and {pawns[1].Name.ToStringShort} became lovers.";
                    break;

                case "Marriage":
                    if (pawns.Count >= 2)
                        return $"{pawns[0].Name.ToStringShort} and {pawns[1].Name.ToStringShort} got married.";
                    break;

                case "Breakup":
                    if (pawns.Count >= 2)
                        return $"{pawns[0].Name.ToStringShort} and {pawns[1].Name.ToStringShort} broke up.";
                    break;

                case "SocialFight":
                    if (pawns.Count >= 2)
                        return $"{pawns[0].Name.ToStringShort} and {pawns[1].Name.ToStringShort} had a fight.";
                    break;

                case "CraftedArt":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} crafted a piece of art.";
                    break;

                case "ReadBook":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} read a book.";
                    break;

                case "Hunted":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} hunted an animal.";
                    break;

                case "TamedAnimal":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} tamed an animal.";
                    break;

                case "TrainedAnimal":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} trained an animal.";
                    break;

                case "BondedWithAnimal":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} bonded with an animal.";
                    break;

                case "Recruited":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} was recruited.";
                    break;

                case "DidSurgery":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} performed a surgery.";
                    break;

                case "GaveBirth":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} gave birth.";
                    break;

                case "HealedMe":
                    if (pawns.Count >= 2)
                        return $"{pawns[0].Name.ToStringShort} healed {pawns[1].Name.ToStringShort}.";
                    break;

                case "MutatedMyArm":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} mutated their arm.";
                    break;

                case "PerformedPsychicRitual":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} performed a psychic ritual.";
                    break;

                case "StudiedEntity":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} studied a strange entity.";
                    break;

                case "UnnaturalDarkness":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} witnessed unnatural darkness.";
                    break;

                case "ClosedTheVoid":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} closed the void.";
                    break;

                case "EmbracedTheVoid":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} embraced the void.";
                    break;

                case "LandedInPod":
                    if (pawns.Count >= 1)
                        return $"{pawns[0].Name.ToStringShort} landed in a pod.";
                    break;

                case "CaravanAmbushedByHumanlike":
                    return $"The caravan was ambushed by humanlike enemies.";
                case "CaravanFled":
                    return $"The caravan fled from danger.";
                case "CaravanAmbushDefeated":
                    return $"The caravan defeated the ambush.";
                case "CaravanAssaultSuccessful":
                    return $"The caravan successfully assaulted the target.";
            }

            return $"An event happened: {taleDef.label}";
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