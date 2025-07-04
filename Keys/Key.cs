using Multicad;
using Multicad.CustomObjectBase;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.Runtime;
using Multicad.Symbols.Specification;
using Multicad.Wpf.Controls;
using System;
using System.Windows;
using System.Windows.Media;

namespace Key_master.Keys
{
    [CustomEntity("1C925FA1-842B-49CD-924F-4ABF9717DB62", "Key", "Key Entity")]
    internal class Key : McCustomBase, IMcSerializable
    {
        protected Point3d point1, point2, center;

        public double Width
        {
            get
            {
                Point3d point3 = new Point3d(point1.X, point2.Y, 0);

                Vector3d widthVector = new Vector3d(point3.X - point1.X, point3.Y - point1.Y,0);

                return widthVector.Length;
            }
        }


        public double Length
        {
            get
            {
                Point3d point4 = new Point3d(point2.X, point1.Y, 0);

                Vector3d lengthVector = new Vector3d(point4.X - point1.X, point4.Y - point1.Y, 0);

                return lengthVector.Length;
            }
        }

        public override void OnDraw(GeometryBuilder dc)
        {
            dc.Clear();



            dc.DrawPolyline(new Point3d[] { point1, new Point3d(point1.X, point2.Y, 0), point2, new Point3d(point2.X, point1.Y, 0), point1 });
        }


        public override hresult PlaceObject(PlaceFlags lInsertType)
        {
            InputJig jig = new InputJig();
            jig.SetInputOptions(InputJig.InputReturnMode.Other);
            jig.ForceInputNumbers = true;

            InputJig.PropertyInpector.SetSource(this);


            jig.GetRealNumber("Длина шпоночного паза: ", out double length);
            if (length == 0)
            {
                MessageBox.Show("Длина шпоночного паза не должна быть равно нулю!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
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

            center = jig.GetPoint("Куда вставить шпоночный паз: ").Point;

            point1 = new Point3d(center.X + length * 0.5, center.Y + width * 0.5, 0);
            point2 = new Point3d(center.X - length * 0.5, center.Y - width * 0.5, 0);

            //point3 = new Point3d(point1.X, point2.Y,0);
            //point4 = new Point3d(point2.X, point1.Y, 0);

            

            DbEntity.Update();

            return hresult.s_Ok;
        }


        public override hresult OnMcSerialization(McSerializationInfo info)
        {
            info.Add("point1", point1);
            info.Add("point2", point2);
            info.Add("center", center);

            return hresult.s_Ok;
        }

        public override hresult OnMcDeserialization(McSerializationInfo info)
        {
            if (!info.GetValue("point1", out point1) || !info.GetValue("point2", out point2) || !info.GetValue("center",out center))
            {
                return hresult.e_Fail;
            }

            return hresult.s_Ok;
        }


        public override void OnTransform(Matrix3d tfm)
        {
            if (!TryModify())
                return;

            point1 = point1.TransformBy(tfm);

            point2 = point1.TransformBy(tfm);

            center = new Point3d((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2, (point1.Z + point2.Z) / 2);
        }





        public override bool GetGripPoints(GripPointsInfo info)
        {


            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            point2,
                            (obj, grip, offset) =>
                            {
                                if (obj.TryModify())
                                {
                                    obj.point2 += offset;

                                    obj.center = new Point3d((obj.point1.X + obj.point2.X) / 2, (obj.point1.Y + obj.point2.Y) / 2, (obj.point1.Z + obj.point2.Z) / 2);
                                }
                            }
                        )
                );
            info.AppendGrip
               (
                   new McSmartGrip<Key>
                       (
                           center,
                           (obj, grip, offset) =>
                           {
                               if (obj.TryModify())
                               {
                                   obj.center += offset;
                                   obj.point1 += offset;
                                   obj.point2 += offset;
                               }
                           }
                       )
               );


            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            point1,
                            (obj, grip, offset) =>
                            {
                                if (obj.TryModify())
                                {
                                    obj.point1 += offset;

                                    obj.center = new Point3d((obj.point1.X + obj.point2.X) / 2, (obj.point1.Y + obj.point2.Y) / 2, (obj.point1.Z + obj.point2.Z) / 2);
                                }
                            }
                        )
                );

            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            new Point3d(point1.X, point2.Y, 0),
                            (obj, grip, offset) =>
                            {
                                if (obj.TryModify())
                                {
                                    obj.point1 = new Point3d(obj.point1.X + offset.X, obj.point1.Y,0);
                                    obj.point2 = new Point3d(obj.point2.X,obj.point2.Y + offset.Y,0);

                                    obj.center = new Point3d((obj.point1.X + obj.point2.X) / 2, (obj.point1.Y + obj.point2.Y) / 2, (obj.point1.Z + obj.point2.Z) / 2);
                                }
                            }
                        )

                );


            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            new Point3d(point2.X, point1.Y, 0),
                            (obj, grip, offset) =>
                            {
                                if (obj.TryModify())
                                {
                                    obj.point1 = new Point3d(obj.point1.X, obj.point1.Y + offset.Y, 0);
                                    obj.point2 = new Point3d(obj.point2.X + offset.X, obj.point2.Y, 0);

                                    obj.center = new Point3d((obj.point1.X + obj.point2.X) / 2, (obj.point1.Y + obj.point2.Y) / 2, (obj.point1.Z + obj.point2.Z) / 2);
                                }
                            }
                        )

                );

            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            new Point3d(point1.X, center.Y, 0),
                            (obj, grip, offset) =>
                            {
                                if (obj.TryModify())
                                {
                                    obj.point1 = new Point3d(obj.point1.X + offset.X, obj.point1.Y, 0);

                                    obj.center = new Point3d((obj.point1.X + obj.point2.X) / 2, (obj.point1.Y + obj.point2.Y) / 2, (obj.point1.Z + obj.point2.Z) / 2);
                                }
                            }
                        )

                );

            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            new Point3d(point2.X, center.Y, 0),
                            (obj, grip, offset) =>
                            {
                                if (obj.TryModify())
                                {
                                    obj.point2 = new Point3d(obj.point2.X + offset.X, obj.point2.Y, 0);

                                    obj.center = new Point3d((obj.point1.X + obj.point2.X) / 2, (obj.point1.Y + obj.point2.Y) / 2, (obj.point1.Z + obj.point2.Z) / 2);
                                }
                            }
                        )

                );

            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            new Point3d(center.X, point1.Y, 0),
                            (obj, grip, offset) =>
                            {
                                if (obj.TryModify())
                                {
                                    obj.point1 = new Point3d(obj.point1.X,offset.Y + obj.point1.Y, 0);

                                    obj.center = new Point3d((obj.point1.X + obj.point2.X) / 2, (obj.point1.Y + obj.point2.Y) / 2, (obj.point1.Z + obj.point2.Z) / 2);
                                }
                            }
                        )

                );

            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            new Point3d(center.X, point2.Y, 0),
                            (obj, grip, offset) =>
                            {
                                if (obj.TryModify())
                                {
                                    obj.point2 = new Point3d(obj.point2.X, offset.Y + obj.point2.Y, 0);

                                    obj.center = new Point3d((obj.point1.X + obj.point2.X) / 2, (obj.point1.Y + obj.point2.Y) / 2, (obj.point1.Z + obj.point2.Z) / 2);
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

