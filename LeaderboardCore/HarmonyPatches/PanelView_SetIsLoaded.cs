using HarmonyLib;
using System;
using System.Reflection;

namespace LeaderboardCore.HarmonyPatches
{
	[HarmonyPatch]
    internal class PanelView_SetIsLoaded
    {
		public static event Action<bool> IsLoadedChanged;

		// ScoreSaber's PanelView moved from ScoreSaber.UI.Leaderboard to
		// ScoreSaber.Features.Leaderboards.UI at some point upstream. Its "isLoaded" property is
		// also no longer declared directly on PanelView itself -- it's inherited from our own
		// LeaderboardCore.Models.UI.ViewControllers.BasicPanelViewController base class, so finding
		// its (protected, non-public) compiler-generated setter requires FlattenHierarchy; without
		// it GetMethod only looks at members declared directly on PanelView and returns null,
		// which is what threw the NullReferenceException here (confirmed on real hardware).
		private static MethodBase TargetMethod() =>
			Plugin.Instance.scoreSaber.Assembly.GetType("ScoreSaber.Features.Leaderboards.UI.PanelView")
				.GetMethod("set_isLoaded", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

		private static void Postfix(bool ____isLoaded)
        {
			IsLoadedChanged?.Invoke(____isLoaded);
        }
	}
}
