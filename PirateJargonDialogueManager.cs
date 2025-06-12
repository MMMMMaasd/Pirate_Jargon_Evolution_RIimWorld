using Verse;
using System.Threading.Tasks;
using PirateJargonEvolution.Utils;
using System.Collections.Generic;

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
            

            MoteBubbleHelper.ThrowStaticText(initiator, "...");

            PirateFactionMemory commonFaction =
                SharedEventUtil.GetFaction(initiator.TryGetComp<CompPirateIdentity>().pirateFactionId);
            
            string prompt1 = OllamaPromptGenerator.GenerateJargonEvolutionPromptInitiator(commonFaction, initiator, recipient, situation);
            string response1 = await OllamaHelper.CallOllamaAsync(prompt1);
            
            MoteBubbleHelper.ThrowStaticText(initiator, response1);
            
            await Task.Delay(3000);
            
            MoteBubbleHelper.ThrowStaticText(recipient, "...");
            string prompt2 = OllamaPromptGenerator.GenerateJargonEvolutionPromptRecipient(commonFaction, recipient, initiator, situation, response1);
            string response2 = await OllamaHelper.CallOllamaAsync(prompt2);

            MoteBubbleHelper.ThrowStaticText(recipient, response2);

            // Leanred the jargon
            List<string> initiatorKnownJargon = initiator.TryGetComp<CompPirateIdentity>().knownJargon;
            List<string> recipientKnownJargon = initiator.TryGetComp<CompPirateIdentity>().knownJargon;

            foreach (string jargon in initiatorKnownJargon)
            {
                if(!recipientKnownJargon.Contains(jargon))
                {
                    recipient.TryGetComp<CompPirateIdentity>().knownJargon.Add((jargon));
                    Log.Message($"initiator learned jargon: {jargon}");
                }
            }
            
            foreach (string jargon in recipientKnownJargon)
            {
                if(!initiatorKnownJargon.Contains(jargon))
                {
                    initiator.TryGetComp<CompPirateIdentity>().knownJargon.Add((jargon));
                    Log.Message($"recipient learned jargon: {jargon}");
                }
            }
            compA.stopThinking();
            compB.stopThinking();
        }
    }
}