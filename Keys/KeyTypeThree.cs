using Multicad;
using Multicad.CustomObjectBase;
using Multicad.Geometry;
using Multicad.Runtime;

namespace Key_master.Keys
{
    [CustomEntity("1C925FA1-842B-49CD-924F-4ABF9717DB63", "Key3", "Шпоночный паз")]
    internal class KeyTypeThree : KeyBasic
    {
        protected double radius;

        protected Point3d arc1MiddlePoint;
        public Point3d Arc1MiddlePoint
        {
            get
            {
                return arc1MiddlePoint;
            }
            set
            {
                if (TryModify())
                {
                    arc1MiddlePoint = value;
                }
            }
        }

        public override Point3d Center
        {
            get
            {
                return center;
            }
            set
            {
                if (TryModify())
                {
                    center = value;

                    radius = Width / 2;

                    arc1MiddlePoint = new Point3d((point1.X < point2.X ? point1.X - radius : point1.X + radius), center.Y, 0);
                }
            }
        }


        public override Point3d Point1
        {
            get => point1;
            set
            {
                point1 = value;
                Center = new Point3d((point2.X + point1.X) * 0.5, (point1.Y + point2.Y) * 0.5, 0);
            }
        }


        public override Point3d Point2
        {
            get => point2;
            set
            {
                point2 = value;
                Center = new Point3d((point2.X + point1.X) * 0.5, (point1.Y + point2.Y) * 0.5, 0);
            }
        }


        public override double Width
        {
            get
            {
                Point3d point3 = new Point3d(point1.X, point2.Y, 0);



                return point1.DistanceTo(point3);
            }
            set
            {
                radius = value * 0.5;

                Point3d oldCenter = center;

                double lengthWithoutArc = Length - radius;
                Point1 = new Point3d(oldCenter.X - lengthWithoutArc * 0.5, oldCenter.Y - radius, 0);
                Point2 = new Point3d(oldCenter.X + lengthWithoutArc * 0.5, oldCenter.Y + radius, 0);

                Center = new Point3d((point2.X + point1.X) * 0.5, (point1.Y + point2.Y) * 0.5, 0);
            }
        }


        public override double Length
        {
            get
            {
                Point3d point4 = new Point3d(point1.X, center.Y, 0);
                Point3d point5 = new Point3d(point2.X + radius, center.Y, 0);

                return new Vector3d(point5.X - point4.X, point5.Y - point4.Y, 0).Length;
            }
            set
            {
                double lengthWithoutArc = value - radius;

                Point3d oldCenter = center;

                Point1 = new Point3d(oldCenter.X - lengthWithoutArc * 0.5, oldCenter.Y - radius, 0);
                Point2 = new Point3d(oldCenter.X + lengthWithoutArc * 0.5, oldCenter.Y + radius, 0);

                Center = new Point3d((point2.X + point1.X) * 0.5, (point1.Y + point2.Y) * 0.5, 0);
            }
        }


        public override void OnDraw(GeometryBuilder dc)
        {
            dc.Clear();


            dc.DrawPolyline
            (
                new Point3d[]
                {
                   new Point3d(point1.X, point2.Y, 0),

                   point2,

                   new Point3d(point2.X, point1.Y, 0),

                   point1,
                }
            );

            Point3d arcCenter = new Point3d(point1.X, center.Y, 0);
            double startAngle, endAngle;


            if (point1.X < point2.X)
            {
                startAngle = 1.57;
                endAngle = 4.71;
            }
            else
            {
                startAngle = 4.71;
                endAngle = 1.57;
            }

            dc.DrawArc(arcCenter, radius, startAngle, endAngle);

            Arc1MiddlePoint = new Point3d((point1.X < point2.X ? point1.X - radius : point1.X + radius), center.Y, 0);
        }



        public override hresult OnMcSerialization(McSerializationInfo info)
        {
            info.Add(nameof(arc1MiddlePoint), arc1MiddlePoint);

            return base.OnMcSerialization(info);
        }


        public override hresult OnMcDeserialization(McSerializationInfo info)
        {
            if (!info.GetValue(nameof(arc1MiddlePoint), out arc1MiddlePoint))
            {
                return hresult.e_Fail;
            }

            return base.OnMcDeserialization(info);
        }


