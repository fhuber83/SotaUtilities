﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XpHelper
{
    public class XpHelpers
    {
        public static void GetXpValues(out ulong? adventurer, out ulong? producer)
        {
            adventurer = null;
            producer = null;

            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var logPath = Path.Combine(appDataPath, "Portalarium\\Shroud of the Avatar\\ChatLogs");

            if (!Directory.Exists(logPath))
                throw new Exception("Sota log directory not found");

            var files = Directory.GetFiles(logPath, "*.txt", SearchOption.TopDirectoryOnly);

            var newestFile = files.Select(x => new { path = x, time = File.GetLastWriteTime(x) }).OrderByDescending(x => x.time).First();

            System.Diagnostics.Debug.Print($"{newestFile.path}");

            var lines = System.IO.File.ReadAllLines(newestFile.path);

            var regexAdv = new Regex(@"^\[\d{1,2}\/\d{1,2}\/\d{4} \d{1,2}:\d{1,2}:\d{1,2} [AP]M\]( \[\d{1,2}:\d{1,2}\])? Adventurer Experience: (?<amount>[\d,]+)$", RegexOptions.Compiled);
            var regexProd = new Regex(@"^\[\d{1,2}\/\d{1,2}\/\d{4} \d{1,2}:\d{1,2}:\d{1,2} [AP]M\]( \[\d{1,2}:\d{1,2}\])? Producer Experience: (?<amount>[\d,]+)$", RegexOptions.Compiled);

            bool haveAdv = false;
            bool haveProd = false;

            foreach (var line in lines.Reverse())
            {
                var matchProd = regexProd.Match(line);

                if(matchProd.Success)
                {
                    producer = ulong.Parse(matchProd.Groups["amount"].Value, NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
                    haveProd = true;
                }

                var matchAdv = regexAdv.Match(line);

                if(matchAdv.Success)
                {
                    adventurer = ulong.Parse(matchAdv.Groups["amount"].Value, NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
                    haveAdv = true;
                }

                if (haveAdv && haveProd)
                    return;
            }
        }
    }
}
