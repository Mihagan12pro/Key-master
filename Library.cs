using Key_master.Keys;
using Multicad.Runtime;

namespace Key_master
{
    internal static class Library
    {
        [CommandMethod("Create_key", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public static void CreateKeyCommand()
        {
            Key key = new Key();

            key.PlaceObject();
           // key.DbEntity.AddToCurrentDocument();
        }
    }
}
