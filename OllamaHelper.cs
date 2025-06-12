using System;
using System.Text.RegularExpressions;
using RestSharp;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;

namespace PirateJargonEvolution
{
    public static class OllamaHelper
    {
        
        private static RestClient client = new RestClient(PirateJargonEvolution.settings.endpoint);
        
        public static async Task<string> CallOllamaAsync(string prompt)
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

                var response = await client.ExecuteAsync(request);

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

        public static void AddCustomLogEntry(Pawn initiator, Pawn recipient, string message)
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