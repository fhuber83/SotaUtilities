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
    /// Interaction logic for PlayerDamageStatsWindow.xaml
    /// </summary>
    public partial class PlayerDamageStatsWindow : Window
    {
        private SotaLogParser.SotaLog Log;
        private string PlayerName;

        public class PlayerSkillStat
        {
            public PlayerSkillStat(string name)
            {
                SkillName = name;
            }

            public void AddSkillUse(CombatLogItem itemBase)
            {
                NumberOfUses += 1;
                DamageTotal += itemBase.Result.Damage;
                LogItems.Add(itemBase);
            }

            public string SkillName { get; }
            public int NumberOfUses { get; private set; } = 0;
            public Int64 DamageTotal { get; private set; } = 0;
            public double DamageAverage => (1.0 * DamageTotal) / NumberOfUses;

            public List<SotaLogParser.CombatLogItem> LogItems { get; } = new List<CombatLogItem>();
        }

        public PlayerDamageStatsWindow(SotaLogParser.SotaLog log, string playerName)
        {
            Log = log;
            PlayerName = playerName;

            InitializeComponent();

            Title = $"Attacks by {playerName}";

            UpdateStats();
        }

        private void UpdateStats()
        {
            var items = new List<PlayerSkillStat>();

            foreach (var skill in Log.CombatItems.Where(x => x.WhoSource.Equals(PlayerName) && !(x.Result.Skill is null)).Select(x => x.Result.Skill).Distinct())
            {
                var skillStat = new PlayerSkillStat(skill);

                foreach (var foo in Log.CombatItems
                    .Where(x => x.WhoSource.Equals(PlayerName) && !(x.Result.Skill is null) && x.Result.Skill.Equals(skill)).Select(x => x))
                {
                    skillStat.AddSkillUse(foo);
                }

                items.Add(skillStat);
            }

            var basicAttackStat = new PlayerSkillStat("(Attack)");
            foreach (var foo in Log.CombatItems
                    .Where(x => x.WhoSource.Equals(PlayerName) && x.Result.Skill is null).Select(x => x))
            {
                basicAttackStat.AddSkillUse(foo);
            }

            if (basicAttackStat.NumberOfUses > 0)
            {
                items.Add(basicAttackStat);
            }

            listViewStats.ItemsSource = items;

            listViewStats.Items.SortDescriptions.Add(new SortDescription("DamageTotal", ListSortDirection.Descending));
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
            if (sender is ListViewItem item)
            {
                if (item.Content is PlayerSkillStat stat)
                {
                    var dlg = new PlayerDamageStatSkillUsesWindow(stat.LogItems)
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
