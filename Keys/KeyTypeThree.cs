using Multicad.CustomObjectBase;
using Multicad.DatabaseServices.StandardObjects;
using Multicad.Geometry;
using Multicad.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Key_master.Keys
{
    [CustomEntity("1C925FA1-842B-49CD-924F-4ABF9717DB63", "Key3", "Key Type 3 Entity")]
    internal class KeyTypeThree : KeyBasic
    {
        public double ArcRadius
        {
            get; set;
        }


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
                ArcRadius = value * 0.5;

                if (TryModify())
                {
                    point1 = new Point3d(center.X +  (Length - ArcRadius) * 0.5, center.Y + ArcRadius, 0);
                    point2 = new Point3d(center.X -  (Length - ArcRadius) * 0.5, center.Y - ArcRadius, 0);

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

                return lengthVector.Length + ArcRadius;
            }
            set
            {
                if (TryModify())
                {
                    point1 = new Point3d(center.X + (value - ArcRadius) * 0.5, center.Y + ArcRadius, 0);
                    point2 = new Point3d(center.X - (value - ArcRadius) * 0.5, center.Y - ArcRadius, 0);

                    center = new Point3d((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2, (point1.Z + point2.Z) / 2);
                }
            }
        }

        protected override void Displace(KeyBasic obj, Point3d grip, Vector3d offset)
        {
           // throw new NotImplementedException();
        }


        protected override void Scale(KeyBasic obj, Point3d grip, Vector3d offset)
        {
            //throw new NotImplementedException();
        }


        public override void OnDraw(GeometryBuilder dc)
        {
            dc.Clear();

            //dc.DrawPolyline(new Point3d[] { new Point3d(point2.X, point1.Y, 0), point2, point1, new Point3d(point1.X, point2.Y, 0) });

            dc.DrawPolyline
                (
                    new Point3d[]
                    {
                        new Point3d(point1.X, point2.Y, 0),

                        point2,

                        new Point3d(point2.X, point1.Y, 0),

                        point1
                    }
                );

            Point3d point3 = new Point3d(point2.X, point1.Y, 0);

           // dc.DrawArc(new Point3d((point1.X + point3.X) / 2, point1.Y, 0), Width / 2,  0, 3.14159);
        }


        public override bool GetGripPoints(GripPointsInfo info)
        {
            info.AppendGrip
                (
                    new McSmartGrip<KeyTypeTwo>
                        (

                            point1,

                            (obj, grip, offset) =>
                            {

                                //obj.Point1 += offset;
                                Scale(obj, obj.Point1, offset);

                            }

                        )
                );//Точка справа


            info.AppendGrip
                (
                    new McSmartGrip<KeyTypeTwo>
                        (

                            new Point3d(point2.X, point1.Y,0),

                            (obj, grip, offset) =>
                            {

                                //obj.Point2 += offset;
                                Scale(obj, new Point3d(point2.X, point1.Y, 0), offset);

                            }

                        )
                );

            return true;
        }


        public KeyTypeThree()
        {
            KeyType = "3";
        }
    }
}
