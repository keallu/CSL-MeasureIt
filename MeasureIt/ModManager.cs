using ColossalFramework.UI;
using System;
using UnityEngine;

namespace MeasureIt
{
    public class ModManager : MonoBehaviour
    {
        private bool _initialized;

        private UIButton _esc;
        private NetTool _netTool;

        //private UIPanel _controlPanel;
        //private UIDragHandle _controlDragHandle;
        //private UIPanel _controlInnerPanel;
        //private UIButton[] _controlButtons;

        private UIPanel _infoPanel;
        private UIDragHandle _infoDragHandle;
        private UIPanel _infoInnerPanel;
        private UILabel[] _infoTagLabels;
        private UILabel[] _infoValueLabels;

        private readonly string[] CARDINALS = { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N" };

        public void Awake()
        {
            try
            {
                if (_esc == null)
                {
                    _esc = GameObject.Find("Esc").GetComponent<UIButton>();
                    ModProperties.Instance.ControlPanelDefaultPositionX = _esc.absolutePosition.x - 336f;
                    ModProperties.Instance.ControlPanelDefaultPositionY = _esc.absolutePosition.y;

                    ModProperties.Instance.InfoPanelDefaultPositionX = ModProperties.Instance.ControlPanelDefaultPositionX + 70f;
                    ModProperties.Instance.InfoPanelDefaultPositionY = ModProperties.Instance.ControlPanelDefaultPositionY;
                }

                if (_netTool == null)
                {
                    _netTool = FindObjectOfType<NetTool>();
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] ModManager:Awake -> Exception: " + e.Message);
            }
        }

        public void Start()
        {
            try
            {
                if (ModConfig.Instance.ControlPanelPositionX == 0.0f)
                {
                    ModConfig.Instance.ControlPanelPositionX = ModProperties.Instance.ControlPanelDefaultPositionX;
                }
                if (ModConfig.Instance.ControlPanelPositionY == 0.0f)
                {
                    ModConfig.Instance.ControlPanelPositionY = ModProperties.Instance.ControlPanelDefaultPositionY;
                }

                if (ModConfig.Instance.InfoPanelPositionX == 0.0f)
                {
                    ModConfig.Instance.InfoPanelPositionX = ModProperties.Instance.InfoPanelDefaultPositionX;
                }
                if (ModConfig.Instance.InfoPanelPositionY == 0.0f)
                {
                    ModConfig.Instance.InfoPanelPositionY = ModProperties.Instance.InfoPanelDefaultPositionY;
                }

                if (_netTool != null)
                {
                    MeasureInfo.Instance.Initialize(_netTool);
                }

                //_controlButtons = new UIButton[2];

                _infoTagLabels = new UILabel[6];
                _infoValueLabels = new UILabel[6];

                CreateUI();
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] ModManager:Start -> Exception: " + e.Message);
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
                //foreach (UIButton button in _controlButtons)
                //{
                //    Destroy(button);
                //}
                //if (_controlInnerPanel != null)
                //{
                //    Destroy(_controlInnerPanel);
                //}
                //if (_controlDragHandle != null)
                //{
                //    Destroy(_controlDragHandle);
                //}
                //if (_controlPanel != null)
                //{
                //    Destroy(_controlPanel);
                //}
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] ModManager:OnDestroy -> Exception: " + e.Message);
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
                    //if (!_controlPanel.isVisible && ModConfig.Instance.ShowControlPanel)
                    //{
                    //    _controlPanel.isVisible = true;
                    //}
                    if (!_infoPanel.isVisible && ModConfig.Instance.ShowInfoPanel)
                    {
                        _infoPanel.isVisible = true;
                    }

                    MeasureInfo.Instance.Update();
                    UpdateInfo();
                }
                else
                {
                    //_controlPanel.isVisible = false;
                    _infoPanel.isVisible = false;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] ModManager:Update -> Exception: " + e.Message);
            }
        }

        private void CreateUI()
        {
            try
            {
                //_controlPanel = UIUtils.CreatePanel("MeasureItControlPanel");
                //_controlPanel.zOrder = 0;
                //_controlPanel.backgroundSprite = "GenericPanelLight";
                //_controlPanel.color = new Color32(96, 96, 96, 255);
                //_controlPanel.size = new Vector2(62f, 98f);
                //_controlPanel.isVisible = false;

                //_controlDragHandle = UIUtils.CreateDragHandle(_controlPanel, "ControlDragHandle");
                //_controlDragHandle.size = new Vector2(_controlDragHandle.parent.width, _controlDragHandle.parent.height);
                //_controlDragHandle.relativePosition = new Vector3(0f, 0f);
                //_controlDragHandle.eventMouseUp += (component, eventParam) =>
                //{
                //    ModConfig.Instance.ControlPanelPositionX = _controlPanel.absolutePosition.x;
                //    ModConfig.Instance.ControlPanelPositionY = _controlPanel.absolutePosition.y;
                //    ModConfig.Instance.Save();
                //};

                //_controlInnerPanel = UIUtils.CreatePanel(_controlPanel, "ControlInnerPanel");
                //_controlInnerPanel.backgroundSprite = "GenericPanelLight";
                //_controlInnerPanel.color = new Color32(206, 206, 206, 255);
                //_controlInnerPanel.size = new Vector2(_controlInnerPanel.parent.width - 16f, _controlInnerPanel.parent.height - 16f);
                //_controlInnerPanel.relativePosition = new Vector3(8f, 8f);

                //for (int i = 0; i < 2; i++)
                //{
                //    UIButton button = UIUtils.CreateButton(_controlInnerPanel, "Control" + (i + 1), (i + 1).ToString());
                //    button.objectUserData = i;
                //    button.tooltip = "Unavailable";
                //    button.relativePosition = new Vector3(5f, 5f + i * 36f);
                //    button.eventClick += (component, eventParam) =>
                //    {
                //        if (!eventParam.used)
                //        {


                //            eventParam.Use();
                //        }
                //    };

                //    _controlButtons[i] = button;
                //}

                _infoPanel = UIUtils.CreatePanel("MeasureItInfoPanel");
                _infoPanel.zOrder = 0;
                _infoPanel.backgroundSprite = "GenericPanelLight";
                _infoPanel.color = new Color32(96, 96, 96, 255);
                _infoPanel.size = new Vector2(190f, 136f);
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
                _infoInnerPanel.backgroundSprite = "GenericPanelLight";
                _infoInnerPanel.color = new Color32(206, 206, 206, 255);
                _infoInnerPanel.size = new Vector2(_infoInnerPanel.parent.width - 16f, _infoInnerPanel.parent.height - 16f);
                _infoInnerPanel.relativePosition = new Vector3(8f, 8f);

                for (int i = 0; i < 6; i++)
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
                Debug.Log("[Measure It!] ModManager:CreateUI -> Exception: " + e.Message);
            }
        }

        private void UpdateUI()
        {
            try
            {
                //_controlPanel.absolutePosition = new Vector3(ModConfig.Instance.ControlPanelPositionX, ModConfig.Instance.ControlPanelPositionY);
                //_controlPanel.isVisible = ModConfig.Instance.ShowControlPanel;

                _infoPanel.absolutePosition = new Vector3(ModConfig.Instance.InfoPanelPositionX, ModConfig.Instance.InfoPanelPositionY);
                _infoPanel.isVisible = ModConfig.Instance.ShowInfoPanel;

                for (int i = 0; i < 6; i++)
                {
                    switch (i)
                    {
                        case 0:
                            _infoTagLabels[i].text = "Elevation (" + GetUnitOfDistanceSymbol(ModConfig.Instance.UnitOfDistance) + ")";
                            _infoTagLabels[i].tooltip = "Elevation is the height above sea level for start point.";
                            break;
                        case 1:
                            _infoTagLabels[i].text = "Relief (" + GetUnitOfDistanceSymbol(ModConfig.Instance.UnitOfDistance) + ")";
                            _infoTagLabels[i].tooltip = "Relief is the height difference between start and end point.";
                            break;
                        case 2:
                            _infoTagLabels[i].text = "Length (" + GetUnitOfDistanceSymbol(ModConfig.Instance.UnitOfDistance) + ")";
                            _infoTagLabels[i].tooltip = "Length is the flat distance between start (and middle) and end point.";
                            break;
                        case 3:
                            _infoTagLabels[i].text = "Distance (" + GetUnitOfDistanceSymbol(ModConfig.Instance.UnitOfDistance) + ")";
                            _infoTagLabels[i].tooltip = "Distance is the straigt distance between start (and middle) and end point.";
                            break;
                        case 4:
                            _infoTagLabels[i].text = "Slope (" + GetUnitOfSlopeSymbol(ModConfig.Instance.UnitOfSlope) + ")";
                            _infoTagLabels[i].tooltip = "Slope is the average incline between start and end point.";
                            break;
                        case 5:
                            _infoTagLabels[i].text = "Direction (" + GetUnitOfDirectionSymbol(ModConfig.Instance.UnitOfDirection) + ")";
                            _infoTagLabels[i].tooltip = "Direction is the cartographic orientation between start or middle and end point.";
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] ModManager:UpdateUI -> Exception: " + e.Message);
            }
        }

        private void UpdateInfo()
        {
            try
            {
                for (int i = 0; i < 6; i++)
                {
                    switch (i)
                    {
                        case 0:
                            _infoValueLabels[i].text = DisplayDistance(ModConfig.Instance.UnitOfDistance, MeasureInfo.Instance.Elevation);
                            break;
                        case 1:
                            _infoValueLabels[i].text = DisplayDistance(ModConfig.Instance.UnitOfDistance, MeasureInfo.Instance.Relief);
                            break;
                        case 2:
                            _infoValueLabels[i].text = DisplayDistance(ModConfig.Instance.UnitOfDistance, MeasureInfo.Instance.Length);
                            break;
                        case 3:
                            _infoValueLabels[i].text = DisplayDistance(ModConfig.Instance.UnitOfDistance, MeasureInfo.Instance.Distance);
                            break;
                        case 4:
                            _infoValueLabels[i].text = DisplaySlope(ModConfig.Instance.UnitOfSlope, MeasureInfo.Instance.Slope);
                            break;
                        case 5:
                            _infoValueLabels[i].text = DisplayDirection(ModConfig.Instance.UnitOfDirection, MeasureInfo.Instance.Direction);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] ModManager:UpdateInfo -> Exception: " + e.Message);
            }
        }

        private string GetUnitOfDistanceSymbol(int unitOfDistance)
        {
            try
            {
                switch (unitOfDistance)
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
                        return "?";
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] ModManager:GetUnitOfDistanceSymbol -> Exception: " + e.Message);
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
                        return "?";
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] ModManager:GetUnitOfSlopeSymbol -> Exception: " + e.Message);
                return string.Empty;
            }
        }

        private string GetUnitOfDirectionSymbol(int unitOfDirection)
        {
            try
            {
                switch (unitOfDirection)
                {
                    case 0:
                        return "°";
                    case 1:
                        return "°";
                    default:
                        return "?";
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] ModManager:GetUnitOfDirectionSymbol -> Exception: " + e.Message);
                return string.Empty;
            }
        }

        private float ConvertDistance(int unitOfDistance, float distance)
        {
            try
            {
                switch (unitOfDistance)
                {
                    case 0:
                        return distance;
                    case 1:
                        return distance;
                    case 2:
                        return distance / 0.9144f;
                    case 3:
                        return distance / 0.3048f;
                    default:
                        return distance;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] ModManager:ConvertDistance -> Exception: " + e.Message);
                return 0f;
            }
        }

        private string DisplayDistance(int unitOfDistance, float distance)
        {
            try
            {
                return $"{ConvertDistance(unitOfDistance, distance):0.##}";
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] ModManager:DisplayDistance -> Exception: " + e.Message);
                return string.Empty;
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
                Debug.Log("[Measure It!] ModManager:ConvertSlope -> Exception: " + e.Message);
                return 0f;
            }
        }

        private string DisplaySlope(int unitOfSlope, float slope)
        {
            try
            {
                return $"{ConvertSlope(unitOfSlope, slope):0.##}";
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] ModManager:DisplaySlope -> Exception: " + e.Message);
                return string.Empty;
            }
        }

        private string DisplayDirection(int unitOfDirection, float direction)
        {
            try
            {
                switch (unitOfDirection)
                {
                    case 0:
                        return $"{direction:0.##}";
                    case 1:
                        return CARDINALS[(int)Math.Round(direction % 360 / 45)];
                    default:
                        return $"{direction:0.##}";
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] ModManager:DisplayDirection -> Exception: " + e.Message);
                return string.Empty;
            }
        }
    }
}