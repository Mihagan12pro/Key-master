using Key_master.Keys;
using Key_master.Services;
using Multicad;
using Multicad.DatabaseServices;
using Multicad.Runtime;
using System.Windows;
using static Multicad.DatabaseServices.McEntity;

namespace Key_master
{
    internal static class Library
    {
        [CommandMethod("Create_key", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public static void CreateKeyCommand()
        {
            Key key = new Key();

            key.DbEntity.AddToCurrentDocument();
        }
    }
}
