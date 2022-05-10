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

namespace LogAnalyzer
{
    /// <summary>
    /// Interaction logic for DamageReceivedWindow.xaml
    /// </summary>
    public partial class DamageReceivedWindow : Window
    {
        private SotaLogParser.SotaLog Log;

        public DamageReceivedWindow(SotaLogParser.SotaLog log)
        {
            Log = log;

            InitializeComponent();

            UpdateStats();
        }

        private void UpdateStats()
        {
            var stats = new List<DamageReceivedStats>();

            foreach (var target in Log.CombatItems.Select(x => x.WhoTarget).Distinct())
            {
                var item = new DamageReceivedStats(target);

                foreach (var foo in Log.CombatItems.Where(x => x.WhoTarget.Equals(target)))
                {
                    item.Add(foo);
                }

                stats.Add(item);
            }

            listViewStats.ItemsSource = stats;
            listViewStats.Items.SortDescriptions.Add(new SortDescription("DamageTotal", ListSortDirection.Descending));
        }


        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem lvi && lvi.Content is DamageReceivedStats stats)
            {
                var dlg = new PlayerDamageReceivedStatsWindow(stats)
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
    }
}
