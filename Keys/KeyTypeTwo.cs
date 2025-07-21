using Multicad.CustomObjectBase;
using Multicad.Geometry;
using Multicad.Runtime;

namespace Key_master.Keys
{
    [CustomEntity("1C925FA1-842B-49CD-924F-4ABF9717DB62", "Key2", "Шпоночный паз")]
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
                    Point3d oldCenter = center;

                    Point1 = new Point3d(oldCenter.X + Length * 0.5, oldCenter.Y + value * 0.5, 0);
                    Point2 = new Point3d(oldCenter.X - Length * 0.5, oldCenter.Y - value * 0.5, 0);
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
                    Point3d oldCenter = center;

                    Point1 = new Point3d(oldCenter.X + value * 0.5, oldCenter.Y + Width * 0.5, 0);
                    Point2 = new Point3d(oldCenter.X - value * 0.5, oldCenter.Y - Width * 0.5, 0);
                }
            }
        }

        public override Point3d Point1 
        { 
            get
            {
                return point1;
            }
            set
            {
                if (TryModify())
                {
                    point1 = value;

                    center = new Point3d((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2, (point1.Z + point2.Z) / 2);
                }
            }
        }


        public override Point3d Point2 
        {
            get
            {
                return point2;
            }
            set
            {
                if (TryModify())
                {
                    point2 = value;

                    center = new Point3d((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2, (point1.Z + point2.Z) / 2);
                }
            }
        }


        public override void OnDraw(GeometryBuilder dc)
        {
            dc.Clear();

            dc.DrawPolyline(new Point3d[] { point1, new Point3d(point1.X, point2.Y, 0), point2, new Point3d(point2.X, point1.Y, 0), point1 });
        }


        public override bool GetGripPoints(GripPointsInfo info)
        {
            info.AppendGrip
               (
                   new McSmartGrip<KeyTypeTwo>
                       (
                           center,

                           (obj, grip, offset) => {

                               obj.Point1 += offset;
                               obj.Point2 += offset;

                           }
                       )
               );


            info.AppendGrip
                (
                    new McSmartGrip<KeyTypeTwo>
                        (

                            point1,

                            (obj, grip, offset) => { 
                                
                                obj.Point1 += offset; 
                            
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

                            }

                        )

                );

            info.AppendGrip
                 (
                     new McSmartGrip<KeyTypeTwo>
                         (

                             new Point3d(point1.X, center.Y, 0),

                             McBaseGrip.GripAppearance.InsertVertex,

                             1.57,

                             "",

                             (obj, grip, offset) =>
                             {
                                 obj.Point1 = new Point3d(obj.Point1.X + offset.X, obj.Point1.Y, 0);

                             }

                         )
                 );


            info.AppendGrip
                (
                    new McSmartGrip<KeyTypeTwo>
                        (

                            new Point3d(point2.X, center.Y, 0),

                            McBaseGrip.GripAppearance.InsertVertex,

                            1.57,

                             "",

                            (obj, grip, offset) => {

                                obj.Point2 = new Point3d(obj.Point2.X + offset.X, obj.Point2.Y, 0);

                            }

                        )

                );

            info.AppendGrip
                (
                    new McSmartGrip<KeyTypeTwo>
                        (

                            new Point3d(center.X, point1.Y, 0),

                            McBaseGrip.GripAppearance.InsertVertex,

                            0,

                            "",

                            (obj, grip, offset) => {

                                obj.Point1 = new Point3d(obj.Point1.X, offset.Y + obj.Point1.Y, 0);

                            }

                        )

                );


            info.AppendGrip
                (
                    new McSmartGrip<KeyTypeTwo>
                        (

                            new Point3d(center.X, point2.Y, 0),

                            McBaseGrip.GripAppearance.InsertVertex,

                            0,

                            "",

                            (obj, grip, offset) => {

                                obj.Point2 = new Point3d(obj.Point2.X, offset.Y + obj.Point2.Y, 0);

                            }

                        )
                );


            return true;
        }

        protected override void BuildStartGeometry(double length, double width)
        {
            DbEntity.AddToCurrentDocument();

            Width = width;
            Length = length;

            DbEntity.Update();
        }


        public KeyTypeTwo()
        {
            KeyType = "2";
        }
    }
}
