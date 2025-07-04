// PirateFactionManager.cs
using System.Collections.Generic;
using RimWorld;
using Verse;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PirateJargonEvolution
{
    public class PirateFactionManager : GameComponent
    {
        public Dictionary<string, PirateFactionMemory> pirateFactions = new Dictionary<string, PirateFactionMemory>();
        public string replacedFactionID;
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
            string[] factionNames =
            {
                "Calico Jack Pirate Group",
                "Blackbeard Pirates",
                "The Red Flag Fleet",
                "Prince Pirates",
                "Redbeard Pirates",
                "Kidd's Pirates"
            };

            Dictionary<string, string> pirateCaptainDict = new Dictionary<string, string>();
            
            pirateCaptainDict.Add("Calico Jack Pirate Group", "Jack Rackham"); // 棉布杰克
            pirateCaptainDict.Add("Blackbeard Pirates", "Blackbeard Edward Teach"); // 黑胡子，Edward Teach
            pirateCaptainDict.Add("The Red Flag Fleet", "Zheng Yi Sao"); // 郑一嫂郑石氏
            pirateCaptainDict.Add("Prince Pirates", "Black Sam"); // Samuel Bellamy, Black Sam, 海盗王子黑萨姆
            pirateCaptainDict.Add("Redbeard Pirates", "Hayreddin Barbarossa"); // 红胡子兄弟
            pirateCaptainDict.Add("Kidd's Pirates", "Captain Kidd"); // 基德船长威廉基德
            
            
            Dictionary<string, JargonEntry[]> pirateJargonDict = new Dictionary<string, JargonEntry[]>();
            
            pirateJargonDict.Add("Calico Jack Pirate Group", new[]
            {
                new JargonEntry("Raise the rag", "Prepare for a quick strike", "Calico Jack's gang would raise a dirty cloth, not a flag, when ready to raid."),
                new JargonEntry("Anne's kiss", "Escape through seduction", "Anne Bonny once flirted with a guard to free a captive—her charm became legend."),
                new JargonEntry("Burn the bottle", "Destroy the evidence", "Used when dumping stolen rum or avoiding naval patrols after a raid.")
            });
            
            pirateJargonDict.Add("Blackbeard Pirates", new[]
            {
                new JargonEntry("Smoke veil", "Prepare for ambush under fog", "Blackbeard lit slow fuses on his beard—his crew called it the smoke veil."),
                new JargonEntry("Choke the moon", "Kill the lanterns before stealth attack", "Sailing under pitch dark was a tactic to mask approach."),
                new JargonEntry("Devil's teeth", "Release full cannon fire", "Refers to the maw of hell, as his ship erupted in smoke and flame.")
            }); 
            
            pirateJargonDict.Add("The Red Flag Fleet", new[]
            {
                new JargonEntry("Dragon wind", "All ships, full speed advance", "Used when the Red Flag Fleet formed an arrowhead assault."),
                new JargonEntry("Silence the pearl", "Prevent secrets from leaking", "Implied executing those who talked to coastal officials."),
                new JargonEntry("Bite the jade", "Enforce death penalty internally", "Borrowed from Chinese idiom meaning martyrdom under law.")
            }); 
                
            pirateJargonDict.Add("Prince Pirates", new[]
            {
                new JargonEntry("Crown's ransom", "Demand full loot", "Used when capturing ships suspected to carry royal goods."),
                new JargonEntry("Dance with iron", "Engage in a duel", "Bartholomew insisted on elegant swordplay before executing traitors."),
                new JargonEntry("Royal hush", "Kill and keep silent", "Implied eliminating captives or witnesses aboard a royal vessel.")
            }); 
            
            pirateJargonDict.Add("Redbeard Pirates", new[]
            {
                new JargonEntry("Sultan's blink", "Ignore the attack order", "Used when choosing diplomacy over cannon fire."),
                new JargonEntry("Chain the strait", "Block the enemy’s escape", "Refers to actual sea chains used at harbor entrances."),
                new JargonEntry("Red crescent", "Mark the target ship", "Placed symbolic flags on ships designated for full plunder.")
            }); 
            
            pirateJargonDict.Add("Kidd's Pirates", new[]
            {
                new JargonEntry("Turn the map", "Switch allegiance", "Kidd's betrayal of the crown made this phrase infamous."),
                new JargonEntry("Ghost flag", "Approach with fake neutrality", "Refers to pretending peaceful intent, then boarding."),
                new JargonEntry("Judge's chain", "Execute by hanging", "A grim reference to Kidd’s own fate at the gallows.")
            }); 
            
            var defNameMap = new Dictionary<string, string>
            {
                { "Calico Jack Pirate Group", "CalicoJackFaction" },
                { "Blackbeard Pirates", "BlackbeardFaction" },
                { "The Red Flag Fleet", "RedFlagFleetFaction" },
                { "Prince Pirates", "PrincePiratesFaction" },
                { "Redbeard Pirates", "RedbeardFaction" },
                { "Kidd's Pirates", "KiddPiratesFaction" }
            };
            
            Dictionary<string, Gender> pirateGenderDict = new Dictionary<string, Gender>();
            pirateGenderDict.Add("Calico Jack Pirate Group", Gender.Male); 
            pirateGenderDict.Add("Blackbeard Pirates", Gender.Male); 
            pirateGenderDict.Add("The Red Flag Fleet", Gender.Female); 
            pirateGenderDict.Add("Prince Pirates", Gender.Male); 
            pirateGenderDict.Add("Redbeard Pirates", Gender.Male); 
            pirateGenderDict.Add("Kidd's Pirates", Gender.Male);
            
            Dictionary<string, string> pirateStyleDict = new Dictionary<string, string>();
            
            pirateStyleDict.Add("Calico Jack Pirate Group", "A frivolous, bohemian Caribbean-style fleet, with hints of sex and rebellion"); // 轻佻，浪荡的加勒比海风格船队, 夹带性暗示和反叛意味
            pirateStyleDict.Add("Blackbeard Pirates", "Gothic horror deterrent wind fleet, gloomy, symbolic intimidation, strong sense of ritual, dark witchcraft"); // 哥特恐怖威慑风船队，阴森、象征恐吓、仪式感强、黑暗巫术感
            pirateStyleDict.Add("The Red Flag Fleet", "The strict oriental fleet, military discipline and oriental metaphors have a strong military structure, and the terminological and figurative language carries an oriental aesthetic meaning."); // 严格的东方船队，军事纪律与东方隐喻，极具军事结构，术语化、比喻型语言带东方美学意味。
            pirateStyleDict.Add("Prince Pirates", "Elegant plundering style, British aristocratic rebel fleet, irony, aristocratic irony, gentleman plundering, royal traitor"); // 优雅掠夺风，英式贵族反叛船队，讽刺、贵族反讽、绅士式掠夺、皇家叛徒
            pirateStyleDict.Add("Redbeard Pirates", "Mediterranean strategist, Ottoman-speaking wind fleet, authorized pirate of the Ottoman Empire, highly organized, often used in diplomatic and coalition operations, language with a strategic sense and Middle Eastern flavor."); // 地中海战略家，奥斯曼帝国语风船队，奥斯曼帝国的授权海盗，组织性强，常用外交和联军行动，语言带有战略感和中东风味。
            pirateStyleDict.Add("Kidd's Pirates", "The Fear of Judgment fleet, dark justice, fear of judgment, blurring the lines between good and evil, and privateering in the gray area"); // 审判恐惧风船队，暗黑正义、审判恐惧、模糊正邪、私掠灰色地带
            
            Dictionary<string, string> pirateStoryDict = new Dictionary<string, string>();
            
    pirateStoryDict.Add("Calico Jack Pirate Group", 
"In 5501, the rogue commander known as Calico Jack defected from the Stellar Navy, escaping with two infamous outlaws—Anne Bonny and Mary Read—aboard a stolen patrol frigate. Establishing the Cotton Jack Pirates on a flooded rimworld, they became a symbol of wild freedom and hedonism, thriving among oceanic archipelagos and derelict sea platforms.");

pirateStoryDict.Add("Blackbeard Pirates", 
"In the early 5500s, Edward 'Blackbeard' Teach, a warlord from a scorched industrial world, gathered a terror fleet of fire-worshipping fanatics and ex-convicts. Masked in smoke and wielding salvaged artillery, they terrorized trade routes and orbital drop zones with psychological warfare and heavy raids.");

pirateStoryDict.Add("The Red Flag Fleet", 
"Led by the battle-hardened admiral Zheng Yisao, the Red Flag Fleet rose from a collapsed Eastern Federation colony ship. Formed of refugees and mercenaries, they hoisted crimson banners and launched fire-raids against Empire outposts, protecting scattered settlements of ocean-born Rimlanders.");

pirateStoryDict.Add("Prince Pirates", 
"Once a noble scion of a fallen glitterworld house, the self-styled 'Prince of Leon' formed his pirate court from exiled officers and synth-silk duelists. With flair and discipline, his crew carves through the southern reefs of the Rim, seeking lost tech, prestige, and revenge on the imperial aristocracy.");

pirateStoryDict.Add("Redbeard Pirates", 
"Descendants of the ancient Redbeard dynasty, these pirates emerged from the wreck of a crashed Ottoman Arkship. Known for their brutal tactics and unmatched navigation through planet-wide storm systems, they raid with fire and blade in honor of their ancestors' solar-spanning legacy.");

pirateStoryDict.Add("Kidd's Pirates", 
"Once branded a pirate by the Empire, Captain Kidd returned from cryo-exile to lead a disciplined brotherhood of fringe-world idealists. They protect scattered outposts from raiders and slavers, sailing modular sea-fortresses armed with long-range coilguns and a code of honor forged in rebellion.");

            int nameIndex = 0;
            int randomNameIndex = 1;
            
            foreach (Faction faction in Find.FactionManager.AllFactionsListForReading)
            {
                // string factionId = faction == Faction.OfPlayer ? "player" : faction.def.defName.ToLowerInvariant();

                string factionId = faction == Faction.OfPlayer
                    ? "player"
                    : faction.GetUniqueLoadID();
                if (faction == Faction.OfPlayer)
                {
                    replacedFactionID = faction.GetUniqueLoadID();
                }
                
                if (!pirateFactions.ContainsKey(factionId))
                {
                    string leaderName;
                    string factionName = "";
                    string originStory = "";
                    string jargonStyle = "";
                    
                    if (nameIndex < 6)
                    {
                        if (faction == Faction.OfPlayer)
                        {
                            factionName = "Red Jacket Pirates";
                            leaderName = faction.leader.Name.ToStringFull;
                            originStory =
                                "Formed in 5502 by deserters from a shattered Imperial naval battalion and rogue privateers, the Red Jacket Pirates are known for their crimson combat coats—relics of a forgotten war. Straddling the line between order and anarchy, they fight with military precision but revel in the chaos of the rim. Operating from drifting sea-fortresses and storm-swept archipelagos, they wage guerrilla warfare against planetary authorities, carving their own bloody vision of freedom into the tides of this lawless";
                            jargonStyle =
                                "The Red Jacket Pirates' jargon is a blend of military terminology and street slang. The language is concise and powerful, full of metaphors and innuendos.";
                        }
                        else
                        {
                            factionName = factionNames[nameIndex];
                            leaderName = pirateCaptainDict[factionName];
                            originStory = pirateStoryDict[factionName];
                            jargonStyle = pirateStyleDict[factionName];
                            // if (defNameMap.TryGetValue(factionName, out string defName))
                            // {
                            // faction.def = DefDatabase<FactionDef>.GetNamed(defName);
                            // }
                            faction.def = FactionDefOf.Pirate;
                            faction.RelationKindWith(Faction.OfPlayer);
                            nameIndex++;
                        }
                    }
                    else
                    {
                        factionName = faction == Faction.OfPlayer ? "Red Jacket Pirates" : $"Small Crew {randomNameIndex++}";
                        leaderName = faction.leader?.Name?.ToStringFull ?? "Unknown";
                        jargonStyle = faction == Faction.OfPlayer ? "The Red Jacket Pirates' jargon is a blend of military terminology and street slang. The language is concise and powerful, full of metaphors and innuendos." : "No Specific Style";
                        originStory = faction == Faction.OfPlayer ? "The Red Jacket Pirates were founded in 1716 by a group of retired navy soldiers and defected privateers. They wore bright red military uniforms, symbolizing revolution and bloody freedom. Unlike traditional pirates, they have strict discipline, but also embrace the chaos and violence at sea. They walk on the edge of the law and guerrilla warfare among the Caribbean islands, becoming synonymous with freedom and resistance." : "A Random Small Crew";
                    }

                    PirateFactionMemory mem = new PirateFactionMemory(factionId, factionName, leaderName, jargonStyle, originStory);

                    if (faction == Faction.OfPlayer)
                    {
                        // mem.CurrentJargon = "Light a match";
                        mem.JargonEvolutionHistory.Add(new JargonEntry("light a match", "Kill this hostage", "During a hijacking of a royal merchant ship, the Red Jacket pirates locked the hostages in the bottom of the cabin. When the captain ordered the execution of the hostages, he said, 'Light a match'. His men immediately understood what he meant and set fire to all the hostages."));
                    }
                    else
                    {
                        // mem.CurrentJargon = initialJargon;
                        if(pirateJargonDict.ContainsKey(factionName))
                        {
                            JargonEntry[] jargonsToAdd = pirateJargonDict[factionName];
                            foreach (JargonEntry entry in jargonsToAdd)
                            {
                                mem.JargonEvolutionHistory.Add(entry);
                            }
                        }
                        // else
                        // {
                            // mem.JargonEvolutionHistory.Add(new JargonEntry("", "", ""));
                        // }
                    }

                    if (pirateCaptainDict.TryGetValue(factionName, out string desiredLeaderName))
                    {
                        if (faction.leader != null && faction.leader.Name is NameTriple)
                        {
                            string[] parts = desiredLeaderName.Split(' ');
                            if (parts.Length >= 2)
                            {
                                string first = parts[0];
                                string nick = parts.Length == 3 ? parts[1] : "";
                                string last = parts.Length == 3 ? parts[2] : parts[1];

                                faction.leader.Name = new NameTriple(first, nick, last);
                            }
                            else
                            {
                                faction.leader.Name = new NameTriple(desiredLeaderName, "", "");
                            }

                            faction.leader.gender = pirateGenderDict[factionName];
                            
                            Log.Message($"[PirateJargon] Set leader of {factionName} to {faction.leader.Name.ToStringFull}");
                        }
                    }
                    pirateFactions[factionId] = mem;
                    faction.Name = factionName;
                    Log.Message($"[PirateJargon] Registered pirate faction: {factionId} as {factionName}");
                    foreach (JargonEntry entry in pirateFactions[factionId].JargonEvolutionHistory)
                    {
                        Log.Message(entry.JargonWord);
                    }
                    Log.Message("----------");
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
        
        public void LogAllPirateFactionsAndMembers()
        {
            foreach (var kv in pirateFactions)
            {
                var mem = kv.Value;
                Log.Message($"Faction: {mem.FactionName} (ID: {kv.Key}) - Leader: {mem.Leader}");

                foreach (var member in mem.Members)
                {
                    var comp = member.TryGetComp<CompPirateIdentity>();
                    string role = comp?.positionInFaction ?? "unknown";
                    Log.Message($"  • {member.Name} ({role})");
                }

                Log.Message("-----------");
            }
        }

    }
}
