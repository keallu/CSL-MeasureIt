using System;
using UnityEngine;

namespace MeasureIt
{
    public class MeasureProperties
    {
        public float ControlPanelDefaultPositionX;
        public float ControlPanelDefaultPositionY;
        public float InfoPanelDefaultPositionX;
        public float InfoPanelDefaultPositionY;

        private static MeasureProperties instance;

        public static MeasureProperties Instance
        {
            get
            {
                return instance ?? (instance = new MeasureProperties());
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
                Debug.Log("[Measure It!] MeasureProperties:ResetControlPanelPosition -> Exception: " + e.Message);
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
                Debug.Log("[Measure It!] MeasureProperties:ResetInfoPanelPosition -> Exception: " + e.Message);
            }
        }
    }
}
