using Kinel.VideoPlayer.Udon;
using Kinel.VideoPlayer.Udon.Module;
using UnityEngine;

namespace KinelVideoPlayerPlugin.Udon
{
    public class KinelVideoPlayerResyncer : KinelModule
    {
        [SerializeField] private KinelVideoPlayer[] videoPlayers;

        public void Start()
        {
        }

        public void ResyncAll()
        {
            Debug.Log($"{DEBUG_PREFIX} ResyncAll");
            for (int i = 0; i < videoPlayers.Length; i++)
            {
                videoPlayers[i].Sync();
            }
        }

        public void ReloadAll()
        {
            Debug.Log($"{DEBUG_PREFIX} ReloadAll");
            for (int i = 0; i < videoPlayers.Length; i++)
            {
                videoPlayers[i].Reload();
            }
        }
    }
}