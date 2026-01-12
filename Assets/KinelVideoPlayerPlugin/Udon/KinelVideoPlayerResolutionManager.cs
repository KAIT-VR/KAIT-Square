using Kinel.VideoPlayer.Udon;
using Kinel.VideoPlayer.Udon.Module;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace KinelVideoPlayerPlugin.Udon
{
    public class KinelVideoPlayerResolutionManager : KinelModule
    {
        [SerializeField] private KinelVideoPlayer[] videoPlayers;
        
        [SerializeField] private int[] resolutionArray = new[] { 144, 240, 360, 480, 720, 1080, 1440, 2160 };

        private int resolutionIndex = 4;

        public void Start()
        {
            if (videoPlayers.Length == 0) return;
            SendCustomEventDelayedSeconds(nameof(SetResolutionForAllVideoPlayers), 1);
        }

        public void SetResolutionForAllVideoPlayers()
        {
            Debug.Log($"{DEBUG_PREFIX} set resolution");
            for (int i = 0; i < videoPlayers.Length; i++)
            {
                var animator = videoPlayers[i].GetComponent<Animator>();
                SetResolution(videoPlayers[i], 2160, animator);
            }
        }

        // Udon#1.x.xでEnum使いたい
        public void SetResolution(KinelVideoPlayer target, int resolution, Animator animator)
        {
            if(GetResolutionIndex(resolution) == -1)
            {
                SetResolution(target, 2160, animator);
                return;
            }
            
            resolutionIndex = GetResolutionIndex(resolution);
            animator.SetInteger("ResolutionIndex", resolutionIndex);
            if (target.IsPlaying)
            {
                target.Reload();
            }
        }
        
        private int GetResolutionIndex(int resolution)
        {
            for (int i = 0; i < resolutionArray.Length; i++)
            {
                if (resolutionArray[i].Equals(resolution))
                    return i;
            }
            return -1;
        }

        
    }
}