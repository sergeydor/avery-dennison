using System;

namespace Common.Domain
{
    [Serializable]
    public class PingTestDomainObject
    {
        public int IntData { get; set; }

        public DateTime DateTimeData { get; set; }

        public override string ToString()
        {
            return " IntData " + IntData + " DateTimeData " + DateTimeData;
        }
    }
}
