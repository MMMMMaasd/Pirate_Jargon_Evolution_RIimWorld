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
        
        public static string GenerateJargonEvolutionPromptInitiator(PirateFactionMemory memory, Pawn initiator, Pawn recipient, string situation, List<string> jargonToUSe)
        {
            float opinion = initiator.relations.OpinionOf(recipient);
            return $@"
                        You are playing a pirate character in a RimWorld modded game with a medieval pirate setting.

                        You are '{initiator.Name.ToStringShort}', speaking to your crewmate '{recipient.Name.ToStringShort}'.
                        Both of you belong to the same pirate faction: '{memory.FactionName ?? "Unknown Pirate Crew"}'.

                        Faction style: {memory.JargonStyle}
                        Faction origin story: {memory.OriginStory}

                        Here are the pirate jargons YOU know:
                        {FormatJargonDictionary(initiator, memory)}

                        Here are the jargon you can speak in this conversation (you can ONLY use these in your message):
                        {string.Join("\n", jargonToUSe)}

                        Some additional information:
                            - Your traits: {string.Join(", ", initiator.story.traits.allTraits)}
                            - Your position: {initiator.TryGetComp<CompPirateIdentity>().positionInFaction}
                            - Crewmate traits: {string.Join(", ", recipient.story.traits.allTraits)}
                            - Crewmate position: {recipient.TryGetComp<CompPirateIdentity>().positionInFaction}
                            - Relationship: {(opinion > 0 ? "Positive" : "Negative")} ({opinion} opinion)
                            - Current situation/context: {situation}

                        ---

                        Now, your task is:

                        1. Write a short (1–2 sentence) message that YOU say to your crewmate, relate to the situation/context.
                        2. Naturally include AT LEAST TWO pirate jargons in your conversation.
                        3. You may only use the jargons you can speak here — do not invent new ones or use unknown ones or those that you know but not allow to say in this conversation.
                        4. The response MUST follow this strict format exactly (Return only the generated message, no additional text or instructions. Just what they said inside quotes):

                        “Your message here, using pirate slang naturally.”.
                        ";
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

                     Here is the detailed information about you and your crewmate:
                     You: {initiator.Name.ToStringShort} (Traits: {string.Join(", ", initiator.story.traits.allTraits)}) 
                     Your's position in this faction: {initiator.TryGetComp<CompPirateIdentity>().positionInFaction}
                     Your Crewmate: {recipient.Name.ToStringShort} (Traits: {string.Join(", ", recipient.story.traits.allTraits)})
                     Your Crewmate's position in this faction: {recipient.TryGetComp<CompPirateIdentity>().positionInFaction}
                     Your relationship with him/her: {(opinion > 0 ? "Positive" : "Negative")} ({opinion} opinion)
                     Current Situation: {situation}
                     
                     Now write a short, 1-2 sentence daily chat message that you reply to your crewmate, using your jargon naturally (you only speak the jargons that you know).
                     Very Important ***In the conversation, if everything your crewmate said to you is what you know, then just reply confidently like a secret jargon pirate talk.
                     However, if there is any thing crewmate said that is quite confused, then this crewmate might include some jargons you don't know, then you should reply with first infer your crewmate's meaning and then reply to your crewmate's message***
                     In this case your reply should be in the format: 
                     'Are you mean: ...? Yes, I agree with you ... '     
                     
                     Here are your known jargon: (these are the jargons you only know)!
                     {FormatJargonDictionary(initiator, memory)}


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