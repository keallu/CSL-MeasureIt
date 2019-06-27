using System;
using UnityEngine;

namespace MeasureIt
{
    public class ModProperties
    {
        public float ControlPanelDefaultPositionX;
        public float ControlPanelDefaultPositionY;
        public float InfoPanelDefaultPositionX;
        public float InfoPanelDefaultPositionY;

        private static ModProperties instance;

        public static ModProperties Instance
        {
            get
            {
                return instance ?? (instance = new ModProperties());
            }
        }

        public void ResetControlPanelPosition()
        {
            try
            {
                ModConfig.Instance.ControlPanelPositionX = ControlPanelDefaultPositionX;
                ModConfig.Instance.ControlPanelPositionY = ControlPanelDefaultPositionY;
                ModConfig.Instance.Save();
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] ModProperties:ResetControlPanelPosition -> Exception: " + e.Message);
            }
        }

        public void ResetInfoPanelPosition()
        {
            try
            {
                ModConfig.Instance.InfoPanelPositionX = InfoPanelDefaultPositionX;
                ModConfig.Instance.InfoPanelPositionY = InfoPanelDefaultPositionY;
                ModConfig.Instance.Save();
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] ModProperties:ResetInfoPanelPosition -> Exception: " + e.Message);
            }
        }
    }
}
