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
        public float Length;
        public float Distance;
        public float Slope;
        public float Direction;

        private TerrainManager _terrainManager;

        private NetTool _netTool;
        private FieldInfo _controlPointCountField;
        private FieldInfo _controlPointsField;

        private int _controlPointCount;
        private NetTool.ControlPoint[] _controlPoints;
        private Vector3 _startPosition;
        private Vector3 _bendPosition;
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

                Elevation = _startHeight - _terrainManager.WaterSimulation.m_currentSeaLevel;

                if (_controlPointCount == 1)
                {
                    _endPosition = _controlPoints[1].m_position;
                    _endHeight = _terrainManager.SampleRawHeightSmooth(_controlPoints[1].m_position) + _controlPoints[1].m_elevation;
                    Length = VectorUtils.LengthXZ(_endPosition - _startPosition); 
                    Distance = Vector3.Distance(_endPosition, _startPosition);
                    Direction = CalculateAngle(Vector3.down, VectorUtils.XZ(_endPosition - _startPosition));
                }
                else if (_controlPointCount == 2)
                {
                    _bendPosition = _controlPoints[1].m_position;
                    _endPosition = _controlPoints[2].m_position;
                    _endHeight = _terrainManager.SampleRawHeightSmooth(_controlPoints[2].m_position) + _controlPoints[2].m_elevation;
                    NetSegment.CalculateMiddlePoints(_controlPoints[0].m_position, _controlPoints[0].m_direction, _controlPoints[2].m_position, _controlPoints[2].m_direction, false, false, out Vector3 middle1Position, out Vector3 middle2Position);
                    Length = CalculateDistance(_startPosition, middle1Position, middle2Position, _endPosition);
                    Distance = CalculateLength(_startPosition, middle1Position, middle2Position, _endPosition); 
                    Direction = CalculateAngle(Vector3.down, VectorUtils.XZ(_endPosition - _bendPosition));
                }

                Relief = _controlPointCount > 0 ? _endHeight - _startHeight : 0f;
                Slope = _controlPointCount > 0 ? (Length > 0f ? Relief / Length : 0f) : 0f;

                _controlPointCount = controlPointCount;
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureInfo:Update -> Exception: " + e.Message);
            }
        }

        private float CalculateLength(Vector3 start, Vector3 middle1, Vector3 middle2, Vector3 end)
        {
            float distance = 0f;

            distance += VectorUtils.LengthXZ(middle1 - start);
            distance += VectorUtils.LengthXZ(middle2 - middle1);
            distance += VectorUtils.LengthXZ(end - middle2);

            return distance;
        }

        private float CalculateDistance(Vector3 start, Vector3 middle1, Vector3 middle2, Vector3 end)
        {
            float length = 0f;

            length += Vector3.Distance(start, middle1);
            length += Vector3.Distance(middle1, middle2);
            length += Vector3.Distance(middle2, end);

            return length;
        }

        private float CalculateAngle(Vector3 from, Vector3 to)
        {
            float angle = Vector3.Angle(from, to);
            Vector3 cross = Vector3.Cross(from, to);

            if (cross.z > 0)
            {
                angle = 360 - angle;
            }

            return angle;
        }
    }
}
