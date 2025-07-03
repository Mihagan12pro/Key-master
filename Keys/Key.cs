using Multicad;
using Multicad.CustomObjectBase;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.Runtime;
using Multicad.Symbols.Specification;
using System;
using System.Windows;
using System.Windows.Media;

namespace Key_master.Keys
{
    [CustomEntity("1C925FA1-842B-49CD-924F-4ABF9717DB62", "Key", "Key Entity")]
    internal class Key : McCustomBase, IMcSerializable
    {
        public Vector3d WidthVector { get; protected set; }
        public Vector3d LengthVector { get; protected set; }
        public Vector3d DiagonalVector { get; protected set; }

        private Point3d _originPoint, _center;


        public override void OnDraw(GeometryBuilder dc)
        {
            dc.Clear();

            Point3d point2 =  _originPoint + DiagonalVector;

            dc.DrawPolyline(new Point3d[] { _originPoint, new Point3d(_originPoint.X, point2.Y, 0), point2, new Point3d(point2.X, _originPoint.Y, 0), _originPoint });
        }


        public override hresult PlaceObject(PlaceFlags lInsertType)
        {
            InputJig jig = new InputJig();
            jig.SetInputOptions(InputJig.InputReturnMode.Other);
            jig.ForceInputNumbers = true;

            InputJig.PropertyInpector.SetSource(this);


            jig.GetRealNumber("Длина шпоночного паза: ",out double length);
            if (length == 0)
            {
                MessageBox.Show("Длина шпоночного паза не должна быть равно нулю!","Ошибка!",MessageBoxButton.OK,MessageBoxImage.Error);
                return hresult.e_Fail;
            }
            length = Math.Abs(length);

            jig.GetRealNumber("Ширина шпоночного паза: ", out double width);
            if (width == 0)
            {
                 MessageBox.Show("Ширина шпоночного паза не должна быть равно нулю!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                 return hresult.e_Fail;
            }
            width = Math.Abs(width);

            DbEntity.AddToCurrentDocument();

            _center = jig.GetPoint("Куда вставить шпоночный паз: ").Point;

            Point3d A = new Point3d(_center.X - length * 0.5, _center.Y - width * 0.5, 0);
            Point3d B = new Point3d(_center.X + length * 0.5, _center.Y - width * 0.5, 0);
            Point3d C = new Point3d(_center.X + length * 0.5, _center.Y + width * 0.5, 0);
            Point3d D = new Point3d(_center.X - length * 0.5, _center.Y + width * 0.5, 0);

            DiagonalVector = new Vector3d(C.X - A.X, C.Y - A.Y, C.Z - A.Z);
            WidthVector = new Vector3d(D.X - A.X, D.Y - A.Y, D.Z - A.Z);
            LengthVector = new Vector3d(B.X - A.X, B.Y - A.Y, B.Z - A.Z);

            _originPoint = A;

            DbEntity.Update();
           
            return hresult.s_Ok;
        }


        public override hresult OnMcSerialization(McSerializationInfo info)
        {
            info.Add("originPoint", _originPoint);
            info.Add("center", _center);

            return hresult.s_Ok;
        }


        public override hresult OnMcDeserialization(McSerializationInfo info)
        {
            if (!info.GetValue("originPoint", out _originPoint) ||  !info.GetValue("center", out _center))
            {
                return hresult.e_Fail;
            }

            return hresult.s_Ok;
        }


        public override void OnTransform(Matrix3d tfm)
        {
            if (!TryModify())
                return;
            
            _originPoint = _originPoint.TransformBy(tfm);
            _center = _center.TransformBy(tfm);

            WidthVector = WidthVector.TransformBy(tfm);
            LengthVector = LengthVector.TransformBy(tfm);
            DiagonalVector = DiagonalVector.TransformBy(tfm);
        }


        public override bool GetGripPoints(GripPointsInfo info)
        {
            //info.AppendGrip
            //    (
            //       new McSmartGrip<Key>
            //       (
            //           _center,

            //           (obj, grip, offset) => DisplaceTransform(obj, _center, offset)
            //       )

            //    );


            //info.AppendGrip
            //    (
            //        new McSmartGrip<Key>
            //        (
            //           _originPoint,

            //           (obj, grip, offset) => ScaleTransform(obj, _originPoint, offset)
            //        )
            //    );

           
            return true;
        }


        private void DisplaceTransform(Key obj, Point3d grip, Vector3d offset)
        {
            if (obj.TryModify())
            {
                if (grip == obj._center)
                {
                    //obj._center += offset;

                    //Matrix3d matrix3dDisplacement = Matrix3d.Displacement(offset);

                    //obj._originPoint = _originPoint.TransformBy(matrix3dDisplacement);
                }
            }
        }


        private void ScaleTransform(Key obj, Point3d grip, Vector3d offset)
        {
            if (obj.TryModify())
            {
                if (grip == obj._originPoint)
                {
                    //Point3d point1 = obj._originPoint;
                    //Point3d point2 = obj._point2;

                    //Vector3d originalDiagonal = new Vector3d(point2.X - point1.X, point2.Y - point1.Y, point2.Z - point1.Z);

                    //obj._originPoint += offset;

                    //point1 = obj._originPoint;

                    //Vector3d scaledDiagonal = new Vector3d(point2.X - point1.X, point2.Y - point1.Y, point2.Z - point1.Z);

                    //double k = (scaledDiagonal.Length / originalDiagonal.Length);

                    //obj.WidthVector = k * obj.WidthVector;
                    //obj.LengthVector = k * obj.LengthVector;

                    //Console.WriteLine(obj.LengthVector);
                    //Console.WriteLine(obj.WidthVector);

                    //obj._center = new Point3d(0.5 * (point1.X + point2.X), 0.5 * (point1.Y + point2.Y), 0.5 * (point1.Z + point2.Z));
                }
            }
        }


        public bool TryModify()
        {
            TryModify(0);
            return true;
        }


        public Key()
        {
            
        }
    }
}

