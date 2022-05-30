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
using System.Windows.Shapes;

namespace XpHelper
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

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            XpHelpers.GetXpValues(out ulong? adv, out ulong? prod);

            if (adv.HasValue)
            {
                TextBoxAdvXp.Text = String.Format("{0:n0}", adv);
            }

            if (prod.HasValue)
            {
                TextBoxProdXp.Text = String.Format("{0:n0}", prod);
            }
        }

        private void ButtonCopyAdvXp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(TextBoxAdvXp.Text);
            }
            catch(Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void ButtonCopyProdXp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(TextBoxProdXp.Text);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
