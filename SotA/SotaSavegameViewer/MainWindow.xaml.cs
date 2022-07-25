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
using System.Windows.Navigation;

namespace SotaSavegameViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var savePath = System.IO.Path.Combine(appData, @"Portalarium\Shroud of the Avatar\SavedGames");

            ListViewSavegames.ItemsSource = System.IO.Directory.GetFiles(savePath, "*.sota")
                .Select(x => new SavegameInfo(System.IO.Path.GetFileNameWithoutExtension(x), x))
                .OrderBy(x => x.Name)
                .ToList();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(e.Source is ListViewItem item && item.Content is SavegameInfo info)
            {
                new SavegameWindow(info)
                {
                    Owner = this
                }
                .Show();
            }
        }
    }
}
