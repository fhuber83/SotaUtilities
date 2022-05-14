using System;
using System.Collections.Generic;
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
    /// Interaction logic for ExportOptionsWindow.xaml
    /// </summary>
    public partial class ExportOptionsWindow : Window
    {
        private SotaLog Log;

        public ExportOptionsWindow(SotaLog log)
        {
            Log = log;

            InitializeComponent();

            foreach (var chatName in Log.ChatItems.Select(x => x.ChatName.Trim()).Distinct().OrderBy(x => x))
            {
                ((groupBoxChat.Content as ScrollViewer)?.Content as StackPanel)?.Children.Add(new CheckBox
                {
                    Content = chatName,
                    IsChecked = true
                });
            }

            foreach(var looterName in log.LootItems.Select(x => x.LooterName?.Trim()).Distinct().OrderBy(x => x))
            { 
                ((groupBoxLoot.Content as ScrollViewer)?.Content as StackPanel)?.Children.Add(new CheckBox
                {
                    Content = String.IsNullOrEmpty(looterName) ? "(Self)" : looterName,
                    IsChecked = true
                });
            }

            foreach (var combattantName in log.CombatItems.Select(x => x.WhoSource).Distinct().OrderBy(x => x))
            {
                ((groupBoxCombat.Content as ScrollViewer)?.Content as StackPanel)?.Children.Add(new CheckBox
                {
                    Content = combattantName,
                    IsChecked = true
                });
            }

            foreach (var healerName in log.HealItems.Select(x => x.HealerName).Distinct().OrderBy(x => x))
            {
                ((groupBoxHeal.Content as ScrollViewer)?.Content as StackPanel)?.Children.Add(new CheckBox
                {
                    Content = healerName,
                    IsChecked = true
                });
            }
        }

        private void CheckBoxChatEnable_OnCheckChanged(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.Parent is GroupBox gb && gb.Content is UIElement uiContent)
            {
                uiContent.IsEnabled = cb.IsChecked == true;
            }
        }

        public List<string>? SelectedChats { get; private set; }
        public List<string>? SelectedLooters { get; private set; }
        public List<string>? SelectedCombattants { get; private set; }
        public List<string>? SelectedHealers { get; private set; }
        public bool ExportMisc { get; private set; } = false;

        private void ButtonOK_OnClick(object sender, RoutedEventArgs e)
        {
            if (groupBoxChat.Header is CheckBox checkBoxChatEnabled && checkBoxChatEnabled.IsChecked == true)
            {
                if (groupBoxChat.Content is ScrollViewer scrollViewerChat &&
                    scrollViewerChat.Content is StackPanel stackPanelChat)
                {
                    SelectedChats = new List<string>();

                    foreach (CheckBox cbChat in stackPanelChat.Children)
                    {
                        if (cbChat.IsChecked == true)
                        {
                            if (cbChat.Content is string strChat)
                            {
                                SelectedChats.Add(strChat);
                            }
                        }
                    }
                }
            }

            if (groupBoxLoot.Header is CheckBox checkBoxLootEnabled && checkBoxLootEnabled.IsChecked == true)
            {
                if (groupBoxLoot.Content is ScrollViewer scrollViewerLoot &&
                    scrollViewerLoot.Content is StackPanel stackPanelLoot)
                {
                    SelectedLooters = new List<string>();

                    foreach (CheckBox cbLoot in stackPanelLoot.Children)
                    {
                        if (cbLoot.IsChecked == true)
                        {
                            if (cbLoot.Content is string strLoot)
                            {
                                SelectedLooters.Add(strLoot);
                            }
                        }
                    }
                }
            }

            if (groupBoxCombat.Header is CheckBox checkBoxCombatEnabled && checkBoxCombatEnabled.IsChecked == true)
            {
                if (groupBoxCombat.Content is ScrollViewer scrollViewerCombat &&
                    scrollViewerCombat.Content is StackPanel stackPanelCombat)
                {
                    SelectedCombattants = new List<string>();

                    foreach (CheckBox cbCombattant in stackPanelCombat.Children)
                    {
                        if (cbCombattant.IsChecked == true)
                        {
                            if (cbCombattant.Content is string strCombattant)
                            {
                                SelectedCombattants.Add(strCombattant);
                            }
                        }
                    }
                }
            }

            if (groupBoxHeal.Header is CheckBox checkBoxHealEnabled && checkBoxHealEnabled.IsChecked == true)
            {
                if (groupBoxHeal.Content is ScrollViewer scrollViewerHeal &&
                    scrollViewerHeal.Content is StackPanel stackPanelHeal)
                {
                    SelectedHealers = new List<string>();

                    foreach (CheckBox cbHealer in stackPanelHeal.Children)
                    {
                        if (cbHealer.IsChecked == true)
                        {
                            if (cbHealer.Content is string strHealer)
                            {
                                SelectedHealers.Add(strHealer);
                            }
                        }
                    }
                }
            }

            ExportMisc = checkBoxMisc.IsChecked == true;

            DialogResult = true;
            Close();
        }
    }
}
