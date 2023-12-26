using System.Collections.Concurrent;
using System.Collections.Generic;
using Client.Server.Communication.RemoteServices.Dtos.Input;

namespace Server.Svc.Context
{/*
	public class TempUnsolicitedCommandsLogQueue
	{
		public ConcurrentQueue<DeviceCommandLogTransferItem> ItemsQueue { get; } = new ConcurrentQueue<DeviceCommandLogTransferItem>();

		public List<DeviceCommandLogTransferItem> DequeueAll()
		{
			List<DeviceCommandLogTransferItem> result = new List<DeviceCommandLogTransferItem>();

			while (!ItemsQueue.IsEmpty)
			{
				DeviceCommandLogTransferItem item;
				while (ItemsQueue.TryDequeue(out item) == false);
				result.Add(item);
			}

			return result;
		}

		public void Clear()
		{
			DeviceCommandLogTransferItem item;
			while (ItemsQueue.TryDequeue(out item));
		}
	}*/
}