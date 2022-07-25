using System;
using System.Windows;

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
            XpHelpers.GetXpValues(out ulong? adv, out ulong? prod, out int? currentAdvLvl, out int? currentProdLvl);

            if (adv.HasValue && currentAdvLvl.HasValue)
            {
                TextBoxAdvXp.Text = string.Format("{0:n0}", adv.Value);
                var xpToNext = XpTable.HowMuchToNextLevel(adv.Value);
                TextBoxAdvXp.ToolTip = $"{xpToNext:n0} to level {currentAdvLvl.Value + 1}";
            }

            if (prod.HasValue && currentProdLvl.HasValue)
            {
                TextBoxProdXp.Text = string.Format("{0:n0}", prod);
                var xpToNext = XpTable.HowMuchToNextLevel(prod.Value);
                TextBoxProdXp.ToolTip = $"{xpToNext:n0} to level {currentProdLvl.Value + 1}";
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

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Topmost = CheckBoxTopmost.IsChecked == true;
        }
    }
}
