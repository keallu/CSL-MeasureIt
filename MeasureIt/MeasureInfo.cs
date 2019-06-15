using ColossalFramework;
using ColossalFramework.Math;
using System;
using System.Reflection;
using UnityEngine;

namespace MeasureIt
{
    public class MeasureInfo
    {
        public float Elevation;
        public float Relief;
        public float Distance;
        public float Slope;

        private TerrainManager _terrainManager;

        private NetTool _netTool;
        private FieldInfo _controlPointCountField;
        private FieldInfo _controlPointsField;

        private int _controlPointCount;
        private NetTool.ControlPoint[] _controlPoints;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private float _startHeight;
        private float _endHeight;

        private static MeasureInfo instance;

        public static MeasureInfo Instance
        {
            get
            {
                return instance ?? (instance = new MeasureInfo());
            }
        }

        public void Initialize(NetTool netTool)
        {
            try
            {
                _terrainManager = Singleton<TerrainManager>.instance;

                _netTool = netTool;
                _controlPointCountField = netTool.GetType().GetField("m_controlPointCount", BindingFlags.NonPublic | BindingFlags.Instance);
                _controlPointsField = netTool.GetType().GetField("m_controlPoints", BindingFlags.NonPublic | BindingFlags.Instance);                
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureInfo:Initialize -> Exception: " + e.Message);
            }
        }

        public void Update()
        {
            try
            {
                int controlPointCount = (int)_controlPointCountField.GetValue(_netTool);

                if (_controlPoints == null)
                {
                    _controlPoints = _controlPointsField.GetValue(_netTool) as NetTool.ControlPoint[];
                }

                _startPosition = _controlPoints[0].m_position;
                _startHeight = _terrainManager.SampleRawHeightSmooth(_controlPoints[0].m_position) + _controlPoints[0].m_elevation;

                if (_controlPointCount == 1)
                {
                    _endPosition = _controlPoints[1].m_position;
                    _endHeight = _terrainManager.SampleRawHeightSmooth(_controlPoints[1].m_position) + _controlPoints[1].m_elevation;
                }
                else if (_controlPointCount == 2)
                {
                    _endPosition = _controlPoints[2].m_position;
                    _endHeight = _terrainManager.SampleRawHeightSmooth(_controlPoints[2].m_position) + _controlPoints[2].m_elevation;
                }

                Elevation = _startHeight - _terrainManager.WaterSimulation.m_currentSeaLevel;
                Relief = _controlPointCount > 0 ? _endHeight - _startHeight : 0f;
                Distance = _controlPointCount > 0 ? VectorUtils.LengthXZ(_startPosition - _endPosition) : 0f;
                Slope = _controlPointCount > 0 ? (Distance > 0f ? Relief / Distance : 0f) : 0f;

                _controlPointCount = controlPointCount;
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureInfo:Update -> Exception: " + e.Message);
            }
        }
    }
}
