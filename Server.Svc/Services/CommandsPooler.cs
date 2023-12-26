using Common.Domain.Device;
using Server.Device.Communication.Domain;
using Server.Svc.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Device.Communication.DataAccess.Repositories;
using Server.Device.Communication.DataAccess.DBEntities;
using MongoDB.Bson;
using MongoDB.Driver;
using Common.Enums;
using Server.Device.Communication.CommandInterpretators;
using Common.Domain.DeviceResults.GSTCommands;
using Common.Domain.DeviceResults.UnsolicitedReplyCommands;
using Common.Infrastructure.Logging;
using Server.Device.Communication.Infrastructure.Logging;
using Common.Domain.Conveyor;

namespace Server.Svc.Services
{
    internal class CommandsPooler
    {
        private readonly ProcessingSvcFacade _processingSvcFacade;
        private readonly CommandsProcessingContext _commandsProcessingContext;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly GSTCommandsInterpretator _GSTCommandsInterpretator;
        private readonly UnsolicitedReplyCommandsInterpretator _unsolicitedReplyCommandsInterpretator;
        private readonly Logger _logger;
        private int _errorCounter = 0;
        const int MaxErrors = 10;

        public CommandsPooler(ProcessingSvcFacade processingSvcFacade, CommandsProcessingContext commandsProcessingContext, IMongoDatabase mongoDatabase,
            GSTCommandsInterpretator gstCommandsInterpretator, UnsolicitedReplyCommandsInterpretator unsolicitedReplyCommandsInterpretator, MongoDbLogger logger)
        {
            _logger = logger;
            _unsolicitedReplyCommandsInterpretator = unsolicitedReplyCommandsInterpretator;
            _processingSvcFacade = processingSvcFacade;
            _commandsProcessingContext = commandsProcessingContext;
            _mongoDatabase = mongoDatabase;
            _GSTCommandsInterpretator = gstCommandsInterpretator;
            Task.Run(() => Pool());
        }

        /// <summary>
        /// communicate with devices here
        /// </summary>
        private async void Pool()
        {
            DateTime lastDbUpdate = DateTime.Now;

            while (true)
            {
                if (_commandsProcessingContext.Devices == null)
                {
                    await Task.Delay(100);
                    continue;
                }

                if ((DateTime.Now - lastDbUpdate).TotalMilliseconds > _commandsProcessingContext.CurrentDbUpdateFrequency)
                {
                    Dictionary<DeviceIdentity, List<DBCollectionCommand>> dbCommandsToInsert = null;

                    try
                    {
                        dbCommandsToInsert = ProcessAndPrepareDbCommands();
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex.ToString(), dbCommandsToInsert);
                        _errorCounter++;
                        Console.WriteLine("Critical error in Pooler " + ex.Message + " " + ex.GetType().Name);
                        if(_errorCounter == MaxErrors)
                        {
                            _logger.LogWarning("The Pooler stopped due " + _errorCounter + " errors within. See logs");
                            Console.WriteLine("The Pooler stopped due " + _errorCounter + " errors within. RESTART IS NEEDED. See logs");
                            //return; // stop puller
                            throw new Exception("The Pooler stopped due " + _errorCounter + " errors within. RESTART IS NEEDED. See logs");
                        }
                    }

                    List<ConveyorShot> shots = _processingSvcFacade.GetShapshotsFromQueue();

                    if(shots != null && shots.Count > 0)
                    {
                        var shotsRepo = new ConveyorShotRepository(_mongoDatabase);
                        await shotsRepo.CreateBatchAsync(shots);
                        lastDbUpdate = DateTime.Now;
                    }

                    if (dbCommandsToInsert != null)
                    {
                        List<Task> tasksAggregation = new List<Task>();

                        foreach (DeviceIdentity device in _commandsProcessingContext.Devices)
                        {
                            var cmdList = dbCommandsToInsert[device];
                            if (cmdList.Count == 0)
                                continue;

                            DeviceCommandsRepository repo = new DeviceCommandsRepository(_mongoDatabase, device);
                            Task task = repo.CreateBatchAsync(cmdList);
                            tasksAggregation.Add(task);
                        }

                        if (tasksAggregation.Count > 0)
                        {
                            try
                            {
                                await Task.WhenAll(tasksAggregation); // create batches in parallel 
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex.ToString());
                                _errorCounter++;
                                Console.WriteLine("Critical error in Pooler " + ex.Message + " " + ex.GetType().Name);
                                if (_errorCounter == MaxErrors)
                                {
                                    _logger.LogWarning("The Pooler stopped due " + _errorCounter + " errors within. See logs");
                                    Console.WriteLine("The Pooler stopped due " + _errorCounter + " errors within. RESTART IS NEEDED. See logs");
                                    throw new Exception("The Pooler stopped due " + _errorCounter + " errors within. RESTART IS NEEDED. See logs");
                                }
                            }
                            lastDbUpdate = DateTime.Now;
                        }
                    }
                }

