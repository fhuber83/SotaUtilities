using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SotaLogParser;

namespace LogWatcherTest
{
    internal class SotaWatcher
    {
        private Dictionary<string, FileInfo> Files = new Dictionary<string, FileInfo>();
        
        private bool verboseOutput_ = false;
        public bool VerboseOutput
        {
            get => verboseOutput_;
            set
            {
                verboseOutput_ = value;
                OutputLine("Verbose output turned " + (verboseOutput_ ? "ON" : "OFF"), ConsoleColor.Blue);
                OutputLine();
            }
        }

        public SotaWatcher()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var logPath = Path.Combine(appDataPath, "Portalarium\\Shroud of the Avatar\\ChatLogs");

            if (!Directory.Exists(logPath))
            {
                throw new Exception("Log directory not found");
            }

            int maxLength = 0;

            foreach (var file in Directory.GetFiles(logPath, "*.txt", SearchOption.TopDirectoryOnly))
            {
                AddFile(file);
                maxLength = Math.Max(maxLength, file.Length);
            }

            if (VerboseOutput)
            {
                OutputLine();
                OutputLine(new String('=', maxLength + 17));
                OutputLine();
            }

            var watch = new FileSystemWatcher(logPath);

            watch.NotifyFilter = NotifyFilters.LastWrite;

            watch.Changed += WatchOnChanged;
            watch.EnableRaisingEvents = true;
        }


