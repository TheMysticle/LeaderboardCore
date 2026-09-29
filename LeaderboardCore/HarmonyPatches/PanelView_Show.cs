using HarmonyLib;
using System;
using System.Reflection;

namespace LeaderboardCore.HarmonyPatches
{
    [HarmonyPatch]
    internal class PanelView_Show
    {
		public static event Action ViewActivated;

		// NOTE: unlike PanelView_SetIsLoaded/PanelView_SetPrompt, this one is a genuine best-effort
		// guess, not a confirmed fix. "Show" doesn't exist anywhere in PanelView's current hierarchy
		// at all (confirmed by reading ScoreSaber's PanelView.cs and every base class in
		// LeaderboardCore's own Models/UI/ViewControllers) -- this looks like a much older break,
		// predating 1.45.1 entirely (LeaderboardCore's git history shows this patch was last touched
		// in 2022, just for the type's namespace, never for the method itself). "Parsed" (BSML's
		// post-setup lifecycle override, public and declared directly on PanelView) is the closest
		// available "the view just became ready" equivalent, but its exact timing relative to the
		// old "Show" semantics (e.g. whether it re-fires every time the panel becomes visible again,
		// vs. only once per BSML parse) is unverified -- needs real hardware confirmation that
		// ViewActivated still fires at sensible times, not just that it fires at all.
		private static MethodBase TargetMethod() =>
			Plugin.Instance.scoreSaber.Assembly.GetType("ScoreSaber.Features.Leaderboards.UI.PanelView")
				.GetMethod("Parsed", BindingFlags.Instance | BindingFlags.Public);

		private static void Postfix()
		{
			ViewActivated?.Invoke();
		}
	}
}
