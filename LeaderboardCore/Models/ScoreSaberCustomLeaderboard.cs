using LeaderboardCore.Interfaces;
using UnityEngine;
using Zenject;

namespace LeaderboardCore.Models
{
    internal class ScoreSaberCustomLeaderboard :  INotifyScoreSaberActivate
    {
        [Inject]
        private readonly PlatformLeaderboardViewController platformLeaderboardViewController = null!;

        private Transform ssLeaderboardElementsTransform;
        private Vector3 ssLeaderboardElementsPosition;

        private Transform ssPanelScreenTransform;
        private Vector3 ssPanelScreenPosition;

        private Vector3 hiddenPosition;

        public void OnScoreSaberActivated()
        {
            hiddenPosition = new Vector3(-999, -999, -999);

            if (ssLeaderboardElementsTransform == null)
            {
                var found = platformLeaderboardViewController.transform.Find("ScoreSaberLeaderboardElements");
                if (found != null)
                {
                    ssLeaderboardElementsTransform = found;
                    ssLeaderboardElementsPosition = found.localPosition;
                }
            }

            if (ssPanelScreenTransform == null)
            {
                var found = platformLeaderboardViewController.transform.Find("ScoreSaberPanelScreen");
                if (found != null)
                {
                    ssPanelScreenTransform = found;
                    ssPanelScreenPosition = found.localPosition;
                }
            }
        }

        public void YeetSS()
        {
            if (ssLeaderboardElementsTransform != null && ssPanelScreenTransform != null && ssLeaderboardElementsTransform.localPosition != hiddenPosition)
            {
                ssLeaderboardElementsTransform.localPosition = hiddenPosition;
                ssPanelScreenTransform.localPosition = hiddenPosition;
            }
        }

        public void UnYeetSS()
        {
            if (ssLeaderboardElementsTransform != null && ssPanelScreenTransform != null && ssLeaderboardElementsTransform.localPosition != ssLeaderboardElementsPosition)
            {
                ssLeaderboardElementsTransform.localPosition = ssLeaderboardElementsPosition;
                ssPanelScreenTransform.localPosition = ssPanelScreenPosition;
            }
        }
    }
}
