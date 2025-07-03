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
    internal class Key : McCustomBase, IMcStreamSerializable
    {
        public Vector3d WidthVector;
        public Vector3d LengthVector;
        public Vector3d DiagonalVector;

        protected Point3d _originPoint, _center;


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


        public override hresult read(McStream stream)
        {
            short major, minor;
            if (!stream.GetVersion(out major, out minor))
                return hresult.e_MakeMeProxy;

            if (!stream.GetPoint(out _center))
                return hresult.e_Fail;
            if (!stream.GetPoint(out _originPoint))
                return hresult.e_Fail;

            if (!stream.GetVector(out WidthVector))
                return hresult.e_Fail;
            if (!stream.GetVector(out LengthVector))
                return hresult.e_Fail;
            if (!stream.GetVector(out DiagonalVector))
                return hresult.e_Fail;

            return hresult.s_Ok;
        }


        public override hresult write(McStream stream)
        {
            stream.PutVersion(1, 0);

            stream.PutVector(DiagonalVector);
            stream.PutVector(LengthVector);
            stream.PutVector(WidthVector);

            stream.PutPoint(_originPoint);
            stream.PutPoint(_center);

            return hresult.s_Ok;
        }


        public override bool SupportMcStreamSerialization()
        {
            return true;
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
            info.AppendGrip
                (
                   new McSmartGrip<Key>
                   (
                       _center,

                       (obj, grip, offset) =>
                       {
                           if (obj.TryModify())
                           {
                               obj._center += offset;

                               Matrix3d matrix3dDisplacement = Matrix3d.Displacement(offset);

                               //obj._originPoint = _originPoint.TransformBy(matrix3dDisplacement);

                               obj.DiagonalVector = obj.DiagonalVector.TransformBy(matrix3dDisplacement);
                               obj.WidthVector = obj.WidthVector.TransformBy(matrix3dDisplacement);
                               obj.LengthVector = obj.LengthVector.TransformBy(matrix3dDisplacement);

                               obj._originPoint = obj._originPoint.TransformBy(matrix3dDisplacement); 
                           }
                       }
                   )

                );


            info.AppendGrip
                (
                    new McSmartGrip<Key>
                    (
                       _originPoint + DiagonalVector,

                       (obj, grip, offset) =>
                       {
                           if (obj.TryModify())
                           {
                               obj.DiagonalVector += offset;
                               Matrix3d matrix3dDisplacement = Matrix3d.Displacement(offset);

                               obj._center = obj._center.TransformBy(matrix3dDisplacement);
                           }
                       }
                    )
                );


            return true;
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

