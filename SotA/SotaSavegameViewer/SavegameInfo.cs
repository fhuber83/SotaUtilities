using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SotaSavegameViewer
{
    public class SavegameInfo
    {
        public string Name { get; set; }
        public string Path  { get; set; }

        public SavegameInfo(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}
