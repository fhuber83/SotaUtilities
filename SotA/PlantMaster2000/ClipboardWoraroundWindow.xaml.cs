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
    public partial class ClipboardWorkaroundWindow : Window
    {
        const string FileName = "plants.txt";


        public ClipboardWorkaroundWindow()
        {
            InitializeComponent();

            LoadFile();
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


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        
        private void LoadFile()
        {
            if(System.IO.File.Exists(FileName))
            {
                try
                {
                    TextBox1.Text = System.IO.File.ReadAllText(FileName);
                }
                catch(Exception exception)
                {
                    MessageBox.Show(this, exception.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }


        private void SaveFile()
        {
            try
            {
                System.IO.File.WriteAllText(FileName, TextBox1.Text);
            }
            catch(Exception exception)
            {
                MessageBox.Show(this, exception.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }


        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.S && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                SaveFile();

                e.Handled = true;
            }
        }
    }
}
