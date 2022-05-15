using SotaLogParser;
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

namespace SotaLogAnalyzer
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
        }

        private void TextBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                var log = new SotaLogParser.SotaLog();

                try
                {
                    var item = log.ParseLine(TextBoxInput.Text);

                    if(item is null)
                    {
                        TextBoxParseResult.Foreground = Brushes.Tomato;
                        TextBoxParseResult.Text = "(Null)";
                    }
                    else
                    {
                        TextBoxParseResult.Foreground = Brushes.Black;

                        var sb = new StringBuilder();

                        sb.AppendLine($"Type      : {item.GetType().Name}");
                        sb.AppendLine($"Timestamp : {item.Timestamp}");
                        sb.AppendLine(new string('-', 40));

                        switch (item)
                        {
                            case HealItem healItem:
                                sb.AppendLine($"Healer    : {healItem.HealerName}");
                                sb.AppendLine($"Patient   : {healItem.PatientName}");
                                sb.AppendLine($"Amount    : {healItem.HealAmount}");
                                sb.AppendLine($"Critical  : {healItem.Critical}");
                                break;

                            case LootItem lootItem:
                                sb.AppendLine($"Item      : {lootItem.ItemName}");
                                sb.AppendLine($"Looter    : {lootItem.LooterName}");
                                sb.AppendLine($"Value     : {lootItem.ItemValue}");
                                break;

                            case CombatLogItem combatItem:
                                sb.AppendLine($"Attacker  : {combatItem.WhoSource}");
                                sb.AppendLine($"Target    : {combatItem.WhoTarget}");
                                sb.AppendLine($"Damage    : {combatItem.Result.Damage}");
                                sb.AppendLine($"Modifier  : {combatItem.Result.Modifier}");
                                sb.AppendLine($"Skill     : {combatItem.Result.Skill}");
                                sb.AppendLine($"Reason    : {combatItem.Result.Reason}");
                                break;

                            default:
                                sb.AppendLine("Not implemented");
                                break;
                        }

                        TextBoxParseResult.Text = sb.ToString();
                    }
                }
                catch(Exception exception)
                {
                    TextBoxParseResult.Foreground = Brushes.Tomato;
                    TextBoxParseResult.Text = "Exception:\n\n" + exception.Message;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextBoxInput.Focus();
        }
    }
}
