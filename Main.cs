using Verse;
using HarmonyLib;

namespace PirateJargonEvolution
{
    [StaticConstructorOnStartup]
    public static class Main
    {
        static Main()
        {
            var harmony = new Harmony("com.michael.piratejargon");
            harmony.PatchAll();
            Log.Message("Michael's New Mod Load Successfully");
        }
    }
}