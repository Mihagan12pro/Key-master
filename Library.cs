using Key_master.Keys;
using Multicad;
using Multicad.DatabaseServices;
using Multicad.DatabaseServices.StandardObjects;
using Multicad.Geometry;
using Multicad.Runtime;

namespace Key_master
{
    internal static class Library
    {
        [CommandMethod("KM_create_key", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public static void CreateKeyCommand()
        {

            InputJig jig = new InputJig();
            jig.SetInputOptions(InputJig.InputReturnMode.Other);
            jig.ForceInputNumbers = true;


            double length;
            if (!jig.GetRealNumber("Длина шпоночного паза: ", out length))
                return;

            if (length == 0)
                length = 50000;

            length = Math.Abs(length);


            if (!jig.GetRealNumber("Ширина шпоночного паза: ", out double width))
                return;

            if (width == 0)
                width = 50000;

            width = Math.Abs(width);


            InputResult result = jig.GetPoint("Выберите, куда вставить шпоночный паз: ");

            if (result.Result == InputResult.ResultCode.Cancel)
                return;

            Point3d center = result.Point;


            KeyTypeTwo typeTwo = new KeyTypeTwo(center, width, length);
            typeTwo.PlaceObject();
        }
    }
}
