using Multicad;
using Multicad.CustomObjectBase;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.Runtime;
using Teigha.BoundaryRepresentation;

namespace Key_master.Keys
{
    [CustomEntity("1C925FA1-842B-49CD-924F-4ABF9717DB62", "Key", "Key Entity")]
    internal class Key : McCustomBase, IMcStreamSerializable
    {
        public double Length { get; set; }

        public double Width { get; set; }

        private Point3d _point = new Point3d(50, 50, 0);
        private Vector3d _vector;


        public Point3d Origin
        {
            get
            {
                return _point;
            }
            set
            {
                if (!TryModify()) 
                    return;

                _point = value;
            }
        }


        public override void OnDraw(GeometryBuilder dc)
        {
            dc.Clear();

            Point3d point2 = _point + _vector;

            dc.DrawPolyline(new Point3d[] { _point, new Point3d(_point.X, point2.Y, 0), point2, new Point3d(point2.X, _point.Y, 0), _point });
        }


        public override hresult PlaceObject(PlaceFlags lInsertType)
        {
            InputJig jig = new InputJig();
            jig.SetInputOptions(InputJig.InputReturnMode.Other);
            jig.ForceInputNumbers = true;

            InputJig.PropertyInpector.SetSource(this);
            InputResult res = jig.GetPoint("Выберите первую точку:");

            if (res.Result != InputResult.ResultCode.Normal)
                return hresult.e_Fail;

            _point = res.Point;

            DbEntity.AddToCurrentDocument();
          
            jig.ExcludeObject(ID);
           
            jig.MouseMove = (s, a) => {
                TryModify(); _vector = a.Point - _point; DbEntity.Update();
                InputJig.PropertyInpector.UpdateProperties();
            };

            res = jig.GetPoint("Выберите вторую точку:");
            if (res.Result != InputResult.ResultCode.Normal)
            {
                DbEntity.Erase();
                return hresult.e_Fail;
            }

            _vector = res.Point - _point;

            InputJig.PropertyInpector.SetSource(null);

            return hresult.s_Ok;
        }


        public override void OnTransform(Matrix3d tfm)
        {
            if (!TryModify())
                return;
            
            _point = _point.TransformBy(tfm);
        }


        public override hresult write(McStream S)
        {
            S.PutVersion(1, 0);

            S.PutPoint(_point);
            S.PutVector(_vector);

            S.WriteVersionBlockEnd();

            return hresult.s_Ok;
        }


        public override hresult read(McStream S)
        {
            short major, minor;
            if (!S.GetVersion(out major, out minor))
                return hresult.e_MakeMeProxy;

            if (major == 1)
            {
                if (!S.GetPoint(out _point))
                    return hresult.e_Fail;
                if (!S.GetVector(out _vector))
                    return hresult.e_Fail;
            }

            if (!S.ReadVersionBlockEnd())
                return hresult.e_Fail;
            return hresult.s_Ok;
        }


        public override bool GetGripPoints(GripPointsInfo info)
        {

            info.AppendGrip(new McSmartGrip<Key>(_point, (obj, grip, offset) => { obj.TryModify(); obj._point += offset; }));
            info.AppendGrip(new McSmartGrip<Key>(_point + _vector, (obj, grip, offset) => { obj.TryModify(); obj._vector += offset; }));
            return true;
        }


        public override bool SupportMcStreamSerialization()
        {
            return true;
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

