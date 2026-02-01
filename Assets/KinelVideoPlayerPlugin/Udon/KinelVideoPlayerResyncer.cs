using Kinel.VideoPlayer.Udon;
using Kinel.VideoPlayer.Udon.Module;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;

namespace KinelVideoPlayerPlugin.Udon
{
    public class KinelVideoPlayerResyncer : KinelModule
    {
        [SerializeField] private KinelVideoPlayer[] videoPlayers;

        [SerializeField] private KinelVideoPlayer targetPlayer;

        [Header("解像度ごとのURL (Inspectorで設定)")]
        [SerializeField] private VRCUrl url540p;   // DropRight
        [SerializeField] private VRCUrl url1080p;  // GrabLeft
        [SerializeField] private VRCUrl url2160p;  // GrabRight

        [Header("外部制御（OSC）を有効にする")]
        [SerializeField] private bool externalControlEnabled = true;

        private float jumpPressedTime = -1f;
        private const float JUMP_HOLD_THRESHOLD = 0.2f; // 秒

        public void ResyncAll()
        {
            Debug.Log($"{DEBUG_PREFIX} ResyncAll");
            for (int i = 0; i < videoPlayers.Length; i++)
            {
                videoPlayers[i].Sync();
            }
        }

        public void ResyncAllGlobal()
        {
            Debug.Log($"{DEBUG_PREFIX} ResyncAllGlobal");
            for (int i = 0; i < videoPlayers.Length; i++)
            {
                videoPlayers[i].SendCustomNetworkEvent(NetworkEventTarget.All, "Sync");
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

        public void ReloadAllGlobal()
        {
            Debug.Log($"{DEBUG_PREFIX} ReloadAllGlobal");
            for (int i = 0; i < videoPlayers.Length; i++)
            {
                videoPlayers[i].ReloadGlobal();
            }
        }

        private void SwitchTo(VRCUrl url)
        {
            if (!externalControlEnabled) return;

            if (targetPlayer == null)
            {
                Debug.LogWarning($"{DEBUG_PREFIX} targetPlayer is null");
                return;
            }

            if (url == null || url.Equals(VRCUrl.Empty))
            {
                Debug.LogWarning($"{DEBUG_PREFIX} url is empty");
                return;
            }

            // 必要ならマスター制限もここに
            // if (!Networking.LocalPlayer.isMaster) return;

            // オーナーを取ってから URL を更新
            if (!Networking.IsOwner(targetPlayer.gameObject))
            {
                targetPlayer.TakeOwnership();
            }

            Debug.Log($"{DEBUG_PREFIX} SwitchTo: {url.Get()}");
            targetPlayer.PlayByURL(url);
        }

        // Drop: LEFT=ReloadAll / RIGHT=540p
        public override void InputDrop(bool value, UdonInputEventArgs args)
        {
            if (!value) return; // 押された瞬間だけ

            if (args.handType == HandType.LEFT)
            {
                Debug.Log($"{DEBUG_PREFIX} InputDrop LEFT received → ReloadAll");
                ReloadAll();
                return;
            }

            if (args.handType == HandType.RIGHT)
            {
                Debug.Log($"{DEBUG_PREFIX} InputDrop RIGHT → 540p");
                SwitchTo(url540p);
                return;
            }
        }

        // Grab: LEFT=1080p / RIGHT=2160p
        public override void InputGrab(bool value, UdonInputEventArgs args)
        {
            if (!value) return; // 押された瞬間だけ
            if (!externalControlEnabled) return;

            if (args.handType == HandType.LEFT)
            {
                Debug.Log($"{DEBUG_PREFIX} InputGrab LEFT → 1080p");
                SwitchTo(url1080p);
            }
            else if (args.handType == HandType.RIGHT)
            {
                Debug.Log($"{DEBUG_PREFIX} InputGrab RIGHT → 2160p");
                SwitchTo(url2160p);
            }
        }

        // Use: LEFT = Respawn
        public override void InputUse(bool value, UdonInputEventArgs args)
        {
            if (!value) return;                 // 押された瞬間だけ
            if (!externalControlEnabled) return;

            if (args.handType == HandType.LEFT)
            {
                Debug.Log($"{DEBUG_PREFIX} InputUse LEFT → Respawn");

                // 自分自身をリスポーン
                Networking.LocalPlayer.Respawn();
            }
        }

    }
}
