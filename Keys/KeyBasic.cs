using Key_master.WPF.Views;
using Multicad;
using Multicad.CustomObjectBase;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.Runtime;
using Multicad.Wpf.Dialogs;
using System.Globalization;

namespace Key_master.Keys
{
    internal abstract class KeyBasic : McCustomBase, IMcSerializable, IGeometryParams
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


        public abstract double Width { get; set; }
        
        
        public abstract double Length { get; set; }


        protected abstract void Scale(KeyBasic obj, Point3d grip, Vector3d offset);


        protected abstract void Displace(KeyBasic obj, Point3d grip, Vector3d offset);


        public bool TryModify()
        {
            TryModify(0);
            return true;
        }


        public override hresult OnEdit(Point3d pnt, EditFlags lFlag)
        {
            EditWindow window = new EditWindow(Width, Length);
            window.Title += $" (исполнение {KeyType})";

            window.ShowDialog();
            if (window.DialogResult == true)
            {
                double width, length;

                if (!double.TryParse(window.WidthTB.Text, out width))
                {
                    double.TryParse(window.WidthTB.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out width);
                }
                Width = width;


                if (!double.TryParse(window.LengthTB.Text, out length))
                {
                    double.TryParse(window.LengthTB.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out length);
                }
                Length = length;
            }

            return hresult.s_Ok;
        }


        public override hresult PlaceObject(PlaceFlags lInsertType)
        {
            InputJig jig = new InputJig();
            jig.SetInputOptions(InputJig.InputReturnMode.Other);
            jig.ForceInputNumbers = true;

            InputJig.PropertyInpector.SetSource(this);


            if (!jig.GetRealNumber("Длина шпоночного паза: ", out double length))
                return hresult.e_Abort; 
           
            length = Math.Abs(length);

            if (length == 0)
                length = 50000;


            if (!jig.GetRealNumber("Ширина шпоночного паза: ", out double width))
                return hresult.e_Abort;

            width = Math.Abs(width);

            if (width == 0)
                width = 50000;

            DbEntity.AddToCurrentDocument();

            InputResult result = jig.GetPoint("Куда вставить шпоночный паз: ");

            if (result.Result == InputResult.ResultCode.Cancel)
                return hresult.e_Abort;

            center = result.Point;

            Width = width;
            Length = length;

            DbEntity.Update();

            return hresult.s_Ok;
        }


        public override hresult OnMcSerialization(McSerializationInfo info)
        {
            info.Add("point1", point1);
            info.Add("point2", point2);
            info.Add("center", center);

            return hresult.s_Ok;
        }


        public override hresult OnMcDeserialization(McSerializationInfo info)
        {
            if (!info.GetValue("point1", out point1) || !info.GetValue("point2", out point2) || !info.GetValue("center", out center))
            {
                return hresult.e_Fail;
            }

            return hresult.s_Ok;
        }


        public override void OnTransform(Matrix3d tfm)
        {
            if (!TryModify())
                return;

            point1 = point1.TransformBy(tfm);

            point2 = point1.TransformBy(tfm);

            center = new Point3d((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2, (point1.Z + point2.Z) / 2);
        }
    }
}
