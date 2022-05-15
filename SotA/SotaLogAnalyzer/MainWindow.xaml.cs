
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SotaLogAnalyzer;
using SotaLogParser;

using Path = System.IO.Path;

namespace LogAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private SotaLog? Log;

        private void ListView1_OnPreviewDragOver(object sender, DragEventArgs e)
        {
            if (ctsCancelLoad != null)
                return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[]) e.Data.GetData(DataFormats.FileDrop);

                if (files.Length == 1)
                {
                    e.Handled = true;
                }
            }
        }


        /// <summary>
        /// User performed a "drop" action on the main ListView. If the drop is a file drop,
        /// attempt to load every file dropped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ListView1_OnDrop(object sender, DragEventArgs e)
        {
            try
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                await DoAsyncLoad(files);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is SotaLogParser.CombatLogItem combatItem)
                {
                    var dlg = new CombatItemWindow(combatItem)
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    dlg.ShowDialog();
                }
                else if (item.Content is SotaLogParser.HealItem healItem)
                {
                    var dlg = new HealItemWindow(healItem)
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    dlg.ShowDialog();
                }
                else if (item.Content is SotaLogParser.ChatItem chatItem)
                {
                    var dlg = new ChatItemWindow(chatItem)
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    dlg.ShowDialog();
                }
                else if (item.Content is SotaLogParser.LootRollItem lootItem)
                {
                    var dlg = new LootRollWindow(lootItem)
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    dlg.ShowDialog();
                }
                else if (item.Content is SotaLogParser.LogItemBase otherItem)
                {
                    var dlg = new MiscItemWindow(otherItem)
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    dlg.ShowDialog();
                }
            }
        }

        private void ListViewCombat_OnHeaderClicked(object sender, RoutedEventArgs e)
        {
            if (sender is GridViewColumnHeader header)
            {
                if (header.Tag is string sortBy)
                {
                    var sortOrder = ListSortDirection.Ascending;

                    if (listViewCombat.Items.SortDescriptions.Count > 0 &&
                        listViewCombat.Items.SortDescriptions[0].PropertyName.Equals(sortBy) &&
                        listViewCombat.Items.SortDescriptions[0].Direction == ListSortDirection.Ascending)
                    {
                        sortOrder = ListSortDirection.Descending;
                    }

                    listViewCombat.Items.SortDescriptions.Clear();
                    listViewCombat.Items.SortDescriptions.Add(new SortDescription(sortBy, sortOrder));
                }
            }
        }

        private void ListViewHeals_OnHeaderClicked(object sender, RoutedEventArgs e)
        {
            if (sender is GridViewColumnHeader header)
            {
                if (header.Tag is string sortBy)
                {
                    var sortOrder = ListSortDirection.Ascending;

                    if (listViewHeals.Items.SortDescriptions.Count > 0 &&
                        listViewHeals.Items.SortDescriptions[0].PropertyName.Equals(sortBy) &&
                        listViewHeals.Items.SortDescriptions[0].Direction == ListSortDirection.Ascending)
                    {
                        sortOrder = ListSortDirection.Descending;
                    }

                    listViewHeals.Items.SortDescriptions.Clear();
                    listViewHeals.Items.SortDescriptions.Add(new SortDescription(sortBy, sortOrder));
                }
            }
        }

        private void MenuItemDamageStats_OnClick(object sender, RoutedEventArgs e)
        {
            if (Log is null)
            {
                MessageBox.Show(this, "Please load a log file first!");
                return;
            }

            var dlg = new DamageStatsWindow(Log)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            dlg.ShowDialog();
        }

        private void MenuItemStatFilter_OnClick(object sender, RoutedEventArgs e)
        {
            if (Log is null)
            {
                MessageBox.Show(this, "Please load a log file first!");
                return;
            }

            var dlg = new StatFilterWindow(Log)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (dlg.ShowDialog() == true)
            {
                var log2 = Log.Filter(dlg.TimeStart, dlg.TimeEnd, dlg.SelectedPlayers, dlg.SelectedChats);

                Log = log2;

                listViewCombat.ItemsSource = Log.CombatItems.Where(x => x.Result.AttackResult == CombatLogItem.CombatResult.AttackResults.Hit).ToList(); ;
                listViewHeals.ItemsSource = Log.HealItems;
                listViewChat.ItemsSource = Log.ChatItems;
                listViewLoot.ItemsSource = Log.LootItems;
                listViewOther.ItemsSource = Log.MiscItems;
            }
        }

        private void ListViewChar_HeaderOnClick(object sender, RoutedEventArgs e)
        {
            if (sender is GridViewColumnHeader header)
            {
                if (header.Tag is string sortBy)
                {
                    var sortOrder = ListSortDirection.Ascending;

                    if (listViewChat.Items.SortDescriptions.Count > 0 &&
                        listViewChat.Items.SortDescriptions[0].PropertyName.Equals(sortBy) &&
                        listViewChat.Items.SortDescriptions[0].Direction == ListSortDirection.Ascending)
                    {
                        sortOrder = ListSortDirection.Descending;
                    }

                    listViewChat.Items.SortDescriptions.Clear();
                    listViewChat.Items.SortDescriptions.Add(new SortDescription(sortBy, sortOrder));
                }
            }
        }

        private void ListViewLoot_HeaderClicked(object sender, RoutedEventArgs e)
        {
            if (sender is GridViewColumnHeader header)
            {
                if (header.Tag is string sortBy)
                {
                    var sortOrder = ListSortDirection.Ascending;

                    if (listViewLoot.Items.SortDescriptions.Count > 0 &&
                        listViewLoot.Items.SortDescriptions[0].PropertyName.Equals(sortBy) &&
                        listViewLoot.Items.SortDescriptions[0].Direction == ListSortDirection.Ascending)
                    {
                        sortOrder = ListSortDirection.Descending;
                    }

                    listViewLoot.Items.SortDescriptions.Clear();
                    listViewLoot.Items.SortDescriptions.Add(new SortDescription(sortBy, sortOrder));
                }
            }
        }

        private void MenuItemHealStats_OnClick(object sender, RoutedEventArgs e)
        {
            if (Log is null)
            {
                MessageBox.Show(this, "Please load a log file first!");
                return;
            }

            var dlg = new HealStatsWindow(Log)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            dlg.ShowDialog();
        }

        private void MenuItemExit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItemSaveAs_OnClick(object sender, RoutedEventArgs e)
        {
            if (Log is null)
            {
                MessageBox.Show(this, "Please load a log file first!");
                return;
            }

            var optionsDlg = new ExportOptionsWindow(Log)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (optionsDlg.ShowDialog() == true)
            {
                var dlg = new Microsoft.Win32.SaveFileDialog()
                {
                    Filter = "Log files|*.txt|All files|*.*"
                };

                if (dlg.ShowDialog() == true)
                {
                    var allItems = new List<LogItemBase>();

                    if(!(optionsDlg.SelectedChats is null))
                    {
                        var chatItems = Log.ChatItems.Where(x => optionsDlg.SelectedChats.Contains(x.ChatName)).Select(x => x);
                        
                        allItems.AddRange(chatItems);
                    }


                    if (!(optionsDlg.SelectedLooters is null))
                    {
                        var lootItems = Log.LootItems.Where(x => optionsDlg.SelectedLooters.Contains(x.LooterName))
                            .Select(x => x);

                        allItems.AddRange(lootItems);
                    }


                    if (!(optionsDlg.SelectedCombattants is null))
                    {
                        var combatItems = Log.CombatItems
                            .Where(x => optionsDlg.SelectedCombattants.Contains(x.WhoSource)).Select(x => x);

                        allItems.AddRange(combatItems);
                    }

                    if (!(optionsDlg.SelectedHealers is null))
                    {
                        var healItems = Log.HealItems.Where(x => optionsDlg.SelectedHealers.Contains(x.HealerName))
                            .Select(x => x);

                        allItems.AddRange(healItems);
                    }

                    if (optionsDlg.ExportMisc)
                    {
                        allItems.AddRange(Log.MiscItems);
                    }

                    File.WriteAllLines(dlg.FileName, allItems.OrderBy(x => x.Timestamp).Select(x => x.Line));
                }
            }
        }

        private void ShowInEditor(LogItemBase itemBase)
        {
            if (itemBase.FileName is not null)
            {
                NotepadPlusPlusHelper.OpenEditor(itemBase.FileName, itemBase.LineNumber);
            }
        }

        private void ListViewOther_OnKeyDown(object sender, KeyEventArgs e)
        {
            // CTRL-F -> Open search window
            if (e.Key == Key.F && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (Log is not null)
                {
                    var dlg = new FindWindow(Log.MiscItems);

                    if (dlg.ShowDialog() == true && dlg.GoToItemBase != null)
                    {
                        listViewOther.ScrollIntoView(dlg.GoToItemBase);
                        listViewOther.SelectedItems.Clear();
                        listViewOther.SelectedItems.Add(dlg.GoToItemBase);
                    }
                }
            }

            // CTRL-E -> Open current item in notepad++
            else if (e.Key == Key.E && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (listViewOther.SelectedItems.Count == 1)
                {
                    if (listViewOther.SelectedItems[0] is LogItemBase selectedItem)
                    {
                        ShowInEditor(selectedItem);
                    }
                }
            }

            // CTRL-T -> Open Test Window
            else if(e.Key == Key.T && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                var dlg = new TestWindow
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                dlg.Show();
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is GridViewColumnHeader header)
            {
                if (header.Tag is string sortBy)
                {
                    var sortOrder = ListSortDirection.Ascending;

                    if (listViewOther.Items.SortDescriptions.Count > 0 &&
                        listViewOther.Items.SortDescriptions[0].PropertyName.Equals(sortBy) &&
                        listViewOther.Items.SortDescriptions[0].Direction == ListSortDirection.Ascending)
                    {
                        sortOrder = ListSortDirection.Descending;
                    }

                    listViewOther.Items.SortDescriptions.Clear();
                    listViewOther.Items.SortDescriptions.Add(new SortDescription(sortBy, sortOrder));
                }
            }
        }

        private void ListViewCombat_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (Log is not null)
                {
                    var dlg = new FindWindow(Log.CombatItems.Select(x => (LogItemBase)x).ToList());

                    if (dlg.ShowDialog() == true && dlg.GoToItemBase != null)
                    {
                        listViewCombat.ScrollIntoView(dlg.GoToItemBase);
                        listViewCombat.SelectedItems.Clear();
                        listViewCombat.SelectedItems.Add(dlg.GoToItemBase);
                    }
                }
            }
            else if (e.Key == Key.E && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (listViewCombat.SelectedItems.Count == 1)
                {
                    if (listViewCombat.SelectedItems[0] is LogItemBase selectedItem)
                    {
                        ShowInEditor(selectedItem);
                    }
                }
            }
        }

        private void ListViewHeals_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (Log is not null)
                {
                    var dlg = new FindWindow(Log.HealItems.Select(x => (LogItemBase)x).ToList());

                    if (dlg.ShowDialog() == true && dlg.GoToItemBase != null)
                    {
                        listViewHeals.ScrollIntoView(dlg.GoToItemBase);
                        listViewHeals.SelectedItems.Clear();
                        listViewHeals.SelectedItems.Add(dlg.GoToItemBase);
                    }
                }
            }
            else if (e.Key == Key.E && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (listViewHeals.SelectedItems.Count == 1)
                {
                    if (listViewHeals.SelectedItems[0] is LogItemBase selectedItem)
                    {
                        ShowInEditor(selectedItem);
                    }
                }
            }
        }

        private void ListViewChat_OnKeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl-F, Search
            if (e.Key == Key.F && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (Log is not null)
                {
                    var dlg = new FindWindow(Log.ChatItems.Select(x => (LogItemBase)x).ToList());

                    if (dlg.ShowDialog() == true && dlg.GoToItemBase != null)
                    {
                        listViewChat.ScrollIntoView(dlg.GoToItemBase);
                        listViewChat.SelectedItems.Clear();
                        listViewChat.SelectedItems.Add(dlg.GoToItemBase);
                    }
                }
            }

            // Ctrl-E, Open in Notepad++
            else if (e.Key == Key.E && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (listViewChat.SelectedItems.Count == 1)
                {
                    if (listViewChat.SelectedItems[0] is LogItemBase selectedItem)
                    {
                        ShowInEditor(selectedItem);
                    }
                }
            }
        }

        private void ListViewLoot_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (Log is not null)
                {
                    var dlg = new FindWindow(Log.LootItems.Select(x => (LogItemBase)x).ToList());

                    if (dlg.ShowDialog() == true && dlg.GoToItemBase != null)
                    {
                        listViewLoot.ScrollIntoView(dlg.GoToItemBase);
                        listViewLoot.SelectedItems.Clear();
                        listViewLoot.SelectedItems.Add(dlg.GoToItemBase);
                    }
                }
            }
            else if (e.Key == Key.E && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (listViewLoot.SelectedItems.Count == 1)
                {
                    if (listViewLoot.SelectedItems[0] is LogItemBase selectedItem)
                    {
                        ShowInEditor(selectedItem);
                    }
                }
            }
        }

        private CancellationTokenSource? ctsCancelLoad = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private async Task DoAsyncLoad(IEnumerable<string> files)
        {
            var fileNo = 0;
            var numFiles = files.Count();

            if (ctsCancelLoad != null)
                return;

            ctsCancelLoad = new CancellationTokenSource();

            var log = new SotaLogParser.SotaLog();

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            foreach (var file in files)
            {
                fileNo += 1;

                await log.LoadAsync(file, d =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        statusBarItem1.Content = numFiles == 1 ? $"{d:F1}%" : $"File {fileNo}/{numFiles} {d:F1}%";
                    });
                }, ctsCancelLoad.Token);
            }

            stopWatch.Stop();

            ctsCancelLoad = null;

            statusBarItem1.Content = $"Parsing done in {stopWatch.Elapsed.TotalSeconds:F3}s.";

            Log = log;

            listViewCombat.ItemsSource = Log.CombatItems.Where(x => x.Result.AttackResult == CombatLogItem.CombatResult.AttackResults.Hit).ToList(); ;
            listViewHeals.ItemsSource = Log.HealItems;
            listViewChat.ItemsSource = Log.ChatItems;
            listViewLoot.ItemsSource = Log.LootItems;
            listViewOther.ItemsSource = Log.MiscItems;
        }

        private void MenuItemLevelUpStats_OnClick(object sender, RoutedEventArgs e)
        {
            if (Log is null)
            {
                MessageBox.Show(this, "Please load a log file first!");
                return;
            }

            var dlg = new LevelUpStatsWindow(Log.LevelUpItems)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            dlg.ShowDialog();
        }

        private void MenuItemDamageReceivedStats_OnClick(object sender, RoutedEventArgs e)
        {
            if (Log is null)
                return;

            var dlg = new DamageReceivedWindow(Log)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            dlg.ShowDialog();
        }

        private void HyperlinkLogPath_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var logPath = Path.Combine(appData, @"Portalarium\Shroud of the Avatar\ChatLogs");

            if (Directory.Exists(logPath))
            {
                Process.Start(@"explorer.exe", logPath);
            }
        }
    }
}