                await Task.Delay(100);
            }
        }

        private Dictionary<DeviceIdentity, List<DBCollectionCommand>> ProcessAndPrepareDbCommands()
        {
            var dbCommandsToInsert = new Dictionary<DeviceIdentity, List<DBCollectionCommand>>();

            foreach (DeviceIdentity device in _commandsProcessingContext.Devices)
            {
                dbCommandsToInsert.Add(device, new List<DBCollectionCommand>());

                List<DeviceIncommingCommand> received = _processingSvcFacade.GetIncommingDataByDevice(device);

                foreach (DeviceIncommingCommand cmd in received)
                {
                    _commandsProcessingContext.SetResult(cmd);
                }

                List<DeviceOutgoingCommand> sent = _processingSvcFacade.GetOutgoingDataByDevice(device);

                foreach (DeviceOutgoingCommand cmd in sent)
                {
                    dbCommandsToInsert[device].Add(new DBCollectionCommand() { Direction = Direction.Out, Id = ObjectId.GenerateNewId(), Device = device, RequestData = cmd.Data, UserFriendlyCmdView = cmd.ToString(), ReceiveDt = cmd.ReceiveDt, AppSessionId = _commandsProcessingContext.AppSession.Id });
                }

                foreach (DeviceIncommingCommand cmd in received)
                {
                    var cmdToInsert = new DBCollectionCommand() { Direction = Direction.In, Id = ObjectId.GenerateNewId(), Device = device, ResponseData = cmd.Data, UserFriendlyCmdView = cmd.ToString(), ReceiveDt = cmd.ReceiveDt, AppSessionId = _commandsProcessingContext.AppSession.Id };

                    
                    if (cmdToInsert.ResponseData.CMD == 0x90 && cmdToInsert.ResponseData.DATA.Length > 1)
                    {
                        HighSpeedGPIOTestResult res = _GSTCommandsInterpretator.ConvertResponseDataToHighSpeedGpioTestStreamingResult(cmdToInsert.ResponseData);
                        cmdToInsert.TempLabelNum = res.Sensor0Count;
                        cmdToInsert.TempLabelOffset = res.SensorACount;
                    }
                    if (cmdToInsert.ResponseData.CMD == 0xE3)
                    {
                        TestE3Result res = _unsolicitedReplyCommandsInterpretator.ConvertResponseDataToTestE3Result(cmdToInsert.ResponseData);
                        cmdToInsert.TempLabelNum = res.TestCount;
                    }

                    //if(_processingSvcFacade DBCollectionCommand.UnsolicitedCommands.Contains(cmdToInsert.ResponseData.CMD))
                    if (_commandsProcessingContext.TestState == CommandsProcessingMode.Running || _commandsProcessingContext.TestState == CommandsProcessingMode.NotRunningPending)
                    {
                        cmdToInsert.IsUnsolicited = true;
                        cmdToInsert.TestId = _commandsProcessingContext.CurrentTestId;

                        if (!_commandsProcessingContext.UnsolicitedStats.ContainsKey(cmdToInsert.ResponseData.CMD))
                            _commandsProcessingContext.UnsolicitedStats.TryAdd(cmdToInsert.ResponseData.CMD, 0);

                        int cnt = _commandsProcessingContext.UnsolicitedStats[cmdToInsert.ResponseData.CMD];
                        _commandsProcessingContext.UnsolicitedStats[cmdToInsert.ResponseData.CMD] = cnt + 1;
                    }

                    dbCommandsToInsert[device].Add(cmdToInsert);
                }
            }
            
            return dbCommandsToInsert;
        }

        
    }        
}