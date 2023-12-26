using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Domain;
using Common.Domain.Conveyor;
using Server.Device.Communication.Domain;
using Server.Svc.Domain;
using System.Collections.Concurrent;
using Common.Domain.Device;
using Server.Device.Communication.DataAccess.DBEntities;
using MongoDB.Bson;
using Common.Enums;
using Common.Infrastructure;

namespace Server.Svc.Context
{
    public class CommandsProcessingContext : IAppContext
    {
        public const int NotRunningDBUpdateFrequencyInMs = 100;
        public const int RunningDBUpdateFrequencyInMs = 3000;
        static object _lockObj = new object();

        public int CurrentDbUpdateFrequency
        {
            get
            {
                lock (_lockObj)
                    return
                        TestState != Common.Enums.CommandsProcessingMode.Running ? NotRunningDBUpdateFrequencyInMs :
                        RunningDBUpdateFrequencyInMs;
            }
        }

        private CommandsProcessingMode _testState = CommandsProcessingMode.NotRunning;
        public CommandsProcessingMode TestState
        {
            get
            {
                lock(_lockObj)
                    return _testState;
            }
            set
            {
                lock (_lockObj)
                    _testState = value;
            }
        }

        public AppMode AppMode { get; set; } = AppMode.NotSet;

        public DBAppSession AppSession { get; set; }

        public DBTest _currentTest = null;
        public DBTest CurrentTest
        {
            get
            {
                lock(_lockObj)
                    return _currentTest;
            }
            set
            {
                lock (_lockObj)
                    _currentTest = value;
            }
        }

	    public ObjectId CurrentSessionId
	    {
		    get
		    {
				lock (_lockObj)
					return AppSession != null ? AppSession.Id : ObjectId.Empty;
			}
	    }

	    public ObjectId CurrentTestId
        {
            get
            {
                lock(_lockObj)
                    return CurrentTest != null ? CurrentTest.Id : ObjectId.Empty;
            }
        }

        public ConcurrentDictionary<byte, int> UnsolicitedStats = new ConcurrentDictionary<byte, int>();

        public List<DeviceIdentity> Devices { get; set; }
        
        public DeviceIdentity GetGPIODevice()
        {
            return this.Devices.FirstOrDefault(d => d.DeviceType == Common.Enums.GSTCommands.HighSpeedTestDeviceType.GPIO);
        }

        public DeviceIdentity GetReaderDeviceByLane(int lane)
        {
            return this.Devices.FirstOrDefault(d => d.Lane == lane);
        }

        public DeviceIdentity GetDeviceByMacAddr(string macaddr)
        {
            return this.Devices.FirstOrDefault(d => d.MacAddress == macaddr);
        }

        private ConcurrentDictionary<int, DeviceCommandsLink> _commands { get; set; } = new ConcurrentDictionary<int, DeviceCommandsLink>();

        public int[] AddRange(DeviceOutgoingCommand[] outgoing)
        {
            int[] hashes = new int[outgoing.Length];
            for(int i=0; i< outgoing.Length; i++)
            {
                hashes[i] = Add(outgoing[i]);
            }
            return hashes;
        }

        public int Add(DeviceOutgoingCommand outgoing)
        {
            int hash = outgoing.ComputeHash();
            if (this._commands.ContainsKey(hash))
                throw new ArgumentException("link already exists");
            DeviceCommandsLink link = new DeviceCommandsLink() { Outgoing = outgoing };
            while (this._commands.TryAdd(hash, link) == false);
            return hash;
        }

        public bool CheckIfProcessed(int hash)
        {
            if (!this._commands.ContainsKey(hash))
                throw new ArgumentException("hash doesn't exist");
            DeviceCommandsLink link = this._commands[hash];
            return link.IsProcessed;
        }

        public void SetResult(DeviceIncommingCommand incomming)
        {
            int hash = incomming.ComputeHash();
            if (!this._commands.ContainsKey(hash))
                return; // that's ok, not all inputs have output-in-pending, e.g. xE3
            DeviceCommandsLink link = this._commands[hash];
            link.Incomming = incomming;
        }

        public DeviceIncommingCommand GetResult(int hash)
        {
            if (!this._commands.ContainsKey(hash))
                throw new ArgumentException("hash doesn't exist");
            DeviceCommandsLink link = this._commands[hash];
            DeviceCommandsLink outp;
            while (this._commands.TryRemove(hash, out outp) == false);
            return link.Incomming;
        }

        public void ClearFaulted(int hash)
        {
            if (!this._commands.ContainsKey(hash))
                throw new ArgumentException("hash doesn't exist");
            DeviceCommandsLink link = this._commands[hash];
            DeviceCommandsLink outp;
            while (this._commands.TryRemove(hash, out outp) == false) ;
        }

	    public void ClearCommandQueues()
	    {
		    _commands.Clear();
	    }
    }
}
