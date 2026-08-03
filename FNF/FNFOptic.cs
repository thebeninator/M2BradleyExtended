using System.Collections.Generic;
using UnityEngine;
using GHPC.Player;
using GHPC.Utility;

namespace M2BradleyExtended.FNF
{
    internal class FNFOptic : MonoBehaviour
    {
        public FNFManager manager;
        public RectTransform gates;
        public RectTransform manual_gates;
        public Transform seek_text;
        public Transform reticles;
        public Transform seeker_box;
        public Transform point_targeting;
        public Dictionary<FNFMode, Transform> modes;
        public GameObject seeker_post;
        public GameObject main_post;

        void Awake()
        {
            manager.SeekerToggled += OnSeekerToggled;
            manager.JustFired += OnFired;
            manager.ModeChanged += OnModeChanged;
            manager.PointTargetingToggled += OnPointTargetingToggled;
            manager.LostTrack += OnLostTrack;
        }

        void OnModeChanged(FNFMode old_mode, FNFMode new_mode)
        {
            modes[old_mode].gameObject.SetActive(false);
            modes[new_mode].gameObject.SetActive(true);
        }

        void OnPointTargetingToggled(bool enabled)
        {
            point_targeting.gameObject.SetActive(enabled);
            manual_gates.gameObject.SetActive(manager.seeker_active);
            reticles.gameObject.SetActive(!manager.seeker_active);
        }

        void OnSeekerToggled(bool enabled)
        {
            reticles.gameObject.SetActive(!enabled);
            seek_text.gameObject.SetActive(enabled);
            seeker_box.gameObject.SetActive(enabled);
            seeker_post.SetActive(enabled);
            main_post.SetActive(!enabled);
            manual_gates.gameObject.SetActive(enabled);
            ResetGates();
        }

        void OnLostTrack()
        {
            reticles.gameObject.SetActive(!manager.seeker_active);
            ResetGates(true);
        }

        void OnFired()
        {
            OnSeekerToggled(false);
        }

        void Update()
        {
            if (!manager.seeker_active) return;

            Vector2 local_positon = manual_gates.transform.localPosition;
            Vector2 size_delta = manual_gates.sizeDelta;

            if (M2Ext.alternative_tracking_gate_controls.Value)
            {
                AlternativeHandleKeyboardInput();
            }
            else
            {
                HandleKeyboardInput();
            }

            if (!manager.point_targeting && manager.target && !manager.target_locked)
            {
                ResolveLock();
            }
        }

        public void ResetGates(bool force_manual_enabled = false)
        {
            gates.gameObject.SetActive(false);
            gates.sizeDelta = new Vector2(1f, 1f);

            if (force_manual_enabled)
            {
                manual_gates.gameObject.SetActive(true);
            }
            manual_gates.sizeDelta = new Vector2(175f, 175f);
            manual_gates.transform.localPosition = new Vector2(-87.5f, -87.5f);
        }

        private void HandleKeyboardInput()
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                UpdateTrackingGateDim(0f, 144f);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                UpdateTrackingGateDim(0f, -144f);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                UpdateTrackingGateDim(144f, 0f);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                UpdateTrackingGateDim(-144f, 0f);
            }
        }

        private void AlternativeHandleKeyboardInput()
        {
            if (PlayerInput.Instance.IsExteriorMode) return;

            if (InputUtil.MainPlayer.GetButton("Movement Y Pos"))
            {
                UpdateTrackingGateDim(0f, 144f);
            }

            if (InputUtil.MainPlayer.GetButton("Movement Y Neg"))
            {
                UpdateTrackingGateDim(0f, -144f);
            }

            if (InputUtil.MainPlayer.GetButton("Movement X Pos"))
            {
                UpdateTrackingGateDim(144f, 0f);
            }

            if (InputUtil.MainPlayer.GetButton("Movement X Neg"))
            {
                UpdateTrackingGateDim(-144f, 0f);
            }
        }
        private void ResolveLock()
        {
            float gate_x = gates.sizeDelta.x;
            float gate_y = gates.sizeDelta.y;
            float diff_x = gate_x * 0.1f;
            float diff_y = gate_y * 0.1f;
            float manual_gate_x = manual_gates.sizeDelta.x;
            float manual_gate_y = manual_gates.sizeDelta.y;

            bool acceptable_x = (manual_gate_x <= gate_x + diff_x * 4f) && (manual_gate_x >= gate_x - diff_x * 4.5f);
            bool acceptable_y = (manual_gate_y <= gate_y + diff_y * 4f) && (manual_gate_y >= gate_y - diff_y * 4.5f);

            if (acceptable_x && acceptable_y)
            {
                manager.SetTargetLocked(true);
                gates.gameObject.SetActive(true);
                manual_gates.gameObject.SetActive(false);
                reticles.gameObject.SetActive(true);
            }
        }

        private void UpdateTrackingGateDim(float dx, float dy) 
        {
            Vector2 local_positon = manual_gates.transform.localPosition;
            Vector2 size_delta = manual_gates.sizeDelta;

            float new_x = size_delta.x + dx * Time.deltaTime;
            float new_y = size_delta.y + dy * Time.deltaTime;

            if (new_x <= 20f) new_x = 20f;
            if (new_y <= 20f) new_y = 20f;

            float factor_x = size_delta.x / new_x;
            float factor_y = size_delta.y / new_y;
            
            manual_gates.sizeDelta = new Vector2(new_x, new_y);
            manual_gates.transform.localPosition = new Vector2
            (
                local_positon.x / factor_x, 
                local_positon.y / factor_y
            );
        }
    }
}
