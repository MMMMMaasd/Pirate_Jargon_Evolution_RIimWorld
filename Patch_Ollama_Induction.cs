using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using HarmonyLib;
using RestSharp;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Verse.AI;

namespace PirateJargonEvolution
{
    public class OllamaSettings : ModSettings
    {
        public string modelName = "llama3";
        public string endpoint = "http://localhost:11434";
        public bool enableMod = true;
        public float temperature = 0.7f;
    }

    public class PirateJargonEvolution : Mod
    {
        public static OllamaSettings settings;

        public PirateJargonEvolution(ModContentPack content) : base(content)
        {
            settings = new OllamaSettings();
            var harmony = new Harmony("com.michael.piratejargon");
            harmony.PatchAll();
        }

        public override string SettingsCategory()
        {
            return "Social Interaction LLM";
        }
    }
    
    
    [HarmonyPatch(typeof(PlayLog), nameof(PlayLog.Add))]
    public static class Verse_PlayLog_Add_Patch
    {
        private static RestClient restClient = new RestClient(PirateJargonEvolution.settings.endpoint);

        private static HashSet<LogEntry> processedEntries = new HashSet<LogEntry>();
        
        private static void Postfix(LogEntry entry)
        {
            if (!PirateJargonEvolution.settings.enableMod) return;

            // Safeguard: Prevent recursion by checking if this entry has already been processed
            if (processedEntries.Contains(entry)) return;

            try
            {
                // Process only entries of type PlayLogEntry_Interaction
                if (!(entry is PlayLogEntry_Interaction interaction)) return;

                // Use reflection to get initiator and recipient
                var initiator = AccessTools.Field(typeof(PlayLogEntry_Interaction), "initiator").GetValue(interaction) as Pawn;
                var recipient = AccessTools.Field(typeof(PlayLogEntry_Interaction), "recipient").GetValue(interaction) as Pawn;

                if (initiator == null || recipient == null) return;

                Regex RemoveColorTag = new Regex("<\\/?color[^>]*>");
                string txt = interaction.ToGameStringFromPOV(initiator);
                string interactionText = RemoveColorTag.Replace(txt, string.Empty);

                // Generate LLM prompt from existing interaction details
                string prompt = GeneratePrompt(initiator, recipient, interactionText);

                // Run the LLM call on a background thread
                Task.Run(async () =>
                {
                    string llmOutput = await CallOllamaAsync(prompt);
                    //initiator.jobs.StopAll();
                    //Job waitJob = new Job(JobDefOf.Wait, 120);
                    //initiator.jobs.StartJob(waitJob, JobCondition.InterruptForced);
                    if (!string.IsNullOrEmpty(llmOutput))
                    {
                        // Add a new message entry to the PlayLog without modifying the original one
                        AddCustomLogEntry(initiator, recipient, llmOutput);
                        ShowSpeechBubble(initiator, llmOutput);

                        await Task.Delay(8000);
                        
                        initiator.TryGetComp<CompThinking>()?.StopThinking();
                        recipient.TryGetComp<CompThinking>()?.StopThinking();
                    }
                    else
                    {
                        initiator.TryGetComp<CompThinking>()?.StopThinking();
                        recipient.TryGetComp<CompThinking>()?.StopThinking();
                    }
                    
                });
                processedEntries.Add(entry);
            }
            catch (Exception e)
            {
                Log.Error($"Postfix: Error processing PlayLogEntry -> {e.Message}\n{e.StackTrace}");
            }
        }
        
            
        public static void ShowSpeechBubble(Pawn speaker, string text)
        {
            if (speaker == null || speaker.Dead || !speaker.Spawned)
            {
                return;
            }
            
            MoteMaker.ThrowText(speaker.DrawPos, speaker.Map, text);
        }
        
        private static string GeneratePrompt(Pawn initiator, Pawn recipient, string originalMessage)
        {
            float opinion = initiator.relations.OpinionOf(recipient);
            return $@"You are helping generate social interaction messages for the game RimWorld.
                     Generate a short, one-sentence interaction message between these two colonists:
                     
                     Initiator: {initiator.Name.ToStringShort} (Traits: {string.Join(", ", initiator.story.traits.allTraits)})
                     Recipient: {recipient.Name.ToStringShort} (Traits: {string.Join(", ", recipient.story.traits.allTraits)})
                     Their relationship: {(opinion > 0 ? "Positive" : "Negative")} ({opinion} opinion)
                     Original message: {originalMessage}
                     
                     Make it more detailed and personality-driven, considering their traits and relationship.
                     Keep it brief and similar in tone to RimWorld's style.
                     Return only the generated message, no additional text or instructions. Just what they said inside quotes.
                     Be short, just do short sentence, because your generation speed need to be very fast";
        }

        private static async Task<string> CallOllamaAsync(string prompt)
        {
            try
            {
                var request = new RestRequest("/api/generate", Method.POST)
                              .AddJsonBody(new
                              {
                                  model = PirateJargonEvolution.settings.modelName,
                                  prompt = EscapeJsonString(prompt),
                                  temperature = PirateJargonEvolution.settings.temperature.ToString("F1"),
                                  stream = false
                              });

                var response = await restClient.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    // Log.Message($"Received API response: {response.Content}");

                    string message = ExtractResponseFromJson(response.Content.ToString());
                    message = StripColorCodes(message);
                    Log.Message($"{message}");

                    return message;
                }

                Log.Error($"Ollama API error: {response.StatusCode} - {response.ErrorMessage}");
                return null;
            }
            catch (Exception e)
            {
                Log.Error($"Error calling Ollama: {e.GetType().Name} - {e.Message}\n{e.StackTrace}");
                return null;
            }
        }

        private static string ExtractResponseFromJson(string jsonResponse)
        {
            try
            {
                int startIndex = jsonResponse.IndexOf("\"response\":\"") + "\"response\":\"".Length;
                int endIndex = jsonResponse.IndexOf("\",\"done\"", startIndex);

                if (startIndex >= 0 && endIndex > startIndex)
                {
                    string extracted = jsonResponse.Substring(startIndex, endIndex - startIndex);
                    string unescaped = Regex.Unescape(extracted);
                    return unescaped;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error extracting response from JSON: {ex.Message}");
            }

            return null;
        }

        private static string StripColorCodes(string input)
        {
            string decoded = input.Replace("\\u003c", "<").Replace("\\u003e", ">");
            var htmlColorTagPattern = @"<color=#\w+>|</color>";
            return Regex.Replace(decoded, htmlColorTagPattern, string.Empty);
        }

        private static string EscapeJsonString(string str)
        {
            return str.Replace("\"", "\\\"")
                     .Replace("\r", "\\r")
                     .Replace("\n", "\\n")
                     .Replace("\t", "\\t");
        }

        private static void AddCustomLogEntry(Pawn initiator, Pawn recipient, string message)
{
    try
    {
        if (initiator == null || recipient == null)
        {
            Log.Error("AddCustomLogEntry: Initiator or recipient is null. Cannot create custom log entry.");
            return;
        }

        // Combine the message for display
        //said to {recipient.Name.ToStringShort}
        var combinedMessage = $"{initiator.Name.ToStringShort}: {message}";

        // Display the message to the player using RimWorld's in-game message system
        // Messages.Message(combinedMessage, MessageTypeDefOf.NeutralEvent, historical: false);

        Log.Message($"Generated LLM Interaction Message: {combinedMessage}");
    }
    catch (Exception ex)
    {
        Log.Error($"Error while adding a new custom log entry: {ex.Message}\n{ex.StackTrace}");
    }
}
    }
}