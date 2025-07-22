using Key_master.Keys;
using Multicad.DatabaseServices;
using Multicad.DatabaseServices.StandardObjects;
using Multicad.Geometry;
using Multicad.Runtime;
using Multicad.Wpf;

namespace Key_master
{
    internal static class Library
    {
        [CommandMethod("","KM_create_key",CommandFlags.NoCheck | CommandFlags.NoPrefix)]
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


        [CommandMethod("KM_key_2", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public static void CreateKeyTypeTwoCommand()
        {
            KeyTypeTwo key = new KeyTypeTwo();

            key.PlaceObject();
        }


        [CommandMethod("KM_key_3", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public static void CreateKeyTypeThreeCommand()
        {
            KeyTypeThree key = new KeyTypeThree();

            key.PlaceObject();
        }
    }
}
