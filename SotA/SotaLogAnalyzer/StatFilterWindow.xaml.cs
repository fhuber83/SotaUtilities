using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

namespace LogAnalyzer
{
    /// <summary>
    /// Interaction logic for StatFilterWindow.xaml
    /// </summary>
    public partial class StatFilterWindow : Window
    {
        public List<string> SelectedPlayers { get; } = new List<string>();
        public List<string> SelectedChats { get; } = new List<string>();
        public DateTime TimeStart;
        public DateTime TimeEnd;

        public class PlayerFilterItem
        {
            public PlayerFilterItem(string name)
            {
                Name = name;
            }

            public bool Enabled { get; set; } = true;
            public string Name { get; set; }
        }

        private Regex regexTime;
        private Regex regexDate;
        private DateTime MinTime;
        private DateTime MaxTime;

        public StatFilterWindow(SotaLogParser.SotaLog log)
        {
            InitializeComponent();

            MinTime = log.Items.Min(x => x.Timestamp);
            MaxTime = log.Items.Max(x => x.Timestamp);

            SetMinTime();
            SetMaxTime();

            regexDate = new Regex(@"^(?<day>\d{2}).(?<month>\d{2}).(?<year>\d{4})$", RegexOptions.Compiled);
            regexTime = new Regex(@"^(?<hour>\d{2}):(?<minute>\d{2})(:(?<second>\d{2}))?$", RegexOptions.Compiled);

            // Create distinct list of player names
            var allNames = new List<string>();
            log.CombatItems.Select(x => x.WhoSource).Distinct().ToList().ForEach(s => allNames.Add(s));
            log.HealItems.Select(x => x.HealerName).Distinct().ToList().ForEach(s => allNames.Add(s));

            listBoxPlayers.ItemsSource =
                allNames.Distinct().OrderBy(x => x).Select(x => new PlayerFilterItem(x)).ToList();

            // Create distinct list of chats
            listBoxChats.ItemsSource = log.ChatItems.Select(x => x.ChatName).Distinct().Select(x => new PlayerFilterItem(x)).ToList();
        }

