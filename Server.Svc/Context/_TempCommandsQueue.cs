using Client.Server.Communication.RemoteServices.Dtos.Input;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Server.Svc.Context
{/*
    public class TempCommandsQueue<TCommand>
    {
        public ConcurrentQueue<TCommand> ItemsQueue { get; } = new ConcurrentQueue<TCommand>();

        public List<TCommand> DequeueAll()
        {
            List<TCommand> result = new List<TCommand>();

            while(!ItemsQueue.IsEmpty)
            {
                TCommand item;
                while (ItemsQueue.TryDequeue(out item) == false);
                result.Add(item);
            }

            return result;
        }

	    public void Clear()
	    {
            TCommand item;
			while (ItemsQueue.TryDequeue(out item));
	    }
    }*/
}
