using Key_master.WPF.Views;
using Multicad;
using Multicad.CustomObjectBase;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.Runtime;
using System.Globalization;
using System.Windows;

namespace Key_master.Keys
{
    [CustomEntity("1C925FA1-842B-49CD-924F-4ABF9717DB62", "Key2", "Key Type 2 Entity")]
    internal class KeyTypeTwo : KeyBasic
    {
        public override double Width
        {
            get
            {
                Point3d point3 = new Point3d(point1.X, point2.Y, 0);

                Vector3d widthVector = new Vector3d(point3.X - point1.X, point3.Y - point1.Y, 0);

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


        public override double Length
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
                    point1 = new Point3d(center.X + value * 0.5 -  Width * 0.5, center.Y + Width * 0.5, 0);
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
                   new McSmartGrip<KeyTypeTwo>
                       (
                           center,

                           (obj, grip, offset) => Displace(obj, center, offset)
                       )
               );


            info.AppendGrip
                (
                    new McSmartGrip<KeyTypeTwo>
                        (

                            point1,

                            (obj, grip, offset) => { 
                                obj.TryModify(); obj.Point1 += offset; 
                                Scale(obj, obj.Point1, offset); 
                            }

                        )
                );


            info.AppendGrip
                (
                    new McSmartGrip<KeyTypeTwo>
                        (

                            point2,

                            (obj, grip, offset) => {

                                obj.Point2 += offset; 
                                Scale(obj, obj.Point2, offset); 

                            }

                        )
                );


            info.AppendGrip
                (
                    new McSmartGrip<KeyTypeTwo>
                        (

                            new Point3d(point1.X, point2.Y, 0),


                            (obj, grip, offset) => { 

                                obj.Point1 = new Point3d(obj.Point1.X + offset.X, obj.Point1.Y, 0);
                                obj.Point2 = new Point3d(obj.Point2.X, obj.Point2.Y + offset.Y, 0); 
                                Scale(obj, new Point3d(obj.Point1.X, obj.Point2.Y, 0), offset); 

                            }

                        )

                );


            info.AppendGrip
                (
                    new McSmartGrip<KeyTypeTwo>
                        (

                            new Point3d(point2.X, point1.Y, 0),

                            (obj, grip, offset) => {

                                obj.Point1 = new Point3d(obj.Point1.X, obj.Point1.Y + offset.Y, 0);
                                obj.Point2 = new Point3d(obj.Point2.X + offset.X, obj.Point2.Y, 0); 
                                Scale(obj, new Point3d(obj.Point2.X, obj.Point1.Y, 0), offset); 

                            }

                        )

                );

            info.AppendGrip
                (
                    new McSmartGrip<KeyTypeTwo>
                        (

                            new Point3d(point1.X, center.Y, 0),

                            (obj, grip, offset) => {

                                obj.Point1 = new Point3d(obj.Point1.X + offset.X, obj.Point1.Y, 0);
                                Scale(obj, new Point3d(obj.Point1.X, obj.Center.Y, 0), offset);

                            }
                        
                        )

                );


            info.AppendGrip
                (
                    new McSmartGrip<KeyTypeTwo>
                        (

                            new Point3d(point2.X, center.Y, 0),

                            (obj, grip, offset) => {

                                obj.Point2 = new Point3d(obj.Point2.X + offset.X, obj.Point2.Y, 0);
                                Scale(obj, new Point3d(obj.Point2.X, obj.Center.Y, 0), offset); 

                            }

                        )

                );

            info.AppendGrip
                (
                    new McSmartGrip<KeyTypeTwo>
                        (

                            new Point3d(center.X, point1.Y, 0),

                            (obj, grip, offset) => {

                                obj.Point1 = new Point3d(obj.Point1.X, offset.Y + obj.Point1.Y, 0);
                                Scale(obj, new Point3d(obj.Center.X, obj.Point1.Y, 0), offset); 
                            
                            }

                        )

                );


            info.AppendGrip
                (
                    new McSmartGrip<KeyTypeTwo>
                        (

                            new Point3d(center.X, point2.Y, 0),

                            (obj, grip, offset) => {

                                obj.Point2 = new Point3d(obj.Point2.X, offset.Y + obj.Point2.Y, 0);
                                Scale(obj, new Point3d(obj.Center.X, obj.Point2.Y, 0), offset); 
                            
                            }

                        )
                );

            return true;
        }


        protected override void Scale(KeyBasic obj, Point3d grip, Vector3d offset)
        {
            if (obj.TryModify())
                obj.Center = new Point3d((obj.Point1.X + obj.Point2.X) / 2, (obj.Point1.Y + obj.Point2.Y) / 2, (obj.Point1.Z + obj.Point2.Z) / 2);
        }


        protected override void Displace(KeyBasic obj, Point3d grip, Vector3d offset)
        {
            if (obj.TryModify())
            {
                if (grip == obj.Center)
                {
                    obj.Center += offset;
                    obj.Point1 += offset;
                    obj.Point2 += offset;
                }
            }
        }


        public KeyTypeTwo()
        {
            KeyType = "2";
        }
    }
}
