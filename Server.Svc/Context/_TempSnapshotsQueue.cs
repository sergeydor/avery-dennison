using Common.Domain.Conveyor;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Server.Svc.Context
{/*
    public class TempSnapshotsQueue
    {
        public ConcurrentQueue<ConveyorShot> Snapshots { get; private set; } = new ConcurrentQueue<ConveyorShot>();

        public List<ConveyorShot> DequeueByTimeRange(DateTime start, DateTime end)
        {
            List<ConveyorShot> result = new List<ConveyorShot>();

            ConveyorShot shot;
            while (!Snapshots.TryPeek(out shot));

            while(shot.CreateDt <= end)
            {                
                while (!Snapshots.TryDequeue(out shot));
                if (shot.CreateDt >= start)
                {
                    result.Add(shot);
                }
                if (Snapshots.IsEmpty)
                {
                    break;
                }
            }

            return result;
        }

	    public void Clear()
	    {
		    ConveyorShot shot;
			while (Snapshots.TryDequeue(out shot));
	    }
    }*/
}
