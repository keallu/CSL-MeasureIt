using ColossalFramework.UI;
using System;
using UnityEngine;

namespace MeasureIt
{
    public class ToggleManager : MonoBehaviour
    {
        private bool _initialized;

        private UIButton _esc;

        private UIPanel _measurePanel;
        private UIDragHandle _measureDragHandle;
        private UIPanel _measureInnerPanel;
        private UIButton[] _measureButtons;

        public void Awake()
        {
            try
            {
                if (_esc == null)
                {
                    _esc = GameObject.Find("Esc").GetComponent<UIButton>();
                    ToggleProperties.Instance.PanelDefaultPositionX = _esc.absolutePosition.x - 550f;
                    ToggleProperties.Instance.PanelDefaultPositionY = _esc.absolutePosition.y;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureManager:Awake -> Exception: " + e.Message);
            }
        }

        public void Start()
        {
            try
            {
                if (ModConfig.Instance.PositionX == 0.0f)
                {
                    ModConfig.Instance.PositionX = ToggleProperties.Instance.PanelDefaultPositionX;
                }

                if (ModConfig.Instance.PositionY == 0.0f)
                {
                    ModConfig.Instance.PositionY = ToggleProperties.Instance.PanelDefaultPositionY;
                }

                _measureButtons = new UIButton[4];

                CreateUI();
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureManager:Start -> Exception: " + e.Message);
            }
        }

        public void OnDestroy()
        {
            try
            {
                foreach (UIButton button in _measureButtons)
                {
                    Destroy(button);
                }
                if (_measureInnerPanel != null)
                {
                    Destroy(_measureInnerPanel);
                }
                if (_measureDragHandle != null)
                {
                    Destroy(_measureDragHandle);
                }
                if (_measurePanel != null)
                {
                    Destroy(_measurePanel);
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureManager:OnDestroy -> Exception: " + e.Message);
            }
        }

        public void Update()
        {
            try
            {
                if (!_initialized || ModConfig.Instance.ConfigUpdated)
                {
                    UpdateUI();

                    _initialized = true;
                    ModConfig.Instance.ConfigUpdated = false;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureManager:Update -> Exception: " + e.Message);
            }
        }

        private void CreateUI()
        {
            try
            {
                _measurePanel = UIUtils.CreatePanel("MeasureItMeasurePanel");
                _measurePanel.zOrder = 0;
                _measurePanel.backgroundSprite = "SubcategoriesPanel";
                _measurePanel.size = new Vector2(170f, 62f);

                _measureDragHandle = UIUtils.CreateDragHandle(_measurePanel, "MeasureDragHandle");
                _measureDragHandle.size = new Vector2(_measureDragHandle.parent.width, _measureDragHandle.parent.height);
                _measureDragHandle.relativePosition = new Vector3(0f, 0f);
                _measureDragHandle.eventMouseUp += (component, eventParam) =>
                {
                    ModConfig.Instance.PositionX = _measurePanel.absolutePosition.x;
                    ModConfig.Instance.PositionY = _measurePanel.absolutePosition.y;
                    ModConfig.Instance.Save();
                };

                _measureInnerPanel = UIUtils.CreatePanel(_measurePanel, "MeasureInnerPanel");
                _measureInnerPanel.backgroundSprite = "GenericPanel";
                _measureInnerPanel.size = new Vector2(_measureInnerPanel.parent.width - 16f, _measureInnerPanel.parent.height - 16f);
                _measureInnerPanel.relativePosition = new Vector3(8f, 8f);

                for (int i = 0; i < 4; i++)
                {
                    UIButton button = UIUtils.CreateButton(_measureInnerPanel, "Measure" + (i + 1), (i + 1).ToString());
                    button.objectUserData = i;
                    button.tooltip = CreateTooltip(i + 1);
                    button.relativePosition = new Vector3(5f + i * 36f, 5f);
                    button.eventClick += (component, eventParam) =>
                    {
                        if (!eventParam.used)
                        {
                            ModConfig.Instance.Cells = (int)button.objectUserData + 1;
                            UpdateButtons(ModConfig.Instance.Cells);

                            eventParam.Use();
                        }
                    };

                    _measureButtons[i] = button;
                }

                UpdateUI();
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureManager:CreateUI -> Exception: " + e.Message);
            }
        }

        private void UpdateUI()
        {
            try
            {
                _measurePanel.absolutePosition = new Vector3(ModConfig.Instance.PositionX, ModConfig.Instance.PositionY);
                _measurePanel.isVisible = ModConfig.Instance.ShowMeasurePanel;

                UpdateButtons(ModConfig.Instance.Cells);
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureManager:UpdateUI -> Exception: " + e.Message);
            }
        }

        private string CreateTooltip(int cells)
        {
            try
            {
                string tooltip = $"Zone radius effect: {cells} x {cells}";

                return tooltip;
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureManager:CreateTooltip -> Exception: " + e.Message);
                return string.Empty;
            }
        }

        private void UpdateButtons(int selected)
        {
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    if ((i + 1) == selected)
                    {
                        _measureButtons[i].normalBgSprite = "OptionBaseFocused";
                    }
                    else
                    {
                        _measureButtons[i].normalBgSprite = "OptionBase";
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureManager:UpdateButtons -> Exception: " + e.Message);
            }
        }
    }
}
