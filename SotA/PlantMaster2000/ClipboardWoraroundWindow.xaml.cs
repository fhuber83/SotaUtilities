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

namespace PlantMaster2000
{
    /// <summary>
    /// Interaction logic for ClipboardWoraroundWindow.xaml
    /// </summary>
    public partial class ClipboardWoraroundWindow : Window
    {
        public ClipboardWoraroundWindow(string? text = null)
        {
            InitializeComponent();

            TextBox1.Text = text;
        }

        public void AppendText(string text)
        {
            if(string.IsNullOrEmpty(TextBox1.Text))
            {
                TextBox1.Text = text;
            }
            else
            {
                TextBox1.Text += text;
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            TextBox1?.Clear();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
    }
}
