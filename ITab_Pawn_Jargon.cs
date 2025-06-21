using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace PirateJargonEvolution
{
    public class ITab_Pawn_Jargon : ITab
    {
        private const float Width = 400f;
        private const float Height = 300f;

        private Pawn PawnForJargon
        {
            get
            {
                if (SelPawn != null)
                {
                    return SelPawn;
                }

                if (SelThing is Corpse corpse)
                {
                    return corpse.InnerPawn;
                }

                return null;
            }
        }

        public ITab_Pawn_Jargon()
        {
            this.size = new Vector2(Width, Height);
            this.labelKey = "TabPirateJargon"; // optional if not localized
            this.tutorTag = "PirateJargon";
        }

        protected override void FillTab()
        {
            Pawn pawn = PawnForJargon;
            if (pawn == null)
            {
                Log.Error("[PirateJargon] Jargon tab couldn't find pawn.");
                return;
            }

            CompPirateIdentity comp = pawn.TryGetComp<CompPirateIdentity>();
            if (comp == null)
            {
                Widgets.Label(new Rect(0, 0, size.x, size.y), "No pirate identity found.");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Known Pirate Jargon:\n");
            foreach (string word in comp.knownJargon)
            {
                sb.AppendLine($"- {word}");
            }

            Widgets.Label(new Rect(10f, 10f, size.x - 20f, size.y - 20f), sb.ToString());
        }
    }
}