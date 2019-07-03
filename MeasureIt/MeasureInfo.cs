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
        public float Radius;
        public float Slope;
        public float Direction;

        private TerrainManager _terrainManager;

        private MeasureTool _measureTool;

        private NetTool _netTool;
        private FieldInfo _controlPointCountField;
        private FieldInfo _controlPointsField;
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

        public void Initialize(MeasureTool measureTool, NetTool netTool)
        {
            try
            {
                _terrainManager = Singleton<TerrainManager>.instance;

                _measureTool = measureTool;
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
                if (_measureTool.enabled)
                {
                    int pointCount = _measureTool.PointCount;

                    _startPosition = _measureTool.StartPosition;
                    _startHeight = _terrainManager.SampleRawHeightSmooth(_measureTool.StartPosition);

                    Elevation = _startHeight - _terrainManager.WaterSimulation.m_currentSeaLevel;

                    if (pointCount == 1)
                    {
                        _endPosition = _measureTool.EndPosition;
                        _endHeight = _terrainManager.SampleRawHeightSmooth(_measureTool.EndPosition);

                        Length = VectorUtils.LengthXZ(_endPosition - _startPosition);
                        Distance = Vector3.Distance(_endPosition, _startPosition);
                        Direction = CalculateAngle(Vector3.down, VectorUtils.XZ(_endPosition - _startPosition));
                    }

                    Relief = pointCount > 0 ? _endHeight - _startHeight : 0f;
                    Length = pointCount > 0 ? Length : 0f;
                    Distance = pointCount > 0 ? Distance : 0f;
                    Radius = 0f;
                    Slope = pointCount > 0 ? (Length > 0f ? Relief / Length : 0f) : 0f;
                    Direction = pointCount > 0 ? Direction : 0f;
                }
                else if (_netTool.enabled)
                {
                    int controlPointCount = (int)_controlPointCountField.GetValue(_netTool);

                    if (_controlPoints == null)
                    {
                        _controlPoints = _controlPointsField.GetValue(_netTool) as NetTool.ControlPoint[];
                    }

                    _startPosition = _controlPoints[0].m_position;
                    _startHeight = _terrainManager.SampleRawHeightSmooth(_controlPoints[0].m_position) + _controlPoints[0].m_elevation;

                    Elevation = _startHeight - _terrainManager.WaterSimulation.m_currentSeaLevel;

                    if (controlPointCount == 1)
                    {
                        _endPosition = _controlPoints[1].m_position;
                        _endHeight = _terrainManager.SampleRawHeightSmooth(_controlPoints[1].m_position) + _controlPoints[1].m_elevation;
                        Length = VectorUtils.LengthXZ(_endPosition - _startPosition);
                        Distance = Vector3.Distance(_endPosition, _startPosition);
                        Radius = 0f;
                        Direction = CalculateAngle(Vector3.down, VectorUtils.XZ(_endPosition - _startPosition));
                    }
                    else if (controlPointCount == 2)
                    {
                        _bendPosition = _controlPoints[1].m_position;
                        _endPosition = _controlPoints[2].m_position;
                        _endHeight = _terrainManager.SampleRawHeightSmooth(_controlPoints[2].m_position) + _controlPoints[2].m_elevation;
                        CalculateMiddlePoints(_startPosition, _bendPosition, _endPosition, out Vector3 middle1Position, out Vector3 middle2Position, out Vector3 middle3Position);
                        Length = CalculateLength(_startPosition, middle1Position, middle2Position, middle3Position, _endPosition);
                        Distance = CalculateDistance(_startPosition, middle1Position, middle2Position, middle3Position, _endPosition);
                        Radius = CalculateRadius(_startPosition, middle2Position, _endPosition);
                        Direction = CalculateAngle(Vector3.down, VectorUtils.XZ(_endPosition - _bendPosition));
                    }

                    Relief = controlPointCount > 0 ? _endHeight - _startHeight : 0f;
                    Length = controlPointCount > 0 ? Length : 0f;
                    Distance = controlPointCount > 0 ? Distance : 0f;
                    Radius = controlPointCount > 0 ? Radius : 0f;
                    Slope = controlPointCount > 0 ? (Length > 0f ? Relief / Length : 0f) : 0f;
                    Direction = controlPointCount > 0 ? Direction : 0f;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] MeasureInfo:Update -> Exception: " + e.Message);
            }
        }

        private void CalculateMiddlePoints(Vector3 start, Vector3 bend, Vector3 end, out Vector3 middle1Position, out Vector3 middle2Position, out Vector3 middle3Position)
        {
            middle1Position = Vector3.Lerp(Vector3.Lerp(start, bend, 0.25f),
                                           Vector3.Lerp(bend, end, 0.25f),
                                           0.25f);

            middle2Position = Vector3.Lerp(Vector3.Lerp(start, bend, 0.5f),
                                           Vector3.Lerp(bend, end, 0.5f),
                                           0.5f);

            middle3Position = Vector3.Lerp(Vector3.Lerp(start, bend, 0.75f),
                                           Vector3.Lerp(bend, end, 0.75f),
                                           0.75f);
        }

        private float CalculateLength(Vector3 start, Vector3 middle1, Vector3 middle2, Vector3 middle3, Vector3 end)
        {
            float distance = 0f;

            distance += VectorUtils.LengthXZ(middle1 - start);
            distance += VectorUtils.LengthXZ(middle2 - middle1);
            distance += VectorUtils.LengthXZ(middle3 - middle2);
            distance += VectorUtils.LengthXZ(end - middle3);

            return distance;
        }

        private float CalculateDistance(Vector3 start, Vector3 middle1, Vector3 middle2, Vector3 middle3, Vector3 end)
        {
            float length = 0f;

            length += Vector3.Distance(start, middle1);
            length += Vector3.Distance(middle1, middle2);
            length += Vector3.Distance(middle2, middle3);
            length += Vector3.Distance(middle3, end);

            return length;
        }

        private float CalculateRadius(Vector3 start, Vector3 middle, Vector3 end)
        {
            float area = Triangle3.Area(start, middle, end);
            float sideLength1 = Vector3.Distance(start, middle);
            float sideLength2 = Vector3.Distance(middle, end);
            float sideLength3 = Vector3.Distance(end, start);

            return 1 / (4 * area / (sideLength1 * sideLength2 * sideLength3));
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
