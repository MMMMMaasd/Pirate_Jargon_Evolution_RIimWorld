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
            }); // 轻佻，浪荡的加勒比海风格船队, 夹带性暗示和反叛意味
            
            pirateJargonDict.Add("Blackbeard Pirates", new[]
            {
                new JargonEntry("Smoke veil", "Prepare for ambush under fog", "Blackbeard lit slow fuses on his beard—his crew called it the smoke veil."),
                new JargonEntry("Choke the moon", "Kill the lanterns before stealth attack", "Sailing under pitch dark was a tactic to mask approach."),
                new JargonEntry("Devil's teeth", "Release full cannon fire", "Refers to the maw of hell, as his ship erupted in smoke and flame.")
            }); // 哥特恐怖威慑风船队，阴森、象征恐吓、仪式感强、黑暗巫术感
            
            pirateJargonDict.Add("The Red Flag Fleet", new[]
            {
                new JargonEntry("Dragon wind", "All ships, full speed advance", "Used when the Red Flag Fleet formed an arrowhead assault."),
                new JargonEntry("Silence the pearl", "Prevent secrets from leaking", "Implied executing those who talked to coastal officials."),
                new JargonEntry("Bite the jade", "Enforce death penalty internally", "Borrowed from Chinese idiom meaning martyrdom under law.")
            }); // 严格的东方船队，军事纪律与东方隐喻，极具军事结构，术语化、比喻型语言带东方美学意味。
                
            pirateJargonDict.Add("Prince Pirates", new[]
            {
                new JargonEntry("Crown's ransom", "Demand full loot", "Used when capturing ships suspected to carry royal goods."),
                new JargonEntry("Dance with iron", "Engage in a duel", "Bartholomew insisted on elegant swordplay before executing traitors."),
                new JargonEntry("Royal hush", "Kill and keep silent", "Implied eliminating captives or witnesses aboard a royal vessel.")
            }); // 优雅掠夺风，英式贵族反叛船队，讽刺、贵族反讽、绅士式掠夺、皇家叛徒
            
            pirateJargonDict.Add("Redbeard Pirates", new[]
            {
                new JargonEntry("Sultan's blink", "Ignore the attack order", "Used when choosing diplomacy over cannon fire."),
                new JargonEntry("Chain the strait", "Block the enemy’s escape", "Refers to actual sea chains used at harbor entrances."),
                new JargonEntry("Red crescent", "Mark the target ship", "Placed symbolic flags on ships designated for full plunder.")
            }); // 地中海战略家，奥斯曼帝国语风船队，奥斯曼帝国的授权海盗，组织性强，常用外交和联军行动，语言带有战略感和中东风味。
            
            pirateJargonDict.Add("Kidd's Pirates", new[]
            {
                new JargonEntry("Turn the map", "Switch allegiance", "Kidd's betrayal of the crown made this phrase infamous."),
                new JargonEntry("Ghost flag", "Approach with fake neutrality", "Refers to pretending peaceful intent, then boarding."),
                new JargonEntry("Judge's chain", "Execute by hanging", "A grim reference to Kidd’s own fate at the gallows.")
            }); // 审判恐惧风船队，暗黑正义、审判恐惧、模糊正邪、私掠灰色地带
            
            var defNameMap = new Dictionary<string, string>
            {
                { "Calico Jack Pirate Group", "CalicoJackFaction" },
                { "Blackbeard Pirates", "BlackbeardFaction" },
                { "The Red Flag Fleet", "RedFlagFleetFaction" },
                { "Prince Pirates", "PrincePiratesFaction" },
                { "Redbeard Pirates", "RedbeardFaction" },
                { "Kidd's Pirates", "KiddPiratesFaction" }
            };
            

            int nameIndex = 0;
            int randomNameIndex = 1;
            
            foreach (Faction faction in Find.FactionManager.AllFactionsListForReading)
            {
                string factionId = faction == Faction.OfPlayer ? "player" : faction.def.defName.ToLowerInvariant();

                if (!pirateFactions.ContainsKey(factionId))
                {
                    string leaderName;
                    string factionName = "";
                    if (nameIndex < 6)
                    {
                        if (faction == Faction.OfPlayer)
                        {
                            factionName = "Red Jacket Pirates";
                            leaderName = faction.leader.Name.ToStringFull;
                        }
                        else
                        {
                            factionName = factionNames[nameIndex];
                            leaderName = pirateCaptainDict[factionName];
                            if (defNameMap.TryGetValue(factionName, out string defName))
                            {
                                faction.def = DefDatabase<FactionDef>.GetNamed(defName);
                            }
                            //faction.def = FactionDefOf.Pirate;
                            //faction.RelationKindWith(Faction.OfPlayer);
                            nameIndex++;
                        }
                    }
                    else
                    {
                        factionName = faction == Faction.OfPlayer ? "Red Jacket Pirates" : $"Small Crew {randomNameIndex++}";
                        leaderName = faction.leader?.Name?.ToStringFull ?? "Unknown";
                    }

                    PirateFactionMemory mem = new PirateFactionMemory(factionId, factionName, leaderName);

                    if (faction == Faction.OfPlayer)
                    {
                        // mem.CurrentJargon = "Light a match";
                        mem.JargonEvolutionHistory.Add(new JargonEntry("Light a match", "Kill this hostage", "During a hijacking of a royal merchant ship, the Red Jacket pirates locked the hostages in the bottom of the cabin. When the captain ordered the execution of the hostages, he said, 'Light a match'. His men immediately understood what he meant and set fire to all the hostages."));
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

                            Log.Message($"[PirateJargon] Set leader of {factionName} to {faction.leader.Name.ToStringFull}");
                        }
                    }
                    pirateFactions[factionId] = mem;
                    faction.Name = factionName;
                    Log.Message($"[PirateJargon] Registered pirate faction: {factionId} as {factionName}");
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