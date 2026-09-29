using HarmonyLib;
using HMUI;
using System;
using System.Reflection;

namespace LeaderboardCore.HarmonyPatches
{
    [HarmonyPatch]
    internal class PanelView_SetPrompt
    {
		public static event Action ScoreUploaded;

		// ScoreSaber's PanelView moved from ScoreSaber.UI.Leaderboard to
		// ScoreSaber.Features.Leaderboards.UI (same as PanelView_SetIsLoaded). SetPrompt is declared
		// directly on PanelView itself (not inherited), so FlattenHierarchy isn't needed here.
		private static MethodBase TargetMethod() =>
			Plugin.Instance.scoreSaber.Assembly.GetType("ScoreSaber.Features.Leaderboards.UI.PanelView")
				.GetMethod("SetPrompt", BindingFlags.Instance | BindingFlags.Public);

		// The CurvedTextMeshPro-typed prompt field was renamed/split: PanelView now has both a
		// "_promptTextComponent" field (CurvedTextMeshPro, what this patch actually wants) and a
		// separate "_promptText" field (a plain string holding the prompt's text content) --
		// confirmed by reading ScoreSaber's own PanelView.cs. Harmony's field-injection convention
		// is 3 marker underscores + the exact field name, so "_promptTextComponent" (one leading
		// underscore) needs 4 total leading underscores here.
		private static void Postfix(ref CurvedTextMeshPro ____promptTextComponent)
		{
			if (____promptTextComponent == null || !____promptTextComponent.text.Contains("Score uploaded"))
				return;

			ScoreUploaded?.Invoke();
		}
	}
}
