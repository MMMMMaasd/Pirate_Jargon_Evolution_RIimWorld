using System.Collections.Generic;
using Verse;
using System.Linq;
using RimWorld;
using System;
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
            return $@"You are in the Rim World game world with a medieval pirates mod, you are playing a pirate in it, 
                     You are a pirate named {initiator.Name.ToStringShort} talking to your crewmate {recipient.Name.ToStringShort}
                     Both of you and your crewmate each belonging to the faction '{memory.FactionName ?? "Unknown Pirate Crew"}'.
                     For your reference:
                     The jargon style of your faction: {memory.JargonStyle};
                     The origin story of your faction: {memory.OriginStory};

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
                     
                     Now write a short, 1-2 sentence daily chat message that you say to your crewmate based on the current Situation, using your jargon naturally (you only speak the jargons that both you and this crewmate know).    
                     Make it more detailed and personality-driven, considering your traits and relationship.
                     Return only the generated message, no additional text or instructions. Just what they said inside quotes.";
        }
        
        public static string GenerateJargonEvolutionPromptRecipient(PirateFactionMemory memory, Pawn initiator, Pawn recipient, string situation, string input)
        {
            float opinion = initiator.relations.OpinionOf(recipient);
            return $@"You are in the Rim World game world with a medieval pirates mod, you are playing a pirate in it, 
                     You are a pirate named {initiator.Name.ToStringShort} reply to your crewmate {recipient.Name.ToStringShort}, who just said {input}.
                     Both of you and your crewmate each belonging to the faction '{memory.FactionName ?? "Unknown Pirate Crew"}'.
                     For your reference:
                     The jargon style of your faction: {memory.JargonStyle};
                     The origin story of your faction: {memory.OriginStory};

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
                     
                     Now write a short, 1-2 sentence daily chat message that you reply to your crewmate, using your jargon naturally (you only speak the jargons that both you and this crewmate know).    
                     Make it more detailed and personality-driven, considering your traits and relationship.
                     Return only the generated message, no additional text or instructions. Just what they said inside quotes.";
        }
        
        public static string GenerateJargonEvolutionPromptTwoNPC(PirateFactionMemory memory, string situation)
        {
            Random random = new Random();
            int randomNum = random.Next(1, 6);
            return $@"
            In a medieval pirate simulation world, each pirate faction has its own evolving slang or jargon language. 
            Your task is to **invent {randomNum} new pirate jargons** for this faction: **{memory.FactionName}**.

            The style and tone of the jargons should follow this faction's linguistic flavor:
                - Jargon style: {memory.JargonStyle}
                - Faction origin story: {memory.OriginStory}

            Previously known jargon from this faction:
            {FormatJargonDictionary(memory)}

            Context for creating new jargons:
            A recent event occurred in this faction: {situation}

            In some of the new jargons, you may creatively refer to names of characters involved in the event (e.g., use someone's name as part of the phrase, or as a metaphor). 
            But don't do this for all jargons — balance it out with metaphorical or stylistic expressions unique to the faction's culture.

            Requirements:
                - Do NOT reuse any existing jargon
                - Do NOT use non-English language
                - Follow the same stylistic tone and metaphor style as this faction's existing jargon
                - Return only the list in this exact format:

            (jargon word, meaning, origin story)

            Example:
            ('Anne’s Kiss', 'Escape through seduction', 'Anne Bonny once flirted with a guard to free a captive—her charm became legend.')

            Return ONLY the list of new jargons using the above format, with parentheses and no extra explanation.";
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