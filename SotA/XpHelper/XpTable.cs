using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpHelper
{
    public class XpTable
    {
        private static ulong[] table;

        static XpTable()
        {
            table = new ulong[201];

            table[0] = 0;

            for (var i = 1; i <= 200; i += 1)
            {
                table[i] = (ulong)(table[i - 1] * 1.1 + 1000);
            }
        }


        /// <summary>
        /// Find out which level a certain amount of XP gets you to
        /// </summary>
        /// <param name="xp">Amount of total XP</param>
        /// <returns>Level that the provided amount of XP gets you</returns>
        public static int WhichLevel(ulong xp)
        {
            for (int i = 1; i < 200; i += 1)
            {
                if (table[i] > xp)
                {
                    return i;
                }
            }

            return 0;
        }


        /// <summary>
        /// Determine how much more XP is needed to level up
        /// </summary>
        /// <param name="currentXp">Current amount of XP</param>
        /// <returns>Amount of XP required to next level</returns>
        public static ulong HowMuchToNextLevel(ulong currentXp)
        {
            var currentLevel = WhichLevel(currentXp);

            if (currentLevel == 200)
                return 0;

            return table[currentLevel] - currentXp;
        }
    }
}
