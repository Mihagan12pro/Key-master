using Multicad;
using Multicad.CustomObjectBase;
using Multicad.Geometry;
using Multicad.Runtime;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace Key_master.Keys
{
    [CustomEntity("1C925FA1-842B-49CD-924F-4ABF9717DB62", "Key", "Key Entity")]
    internal class Key : McCustomBase, IMcSerializable
    {
        public double Length { get; set; }

        public double Width { get; set; }

        private Point3d _point1, _point2, _centerOfKey;

        private Point3d _topPoint, _bottomPoint;

        private Point3d _rightPoint, _leftPoint;

        public delegate void MoveGripDelegate(Point3d gripPoint, Vector3d offset);


       
        public override void OnDraw(GeometryBuilder dc)
        {
            dc.Clear();

            dc.DrawPolyline
            (
                new Point3d[] 
                {
                    _point1,
                    new Point3d(_point1.X, _point2.Y, 0),
                    _point2,
                    new Point3d(_point2.X, _point1.Y, 0),
                    _point1
                }
            );

            _centerOfKey = new Point3d(0.5 * (_point1.X + _point2.X), 0.5 * (_point1.Y + _point2.Y), 0.5 * (_point1.Z + _point2.Z));
        }

        public override hresult OnMcSerialization(McSerializationInfo info)
        {
            info.Add("point1", _point1);
            info.Add("point2", _point2);

            return hresult.s_Ok;
        }

        public override hresult OnMcDeserialization(McSerializationInfo info)
        {
            if (!info.GetValue("point1",out _point1) || !info.GetValue("point2",out _point2))
            {
                return hresult.e_Fail;
            }

            return hresult.s_Ok;
        }


        public override void OnTransform(Matrix3d tfm)
        {
            if (!TryModify())
                return;

            _point1 = _point1.TransformBy(tfm);
            _point2 = _point2.TransformBy(tfm);

            _centerOfKey = _centerOfKey.TransformBy(tfm);

            _topPoint = _topPoint.TransformBy(tfm);
            _bottomPoint = _bottomPoint.TransformBy(tfm);

            _rightPoint = _rightPoint.TransformBy(tfm);
            _leftPoint = _leftPoint.TransformBy(tfm);
        }


        public override bool GetGripPoints(GripPointsInfo info)
        {
            McSmartGrip<Key> centerGrip = new McSmartGrip<Key>(_centerOfKey, (obj,grip,offset) => CenterMove(obj,_centerOfKey,offset));

            McSmartGrip<Key> scaleTop = new McSmartGrip<Key>(_topPoint,(obj,grip,offset) => ScaleWidth(obj,_topPoint,offset));
            McSmartGrip<Key> scaleBottom = new McSmartGrip<Key>(_bottomPoint,(obj,grip,offset) => ScaleWidth(obj,_bottomPoint,offset));
           
            McSmartGrip<Key> scaleRight = new McSmartGrip<Key>(_rightPoint, (obj, grip, offset) => ScaleLength(obj, _rightPoint, offset));
            McSmartGrip<Key> scaleLeft = new McSmartGrip<Key>(_leftPoint, (obj,grip,offset) => ScaleLength(obj,_leftPoint,offset));

            info.AppendGrip(centerGrip);

            info.AppendGrip(scaleTop);
            info.AppendGrip(scaleBottom);

            info.AppendGrip(scaleRight);
            info.AppendGrip(scaleLeft);


            return true;
        }

        private void ScaleWidth(Key obj, Point3d gripPoint, Vector3d offset)
        {
            if (obj.TryModify())
            {
                if (gripPoint == _topPoint)
                {
                    _topPoint = new Point3d(_topPoint.X, _topPoint.Y + offset.Y, 0);
                    _point2 = new Point3d(_point2.X, _point2.Y + offset.Y, 0);

                    _rightPoint = new Point3d(_point1.X, (_point1.Y + _point2.Y) / 2, 0);
                    _leftPoint = new Point3d(_point2.X, (_point1.Y + _point2.Y) / 2, 0);
                    _bottomPoint = new Point3d((_point1.X + _point2.X) / 2, _point1.Y, 0);
                }
                else if (gripPoint == _bottomPoint)
                {
                    _bottomPoint = new Point3d(_bottomPoint.X, _bottomPoint.Y + offset.Y, 0);
                    _point1 = new Point3d(_point1.X, _point1.Y + offset.Y, 0);

                    _rightPoint = new Point3d(_point1.X, (_point1.Y + _point2.Y) / 2, 0);
                    _leftPoint = new Point3d(_point2.X, (_point1.Y + _point2.Y) / 2, 0);
                    _topPoint = new Point3d((_point1.X + _point2.X) / 2, _point2.Y, 0);
                }
            }

            Invalidate();
        }


        private void ScaleLength(Key obj, Point3d gripPoint, Vector3d offset)
        {
            if (obj.TryModify())
            {
                if (gripPoint == _rightPoint)
                {
                    _rightPoint = new Point3d(_rightPoint.X + offset.X, _rightPoint.Y,0);
                    _point1 = new Point3d(_point1.X + offset.X, _point1.Y, 0);

                    _topPoint = new Point3d((_point1.X + _point2.X) / 2, _point2.Y, 0);
                    _leftPoint = new Point3d(_point2.X, (_point1.Y + _point2.Y) / 2, 0);
                    _bottomPoint = new Point3d((_point1.X + _point2.X) / 2, _point1.Y, 0);
                }
                else if (gripPoint == _leftPoint)
                {
                    _leftPoint = new Point3d(_leftPoint.X + offset.X, _leftPoint.Y,0);
                    _point2 = new Point3d(_point2.X + offset.X, _point2.Y,0);

                    _topPoint = new Point3d((_point1.X + _point2.X) / 2, _point2.Y, 0);
                    _rightPoint = new Point3d(_point1.X, (_point1.Y + _point2.Y) / 2, 0);
                    _bottomPoint = new Point3d((_point1.X + _point2.X) / 2, _point1.Y, 0);
                }
            }
            Invalidate();
        }
        
        private void CenterMove(Key obj, Point3d gripPoint, Vector3d offset)
        {
            if (obj.TryModify())
            {
               if (gripPoint == _centerOfKey)
               {
                    Vector3d vector = _centerOfKey.GetVectorTo(_centerOfKey + offset);


                    Matrix3d matrixDisplacement = Matrix3d.Displacement(vector);


                    obj.OnTransform(matrixDisplacement);
                }
            }

            Invalidate();
        }

         
        public bool TryModify()
        {
            TryModify(0);
            return true;
        }


        public Key()
        {
            _point1 = new Point3d(50, 50, 0);
            _point2 = new Point3d(350, 150, 0);

            _topPoint = new Point3d((_point1.X + _point2.X) / 2, _point2.Y,0);
            _bottomPoint = new Point3d((_point1.X + _point2.X) / 2, _point1.Y, 0);

            _rightPoint = new Point3d(_point1.X, (_point1.Y + _point2.Y) / 2, 0);
            _leftPoint = new Point3d(_point2.X, (_point1.Y + _point2.Y) / 2, 0);
        }
    }
}

