using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UIApp.UIElements.LogView
{
    public enum LogItemType
    {
        NotDefined = -1,
        Info = 0,
        Warning = 1
    }

    public abstract class LogItem
    {
        public virtual string Message { get; set; }
        public LogItemType LogItemType { get; set; } = LogItemType.Info;
    }
}
