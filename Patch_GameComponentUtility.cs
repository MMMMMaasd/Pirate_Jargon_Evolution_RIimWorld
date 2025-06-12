using HarmonyLib;
using Verse;

namespace PirateJargonEvolution
{
    [HarmonyPatch(typeof(GameComponentUtility), "StartedNewGame")]
    public static class Patch_GameComponentUtility
    {
        public static void Postfix()
        {
            Current.Game.components.Add(new PirateCompAutoInitializer(Current.Game));
        }
    }

}