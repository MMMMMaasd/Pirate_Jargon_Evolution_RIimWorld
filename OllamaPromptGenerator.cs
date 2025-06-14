using System.Collections.Generic;
using Verse;
using System.Linq;
using RimWorld;
namespace PirateJargonEvolution
{
    public static class OllamaPromptGenerator
    {
        public static string GenerateSocialInteractionPrompt(Pawn initiator, Pawn recipient, string originalMessage)
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
                     Return only the generated message, no additional text or instructions. Just what they said inside quotes.";
        }
        
        public static string GenerateJargonEvolutionPromptInitiator(PirateFactionMemory memory, Pawn initiator, Pawn recipient, string situation)
        {
            float opinion = initiator.relations.OpinionOf(recipient);
            return $@"You are in a medieval pirates game world, 
                     You are a pirate named {initiator.Name.ToStringShort} talking to your crewmate {recipient.Name.ToStringShort}
                     Both of you and your crewmate each belonging to the faction '{memory.FactionName ?? "Unknown Pirate Crew"}'.
                     Very Important ***In their conversation, you only speak the jargon that both of you and you crewmate know. *** 
                     Here are your (as the initiator of this talk) known jargon:
                     {FormatJargonDictionary(initiator, memory)}
                     Here are your crewmate's (as the recipient of this talk) known jargon:
                     {FormatJargonDictionary(recipient, memory)}

                     Here is the detailed information about you and your crewmate:
                     You: {initiator.Name.ToStringShort} (Traits: {string.Join(", ", initiator.story.traits.allTraits)}) 
                     Your's position in this faction: {initiator.TryGetComp<CompPirateIdentity>().positionInFaction}
                     Recipient: {recipient.Name.ToStringShort} (Traits: {string.Join(", ", recipient.story.traits.allTraits)})
                     Recipient's position in this faction: {recipient.TryGetComp<CompPirateIdentity>().positionInFaction}
                     Your relationship with him/her: {(opinion > 0 ? "Positive" : "Negative")} ({opinion} opinion)
                     Current Situation: {situation}
                     
                     Now write a short, 1-2 sentence message that you say to your crewmate based on the current Situation, using your jargon naturally (you only speak the jargons that both you and this crewmate know).    
                     Make it more detailed and personality-driven, considering your traits and relationship.
                     Return only the generated message, no additional text or instructions. Just what they said inside quotes.";
        }
        
        public static string GenerateJargonEvolutionPromptRecipient(PirateFactionMemory memory, Pawn initiator, Pawn recipient, string situation, string input)
        {
            float opinion = initiator.relations.OpinionOf(recipient);
            return $@"You are in a medieval pirates game world, 
                     You are a pirate named {initiator.Name.ToStringShort} reply to your crewmate {recipient.Name.ToStringShort}, who just said {input}.
                     Both of you and your crewmate each belonging to the faction '{memory.FactionName ?? "Unknown Pirate Crew"}'.
                     Very Important ***In their conversation, you only speak the jargon that both of you and you crewmate know. *** 
                     Here are your known jargon:
                     {FormatJargonDictionary(initiator, memory)}
                     Here are your crewmate's known jargon:
                     {FormatJargonDictionary(recipient, memory)}

                     Here is the detailed information about you and your crewmate:
                     You: {initiator.Name.ToStringShort} (Traits: {string.Join(", ", initiator.story.traits.allTraits)}) 
                     Your's position in this faction: {initiator.TryGetComp<CompPirateIdentity>().positionInFaction}
                     Your Crewmate: {recipient.Name.ToStringShort} (Traits: {string.Join(", ", recipient.story.traits.allTraits)})
                     Your Crewmate's position in this faction: {recipient.TryGetComp<CompPirateIdentity>().positionInFaction}
                     Your relationship with him/her: {(opinion > 0 ? "Positive" : "Negative")} ({opinion} opinion)
                     Current Situation: {situation}
                     
                     Now write a short, 1-2 sentence message that you reply to your crewmate, using your jargon naturally (you only speak the jargons that both you and this crewmate know).    
                     Make it more detailed and personality-driven, considering your traits and relationship.
                     Return only the generated message, no additional text or instructions. Just what they said inside quotes.";
        }
        
        public static string GenerateJargonEvolutionPromptTwoNPC(PirateFactionMemory memory, string situation)
        {
            return $@"In a medieval pirate simulation world, each faction has its own evolving slang/jargon language. 
                     Here is the jargon this faction knows and speaks:
                     {FormatJargonDictionary(memory)}.
                     You are about to create a new jargon word based on this situation/event that occurred: {situation}.
                     Now generate some new pirate jargon words and their meaning (less than 5 jargon words in total).
                     Everything, including the jargon itself, must be in English, no other language!
                     Don't reuse existing jargon! And your newly created jargon should somehow reflecting this event happening.
                     Keep the same style and tone of the existing known jargon in this faction as much as possible!

                     Return your answer in the format of a list of items, each item is: (jargon word, meaning, origin story of it)";
        }
        
        
        private static string FormatJargonDictionary(Pawn pawn, PirateFactionMemory memory)
        {
            var lines = new List<string>();
            foreach (var kvp in pawn.TryGetComp<CompPirateIdentity>().knownJargon)
            {
                JargonEntry foundjargonEntry = memory.GetJargonInfo(kvp);
                lines.Add($"{foundjargonEntry.getJargon()}: {foundjargonEntry.getMeaning()}");
            }
            return string.Join("\n", lines);
        }

        private static string FormatJargonDictionary(PirateFactionMemory memory)
        {
            var lines = new List<string>();
            foreach (var jargon in memory.JargonEvolutionHistory)
            {
                lines.Add(jargon.ToString());
            }

            return string.Join("\n", lines);
        }
    }
}