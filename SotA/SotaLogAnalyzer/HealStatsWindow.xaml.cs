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
    /// Interaction logic for HealStatsWindow.xaml
    /// </summary>
    public partial class HealStatsWindow : Window
    {
        public class HealStat
        {
            public HealStat(string name)
            {
                HealerName = name;
            }

            public void Add(HealItem item)
            {
                Items.Add(item);
                TotalAmountHealed += item.HealAmount;
            }

            public string HealerName { get; }
            public Int64 TotalAmountHealed { get; set; } = 0;
            public double PercentOfAllHealing { get; set; } = 0.0;

            public double Average { get; set; } = 0.0;

            public int NumberOfHeals
            {
                get => Items.Count;
            }

            public List<HealItem> Items { get; } = new List<HealItem>();
        }

        private SotaLogParser.SotaLog Log;

        public HealStatsWindow(SotaLogParser.SotaLog log)
        {
            Log = log;

            InitializeComponent();

            UpdateStats();
        }

        private void UpdateStats()
        {
            var items = new List<HealStat>();

            Int64 totalHealAmount = 0;

            // Sum up total amount healed by all players
            foreach (var healerName in Log.HealItems.Select(x => x.HealerName).Distinct())
            {
                var healerStat = new HealStat(healerName);

                foreach (var healEvent in Log.HealItems.Where(x => x.HealerName.Equals(healerName)))
                {
                    healerStat.Add(healEvent);

                    totalHealAmount += healEvent.HealAmount;
                }

                items.Add(healerStat);
            }

            // For each healer, calculate their contribution percentage to all heals
            foreach (var item in items)
            {
                item.PercentOfAllHealing = (item.TotalAmountHealed * 100.0) / totalHealAmount;
                item.Average = 1.0 * item.TotalAmountHealed / item.NumberOfHeals;
            }

            listViewStats.ItemsSource = items.OrderByDescending(x => x.TotalAmountHealed).ToList();
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
            if(sender is ListViewItem lvi && lvi.Content is HealStat item)
            {
                var dlg = new PlayerHealStatsWindow(item.Items)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                dlg.ShowDialog();
            }
        }
    }
}