        private void ButtonOK_OnClick(object sender, RoutedEventArgs e)
        {
            var matchMinDate = regexDate.Match(textBoxDateStart.Text.Trim());
            var matchMinTime = regexTime.Match(textBoxTimeStart.Text.Trim());
            var matchMaxDate = regexDate.Match(textBoxDateEnd.Text.Trim());
            var matchMaxTime = regexTime.Match(textBoxTimeEnd.Text.Trim());

            DateTime? minTime = null;
            DateTime? maxTime = null;

            if (matchMinTime.Success && matchMinDate.Success)
            {
                var day = int.Parse(matchMinDate.Groups["day"].Value);
                var month = int.Parse(matchMinDate.Groups["month"].Value);
                var year = int.Parse(matchMinDate.Groups["year"].Value);

                var hour = int.Parse(matchMinTime.Groups["hour"].Value);
                var minute = int.Parse(matchMinTime.Groups["minute"].Value);
                var second = matchMinTime.Groups["second"].Success ? int.Parse(matchMinTime.Groups["second"].Value) : 0;

                minTime = new DateTime(year, month, day, hour, minute, second);
            }

            if (!minTime.HasValue)
            {
                MessageBox.Show(this, "Start time is invalid", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (matchMaxTime.Success && matchMaxDate.Success)
            {
                var day = int.Parse(matchMaxDate.Groups["day"].Value);
                var month = int.Parse(matchMaxDate.Groups["month"].Value);
                var year = int.Parse(matchMaxDate.Groups["year"].Value);

                var hour = int.Parse(matchMaxTime.Groups["hour"].Value);
                var minute = int.Parse(matchMaxTime.Groups["minute"].Value);
                var second = matchMaxTime.Groups["second"].Success ? int.Parse(matchMaxTime.Groups["second"].Value) : 0;

                maxTime = new DateTime(year, month, day, hour, minute, second);
            }

            if (!maxTime.HasValue)
            {
                MessageBox.Show(this, "End time is invalid", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }


            SelectedPlayers.Clear();
            
            foreach (PlayerFilterItem item in listBoxPlayers.Items)
            {
                if (item.Enabled)
                {
                    SelectedPlayers.Add(item.Name);
                }
            }

            if (SelectedPlayers.Count == 0)
            {
                MessageBox.Show(this, "Please select at least one player", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            SelectedChats.Clear();

            foreach (PlayerFilterItem item in listBoxChats.Items)
            {
                if (item.Enabled)
                {
                    SelectedChats.Add(item.Name);
                }
            }

            TimeStart = minTime.Value;
            TimeEnd = maxTime.Value;

            DialogResult = true;
            Close();
        }

        private void ButtonAllPlayers_OnClick(object sender, RoutedEventArgs e)
        {
            foreach(PlayerFilterItem item in listBoxPlayers.Items)
            {
                item.Enabled = true;
            }

            CollectionViewSource.GetDefaultView(listBoxPlayers.ItemsSource).Refresh();
        }

        private void ButtonNonePlayers_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (PlayerFilterItem item in listBoxPlayers.Items)
            {
                item.Enabled = false;
            }

            CollectionViewSource.GetDefaultView(listBoxPlayers.ItemsSource).Refresh();
        }

        private void TextBoxTimeStart_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (regexTime is null || textBoxTimeStart is null)
                return;

            var valid = false;
            var message = "Invalid format";

            var match = regexTime.Match(textBoxTimeStart.Text.Trim());

            if (match.Success)
            {
                var hour = int.Parse(match.Groups["hour"].Value);
                var minute= int.Parse(match.Groups["minute"].Value);
                var second = match.Groups["second"].Success ? int.Parse(match.Groups["second"].Value) : 0;

                if ((hour >= 0 && hour < 24) &&
                    (minute >= 0 && minute < 60) &&
                    (second >= 0 && second < 60))
                {
                    var timeEntered = new DateTime(MinTime.Year, MinTime.Month, MinTime.Day, hour, minute, second);

                    //if (timeEntered < MinTime)
                    //{
                    //    message = $"Start time must be at or after {MinTime.Hour:D2}:{MinTime.Minute:D2}:{MinTime.Second:D2}";
                    //}
                    //else
                    {
                        message = null;
                        valid = true;
                    }
                }
            }

            textBoxTimeStart.Background = valid ? Brushes.White : Brushes.Tomato;
            textBoxTimeStart.ToolTip = message;
        }

        private void TextBoxTimeEnd_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (regexTime is null || textBoxTimeEnd is null)
                return;

            var valid = false;
            var message = "Invalid format";

            var match = regexTime.Match(textBoxTimeEnd.Text.Trim());

            if (match.Success)
            {
                var hour = int.Parse(match.Groups["hour"].Value);
                var minute = int.Parse(match.Groups["minute"].Value);
                var second = match.Groups["second"].Success ? int.Parse(match.Groups["second"].Value) : 0;

                if ((hour >= 0 && hour < 24) &&
                    (minute >= 0 && minute < 60) &&
                    (second >= 0 && second < 60))
                {
                    var timeEntered = new DateTime(MaxTime.Year, MaxTime.Month, MaxTime.Day, hour, minute, second);

                    //if (timeEntered > MaxTime)
                    //{
                    //    message = $"End time must be at or before {MaxTime.Hour:D2}:{MaxTime.Minute:D2}:{MaxTime.Second:D2}";
                    //}
                    //else
                    {
                        message = null;
                        valid = true;
                    }
                }
            }

            textBoxTimeEnd.Background = valid ? Brushes.White : Brushes.Tomato;
            textBoxTimeEnd.ToolTip = message;
        }

        private void TextBoxDateStart_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (regexDate is null || textBoxDateStart is null)
                return;

            var valid = false;
            var message = "Invalid format";

            var match = regexDate.Match(textBoxDateStart.Text.Trim());

            if (match.Success)
            {
                var day = int.Parse(match.Groups["day"].Value);
                var month = int.Parse(match.Groups["month"].Value);
                var year = int.Parse(match.Groups["year"].Value);

                if (day >= 1 && day <= 31 &&
                    month >= 1 && month <= 12)
                {
                    message = null;
                    valid = true;
                }
            }

            textBoxDateStart.Background = valid ? Brushes.White : Brushes.Tomato;
            textBoxDateStart.ToolTip = message;
        }

        private void TextBoxDateEnd_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (regexDate is null || textBoxDateEnd is null)
                return;

            var valid = false;
            var message = "Invalid format";

            var match = regexDate.Match(textBoxDateEnd.Text.Trim());

            if (match.Success)
            {
                var day = int.Parse(match.Groups["day"].Value);
                var month = int.Parse(match.Groups["month"].Value);
                var year = int.Parse(match.Groups["year"].Value);

                if (day >= 1 && day <= 31 &&
                    month >= 1 && month <= 12)
                {
                    message = null;
                    valid = true;
                }
            }

            textBoxDateEnd.Background = valid ? Brushes.White : Brushes.Tomato;
            textBoxDateEnd.ToolTip = message;
        }

        private void SetMinTime()
        {
            textBoxDateStart.Text = $"{MinTime.Day:D2}.{MinTime.Month:D2}.{MinTime.Year:D4}";
            textBoxTimeStart.Text = $"{MinTime.Hour:D2}:{MinTime.Minute:D2}:{MinTime.Second:D2}";
        }

        private void SetMaxTime()
        {
            textBoxDateEnd.Text = $"{MaxTime.Day:D2}.{MaxTime.Month:D2}.{MaxTime.Year:D4}";
            textBoxTimeEnd.Text = $"{MaxTime.Hour:D2}:{MaxTime.Minute:D2}:{MaxTime.Second:D2}";
        }

        private void ButtonSetMinTime_OnClick(object sender, RoutedEventArgs e)
        {
            SetMinTime();
        }

        private void ButtonSetMaxTime_OnClick(object sender, RoutedEventArgs e)
        {
            SetMaxTime();
        }

        private void TextBoxTime_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.SelectAll();
            }
        }

        private void ButtonAllChats_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (PlayerFilterItem item in listBoxChats.Items)
            {
                item.Enabled = true;
            }

            CollectionViewSource.GetDefaultView(listBoxChats.ItemsSource).Refresh();
        }

        private void ButtonNoChats_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (PlayerFilterItem item in listBoxChats.Items)
            {
                item.Enabled = false;
            }

            CollectionViewSource.GetDefaultView(listBoxChats.ItemsSource).Refresh();
        }
    }
}
