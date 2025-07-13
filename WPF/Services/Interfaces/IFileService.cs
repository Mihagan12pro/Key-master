using Key_master.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Key_master.WPF.Services.Interfaces
{
    internal interface IFileService
    {
        List<IGeometryParams> Open(string filename);
        void Save(string filename, List<IGeometryParams> paramsList);
    }
}