        private void OutputLine(string line = "\n", ConsoleColor color = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = color;
            Console.BackgroundColor = backgroundColor;
            Console.WriteLine("   " + line);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public bool ShowMiscItems = false;
        public bool ShowUniverseChat = true;
        public bool ShowGuildChat = true;
        public bool ShowTradeChat = true;
        public bool ShowLevelUps = false;
        public bool ShowHeals = false;
        public bool ShowPartyChat = true;
        public bool ShowLoot = false;
        public bool ShowZoneChat = true;

        protected virtual void OnNewLine(FileInfo fileInfo, string line)
        {
            var item = fileInfo.Log.ParseLine(line);

            if (item == null)
            {
                if (verboseOutput_)
                {
                    OutputLine($"Unable to parse line: \"{line}\"", ConsoleColor.Red);
                }
            }
            else
            {
                switch (item)
                {
                    case SotaLogParser.ChatItem chatItem:

                        var color = ConsoleColor.Blue;
                        var show = true;
                        
                        switch (chatItem.ChatName)
                        {
                            case "Universe":
                                color = ConsoleColor.White;
                                show = ShowUniverseChat;
                                break;
                            case "Guild":
                                color = ConsoleColor.DarkYellow;
                                show = ShowGuildChat;
                                break;
                            case "Traders":
                                color = ConsoleColor.Yellow;
                                show = ShowTradeChat;
                                break;
                            case "Party":
                                color = ConsoleColor.Green;
                                show = ShowPartyChat;
                                break;
                            case "Zone":
                                color = ConsoleColor.Magenta;
                                show = ShowZoneChat;
                                break;
                        }

                        if (show)
                        {
                            OutputLine(
                                $"[{chatItem.Timestamp.Hour:D2}:{chatItem.Timestamp.Minute:D2}] {chatItem.Name}: {chatItem.Message}",
                                color);
                        }

                        break;
                    
                    case SotaLogParser.LevelUpItem levelUpItem:
                        var isAdvLevel = levelUpItem.Skill == "Adventurer Level";
                        var isProdLevel = levelUpItem.Skill == "Producer Level";
                        if (ShowLevelUps || isAdvLevel || isProdLevel)
                        {
                            if (isAdvLevel || isProdLevel)
                            {
                                OutputLine($"{levelUpItem.RestOfLine}", ConsoleColor.White, ConsoleColor.Red);
                            }
                            else
                            {
                                OutputLine($"{levelUpItem.RestOfLine}", ConsoleColor.Red, ConsoleColor.White);
                            }
                        }
                        break;
                    
                    case SotaLogParser.HealItem healItem:
                        if (ShowHeals)
                        {
                            OutputLine($"{healItem.RestOfLine}", ConsoleColor.Green);
                        }
                        break;
                    
                    case SotaLogParser.LootItem lootItem:
                        if (ShowLoot)
                        {
                            if (lootItem is LootRollItem rollItem)
                            {
                                OutputLine($"{lootItem.LooterName} looted {lootItem.ItemName}", ConsoleColor.Magenta);
                                
                                if (verboseOutput_)
                                {
                                    OutputLine($"  Rolls:", ConsoleColor.DarkMagenta);

                                    foreach (var roll in rollItem.Rolls)
                                    {
                                        OutputLine($"    {roll.Value:D3}  {roll.Key}");
                                    }
                                }
                            }
                            else
                            {
                                OutputLine($"{lootItem.LooterName} looted {lootItem.ItemName}", ConsoleColor.Magenta);
                            }
                        }

                        break;

                    default:
                        if (ShowMiscItems)
                        {
                            OutputLine(item.RestOfLine, ConsoleColor.DarkGray);
                        }

                        break;
                }
            }
        }
        
        private byte[] buffer = new byte[4096];

        private void WatchOnChanged(object sender, FileSystemEventArgs e)
        {
            if (!Files.ContainsKey(e.FullPath))
            {
                AddFile(e.FullPath);
            }

            else
            {
                if (VerboseOutput)
                {
                    OutputLine($"Existing file: {e.FullPath}", ConsoleColor.Green);
                }

                var oldInfo = Files[e.FullPath];
                var newInfo = new System.IO.FileInfo(e.FullPath);

                var delta = newInfo.Length - oldInfo.LastSize;

                if (delta == 0)
                {
                    if (VerboseOutput)
                    {
                        Console.WriteLine($"   No new bytes? Old = {oldInfo.LastSize}, New = {newInfo.Length}]");
                    }
                }

                else
                {
                    if (VerboseOutput)
                    {
                        Console.WriteLine($"   {delta} new bytes [Old={oldInfo.LastSize}; New={newInfo.Length}]");
                    }

                    try
                    {
                        // Lazily resize buffer to accomodate new file contents
                        if (buffer.Length < delta)
                        {
                            var oldSize = buffer.Length;
                            var newSize = buffer.Length;

                            // Only resize to powers of two
                            do
                            {
                                newSize *= 2;
                            } while (newSize < delta);

                            buffer = new byte[newSize];

                            if (verboseOutput_)
                            {
                                OutputLine($"   Buffer resized [{oldSize} -> {buffer.Length}]", ConsoleColor.Blue);
                            }
                        }

                        using (var stream = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            var offset = (int)oldInfo.LastSize;
                            var bytesToRead = (int)delta;

                            if (VerboseOutput)
                            {
                                Console.WriteLine($"   Reading {bytesToRead} bytes, starting at offset {offset}");
                            }

                            stream.Seek(oldInfo.LastSize, SeekOrigin.Begin);

                            var bytesActuallyRead = stream.Read(buffer, 0, bytesToRead);

                            if (VerboseOutput)
                            {
                                Console.WriteLine($"   Read {bytesActuallyRead} bytes");
                            }
                        }

                        var message = Encoding.UTF8.GetString(buffer, 0, (int)delta);

                        // Actual chat output happens here
                        foreach (var line in message.Split("\r\n"))
                        {
                            if (!String.IsNullOrEmpty(line))
                            {
                                OnNewLine(oldInfo, line);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        OutputLine($"Exception: \"{exception.Message}\"", ConsoleColor.Red);
                    }
                }

                oldInfo.LastSize = newInfo.Length;

                if (VerboseOutput)
                {
                    Console.WriteLine();
                }
            }
        }

        private void AddFile(string path)
        {
            var name = Path.GetFileName(path);

            if (VerboseOutput)
            {
                OutputLine($"Adding new file: {path}", ConsoleColor.Green);
            }

            Files.Add(path, new FileInfo(path));
        }
    }
}
