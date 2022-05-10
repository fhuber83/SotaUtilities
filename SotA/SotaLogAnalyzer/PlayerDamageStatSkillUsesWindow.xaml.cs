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
    /// Interaction logic for PlayerDamageStatSkillUsesWindow.xaml
    /// </summary>
    public partial class PlayerDamageStatSkillUsesWindow : Window
    {
        public PlayerDamageStatSkillUsesWindow(List<CombatLogItem> items)
        {
            InitializeComponent();

            var skillName = string.IsNullOrEmpty(items[0].Result.Skill) ? "(Attack)" : items[0].Result.Skill;
            Title = $"Attacks done by {items[0].WhoSource} with {skillName}";

            listViewStats.ItemsSource = items;
        }

        private void ListViewCombat_OnHeaderClicked(object sender, RoutedEventArgs e)
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
            if (sender is ListViewItem item)
            {
                if (item.Content is CombatLogItem logItem)
                {
                    var dlg = new CombatItemWindow(logItem)
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    dlg.ShowDialog();
                }
            }
        }
    }
}
