using Verse;
namespace PirateJargonEvolution
{
    public class JargonEntry : IExposable
    {
        // private IExposable _exposableImplementation;
        public string JargonWord;
        public string Meaning;
        public string OriginStory;

        public JargonEntry(string jargonWord, string meaning, string originStory)
        {
            JargonWord = jargonWord;
            Meaning = meaning;
            OriginStory = originStory;
        }
        public void ExposeData()
        {
            Scribe_Values.Look(ref JargonWord, "JargonWord");
            Scribe_Values.Look(ref Meaning, "Meaning");
            Scribe_Values.Look(ref OriginStory, "OriginStory");
            // _exposableImplementation.ExposeData();
        }

        public override string ToString()
        {
            return $"{JargonWord} = {Meaning} ({OriginStory})";
        }
    }
}