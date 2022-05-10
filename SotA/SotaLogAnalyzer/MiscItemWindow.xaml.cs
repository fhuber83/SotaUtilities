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
using Microsoft.Win32;
using SotaLogParser;

namespace LogAnalyzer
{
    /// <summary>
    /// Interaction logic for MiscItemWindow.xaml
    /// </summary>
    public partial class MiscItemWindow : Window
    {
        private LogItem Item { get; }

        public MiscItemWindow(LogItem item)
        {
            Item = item;
            DataContext = item;
            InitializeComponent();

            Title = $"{item.FileName}, line {item.LineNumber}";

            textBoxDate.Text = $"{Item.Timestamp.Day:D2}.{Item.Timestamp.Month:D2}.{Item.Timestamp.Year:D4}";
            textBoxTime.Text = $"{Item.Timestamp.Hour:D2}:{Item.Timestamp.Minute:D2}:{Item.Timestamp.Second:D2}";
        }

        private void ButtonOpenInEditor_Clicked(object sender, RoutedEventArgs e)
        {
            if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Notepad++", null, null) is string notepadPath)
            {
                System.Diagnostics.Process.Start(notepadPath + @"\notepad++.exe", $"-n{Item.LineNumber} \"{Item.FileName}\"");
            }

            else
            {
                MessageBox.Show("Notepad++ not found", null, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
