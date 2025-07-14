using Key_master.Keys;
using Multicad.Runtime;

namespace Key_master
{
    internal static class Library
    {
        [CommandMethod("KM_create_key", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public static void CreateKeyCommand()
        {
            KeyTypeTwo key = new KeyTypeTwo();

            key.PlaceObject();
        }
    }
}
