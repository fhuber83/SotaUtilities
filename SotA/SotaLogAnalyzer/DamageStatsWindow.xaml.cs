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
    /// Interaction logic for DamageStatsWindow.xaml
    /// </summary>
    public partial class DamageStatsWindow : Window
    {
        public class PlayerDamageStatItem
        {
            public PlayerDamageStatItem(string name)
            {
                PlayerName = name;
                DamageTotal = 0;
            }

            public void AddDamage(int amount, string skill)
            {
                DamageTotal += amount;
            }

            public void CalculateDamagePercent(Int64 totalDamage)
            {
                DamagePercent = (DamageTotal * 100.0) / totalDamage;
            }

            public void CalculateDamagePerSecond(double seconds)
            {
                if (seconds < 1.0)
                    DamagePerSecond = 0.0;

                else DamagePerSecond = DamageTotal / seconds;
            }

            public string PlayerName { get; }
            public Int64 DamageTotal { get; private set; }
            public double DamagePercent { get; private set; }
            public double DamagePerSecond { get; private set; }
        }

        private SotaLog Log;

        public DamageStatsWindow(SotaLog log)
        {
            Log = log;

            InitializeComponent();

            CalculateStats();
        }

        private void CalculateStats()
        {
            var stats = new List<PlayerDamageStatItem>();

            var players = Log.CombatItems.Select(x => x.WhoSource).Distinct().ToList();

            Int64 SumAllDamage = 0;

            foreach (var player in players)
            {
                var playerStats = new PlayerDamageStatItem(player);

                foreach(var foo in Log.CombatItems.Where(x => x.WhoSource.Equals(player)).Select(x => x))
                {
                    playerStats.AddDamage(foo.Result.Damage, foo.Result.Skill);

                    SumAllDamage += foo.Result.Damage;
                }

                stats.Add(playerStats);
            }

            // Second pass: Calculate percent of total and DPS
            var timeStart = Log.CombatItems.Min(x => x.Timestamp);
            var timeEnd = Log.CombatItems.Max(x => x.Timestamp);
            var seconds = (timeEnd - timeStart).TotalSeconds;

            foreach (PlayerDamageStatItem player in stats)
            {
                // Experiment
                var foo = Log.CombatItems.Where(x => x.WhoSource.Equals(player.PlayerName)).OrderBy(x => x.Timestamp)
                    .Select(x => x.Timestamp);
                var t_min = foo.Min();
                var t_max = foo.Max();
                var t_delta = t_max - t_min;



                player.CalculateDamagePercent(SumAllDamage);
                player.CalculateDamagePerSecond(t_delta.TotalSeconds /*seconds*/);
            }

            listViewStats.ItemsSource = stats;
            listViewStats.Items.SortDescriptions.Add(new SortDescription("DamageTotal", ListSortDirection.Descending));
        }

        private void ListViewItem_DoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is PlayerDamageStatItem statItem)
                {
                    var dlg = new PlayerDamageStatsWindow(Log, statItem.PlayerName)
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    dlg.ShowDialog();
                }
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
    }
}
