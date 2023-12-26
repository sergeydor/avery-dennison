using System.Runtime.Serialization;

namespace Common.Services.Input
{
    [DataContract]
	public class SvcInputGeneric<TInput> : SvcInputBase
    {
		[DataMember]
        public TInput Input { get; set; }

        public override string ToString()
        {
            return " Input " + Input;
        }
    }
}
