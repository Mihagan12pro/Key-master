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
        public double Length { get; set; }

        public double Width { get; set; }

        private Point3d _point1, _point2, _center;


        public override void OnDraw(GeometryBuilder dc)
        {
            dc.Clear();

       

            dc.DrawPolyline(new Point3d[] { _point1, new Point3d(_point1.X, _point2.Y, 0), _point2, new Point3d(_point2.X, _point1.Y, 0), _point1 });
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
            Length = Math.Abs(length);

            jig.GetRealNumber("Ширина шпоночного паза: ", out double width);
            if (width == 0)
            {
                 MessageBox.Show("Ширина шпоночного паза не должна быть равно нулю!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                 return hresult.e_Fail;
            }
            Width = Math.Abs(width);

            DbEntity.AddToCurrentDocument();

            _center = jig.GetPoint("Куда вставить шпоночный паз: ").Point;

            _point1= new Point3d(_center.X + Length * 0.5, _center.Y + Width * 0.5, 0);
            _point2 = new Point3d(_center.X - Length * 0.5, _center.Y - Width * 0.5, 0);

        

            DbEntity.Update();
           
            return hresult.s_Ok;
        }


        public override hresult OnMcSerialization(McSerializationInfo info)
        {
            info.Add("point1", _point1);
            info.Add("point2", _point2);
            info.Add("center", _center);

            return hresult.s_Ok;
        }

        public override hresult OnMcDeserialization(McSerializationInfo info)
        {
            if (!info.GetValue("point1", out _point1) || !info.GetValue("point2", out _point2) || !info.GetValue("center", out _center))
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

            _point2 = _point1.TransformBy(tfm);

            _center = _center.TransformBy(tfm);
        }



        

        public override bool GetGripPoints(GripPointsInfo info)
        {
            info.AppendGrip
                (
                   new McSmartGrip<Key>(
                           _center,

                           (obj, grip, offset) =>
                           {
                               if (obj.TryModify())
                               {
                                   obj._center += offset;

                                   Matrix3d matrix3dDisplacement = Matrix3d.Displacement(offset);

                                   obj._point1 = _point1.TransformBy(matrix3dDisplacement);
                                   obj._point2 = _point2.TransformBy(matrix3dDisplacement);
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