        public override void OnTransform(Matrix3d tfm)
        {
            base.OnTransform(tfm);

            Arc1MiddlePoint = arc1MiddlePoint.TransformBy(tfm);
        }


        public override bool GetGripPoints(GripPointsInfo info)
        {
            info.AppendGrip
               (

                   new McSmartGrip<KeyTypeThree>
                       (
                           center,

                           (obj, grip, offset) =>
                           {

                               obj.Point1 += offset;
                               obj.Point2 += offset;

                           }
                       )
               );

            info.AppendGrip
               (

                   new McSmartGrip<KeyTypeThree>
                       (
                           point2,

                           (obj, grip, offset) =>
                           {
                               if (TryModify())
                               {
                                   obj.Point2 += offset;
                               }

                           }
                       )
               );

            info.AppendGrip
               (

                   new McSmartGrip<KeyTypeThree>
                       (
                           point1,

                           (obj, grip, offset) =>
                           {

                               if (TryModify())
                               {
                                   obj.Point1 += offset;
                               }

                           }
                       )
               );

            info.AppendGrip
                (

                    new McSmartGrip<KeyTypeThree>
                        (

                            new Point3d(point2.X, point1.Y, 0),

                            (obj, grip, offset) =>
                            {

                                if (TryModify())
                                {
                                    obj.Point1 = new Point3d(obj.Point1.X, obj.Point1.Y + offset.Y, 0);
                                    obj.Point2 = new Point3d(obj.Point2.X + offset.X, obj.Point2.Y, 0);
                                }

                            }

                        )

                );

            info.AppendGrip
                (

                    new McSmartGrip<KeyTypeThree>
                        (

                            new Point3d(center.X, point2.Y, 0),

                            McBaseGrip.GripAppearance.InsertVertex,

                            0,

                            "",

                            (obj, grip, offset) =>
                            {

                                obj.Point2 = new Point3d(obj.Point2.X, offset.Y + obj.Point2.Y, 0);

                            }

                        )

                );

            info.AppendGrip
              (

                  new McSmartGrip<KeyTypeThree>
                      (

                          new Point3d(center.X, point1.Y, 0),

                          McBaseGrip.GripAppearance.InsertVertex,

                          0,

                          "",

                          (obj, grip, offset) =>
                          {

                              obj.Point1 = new Point3d(obj.Point1.X, offset.Y + obj.Point1.Y, 0);

                          }

                      )

              );


            info.AppendGrip
               (
                   new McSmartGrip<KeyTypeThree>
                       (

                           new Point3d(point1.X, point2.Y, 0),


                           (obj, grip, offset) => {

                               obj.Point1 = new Point3d(obj.Point1.X + offset.X, obj.Point1.Y, 0);
                               obj.Point2 = new Point3d(obj.Point2.X, obj.Point2.Y + offset.Y, 0);

                           }

                       )

               );

            info.AppendGrip
                (

                    new McSmartGrip<KeyTypeThree>
                        (

                            new Point3d(point2.X, center.Y, 0),

                            McBaseGrip.GripAppearance.InsertVertex,

                            1.57,

                             "",

                            (obj, grip, offset) =>
                            {

                                if (TryModify())
                                {
                                    obj.Point2 = new Point3d(obj.Point2.X + offset.X, obj.Point2.Y, 0);
                                }

                            }

                        )

                );


            info.AppendGrip
                (

                    new McSmartGrip<KeyTypeThree>
                        (

                            arc1MiddlePoint,

                            McBaseGrip.GripAppearance.InsertVertex,

                            1.57,

                             "",

                            (obj, grip, offset) =>
                            {

                                if (TryModify())
                                {
                                    obj.Point1 = new Point3d(obj.Point1.X + offset.X, obj.Point1.Y, 0);
                                }

                            }

                        )

                );

            return true;
        }


        protected override void BuildStartGeometry(double length, double width)
        {
            DbEntity.AddToCurrentDocument();

            radius = width * 0.5;

            double lengthWithoutArc = length - radius;
            point1 = new Point3d(center.X - lengthWithoutArc * 0.5, center.Y - radius, 0);
            point2 = new Point3d(center.X + lengthWithoutArc * 0.5, center.Y + radius, 0);
            
            DbEntity.Update();
        }


        public KeyTypeThree()
        {
            KeyType = "3";
        }
    }
}