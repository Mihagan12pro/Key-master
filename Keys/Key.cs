using Multicad;
using Multicad.CustomObjectBase;
using Multicad.Geometry;
using Multicad.Runtime;
using System.Configuration;

namespace Key_master.Keys
{
    [CustomEntity("1C925FA1-842B-49CD-924F-4ABF9717DB62", "Key", "Key Entity")]
    internal class Key : McCustomBase, IMcSerializable
    {
        public double Length { get; set; }

        public double Width { get; set; }

        private Point3d _point1, _point2;


        public double OriginPointX
        {
            get { return _point1.X; }
            set
            {
                if (!TryModify()) 
                    return; 
                _point1.X = value;
            }
        }

       
        public double OriginPointY
        {
            get { return _point1.Y; }
            set
            {
                if (!TryModify()) 
                    return; 
                _point1.Y = value;
            }
        }


        public override void OnDraw(GeometryBuilder dc)
        {
            dc.Clear();

            dc.DrawPolyline
            (
                new Point3d[] 
                {
                    _point1,
                    new Point3d(_point1.X, _point2.Y, 0),
                    _point2,
                    new Point3d(_point2.X, _point1.Y, 0),
                    _point1
                }
            );
        }

        public override hresult OnMcSerialization(McSerializationInfo info)
        {
            info.Add("point1", _point1);
            info.Add("point2", _point2);

            return hresult.s_Ok;
        }

        public override hresult OnMcDeserialization(McSerializationInfo info)
        {
            if (!info.GetValue("point1",out _point1) || !info.GetValue("point2",out _point2))
            {
                return hresult.e_Fail;
            }

            return hresult.s_Ok;
        }


        public override void OnTransform(Matrix3d tfm)
        {
            if (!TryModify())
                return;

            _point1 = _point1.TransformBy(tfm);
            _point2 = _point2.TransformBy(tfm);
        }
         
        public bool TryModify()
        {
            TryModify(0);
            return true;
        }

        public Key()
        {
            _point1 = new Point3d(50, 50, 0);

            _point2 = new Point3d(350, 150, 0);
        }
    }
}

