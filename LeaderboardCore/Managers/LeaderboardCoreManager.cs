using System;
using System.Collections.Generic;
using Zenject;
using LeaderboardCore.Interfaces;
using LeaderboardCore.HarmonyPatches;

namespace LeaderboardCore.Managers
{
    internal class LeaderboardCoreManager : IInitializable, IDisposable
    {
        private readonly PlatformLeaderboardViewController platformLeaderboardViewController;
        private readonly List<INotifyScoreSaberActivate> notifyScoreSaberActivates;
        private readonly List<INotifyLeaderboardLoad> notifyLeaderboardLoads;
        private readonly List<INotifyScoreUpload> notifyScoreUploads;

        public LeaderboardCoreManager(PlatformLeaderboardViewController platformLeaderboardViewController,
            List<INotifyScoreSaberActivate> notifyScoreSaberActivates,
            List<INotifyLeaderboardLoad> notifyLeaderboardLoads, List<INotifyScoreUpload> notifyScoreUploads)
        {
            this.platformLeaderboardViewController = platformLeaderboardViewController;
            this.notifyScoreSaberActivates = notifyScoreSaberActivates;
            this.notifyLeaderboardLoads = notifyLeaderboardLoads;
            this.notifyScoreUploads = notifyScoreUploads;
        }

        public void Initialize()
        {
            // Not PanelView_Show/Harmony: the old "Show" method this used to hook doesn't exist
            // anywhere in ScoreSaber's current PanelView hierarchy at all, and the best-effort
            // replacement (Parsed()) fired too early -- before PlatformLeaderboardViewController's
            // own "ScoreSaberLeaderboardElements"/"ScoreSaberPanelScreen" children exist, which
            // threw a NullReferenceException on real hardware. didActivateEvent is the real base
            // game event for "this leaderboard view controller is now active", already used the
            // same way by LeaderboardNavigationButtonsController elsewhere in this codebase.
            platformLeaderboardViewController.didActivateEvent += LeaderboardActivated;
            PanelView_SetIsLoaded.IsLoadedChanged += PanelViewLoadingChanged;
            PanelView_SetPrompt.ScoreUploaded += ScoreUploaded;
        }

        public void Dispose()
        {
            platformLeaderboardViewController.didActivateEvent -= LeaderboardActivated;
            PanelView_SetIsLoaded.IsLoadedChanged -= PanelViewLoadingChanged;
            PanelView_SetPrompt.ScoreUploaded -= ScoreUploaded;
        }

        private void LeaderboardActivated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            foreach (var notifyScoreSaberActivate in notifyScoreSaberActivates)
            {
                notifyScoreSaberActivate.OnScoreSaberActivated();
            }
        }

        private void PanelViewLoadingChanged(bool loaded)
        {
            foreach (var notifyLeaderboardLoad in notifyLeaderboardLoads)
            {
                notifyLeaderboardLoad.OnLeaderboardLoaded(loaded);
            }
        }

        private void ScoreUploaded()
        {
            foreach (var notifyScoreUpload in notifyScoreUploads)
            {
                notifyScoreUpload.OnScoreUploaded();
            }
        }
    }
}
