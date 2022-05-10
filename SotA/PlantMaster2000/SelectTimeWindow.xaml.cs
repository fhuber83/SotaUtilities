using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for SelectTimeWindow.xaml
    /// </summary>
    public partial class SelectTimeWindow : Window
    {
        public SelectTimeWindow(DateTime initialTime)
        {
            SelectedTime = initialTime;

            InitializeComponent();

            TextBoxDate.Text = $"{SelectedTime.Day:D2}.{SelectedTime.Month:D2}.{SelectedTime.Year:D4}";
            TextBoxTime.Text = $"{SelectedTime.Hour:D2}:{SelectedTime.Minute:D2}";

            TextBoxTime.SelectAll();
            TextBoxTime.Focus();
        }

        public DateTime SelectedTime { get; set; }

        private void ButtonOkClicked(object sender, RoutedEventArgs e)
        {
            var regexDate = new Regex(@"^(\d{2})\.(\d{2})\.(\d{4})$");
            var regexTime = new Regex(@"^(\d{2}):(\d{2})$");

            var matchDate = regexDate.Match(TextBoxDate.Text.Trim());
            var matchTime = regexTime.Match(TextBoxTime.Text.Trim());

            if (matchDate.Success && matchTime.Success)
            {
                var day = int.Parse(matchDate.Groups[1].Value);
                var month = int.Parse(matchDate.Groups[2].Value);
                var year = int.Parse(matchDate.Groups[3].Value);
                var hour = int.Parse(matchTime.Groups[1].Value);
                var minute = int.Parse(matchTime.Groups[2].Value);

                SelectedTime = new DateTime(year, month, day, hour, minute, 0);

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show(this, "Format error", this.Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
