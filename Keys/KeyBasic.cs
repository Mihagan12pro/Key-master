using Key_master.WPF.Views;
using Multicad;
using Multicad.CustomObjectBase;
using Multicad.Geometry;
using System.Globalization;
using System.Windows.Input;

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


        public KeyBasic(Point3d center, double width, double length)
        {
            this.center = center;

            Width = width;

            Length = length;
        }
    }
}
