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
using SotaLogAnalyzer;
using SotaLogParser;

namespace LogAnalyzer
{
    /// <summary>
    /// Interaction logic for ChatItemWindow.xaml
    /// </summary>
    public partial class ChatItemWindow : Window
    {
        private ChatItem Item { get; }

        public ChatItemWindow(ChatItem item)
        {
            Item = item;
            DataContext = item;

            InitializeComponent();
        }

        private void ButtonOpenInEditor_Clicked(object sender, RoutedEventArgs e)
        {
            if (Item.FileName is null)
                return;

            NotepadPlusPlusHelper.OpenEditor(Item.FileName, Item.LineNumber);
        }
    }
}
