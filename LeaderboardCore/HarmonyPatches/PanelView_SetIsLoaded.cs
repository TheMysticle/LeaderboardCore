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

		// Harmony's field-injection convention is exactly 3 marker underscores + the field's own
		// exact name. The backing field is "__isLoaded" (two underscores, declared on our own
		// BasicPanelViewController), so the parameter needs 3+2 = 5 leading underscores here, not 4
		// -- confirmed via the real error text on real hardware ("No such field defined... Parameter
		// name: _isLoaded", meaning Harmony was stripping to a single leftover underscore before).
		private static void Postfix(bool _____isLoaded)
        {
			IsLoadedChanged?.Invoke(_____isLoaded);
        }
	}
}
