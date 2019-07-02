using ICities;
using System;
using UnityEngine;

namespace MeasureIt
{
    public class Loading : LoadingExtensionBase
    {
        private GameObject _modManagerGameObject;

        public override void OnLevelLoaded(LoadMode mode)
        {
            try
            {
                ToolController toolController = UnityEngine.Object.FindObjectOfType<ToolController>();
                if (toolController != null)
                {
                    MeasureTool.Instance = toolController.gameObject.AddComponent<MeasureTool>();
                }

                _modManagerGameObject = new GameObject("MeasureItModManager");
                _modManagerGameObject.AddComponent<ModManager>();
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
                if (_modManagerGameObject != null)
                {
                    UnityEngine.Object.Destroy(_modManagerGameObject);
                }

                if (MeasureTool.Instance != null)
                {
                    UnityEngine.Object.Destroy(MeasureTool.Instance);
                }

            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] Loading:OnLevelUnloading -> Exception: " + e.Message);
            }
        }
    }
}