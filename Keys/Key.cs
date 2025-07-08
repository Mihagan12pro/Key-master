using Key_master.WPF.Views;
using Multicad;
using Multicad.CustomObjectBase;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.Runtime;
using System.Windows;

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
            set
            {
                if (TryModify())
                {
                    point1 = new Point3d(center.X + Length * 0.5, center.Y + value * 0.5, 0);
                    point2 = new Point3d(center.X - Length * 0.5, center.Y - value * 0.5, 0);

                    center = new Point3d((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2, (point1.Z + point2.Z) / 2);
                }
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
            set
            {
                if (TryModify())
                {
                    point1 = new Point3d(center.X + value * 0.5, center.Y + Width * 0.5, 0);
                    point2 = new Point3d(center.X - value * 0.5, center.Y - Width * 0.5, 0);

                    center = new Point3d((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2, (point1.Z + point2.Z) / 2);
                }
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


        public override hresult OnEdit(Point3d pnt, EditFlags lFlag)
        {
            EditWindow window = new EditWindow(Width,Length);

            window.ShowDialog();
            if (window.DialogResult == true)
            {
                Width = Convert.ToDouble(window.WidthTB.Text);

                Length = Convert.ToDouble(window.LengthTB.Text);
            }

            return hresult.s_Ok;
        }


        public override bool GetGripPoints(GripPointsInfo info)
        {
            info.AppendGrip
               (
                   new McSmartGrip<Key>
                       (
                           center,

                           (obj, grip, offset) => Displace(obj, center, offset)
                       )
               );


            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            point1,

                            (obj, grip, offset) => Scale(obj, point1, offset)
                        )
                );


            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            point2,

                            (obj, grip, offset) => Scale(obj, point2, offset)
                        )
                );


            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            new Point3d(point1.X, point2.Y, 0),

                            (obj, grip, offset) => Scale(obj, new Point3d(point1.X, point2.Y, 0),offset)
                        )

                );


            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            new Point3d(point2.X, point1.Y, 0),

                            (obj, grip, offset) => Scale(obj, new Point3d(point2.X, point1.Y, 0),offset)
                        )

                );

            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            new Point3d(point1.X, center.Y, 0),

                            (obj, grip, offset) => Scale(obj, new Point3d(point1.X, center.Y, 0),offset)
                        )

                );


            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            new Point3d(point2.X, center.Y, 0),

                            (obj, grip, offset) => Scale(obj, new Point3d(point2.X, center.Y, 0),offset)
                        )

                );

            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            new Point3d(center.X, point1.Y, 0),

                            (obj, grip, offset) => Scale(obj, new Point3d(center.X, point1.Y, 0), offset)
                        )

                );


            info.AppendGrip
                (
                    new McSmartGrip<Key>
                        (
                            new Point3d(center.X, point2.Y, 0),

                            (obj, grip, offset) => Scale(obj, new Point3d(center.X, point2.Y, 0), offset)
                        )

                );

            return true;
        }


        protected virtual void Displace(Key obj, Point3d grip, Vector3d offset)
        {
            if (obj.TryModify())
            {
               if (grip == obj.center)
                {
                    obj.center += offset;
                    obj.point1 += offset;
                    obj.point2 += offset;
                }
            }
        }


        protected virtual void Scale(Key obj, Point3d grip, Vector3d offset)
        {
            Point3d point1 = obj.point1;
            Point3d point2 = obj.point2;
            Point3d center = obj.center;

            Point3d point3 = new Point3d(point1.X, point2.Y, 0);
            Point3d point4 = new Point3d(point2.X, point1.Y, 0);

            Point3d rightPoint = new Point3d(point1.X, center.Y, 0);
            Point3d leftPoint = new Point3d(point2.X, center.Y, 0);
            Point3d topPoint = new Point3d(center.X, point1.Y, 0);
            Point3d bottomPoint = new Point3d(center.X, point2.Y, 0);

            if (obj.TryModify())
            {
                if (grip == obj.point1 || grip == obj.point2 || grip == point3 || grip == point4 || grip == rightPoint || grip == leftPoint || grip == topPoint || grip == bottomPoint)
                {
                    if (grip == obj.point1)
                    {
                        obj.point1 += offset;
                    }
                    else if (grip == obj.point2) 
                    {
                        obj.point2 += offset;
                    }
                
                    else if (grip == point3)
                    {
                        obj.point1 = new Point3d(point1.X + offset.X, point1.Y, 0);
                        obj.point2 = new Point3d(point2.X, point2.Y + offset.Y, 0);
                    }
                    else if (grip == point4)
                    {
                        obj.point1 = new Point3d(point1.X, point1.Y + offset.Y, 0);
                        obj.point2 = new Point3d(point2.X + offset.X, point2.Y, 0);
                    }
                    else if (grip == rightPoint)
                    {
                        obj.point1 = new Point3d(point1.X + offset.X, point1.Y, 0);
                    }
                    else if (grip == leftPoint)
                    {
                        obj.point2 = new Point3d(point2.X + offset.X, point2.Y, 0);
                    }
                    else if (grip == topPoint)
                    {
                        obj.point1 = new Point3d(point1.X, offset.Y + point1.Y, 0);
                    }
                    else if (grip == bottomPoint)
                    {
                        obj.point2 = new Point3d(point2.X, offset.Y + point2.Y, 0);
                    }

                    point1 = obj.point1;
                    point2 = obj.point2;
                }

               obj.center = new Point3d((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2, (point1.Z + point2.Z) / 2);
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

