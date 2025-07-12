using Multicad;
using Multicad.Geometry;

namespace Key_master.Keys
{
    internal interface IKey : IMcSerializable
    {
        Point3d Point1 { get; protected set; }

        Point3d Point2 { get; protected set; }
        
        Point3d Center { get; protected set; }

        string KeyType { get; protected set; }

        double Width {get; set; }

        public double Length { get; set; }

        void Displace(IKey obj, Point3d grip, Vector3d offset);

        void Scale(IKey obj, Point3d grip, Vector3d offset);

        bool TryModify();
    }
}

