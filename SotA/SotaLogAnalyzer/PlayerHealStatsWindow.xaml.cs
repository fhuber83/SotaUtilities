using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using SotaLogAnalyzer;
using SotaLogParser;

namespace LogAnalyzer
{
    /// <summary>
    /// Interaction logic for PlayerHealStatsWindow.xaml
    /// </summary>
    public partial class PlayerHealStatsWindow : Window
    {
        public List<SotaLogParser.HealItem> Items { get; private set; }

        public PlayerHealStatsWindow(List<SotaLogParser.HealItem> items)
        {
            Items = items;

            InitializeComponent();

            Title = $"Heals performed by {items[0].HealerName}";

            listViewStats.ItemsSource = Items;
            listViewStats.Items.SortDescriptions.Add(new SortDescription("Timestamp", ListSortDirection.Ascending));
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

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            if(sender is ListViewItem lvi && lvi.Content is SotaLogParser.HealItem item)
            {
                var dlg = new HealItemWindow(item)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                dlg.ShowDialog();
            }
        }

        private void ListViewStats_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.E && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (listViewStats.SelectedItems.Count == 1 && listViewStats.SelectedItems[0] is HealItem item)
                {
                    NotepadPlusPlusHelper.OpenEditor(item.FileName, item.LineNumber);
                }
            }
        }
    }
}
