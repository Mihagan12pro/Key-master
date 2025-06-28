using Key_master.Services;
using Multicad.Runtime;

namespace Key_master
{
    internal static class Library
    {
        [CommandMethod("TestMcCAD", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public static void TestMcCADCommands()
        {
            DotNetConstantsService.DraftEditor.WriteMessage("Hello  Multicad!");
        }
    }
}
