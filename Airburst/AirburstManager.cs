using GHPC;
using GHPC.Player;
using GHPC.Utility;
using GHPC.Weapons;
using UnityEngine;

namespace M2BradleyExtended.Airburst
{
    internal class AirburstManager : MonoBehaviour
    {
        public bool AirburstActive { get; set; }
        public int? AmmoKeyIdx { get; set; }
        public AmmoType AirburstAmmo { get; set; }
        public GameObject AirburstUIIndicator { get; set; }

        private WeaponSystem weapon_system;
        private AmmoFeed feed;
        private int unit_id;
        private bool airburst_ammo_loaded = false;
        private static GameObject airburst_text_ui;

        private void OnAmmoTypeChanged(AmmoType ammo)
        {
            airburst_ammo_loaded = ammo.CachedIndex == AirburstAmmo.CachedIndex;
            if (!airburst_ammo_loaded)
            {
                SetUIIndicatorActive(false);
            }
        }

        private void OnPlayerUnitChanged(Unit unit)
        {
            AirburstActive = false;
            this.enabled = unit.GetInstanceID() == unit_id;
        }

        private void SetUIIndicatorActive(bool enabled)
        {
            if (AirburstUIIndicator)
            {
                AirburstUIIndicator.SetActive(enabled);
            }
        }

        void Awake()
        {
            if (airburst_text_ui != null) return;

            airburst_text_ui = GameObject.Instantiate(Assets.airburst_text_ui, GameObject.Find("_APP_GHPC_").transform.Find("UIHUDCanvas/weapons text"));
            airburst_text_ui.transform.localPosition = new Vector3(50f, -10f, 0f);

            airburst_text_ui.SetActive(false);
        }

        void Start()
        {
            weapon_system = this.GetComponent<WeaponSystem>();
            feed = weapon_system.Feed;
            unit_id = weapon_system._unit.GetInstanceID();

            weapon_system.FCS.AmmoTypeChanged += OnAmmoTypeChanged;
            PlayerInput.Instance.PlayerUnitChangedByUnit += OnPlayerUnitChanged;
            this.enabled = PlayerInput.Instance.CurrentPlayerUnit.GetInstanceID() == unit_id;
        }

        void Update()
        {
            if (AmmoKeyIdx == null) return;
            if (!airburst_ammo_loaded && 
                feed.QueuedClipType.MinimalPattern[0].AmmoType.CachedIndex != AirburstAmmo.CachedIndex) return;

            if (InputUtil.MainPlayer.GetButtonDoublePressDown(PlayerInput.ammoKeys[AmmoKeyIdx.Value]))
            {
                AirburstActive = !AirburstActive;
                airburst_text_ui.SetActive(AirburstActive);
                SetUIIndicatorActive(AirburstActive);
            }
        }
    }
}
