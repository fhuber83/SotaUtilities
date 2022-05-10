using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for PlayerDamageReceivedStatsWindow.xaml
    /// </summary>
    public partial class PlayerDamageReceivedStatsWindow : Window
    {
        
        public PlayerDamageReceivedStatsWindow(DamageReceivedStats stats)
        {
            InitializeComponent();

            Title = $"Damage received by {stats.Name}";

            ListViewStats.ItemsSource = stats.Items;
        }

        private void ItemDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is ListViewItem item)
            {
                if (item.Content is CombatLogItem combatItem)
                {
                    var dlg = new CombatItemWindow(combatItem)
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    dlg.ShowDialog();
                }
            }
        }

        private void HeaderClicked(object sender, RoutedEventArgs e)
        {
            if (sender is GridViewColumnHeader header)
            {
                if (header.Tag is string sortBy)
                {
                    var sortOrder = ListSortDirection.Ascending;

                    if (ListViewStats.Items.SortDescriptions.Count > 0 &&
                        ListViewStats.Items.SortDescriptions[0].PropertyName.Equals(sortBy) &&
                        ListViewStats.Items.SortDescriptions[0].Direction == ListSortDirection.Ascending)
                    {
                        sortOrder = ListSortDirection.Descending;
                    }

                    ListViewStats.Items.SortDescriptions.Clear();
                    ListViewStats.Items.SortDescriptions.Add(new SortDescription(sortBy, sortOrder));
                }
            }
        }
    }
}
