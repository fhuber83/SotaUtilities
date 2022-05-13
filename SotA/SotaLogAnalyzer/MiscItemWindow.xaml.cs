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
        private LogItemBase ItemBase { get; }

        public MiscItemWindow(LogItemBase itemBase)
        {
            ItemBase = itemBase;
            DataContext = itemBase;
            InitializeComponent();

            Title = $"{itemBase.FileName}, line {itemBase.LineNumber}";

            textBoxDate.Text = $"{ItemBase.Timestamp.Day:D2}.{ItemBase.Timestamp.Month:D2}.{ItemBase.Timestamp.Year:D4}";
            textBoxTime.Text = $"{ItemBase.Timestamp.Hour:D2}:{ItemBase.Timestamp.Minute:D2}:{ItemBase.Timestamp.Second:D2}";
        }

        private void ButtonOpenInEditor_Clicked(object sender, RoutedEventArgs e)
        {
            if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Notepad++", null, null) is string notepadPath)
            {
                System.Diagnostics.Process.Start(notepadPath + @"\notepad++.exe", $"-n{ItemBase.LineNumber} \"{ItemBase.FileName}\"");
            }

            else
            {
                MessageBox.Show("Notepad++ not found", null, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
