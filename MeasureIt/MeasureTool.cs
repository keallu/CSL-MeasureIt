using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.UI;
using System;
using System.Collections;
using UnityEngine;

namespace MeasureIt
{
    public class MeasureTool : ToolBase
    {
        public static MeasureTool Instance { get; set; }

        public int PointCount { get; set; }
        public Vector3 StartPosition { get; set; }
        public Vector3 EndPosition { get; set; }

        private UIComponent _pauseMenu;
        private Ray _mouseRay;
        private float _mouseRayLength;
        private Vector3 _hitPosition;

        private const string TOOL_INFO = "Add measure point";

        protected override void Awake()
        {
            try
            {
                base.Awake();

                enabled = false;

                _pauseMenu = UIView.library.Get("PauseMenu");
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureTool:Awake -> Exception: " + e.Message);
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                enabled = false;

                base.OnDestroy();
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureTool:OnDestroy -> Exception: " + e.Message);
            }
        }

        protected override void OnEnable()
        {
            try
            {
                base.OnEnable();

                m_toolController.ClearColliding();
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureTool:OnEnable -> Exception: " + e.Message);
            }
        }

        protected override void OnDisable()
        {
            try
            {
                base.OnDisable();

                ToolCursor = null;
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureTool:OnDisable -> Exception: " + e.Message);
            }
        }

        protected override void OnToolGUI(Event eventParam)
        {
            try
            {
                base.OnToolGUI(eventParam);

                if (m_toolController.IsInsideUI || eventParam.type != 0)
                {
                    return;
                }

                if (eventParam.button == 0)
                {
                    Singleton<SimulationManager>.instance.AddAction(AddPosition(_hitPosition));
                }
                else if (eventParam.button == 1)
                {
                    Singleton<SimulationManager>.instance.AddAction(RemoveLastPosition());
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureTool:OnToolGUI -> Exception: " + e.Message);
            }
        }

        protected override void OnToolUpdate()
        {
            try
            {
                base.OnToolUpdate();

                if (_pauseMenu != null && _pauseMenu.isVisible)
                {
                    ToolsModifierControl.SetTool<DefaultTool>();

                    UIView.library.Hide("PauseMenu");
                }

                ShowToolInfo(true, TOOL_INFO, _hitPosition);

            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureTool:OnToolUpdate -> Exception: " + e.Message);
            }
        }

        protected override void OnToolLateUpdate()
        {
            try
            {
                base.OnToolLateUpdate();

                _mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                _mouseRayLength = Camera.main.farClipPlane;
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureTool:OnToolLateUpdate -> Exception: " + e.Message);
            }
        }

        public override void SimulationStep()
        {
            try
            {
                base.SimulationStep();

                RaycastInput input = new RaycastInput(_mouseRay, _mouseRayLength);
                if (RayCast(input, out RaycastOutput output))
                {
                    _hitPosition = output.m_hitPos;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureTool:SimulationStep -> Exception: " + e.Message);
            }
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            try
            {
                base.RenderOverlay(cameraInfo);

                Color color = new Color(1f, 0f, 1f, 0.75f);

                Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(cameraInfo, color, _hitPosition, 25f, _hitPosition.y - 100f, _hitPosition.y + 100f, false, true);

                if (StartPosition != Vector3.zero)
                {
                    Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(cameraInfo, color, StartPosition, 25f, StartPosition.y - 100f, StartPosition.y + 100f, false, true);
                }
                if (EndPosition != Vector3.zero)
                {
                    Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(cameraInfo, color, EndPosition, 25f, EndPosition.y - 100f, EndPosition.y + 100f, false, true);
                }
                if (StartPosition != Vector3.zero && EndPosition != Vector3.zero)
                {
                    var minHeight = Mathf.Min(StartPosition.y, EndPosition.y);
                    var maxHeight = Mathf.Max(StartPosition.y, EndPosition.y);

                    Singleton<RenderManager>.instance.OverlayEffect.DrawSegment(cameraInfo, color, new Segment3(StartPosition, EndPosition), 1f, 5f, minHeight - 20f, maxHeight + 20f, false, true);
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureTool:RenderOverlay -> Exception: " + e.Message);
            }
        }

        public IEnumerator AddPosition(Vector3 position)
        {
            try
            {
                if (StartPosition == Vector3.zero)
                {
                    StartPosition = position;
                    PointCount = 0;
                }
                else if (EndPosition == Vector3.zero)
                {
                    EndPosition = position;
                    PointCount = 1;
                }
                else
                {
                    StartPosition = EndPosition;
                    EndPosition = position;
                    PointCount = 1;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureTool:AddPosition -> Exception: " + e.Message);
            }

            yield return 0;
        }

        public IEnumerator RemoveLastPosition()
        {
            try
            {
                if (PointCount == 1)
                {
                    EndPosition = Vector3.zero;
                    PointCount = 0;
                }
                else
                {
                    StartPosition = Vector3.zero;
                    PointCount = 0;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureTool:RemoveLastPosition -> Exception: " + e.Message);
            }

            yield return 0;
        }
    }
}
