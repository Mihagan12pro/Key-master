using Multicad;
using Multicad.CustomObjectBase;
using Multicad.Geometry;
using System.Windows.Input;

namespace Key_master.Keys
{
    internal abstract class KeyBasic : McCustomBase, IMcSerializable
    {
        protected Point3d center, point1, point2;


        public Point3d Center
        {
            get => center;

            set => center = value;
        }


        public Point3d Point1
        {
            get => point1;

            set => point1 = value;
        }


        public Point3d Point2
        {
            get => point2;

            set => point2 = value;
        }


        public string ?KeyType { get; protected set; }

        protected abstract void Scale(KeyBasic obj, Point3d grip, Vector3d offset);

        protected abstract void Displace(KeyBasic obj, Point3d grip, Vector3d offset);

        public bool TryModify()
        {
            TryModify(0);
            return true;
        }
    }
}
