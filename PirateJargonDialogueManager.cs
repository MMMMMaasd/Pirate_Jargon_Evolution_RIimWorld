using System;
using Verse;
using System.Threading.Tasks;
using PirateJargonEvolution.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace PirateJargonEvolution
{
    public static class PirateJargonDialogueManager
    {
        public static async void StartDialogue(Pawn initiator, Pawn recipient, string situation)
        {
            var compA = initiator.TryGetComp<CompThinking>();
            if (compA != null)
                compA.startThinking();
            
            var compB = recipient.TryGetComp<CompThinking>();
            if (compB != null)
                compB.startThinking();
            
            var moteA = initiator.TryGetComp<CompMoteRepeater>();
            var moteB = recipient.TryGetComp<CompMoteRepeater>();

            moteA?.StartRepeating();
            // MoteBubbleHelper.ThrowStaticText(initiator, "...");

            // Leanred the jargon

            var compAPirate = initiator.TryGetComp<CompPirateIdentity>();
            var compBPirate = recipient.TryGetComp<CompPirateIdentity>();
            
            int maxSpread = 3; // control variable
            
            List<string> wordToUse = compAPirate.knownJargon
                .OrderBy(_ => Rand.Value)
                .Take(maxSpread)
                .ToList();
            
            PirateFactionMemory commonFaction =
                SharedEventUtil.GetFaction(initiator.TryGetComp<CompPirateIdentity>().pirateFactionId);
            
            string prompt1 = OllamaPromptGenerator.GenerateJargonEvolutionPromptInitiator(commonFaction, initiator, recipient, situation, wordToUse);
            Log.Message(prompt1);
            string response1 = await OllamaHelper.CallOllamaAsync(prompt1);
            var usedWords = ParseUsedJargon(response1);
            foreach (string word in usedWords)
            {
                Log.Message($"word to spread: {word}");
            }
            
            moteA?.StopRepeating();
            MoteBubbleHelper.ThrowStaticText(initiator, response1);
            
            await Task.Delay(3000);
            
            moteB?.StartRepeating();
            // MoteBubbleHelper.ThrowStaticText(recipient, "...");
            string prompt2 = OllamaPromptGenerator.GenerateJargonEvolutionPromptRecipient(commonFaction, recipient, initiator, situation, response1);
            string response2 = await OllamaHelper.CallOllamaAsync(prompt2);
            Log.Message(prompt2);
            moteB?.StopRepeating();
            MoteBubbleHelper.ThrowStaticText(recipient, response2);
            
                        
            List<string> initiatorKnownJargon = initiator.TryGetComp<CompPirateIdentity>().knownJargon;
            List<string> recipientKnownJargon = recipient.TryGetComp<CompPirateIdentity>().knownJargon;
            
            foreach (var jargon in wordToUse)
            {
                if (!recipientKnownJargon.Contains(jargon))
                {
                    recipient.TryGetComp<CompPirateIdentity>().knownJargon.Add(jargon);
                    MoteBubbleHelper.ThrowStaticText(recipient, $"Learned: {jargon}");
                    Log.Message($"[PirateJargon] {recipient.Name} learned '{jargon}' fron LLM dialog.");

                }
            }
            
           // foreach (string jargon in initiatorKnownJargon)
           // {
           //     if(!recipientKnownJargon.Contains(jargon))
           //     {
           //         recipient.TryGetComp<CompPirateIdentity>().knownJargon.Add((jargon));
           //         Log.Message($"initiator learned jargon: {jargon}");
           //     }
           // }
            
           // foreach (string jargon in recipientKnownJargon)
           // {
           //     if(!initiatorKnownJargon.Contains(jargon))
           //     {
           //         initiator.TryGetComp<CompPirateIdentity>().knownJargon.Add((jargon));
           //         Log.Message($"recipient learned jargon: {jargon}");
           //     }
           // }
            compA.stopThinking();
            compB.stopThinking();
        }
        
        public static List<string> ParseUsedJargon(string rawText)
        {
            var used = new List<string>();

            var match = Regex.Match(rawText, @"UsedJargon:\s*\((.*?)\)");
            if (match.Success)
            {
                string content = match.Groups[1].Value;
                var parts = content.Split(',');
                foreach (var p in parts)
                {
                    string term = p.Trim();
                    if (!string.IsNullOrWhiteSpace(term))
                        used.Add(term.ToLowerInvariant()); // 统一小写匹配
                }
            }
            else
            {
                Log.Warning("[PirateJargon] LLM response didn't include UsedJargon list.");
            }

            return used;
        }
        
        public static string ExtractQuotedLine(string response)
        {
            var match = Regex.Match(response, "\"(.*?)\"");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            
            return response.Split(new[] { "UsedJargon:" }, StringSplitOptions.None)[0].Trim();
        }
    }
}