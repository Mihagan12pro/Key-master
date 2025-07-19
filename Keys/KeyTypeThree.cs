using Multicad.CustomObjectBase;
using Multicad.Geometry;
using Multicad.Runtime;

namespace Key_master.Keys
{
    [CustomEntity("1C925FA1-842B-49CD-924F-4ABF9717DB63", "Key3", "Шпоночный паз")]
    internal class KeyTypeThree : KeyBasic
    {
        protected double radius;


        public override Point3d Point1 
        {
            get => point1;
            set => point1 = value;
        }


        public override Point3d Point2
        {
            get => point2;
            set => point2 = value;
        }


        public override double Width 
        {
            get
            {
                Point3d point3 = new Point3d(point1.X, point2.Y,0);

                return new Vector3d(point1.X - point3.X, point1.Y - point3.Y, 0).Length;
            }
            set
            {
                radius = value * 0.5;

                double lengthWithoutArc = Length - radius;
                Point1 = new Point3d(center.X - lengthWithoutArc * 0.5, center.Y - radius, 0);
                Point2 = new Point3d(center.X + lengthWithoutArc * 0.5, center.Y + radius, 0);
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
                Point1 = new Point3d(center.X - lengthWithoutArc * 0.5, center.Y - radius, 0);
                Point2 = new Point3d(center.X + lengthWithoutArc * 0.5, center.Y + radius, 0);
            }
        }


        public override void OnDraw(GeometryBuilder dc)
        {
            dc.Clear();


            dc.DrawPolyline(new Point3d[] { point1, point2 });
        }


        public KeyTypeThree()
        {
            KeyType = "3";
        }

     
    }
}