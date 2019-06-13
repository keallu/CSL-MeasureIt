using ICities;
using System;
using UnityEngine;

namespace MeasureIt
{
    public class Loading : LoadingExtensionBase
    {
        private GameObject _measureManagerGameObject;

        public override void OnLevelLoaded(LoadMode mode)
        {
            try
            {
                _measureManagerGameObject = new GameObject("MeasureItMeasureManager");
                _measureManagerGameObject.AddComponent<ToggleManager>();
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] Loading:OnLevelLoaded -> Exception: " + e.Message);
            }
        }

        public override void OnLevelUnloading()
        {
            try
            {
                if (_measureManagerGameObject != null)
                {
                    UnityEngine.Object.Destroy(_measureManagerGameObject);
                }

            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] Loading:OnLevelUnloading -> Exception: " + e.Message);
            }
        }
    }
}