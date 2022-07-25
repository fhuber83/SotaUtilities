using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SotaSavegameViewer
{
    public partial class SavegameWindow : Window
    {
        private SavegameInfo Info { get; init; }

        private SotaSavegameLib.SotaSavegame SaveGame { get; set; }

        public SavegameWindow(SavegameInfo info)
        {
            if(!System.IO.File.Exists(info.Path))
            {
                throw new ArgumentException("File does not exist", nameof(info));
            }

            Info = info;

            InitializeComponent();

            Title = Info.Name;

            SaveGame = new SotaSavegameLib.SotaSavegame(Info.Path);
        }
    }
}
