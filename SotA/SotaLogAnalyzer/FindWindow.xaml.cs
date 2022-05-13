using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Threading;
using SotaLogParser;
using Path = System.IO.Path;

namespace LogAnalyzer
{
    /// <summary>
    /// Interaction logic for FindWindow.xaml
    /// </summary>
    public partial class FindWindow : Window
    {
        private List<SotaLogParser.LogItemBase> Items;

        public FindWindow(List<SotaLogParser.LogItemBase> items)
        {
            Items = items;

            timerUpdate = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500),
            };

            timerUpdate.Tick += (s, a) =>
            {
                timerUpdate.Stop();
                ApplyFilter();
            };

            InitializeComponent();

            listViewResults.ItemsSource = Items;

            listViewResults.Items.SortDescriptions.Add(new SortDescription("Timestamp", ListSortDirection.Ascending));

            textBoxPattern.Focus();
        }

        private readonly DispatcherTimer timerUpdate;

        private static Regex regexPatternSplit;

        static FindWindow()
        {
            regexPatternSplit = new Regex(@"(['\""])(?<value>.+?)\1|(?<value>[^ ]+)", RegexOptions.Compiled);
        }

        private void ApplyFilter()
        {
            var oldSort = listViewResults.Items?.SortDescriptions;

            if (string.IsNullOrEmpty(textBoxPattern.Text))
            {
                listViewResults.ItemsSource = Items;
            }
            else
            {
                var pattern = checkBoxIgnoreCase.IsChecked == true
                    ? textBoxPattern.Text.ToUpper()
                    : textBoxPattern.Text;

                var parts = regexPatternSplit.Matches(pattern).Cast<Match>().Select(x => x.Groups["value"].Value).ToList();

                if (checkBoxIgnoreCase.IsChecked == true)
                {
                    listViewResults.ItemsSource = Items.Where(x => parts.All(p => x.Line.ToUpper().Contains(p))).Select(x => x);    
                }

                else
                {
                    listViewResults.ItemsSource = Items.Where(x => parts.All(p => x.Line.Contains(p))).Select(x => x);    
                }
            }

            // Restore old sort order
            listViewResults.Items.SortDescriptions.Clear();
            foreach (var sd in oldSort)
            {
                listViewResults.Items.SortDescriptions.Add(new SortDescription(sd.PropertyName, sd.Direction));
            }
        }

        private void TextBoxPattern_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            timerUpdate.Start();
        }

        private void TextBoxPattern_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                timerUpdate.Stop();
                ApplyFilter();
            }

            else
            {
                timerUpdate.Start();
            }
        }

        private void ButtonClear_OnClick(object sender, RoutedEventArgs e)
        {
            textBoxPattern.Text = "";
            ApplyFilter();
        }

        private void TextBoxPattern_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.SelectAll();
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is GridViewColumnHeader header)
            {
                if (header.Tag is string sortBy)
                {
                    var sortOrder = ListSortDirection.Ascending;

                    if (listViewResults.Items.SortDescriptions.Count > 0 &&
                        listViewResults.Items.SortDescriptions[0].PropertyName.Equals(sortBy) &&
                        listViewResults.Items.SortDescriptions[0].Direction == ListSortDirection.Ascending)
                    {
                        sortOrder = ListSortDirection.Descending;
                    }

                    listViewResults.Items.SortDescriptions.Clear();
                    listViewResults.Items.SortDescriptions.Add(new SortDescription(sortBy, sortOrder));
                }
            }
        }

        private void CheckBoxIgnoreCase_OnCheckChanged(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem lvi)
            {
                if (lvi.Content is LogItemBase item)
                {
                    var dlg = new MiscItemWindow(item)
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    dlg.ShowDialog();
                }
            }
        }

        private void ShowInEditor(LogItemBase itemBase)
        {
            //(Application.Current as App)?.LaunchEditor(itemBase.FileName, itemBase.LineNumber);
        }

        public LogItemBase GoToItemBase { get; private set; } = null;

        private void ListViewResults_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.E && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (listViewResults.SelectedItems.Count == 1 && listViewResults.SelectedItems[0] is LogItemBase logItem)
                {
                    ShowInEditor(logItem);
                }
            }
            else if(e.Key  == Key.G && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (listViewResults.SelectedItems.Count == 1 && listViewResults.SelectedItems[0] is LogItemBase logItem)
                {
                    GoToItemBase = logItem;
                    DialogResult = true;
                    Close();
                }
            }
        }
    }
}
