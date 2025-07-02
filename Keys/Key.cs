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
        public double Length { get; set; }

        public double Width { get; set; }

        private Point3d _point,_center;
        private Vector3d _diagonal;

        


        public Point3d Origin
        {
            get
            {
                return _point;
            }
            set
            {
                if (!TryModify()) 
                    return;

                _point = value;
            }
        }


        public override void OnDraw(GeometryBuilder dc)
        {
            dc.Clear();

            Point3d point2 = _point + _diagonal;

            dc.DrawPolyline(new Point3d[] { _point, new Point3d(_point.X, point2.Y, 0), point2, new Point3d(point2.X, _point.Y, 0), _point });
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

            _point= new Point3d(_center.X + Length * 0.5, _center.Y + Width * 0.5, 0);
            Point3d point2 = new Point3d(_center.X - Length * 0.5, _center.Y - Width * 0.5, 0);

            _diagonal = new Vector3d(point2.X - _point.X,point2.Y - _point.Y,point2.Z - _point.Z);

            DbEntity.Update();
           
            return hresult.s_Ok;
        }


        public override void OnTransform(Matrix3d tfm)
        {
            if (!TryModify())
                return;
            
            _point = _point.TransformBy(tfm);

            _center = _center.TransformBy(tfm);

            _diagonal = _diagonal.TransformBy(tfm);
        }


        public override hresult write(McStream S)
        {
            S.PutVersion(1, 0);

            S.PutPoint(_point);
            S.PutPoint(_center);
            S.PutVector(_diagonal);

            S.WriteVersionBlockEnd();

            return hresult.s_Ok;
        }


        public override hresult read(McStream S)
        {
            short major, minor;
            if (!S.GetVersion(out major, out minor))
                return hresult.e_MakeMeProxy;

            if (major == 1)
            {
                if (!S.GetPoint(out _point))
                    return hresult.e_Fail;
                if (!S.GetVector(out _diagonal))
                    return hresult.e_Fail;
                if (!S.GetPoint(out _center))
                    return hresult.e_Fail;
            }

            if (!S.ReadVersionBlockEnd())
                return hresult.e_Fail;
            return hresult.s_Ok;
        }


        public override bool GetGripPoints(GripPointsInfo info)
        {
            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            _center, 
                            (obj, grip, offset) => {
                                obj.TryModify();
                                Vector3d vector = _center.GetVectorTo(_center + offset); 


                                Matrix3d matrixDisplacement = Matrix3d.Displacement(vector);


                                obj.OnTransform(matrixDisplacement);

                                Invalidate();
                            })
                );

            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            _point,
                            (obj,grip,offset) => { 
                                obj.TryModify(); 
                                obj._diagonal += offset; 
                            }));

            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            _point + _diagonal, 
                            (obj, grip, offset) => { 
                                obj.TryModify(); 
                                obj._diagonal += offset; 
                            }));

            return true;
        }


        public override bool SupportMcStreamSerialization()
        {
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

