using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PlantMaster2000
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TextBoxPlantingTime.Text = FormatTime(PlantTime).Trim();

            UpdateWaterTimes(PlantTime);
        }

        public DateTime PlantTime { get; set; } = DateTime.Now;

        private void PlantingLocationChanged(object sender, RoutedEventArgs e) => UpdateWaterTimes();
        private void PlantChanged(object sender, SelectionChangedEventArgs e) => UpdateWaterTimes();
        private string FormatTime(DateTime time) => $"{time.DayOfWeek,10}, {time.Day:D2}.{time.Month:D2}, {time.Hour:D2}:{time.Minute:D2}";

        private int GetHoursBetweenWatering()
        {
            if (PlantType.SelectedItem is PlantInfo plant)
            {
                var factor = LocationGreenhouse.IsChecked == true ? 1 : (LocationOutside.IsChecked == true ? 2 : 20);
                var hoursBetweenWatering = plant.HoursToGrowInGreenhouse * factor;

                return hoursBetweenWatering;
            }

            return 0;
        }

        /// <summary>
        /// Update first, second and third watering times, depending on plant type and location
        /// </summary>
        /// <param name="time">Time of planting; if null is passed, it uses the current time</param>
        private void UpdateWaterTimes(DateTime? time = null)
        {
            if (!IsInitialized)
                return;

            if (PlantType.SelectedItem is PlantInfo plant)
            {
                if (time is null)
                    time = DateTime.Now;

                var hoursBetweenWatering = GetHoursBetweenWatering();

                var secondTime = time.Value.AddHours(hoursBetweenWatering);
                var thirdTime = secondTime.AddHours(hoursBetweenWatering);
                var lastChanceTime = thirdTime.AddHours(hoursBetweenWatering).Subtract(TimeSpan.FromMinutes(30));
                
                FirstWaterTime.Text = FormatTime(time.Value);
                FirstWaterTime.Tag = time.Value;

                SecondWaterTime.Text = FormatTime(secondTime);
                SecondWaterTime.Tag = secondTime;

                ThirdWaterTime.Text = FormatTime(thirdTime);
                ThirdWaterTime.Tag = thirdTime;

                LastChanceToWater.Text = FormatTime(lastChanceTime);
                LastChanceToWater.Tag = lastChanceTime;
            }
        }


        /// <summary>
        /// User clicked button "To Clipboard"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCopyToClipboard(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(GetClipboardText() ?? "(Null)");
            }
            catch(Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private string? GetClipboardText()
        {
            if (PlantType.SelectedItem is PlantInfo plant)
            {
                var sb = new StringBuilder();



                string location = LocationGreenhouse.IsChecked == true
                    ? "Greenhouse"
                    : (LocationOutside.IsChecked == true ? "Outside" : "Indoors");

                string description = ComboBoxLocation.Text.Trim();

                if (description.Length > 0)
                {
                    sb.AppendLine($"{plant.Name} @ {description} ({location})");
                }
                else
                {
                    sb.AppendLine($"{plant.Name} @ {location}");
                }

                sb.AppendLine($"    Planted on {FirstWaterTime.Text.Trim()}");
                sb.AppendLine($"    [ ] Water on {SecondWaterTime.Text.Trim()}");
                sb.AppendLine($"    [ ] Water on {ThirdWaterTime.Text.Trim()}");

                if (ThirdWaterTime.Tag is DateTime thirdWaterTime)
                {
                    var hoursBetweenWatering = GetHoursBetweenWatering();

                    if (hoursBetweenWatering > 0)
                    {
                        var harvestTime = thirdWaterTime.AddHours(hoursBetweenWatering);
                        sb.AppendLine($"    [ ] Harvest: {FormatTime(harvestTime).Trim()}; Harvested amount: ");
                    }
                }

                sb.AppendLine();

                return sb.ToString();
            }

            return null;
        }
        
        /// <summary>
        /// Set planting time to current local time
        /// </summary>
        /// <param name="sender"></param>
        /// 
        /// <param name="e"></param>
        private void ButtonNowClicked(object sender, RoutedEventArgs e)
        {
            PlantTime = DateTime.Now;

            TextBoxPlantingTime.Text = FormatTime(PlantTime).Trim().Trim();

            UpdateWaterTimes(PlantTime);
        }


        /// <summary>
        /// User clicked "Specific..." button to set a specific planting date and time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetSpecificTimeClicked(object sender, RoutedEventArgs e)
        {
            var dlg = new SelectTimeWindow(PlantTime)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (dlg.ShowDialog() == true)
            {
                PlantTime = dlg.SelectedTime;
                TextBoxPlantingTime.Text = FormatTime(PlantTime).Trim();
                UpdateWaterTimes(PlantTime);
            }
        }


        /// <summary>
        /// Checkbox "Topmost" has been checked or unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TopmostWindowCheckboxChanged(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb)
            {
                this.Topmost = cb.IsChecked == true;
            }
        }

        private ClipboardWoraroundWindow? clipboardWindow = null;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var text = GetClipboardText();

            if (text is not null)
            {
                if (clipboardWindow is null)
                {
                    clipboardWindow = new ClipboardWoraroundWindow(text)
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };
                }
                else
                {
                    clipboardWindow.AppendText(text);
                }
            }

            clipboardWindow.Show();
        }
    }
}
