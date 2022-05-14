using System;
using System.Collections.Generic;
using System.Text;

namespace SotaLogParser
{
    public class ChatItem : LogItemBase
    {
        public ChatItem(DateTime time, string? path, int lineNumber,  string line, string restOfLine, string name, string chatName, string message) : base(time, path, lineNumber, line, restOfLine)
        {
            Name = name;
            ChatName = chatName;
            Message = message;
        }

        public string Name { get; }
        public string ChatName { get; }
        public string Message { get; }
    }
}
