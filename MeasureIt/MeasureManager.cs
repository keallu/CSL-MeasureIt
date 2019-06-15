using ColossalFramework.UI;
using System;
using UnityEngine;

namespace MeasureIt
{
    public class MeasureManager : MonoBehaviour
    {
        private bool _initialized;

        private UIButton _esc;
        private NetTool _netTool;

        private UIPanel _controlPanel;
        private UIDragHandle _controlDragHandle;
        private UIPanel _controlInnerPanel;
        private UIButton[] _controlButtons;

        private UIPanel _infoPanel;
        private UIDragHandle _infoDragHandle;
        private UIPanel _infoInnerPanel;
        private UILabel[] _infoTagLabels;
        private UILabel[] _infoValueLabels;

        public void Awake()
        {
            try
            {
                if (_esc == null)
                {
                    _esc = GameObject.Find("Esc").GetComponent<UIButton>();
                    MeasureProperties.Instance.ControlPanelDefaultPositionX = _esc.absolutePosition.x - 550f;
                    MeasureProperties.Instance.ControlPanelDefaultPositionY = _esc.absolutePosition.y;

                    MeasureProperties.Instance.InfoPanelDefaultPositionX = MeasureProperties.Instance.ControlPanelDefaultPositionX + 175f;
                    MeasureProperties.Instance.InfoPanelDefaultPositionY = MeasureProperties.Instance.ControlPanelDefaultPositionY;
                }

                if (_netTool == null)
                {
                    _netTool = FindObjectOfType<NetTool>();
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
                if (ModConfig.Instance.ControlPanelPositionX == 0.0f)
                {
                    ModConfig.Instance.ControlPanelPositionX = MeasureProperties.Instance.ControlPanelDefaultPositionX;
                }
                if (ModConfig.Instance.ControlPanelPositionY == 0.0f)
                {
                    ModConfig.Instance.ControlPanelPositionY = MeasureProperties.Instance.ControlPanelDefaultPositionY;
                }

                if (ModConfig.Instance.InfoPanelPositionX == 0.0f)
                {
                    ModConfig.Instance.InfoPanelPositionX = MeasureProperties.Instance.InfoPanelDefaultPositionX;
                }
                if (ModConfig.Instance.InfoPanelPositionY == 0.0f)
                {
                    ModConfig.Instance.InfoPanelPositionY = MeasureProperties.Instance.InfoPanelDefaultPositionY;
                }

                if (_netTool != null)
                {
                    MeasureInfo.Instance.Initialize(_netTool);
                }

                _controlButtons = new UIButton[4];

                _infoTagLabels = new UILabel[4];
                _infoValueLabels = new UILabel[4];

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
                foreach (UILabel label in _infoValueLabels)
                {
                    Destroy(label);
                }
                foreach (UILabel label in _infoTagLabels)
                {
                    Destroy(label);
                }
                if (_infoInnerPanel != null)
                {
                    Destroy(_infoInnerPanel);
                }
                if (_infoDragHandle != null)
                {
                    Destroy(_infoDragHandle);
                }
                if (_infoPanel != null)
                {
                    Destroy(_infoPanel);
                }
                foreach (UIButton button in _controlButtons)
                {
                    Destroy(button);
                }
                if (_controlInnerPanel != null)
                {
                    Destroy(_controlInnerPanel);
                }
                if (_controlDragHandle != null)
                {
                    Destroy(_controlDragHandle);
                }
                if (_controlPanel != null)
                {
                    Destroy(_controlPanel);
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

                if (_netTool != null && _netTool.enabled && _netTool.m_mode != NetTool.Mode.Upgrade)
                {
                    if (!_controlPanel.isVisible)
                    {
                        _controlPanel.isVisible = true;
                    }
                    if (!_infoPanel.isVisible)
                    {
                        _infoPanel.isVisible = true;
                    }

                    MeasureInfo.Instance.Update();
                    UpdateInfo();
                }
                else
                {
                    _controlPanel.isVisible = false;
                    _infoPanel.isVisible = false;
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
                _controlPanel = UIUtils.CreatePanel("MeasureItControlPanel");
                _controlPanel.zOrder = 0;
                _controlPanel.backgroundSprite = "SubcategoriesPanel";
                _controlPanel.size = new Vector2(170f, 62f);
                _controlPanel.isVisible = false;

                _controlDragHandle = UIUtils.CreateDragHandle(_controlPanel, "ControlDragHandle");
                _controlDragHandle.size = new Vector2(_controlDragHandle.parent.width, _controlDragHandle.parent.height);
                _controlDragHandle.relativePosition = new Vector3(0f, 0f);
                _controlDragHandle.eventMouseUp += (component, eventParam) =>
                {
                    ModConfig.Instance.ControlPanelPositionX = _controlPanel.absolutePosition.x;
                    ModConfig.Instance.ControlPanelPositionY = _controlPanel.absolutePosition.y;
                    ModConfig.Instance.Save();
                };

                _controlInnerPanel = UIUtils.CreatePanel(_controlPanel, "ControlInnerPanel");
                _controlInnerPanel.backgroundSprite = "GenericPanel";
                _controlInnerPanel.size = new Vector2(_controlInnerPanel.parent.width - 16f, _controlInnerPanel.parent.height - 16f);
                _controlInnerPanel.relativePosition = new Vector3(8f, 8f);

                for (int i = 0; i < 4; i++)
                {
                    UIButton button = UIUtils.CreateButton(_controlInnerPanel, "Control" + (i + 1), (i + 1).ToString());
                    button.objectUserData = i;
                    button.tooltip = $"Zone radius effect: {i + 1} x {i + 1}";
                    button.relativePosition = new Vector3(5f + i * 36f, 5f);
                    button.eventClick += (component, eventParam) =>
                    {
                        if (!eventParam.used)
                        {
                            ModConfig.Instance.Cells = (int)button.objectUserData + 1;
                            UpdateButtons(ModConfig.Instance.Cells);
                            ModConfig.Instance.Save();

                            eventParam.Use();
                        }
                    };

                    _controlButtons[i] = button;
                }

                _infoPanel = UIUtils.CreatePanel("MeasureItInfoPanel");
                _infoPanel.zOrder = 0;
                _infoPanel.backgroundSprite = "SubcategoriesPanel";
                _infoPanel.size = new Vector2(190f, 110f);
                _infoPanel.isVisible = false;

                _infoDragHandle = UIUtils.CreateDragHandle(_infoPanel, "InfoDragHandle");
                _infoDragHandle.size = new Vector2(_infoDragHandle.parent.width, _infoDragHandle.parent.height);
                _infoDragHandle.relativePosition = new Vector3(0f, 0f);
                _infoDragHandle.eventMouseUp += (component, eventParam) =>
                {
                    ModConfig.Instance.InfoPanelPositionX = _infoPanel.absolutePosition.x;
                    ModConfig.Instance.InfoPanelPositionY = _infoPanel.absolutePosition.y;
                    ModConfig.Instance.Save();
                };

                _infoInnerPanel = UIUtils.CreatePanel(_infoPanel, "InfoInnerPanel");
                _infoInnerPanel.backgroundSprite = "GenericPanel";
                _infoInnerPanel.size = new Vector2(_infoInnerPanel.parent.width - 16f, _infoInnerPanel.parent.height - 16f);
                _infoInnerPanel.relativePosition = new Vector3(8f, 8f);

                for (int i = 0; i < 4; i++)
                {
                    _infoTagLabels[i] = UIUtils.CreateLabel(_infoInnerPanel, "InfoTagLabel" + i, "");
                    _infoTagLabels[i].textScale = 0.625f;
                    _infoTagLabels[i].relativePosition = new Vector3(16f, 16f * (i + 1));

                    _infoValueLabels[i] = UIUtils.CreateLabel(_infoInnerPanel, "InfoValueLabel" + i, "");
                    _infoValueLabels[i].textScale = 0.625f;
                    _infoValueLabels[i].relativePosition = new Vector3(_infoInnerPanel.width / 2f + 16f, 16f * (i + 1));
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
                _controlPanel.absolutePosition = new Vector3(ModConfig.Instance.ControlPanelPositionX, ModConfig.Instance.ControlPanelPositionY);
                _controlPanel.isVisible = ModConfig.Instance.ShowControlPanel;

                UpdateButtons(ModConfig.Instance.Cells);

                _infoPanel.absolutePosition = new Vector3(ModConfig.Instance.InfoPanelPositionX, ModConfig.Instance.InfoPanelPositionY);
                _infoPanel.isVisible = ModConfig.Instance.ShowInfoPanel;

                for (int i = 0; i < 4; i++)
                {
                    switch (i)
                    {
                        case 0:
                            _infoTagLabels[i].text = "Elevation (" + GetUnitOfLengthSymbol(ModConfig.Instance.UnitOfLength) + ")";
                            _infoTagLabels[i].tooltip = "Elevation is the height above sea level for start point.";
                            break;
                        case 1:
                            _infoTagLabels[i].text = "Relief (" + GetUnitOfLengthSymbol(ModConfig.Instance.UnitOfLength) + ")";
                            _infoTagLabels[i].tooltip = "Relief is the height difference between start and end point.";
                            break;
                        case 2:
                            _infoTagLabels[i].text = "Distance (" + GetUnitOfLengthSymbol(ModConfig.Instance.UnitOfLength) + ")";
                            _infoTagLabels[i].tooltip = "Distance is the flat distance between start and end point.";
                            break;
                        case 3:
                            _infoTagLabels[i].text = "Slope (" + GetUnitOfSlopeSymbol(ModConfig.Instance.UnitOfSlope) + ")";
                            _infoTagLabels[i].tooltip = "Slope is the average incline between start and end point.";
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureManager:UpdateUI -> Exception: " + e.Message);
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
                        _controlButtons[i].normalBgSprite = "OptionBaseFocused";
                    }
                    else
                    {
                        _controlButtons[i].normalBgSprite = "OptionBase";
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureManager:UpdateButtons -> Exception: " + e.Message);
            }
        }

        private void UpdateInfo()
        {
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    switch (i)
                    {
                        case 0:
                            _infoValueLabels[i].text = $"{ConvertLength(ModConfig.Instance.UnitOfLength, MeasureInfo.Instance.Elevation):0.##}";
                            break;
                        case 1:
                            _infoValueLabels[i].text = $"{ConvertLength(ModConfig.Instance.UnitOfLength, MeasureInfo.Instance.Relief):0.##}";
                            break;
                        case 2:
                            _infoValueLabels[i].text = $"{ConvertLength(ModConfig.Instance.UnitOfLength, MeasureInfo.Instance.Distance):0.##}";
                            break;
                        case 3:
                            _infoValueLabels[i].text = $"{ConvertSlope(ModConfig.Instance.UnitOfSlope, MeasureInfo.Instance.Slope):0.##}";
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureManager:UpdateInfo -> Exception: " + e.Message);
            }
        }

        private string GetUnitOfLengthSymbol(int unitOfLength)
        {
            try
            {
                switch (unitOfLength)
                {
                    case 0:
                        return "u";
                    case 1:
                        return "m";
                    case 2:
                        return "yd";
                    case 3:
                        return "ft";
                    default:
                        return "u";
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureManager:GetUnitOfLengthSymbol -> Exception: " + e.Message);
                return string.Empty;
            }
        }

        private string GetUnitOfSlopeSymbol(int unitOfSlope)
        {
            try
            {
                switch (unitOfSlope)
                {
                    case 0:
                        return "°";
                    case 1:
                        return "%";
                    default:
                        return "°";
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureManager:GetUnitOfSlopeSymbol -> Exception: " + e.Message);
                return string.Empty;
            }
        }

        private float ConvertLength(int unitOfLength, float length)
        {
            try
            {
                switch (unitOfLength)
                {
                    case 0:
                        return length;
                    case 1:
                        return length;
                    case 2:
                        return length / 0.9144f;
                    case 3:
                        return length / 0.3048f;
                    default:
                        return length;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureManager:ConvertLength -> Exception: " + e.Message);
                return 0f;
            }
        }

        private float ConvertSlope(int unitOfSlope, float slope)
        {
            try
            {
                switch (unitOfSlope)
                {
                    case 0:
                        return (float)(Math.Atan(slope) * (180 / Math.PI));
                    case 1:
                        return slope * 100;
                    default:
                        return slope;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureManager:ConvertSlope -> Exception: " + e.Message);
                return 0f;
            }
        }
    }
}
