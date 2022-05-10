using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using SotaLogParser;

namespace LogAnalyzer
{
    /// <summary>
    /// Interaction logic for LevelUpStatsWindow.xaml
    /// </summary>
    public partial class LevelUpStatsWindow : Window
    {
        public List<SotaLogParser.LevelUpItem> Items;

        public LevelUpStatsWindow(List<SotaLogParser.LevelUpItem> items)
        {
            Items = items;

            InitializeComponent();

            foreach (var name in items.Select(x => x.Name).Distinct().OrderBy(x => x))
            {
                comboBoxPlayer.Items.Add(name);
            }

            foreach (var skill in items.Select(x => x.Skill).Distinct().OrderBy(x => x))
            {
                comboBoxSkill.Items.Add(skill);
            }

            listViewStats.ItemsSource = items;

            listViewStats.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            listViewStats.Items.SortDescriptions.Add(new SortDescription("Skill", ListSortDirection.Ascending));
            listViewStats.Items.SortDescriptions.Add(new SortDescription("Level", ListSortDirection.Ascending));
        }

        private void ListViewStats_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.E && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (listViewStats.SelectedItems.Count == 1 && listViewStats.SelectedItems[0] is LevelUpItem item)
                {
                    //(Application.Current as App)?.LaunchEditor(item.FileName, item.LineNumber);
                }
            }
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem lvi && lvi.Content is LevelUpItem item)
            {
                var dlg = new MiscItemWindow(item)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                dlg.ShowDialog();
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is GridViewColumnHeader header)
            {
                if (header.Tag is string sortBy)
                {
                    var sortOrder = ListSortDirection.Ascending;

                    if (listViewStats.Items.SortDescriptions.Count > 0 &&
                        listViewStats.Items.SortDescriptions[0].PropertyName.Equals(sortBy) &&
                        listViewStats.Items.SortDescriptions[0].Direction == ListSortDirection.Ascending)
                    {
                        sortOrder = ListSortDirection.Descending;
                    }

                    listViewStats.Items.SortDescriptions.Clear();
                    listViewStats.Items.SortDescriptions.Add(new SortDescription(sortBy, sortOrder));
                }
            }
        }

        private void ComboBoxSkill_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxPlayer is null || comboBoxSkill is null || Items is null || listViewStats is null)
                return;

            var player = comboBoxPlayer.SelectedItem as string;
            var skill = comboBoxSkill.SelectedItem as string;

            if (player is null && skill is null)
            {
                listViewStats.ItemsSource = Items;
            }
            else
            {
                listViewStats.ItemsSource = Items.Where(x => (player is null || x.Name.Equals(player)) && (skill is null || x.Skill.Equals(skill)))
                    .Select(x => x);
            }
        }

        private void ButtonExportCSV_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV File|*.csv|All files|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                var lines = new List<string>();

                string quot = "\"";
                string sep = ",";

                lines.Add($"{quot}Date{quot}{sep}{quot}Time{quot}{sep}{quot}Player{quot}{sep}{quot}Skill{quot}{sep}{quot}Level{quot}");


                foreach (LevelUpItem item in listViewStats.Items)
                {
                    var date = $"{item.Timestamp.Year:D4}-{item.Timestamp.Month:D2}-{item.Timestamp.Day:D2}";
                    var time = $"{item.Timestamp.Hour:D2}:{item.Timestamp.Minute:D2}:{item.Timestamp.Second:D2}";

                    lines.Add($"{quot}{date}{quot}{sep}{quot}{time}{quot}{sep}{quot}{item.Name}{quot}{sep}{quot}{item.Skill}{quot}{sep}{quot}{item.Level}{quot}");
                }

                File.WriteAllLines(dlg.FileName, lines);
            }
        }
    }
}
