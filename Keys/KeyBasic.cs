using Multicad;
using Multicad.CustomObjectBase;
using Multicad.Geometry;
using System.Windows.Input;

namespace Key_master.Keys
{
    internal abstract class KeyBasic : McCustomBase, IMcSerializable
    {
        public string ?KeyType { get; protected set; }

        protected abstract void Scale(KeyBasic obj, Point3d grip, Vector3d offset);

        protected abstract void Displace(Key obj, Point3d grip, Vector3d offset);

        public bool TryModify()
        {
            TryModify(0);
            return true;
        }
    }
}
