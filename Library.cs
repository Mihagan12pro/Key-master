using Key_master.Keys;
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
            InputJig input = new InputJig();

            int value = 0;

        
            while (true)
            {
                if (!input.GetIntNumber("Выберите исполнение шпоночного паза: 1/2/3", out value))
                    return;

                if (value >= 1 && value <= 3)
                    break;
            }


            KeyBasic key;

            switch(value)
            {
                case 1:
                    key = new KeyTypeTwo();
                    break;

                case 2:
                    key = new KeyTypeTwo();
                    break;

                default:
                    key = new KeyTypeThree();
                    break;
            }

            key.PlaceObject();
        }


        [CommandMethod("KM_test", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public static void TestThirdType()
        {
            DbLine dbLine = new DbLine() { StartPoint = new Point3d(44688.4342, 33320.3643, 0), EndPoint = new Point3d(55688.4342, 33320.3643, 0)};
            DbLine dbLine2 = new DbLine() { StartPoint = new Point3d(55688.4342, 33320.3643, 0), EndPoint = new Point3d(55688.4342, 27320.3643, 0) };
            DbLine dbLine3 = new DbLine() { StartPoint = new Point3d(55688.4342, 27320.3643, 0), EndPoint = new Point3d(44688.4342, 27320.3643, 0) };

            DbCircArc dbArc = new DbCircArc();
            CircArc3d circArc3 = new CircArc3d(new Point3d(44688.4342, 33320.3643,0), new Point3d(44688.4342, 30320.3643,0),new Point3d(44688.4342, 27320.3643,0));
            dbArc.Radius = 3000;
            dbArc.Set(new Point3d(44688.4342, 30320.3643, 0),3000, 1.5708, 4.71239);

            dbLine.DbEntity.AddToCurrentDocument();
            dbLine2.DbEntity.AddToCurrentDocument();
            dbLine3.DbEntity.AddToCurrentDocument();

            dbArc.DbEntity.AddToCurrentDocument();
        }
    }
}
