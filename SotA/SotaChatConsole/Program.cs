namespace LogWatcherTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("SotA Console Watcher");
            Console.WriteLine("Press [CTRL-D] to stop, [F5] to toggle verbose output, [CTRL-L] to clear screen");
            Console.WriteLine();

            Console.CursorVisible = false;

            var sotaWatch = new SotaWatcher();

            var done = false;


            //
            //  Main user input loop. Log watcher is running in the background
            //
            while (!done)
            {
                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    //
                    // Ctrl-D or [Esc] -> Exit program
                    //
                    case ConsoleKey.D:
                        if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
                        { 
                            Console.WriteLine("Exiting...");
                            done = true;
                        }
                        break;
                    case ConsoleKey.Escape:
                        Console.WriteLine("Exiting...");
                        done = true;
                        break;


                    //
                    // [F5] -> Toggle verbose output
                    //
                    case ConsoleKey.F5:
                        sotaWatch.VerboseOutput = !sotaWatch.VerboseOutput;
                        break;


                    //
                    // CTRL-L or [Del] -> Clear screen
                    //
                    case ConsoleKey.Delete:
                        Console.Clear();
                        break;
                    case ConsoleKey.L:
                        if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
                        {
                            Console.Clear();
                        }
                        else
                        {
                            sotaWatch.ShowLevelUps = !sotaWatch.ShowLevelUps;
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("   [Show level ups : " + (sotaWatch.ShowLevelUps ? "ON" : "OFF") + "]");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        break;


                    //
                    // M -> Toggle Miscellaneous items
                    //
                    case ConsoleKey.M:
                        sotaWatch.ShowMiscItems = !sotaWatch.ShowMiscItems;
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("   [Misc items : " + (sotaWatch.ShowMiscItems ? "ON" : "OFF") + "]");
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    

                    //
                    // T -> Toggle Trader chat
                    //
                    case ConsoleKey.T:
                        sotaWatch.ShowTradeChat = !sotaWatch.ShowTradeChat;
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("   [Trader chat : " + (sotaWatch.ShowTradeChat ? "ON" : "OFF") + "]");
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    

                    //
                    // U -> Toggle Universe chat
                    //
                    case ConsoleKey.U:
                        sotaWatch.ShowUniverseChat = !sotaWatch.ShowUniverseChat;
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("   [Universe chat : " + (sotaWatch.ShowUniverseChat ? "ON" : "OFF") + "]");
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    

                    //
                    // H -> Toggle "show heals"
                    //
                    case ConsoleKey.H:
                        sotaWatch.ShowHeals = !sotaWatch.ShowHeals;
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("   [Show heals : " + (sotaWatch.ShowHeals ? "ON" : "OFF") + "]");
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    

                    //
                    // O -> Toggle "Show Loot"
                    //
                    case ConsoleKey.O:
                        sotaWatch.ShowLoot = !sotaWatch.ShowLoot;
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("   [Show loot : " + (sotaWatch.ShowLoot ? "ON" : "OFF") + "]");
                        Console.ForegroundColor = ConsoleColor.White;
                        break;


                    //
                    // G -> Toggle guild chat
                    //
                    case ConsoleKey.G:
                        sotaWatch.ShowGuildChat = !sotaWatch.ShowGuildChat;
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("   [Guild chat : " + (sotaWatch.ShowGuildChat ? "ON" : "OFF") + "]");
                        Console.ForegroundColor = ConsoleColor.White;
                        break;


                    //
                    // Z -> Toggle Zone Chat
                    //
                    case ConsoleKey.Z:
                        sotaWatch.ShowZoneChat = !sotaWatch.ShowZoneChat;
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("   [Zone chat : " + (sotaWatch.ShowZoneChat ? "ON" : "OFF") + "]");
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
            }
        }
    }
}
