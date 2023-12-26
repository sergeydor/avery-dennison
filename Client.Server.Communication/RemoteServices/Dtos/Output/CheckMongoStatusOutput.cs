using Common.Services.Output;

namespace Client.Server.Communication.RemoteServices.Dtos.Output
{
	public class CheckMongoStatusOutput : SvcOutputBase
	{
		public bool IsMongoDbStopped { get; set; }
        public bool IsCheckError { get; set; }
    }
}