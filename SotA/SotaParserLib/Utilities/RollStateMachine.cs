using System.Text;

namespace SotaLogParser
{
    /// <summary>
    /// Helper class that takes care of rolling for loot items in a party. These log items
    /// consist of multiple items and therefore need a state machine to process.
    /// </summary>
    public class RollStateMachine
    {
        public enum States
        {
            Idle,
            Rolling,
            Ended,
        }

        private States State = States.Idle;

        public DateTime Timestamp { get; private set; }
        public string ItemName { get; private set; }
        public int ItemValue { get; private set; }
        public int LineNumber { get; private set; }
        public string FilePath { get; private set; }

        public void Reset()
        {
            ItemName = null;
            ItemValue = 0;
            Rolls.Clear();
            WinnerName = null;

            State = States.Idle;
        }

        public void Begin(DateTime time, string path, int lineNumber, string itemName, int value, string line, string lineRest)
        {
            Timestamp = time;
            ItemName = itemName;
            ItemValue = value;
            LineNumber = lineNumber;
            FilePath = path;

            Rolls.Clear();

            LineBuilder.Clear();
            LineBuilder.AppendLine(line);

            LineRestBuilder.Clear();
            LineRestBuilder.Append(lineRest);

            State = States.Rolling;
        }

        public string WinnerName { get; private set; }

        public void Roll(string name, int roll, bool winner, string line, string lineRest)
        {
            if(State != States.Rolling)
                throw new InvalidOperationException("Cannot roll unless in state \"Rolling\"");

            Rolls.Add(name, roll);

            LineBuilder.AppendLine(line);
            //LineRestBuilder.Append(lineRest);

            if (winner)
            {
                WinnerName = name;
            }
        }

        public bool HasValidItem
        {
            get => State != States.Idle;
        }

        public bool HasWinner
        {
            get => HasValidItem && !(WinnerName is null);
        }

            
        public LootRollItem GetItem()
        {
            if(State == States.Idle)
                throw new InvalidOperationException("Cannot end unless in state \"Rolling\"");

            var item = new LootRollItem(Timestamp, FilePath, LineNumber, LineBuilder.ToString(), LineRestBuilder.ToString(), ItemName, WinnerName, ItemValue);

            foreach (var rollPair in Rolls)
            {
                item.AddRoll(rollPair.Key, rollPair.Value);
            }

            Reset();

            return item;
        }

        private StringBuilder LineBuilder { get; } = new StringBuilder();
        private StringBuilder LineRestBuilder { get; } = new StringBuilder();
        private Dictionary<string, int> Rolls { get; } = new Dictionary<string, int>();
    }
}