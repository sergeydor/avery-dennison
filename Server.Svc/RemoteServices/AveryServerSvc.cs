using Client.Server.Communication.RemoteServices.ServiceContracts;
using Common.Services.Input;
using Common.Services.Output;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceProcess;
using Client.Server.Communication.RemoteServices.Dtos.Input;
using Client.Server.Communication.RemoteServices.Dtos.Output;
using Common.Domain.Conveyor;
using Common.Domain.Device;
using Common.Domain.DeviceResults;
using Common.Domain.TestModuleCommands;
using Common.Infrastructure.Devices;
using Common.Infrastructure.Extensions;
using Server.Svc.Context;
using Server.Svc.Services;
using Server.Device.Communication.Domain;
using Server.Device.Communication.CommandInterpretators;
using Server.Device.Communication;
using Common.Enums.GSTCommands;
using Common.Infrastructure.ErrorHandling.Exceptions;
using Common.Infrastructure.ErrorHandling.Enums;
using Common.Infrastructure.ErrorHandling.Output;
using Common.Domain.DeviceResults.GSTCommands;
using Common.Domain.DeviceResults.TestModuleCommands;
using Common.Enums;
using Common.Domain.TestSetupCommands;
using System.Threading;
using Common.Domain.ExtendedTestSetupCommands;
using Common.Domain.GSTCommands;
using Server.Device.Communication.DataAccess.Repositories;
using Server.Device.Communication.DataAccess.DBEntities;
using MongoDB.Driver;
using MongoDB.Bson;
using Client.Server.Communication.Domain;
using Server.Device.Communication.Infrastructure.Logging;
using ServiceBase = Common.Services.ServiceBase;
using TimeoutException = System.TimeoutException;
using Common.Infrastructure.ErrorHandling.Helpers;

namespace Server.Svc.RemoteServices
{
    public class AveryServerSvc : ServiceBase, IAveryServerSvc
    {
        private readonly ProcessingSvcFacade _processingSvcFacade;
        private readonly CommandsProcessingContext _commandsProcessingContext;
        private readonly WaitForCommandResultService _waitForCommandResultService;
        private readonly TestModuleCommandsInterpretator _testModuleInterpretator;
        private readonly TestActionCommandsInterpretator _testActionCommandsInterpretator;
        private readonly TestSetupCommandsInterpretator _testSetupCommandsInterpretator;
        private readonly GSTCommandsInterpretator _gstCommandsInterpretator;
        private readonly GetTestSetupCommandsInterpretator _getTestSetupCommandsInterpretator;
        private readonly ExtendedGetTestSetupCommandsInterpretator _extendedGetTestSetupCommandsInterpretator;
        private readonly ExtendedTestSetupCommadsInterpretator _extendedTestSetupCommadsInterpretator;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly AppSessionRepository _appSessionRepository;
        private readonly DBTestRepository _dBTestRepository;

        //private ConveyorSettings _conveyorSettings = new ConveyorSettings();

        private const string InitMockMode = "InitMockMode";

        public AveryServerSvc(MongoDbLogger logger, ProcessingSvcFacade processingSvcFacade, CommandsProcessingContext commandsProcessingContext,
            WaitForCommandResultService waitForCommandResultService, TestModuleCommandsInterpretator testModuleInterpretator,
            TestSetupCommandsInterpretator testSetupCommandsInterpretator, GetTestSetupCommandsInterpretator getTestSetupCommandsInterpretator,
            TestActionCommandsInterpretator testActionCommandsInterpretator, GSTCommandsInterpretator gstCommandsInterpretator,
            ExtendedGetTestSetupCommandsInterpretator extendedGetTestSetupCommandsInterpretator, ExtendedTestSetupCommadsInterpretator extendedTestSetupCommadsInterpretator,
            AppSessionRepository appSessionRepository, DBTestRepository dBTestRepository,
            IMongoDatabase mongoDatabase
            ) : base(logger)
        {
            _testModuleInterpretator = testModuleInterpretator;
            _testSetupCommandsInterpretator = testSetupCommandsInterpretator;
            _getTestSetupCommandsInterpretator = getTestSetupCommandsInterpretator;
            _commandsProcessingContext = commandsProcessingContext;
            _processingSvcFacade = processingSvcFacade;
            _waitForCommandResultService = waitForCommandResultService;
            _testActionCommandsInterpretator = testActionCommandsInterpretator;
            _gstCommandsInterpretator = gstCommandsInterpretator;
            _extendedGetTestSetupCommandsInterpretator = extendedGetTestSetupCommandsInterpretator;
            _extendedTestSetupCommadsInterpretator = extendedTestSetupCommadsInterpretator;
            _mongoDatabase = mongoDatabase;
            _appSessionRepository = appSessionRepository;
            _dBTestRepository = dBTestRepository;
        }

        private bool IsInitMockMode
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings[InitMockMode]);
            }
        }

        public SvcOutputGeneric<GeneralDeviceResult> Ping(SvcInputGeneric<string> pingInput)
        {
            var result = new SvcOutputGeneric<GeneralDeviceResult>();

            base.RunCode(pingInput, result, () =>
            {
                DeviceIdentity device = _commandsProcessingContext.GetDeviceByMacAddr(pingInput.Input);
                if (device == null)
                {
                    throw new BusinessLogicException(ErrorCode.DEVICE_NOT_FOUND);
                }
                CommandData outData = _testModuleInterpretator.GetPingCommandData();
                DeviceOutgoingCommand outCmd = new DeviceOutgoingCommand() { DeviceIdentity = device, Data = outData };
                _commandsProcessingContext.Add(outCmd);
                _processingSvcFacade.SendDataToDevice(outCmd);
                DeviceIncommingCommand inpCmd = _waitForCommandResultService.WaitForResult(outCmd);
                GeneralDeviceResult procResult = _testModuleInterpretator.ConvertResponseDataToGeneralDeviceResult(inpCmd.Data);
                result.Output = procResult;
            });
            return result;
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetDateTime(SetDataToDeviceSvcInput<LaneDateTime> setDateTimeInput) // in mvp scheme targets are readers and gpio, in case of gpio what is lane? Should we pass lane number to any reader if we have direct pointer to this device.. ?
        {
            var result = new SvcOutputGeneric<GeneralDeviceResult>();
            bool initMockMode = bool.Parse(ConfigurationManager.AppSettings[InitMockMode]);
            if (initMockMode)
            {
                result.Output = new GeneralDeviceResult() { Status = Common.Enums.StatusCode.OK, Timer = 123 };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(setDateTimeInput, result, () =>
            {
                DeviceIdentity device = _commandsProcessingContext.GetDeviceByMacAddr(setDateTimeInput.DeviceMacAddr);
                if (device != null && device.DeviceType == HighSpeedTestDeviceType.Reader && device.Lane != setDateTimeInput.Input.Lane)
                {
                    throw new ArgumentOutOfRangeException("dateTimeInput.Input.LaneDateTime.Lane");
                }
                ExecuteDeviceCommandSetPattern(setDateTimeInput, result, _testModuleInterpretator.ConvertLaneDateTimeToCommandData);
            });
            return result;
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetTagIdFilterSettings(SetDataToDeviceSvcInput<TagIDFilterSettings> setTagIdFilterInput)
        {
            var result = new SvcOutputGeneric<GeneralDeviceResult>();
            bool initMockMode = bool.Parse(ConfigurationManager.AppSettings[InitMockMode]);
            if (initMockMode)
            {
                result.Output = new GeneralDeviceResult() { Status = Common.Enums.StatusCode.OK, Timer = 123 };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(setTagIdFilterInput, result, () =>
            {
                ExecuteDeviceCommandSetPattern(setTagIdFilterInput, result, _testSetupCommandsInterpretator.ConvertTagIDFilterSettingsToCommandData);
            });
            return result;
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetTestSettings(SetDataToDeviceSvcInput<TestSettings> setTestSettingsInput)
        {
            var result = new SvcOutputGeneric<GeneralDeviceResult>();
            if (IsInitMockMode)
            {
                result.Output = new GeneralDeviceResult() { Status = Common.Enums.StatusCode.OK, Timer = 123 };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(setTestSettingsInput, result, () =>
            {
                ExecuteDeviceCommandSetPattern(setTestSettingsInput, result, _testSetupCommandsInterpretator.ConvertTestSettingsToCommandData);
            });

            return result;
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetMarkerSettings(SetDataToDeviceSvcInput<MarkerSettings> setMarkerSettingsInput)
        {
            var result = new SvcOutputGeneric<GeneralDeviceResult>();
            if (IsInitMockMode)
            {
                result.Output = new GeneralDeviceResult { Status = StatusCode.OK, Timer = 123 };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(setMarkerSettingsInput, result, () =>
            {
                ExecuteDeviceCommandSetPattern(setMarkerSettingsInput, result, _testSetupCommandsInterpretator.ConvertMarkerSettingsToCommandData);
            });

            return result;
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetTriggerInputSettings(SetDataToDeviceSvcInput<TriggerInputSettings> setTriggerInSettingsInput)
        {
            var result = new SvcOutputGeneric<GeneralDeviceResult>();
            if (IsInitMockMode)
            {
                result.Output = new GeneralDeviceResult { Status = StatusCode.OK, Timer = 123 };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(setTriggerInSettingsInput, result, () =>
            {
                ExecuteDeviceCommandSetPattern(setTriggerInSettingsInput, result, _testSetupCommandsInterpretator.ConvertTriggerInputSettingsToCommandData);
            });

            return result;
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetReaderSettings(SetDataToDeviceSvcInput<TesterSettings> setTesterSettingsInput)
        {
            var result = new SvcOutputGeneric<GeneralDeviceResult>();
            if (IsInitMockMode)
            {
                result.Output = new GeneralDeviceResult { Status = StatusCode.OK, Timer = 123 };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(setTesterSettingsInput, result, () =>
            {
                ExecuteDeviceCommandSetPattern(setTesterSettingsInput, result, _testSetupCommandsInterpretator.ConvertTesterSettingsToCommandData);
            });

            return result;
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetTestStatistics(SetDataToDeviceSvcInput<TestStatistics> setTestStatisticsInput)
        {
            var result = new SvcOutputGeneric<GeneralDeviceResult>();
            if (IsInitMockMode)
            {
                result.Output = new GeneralDeviceResult() { Status = Common.Enums.StatusCode.OK, Timer = 123 };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(setTestStatisticsInput, result, () =>
            {
                ExecuteDeviceCommandSetPattern(setTestStatisticsInput, result, _testSetupCommandsInterpretator.ConvertTestStatisticsToCommandData);
            });

            return result;
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetAntennaSettings(SetDataToDeviceSvcInput<AntennaSettings> setAntennaSettingsInput)
        {
            var result = new SvcOutputGeneric<GeneralDeviceResult>();
            if (IsInitMockMode)
            {
                result.Output = new GeneralDeviceResult { Status = StatusCode.OK, Timer = 123 };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(setAntennaSettingsInput, result, () =>
            {
                ExecuteDeviceCommandSetPattern(setAntennaSettingsInput, result, _testSetupCommandsInterpretator.GetAntennaSettingsCommandData);
            });

            return result;
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetSensitivityTestSettings(SetDataToDeviceSvcInput<SensitivityTestSettings> setSensitibityTestSettingsInput)
        {
            var result = new SvcOutputGeneric<GeneralDeviceResult>();
            if (IsInitMockMode)
            {
                result.Output = new GeneralDeviceResult() { Status = Common.Enums.StatusCode.OK, Timer = 123 };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(setSensitibityTestSettingsInput, result, () =>
            {
                ExecuteDeviceCommandSetPattern(setSensitibityTestSettingsInput, result, _testSetupCommandsInterpretator.ConvertSensitivityTestSettingsToCommandData);
            });

            return result;
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetTIDTestSettings(SetDataToDeviceSvcInput<TIDTestSettings> setTidTestSettingsInput)
        {
            var result = new SvcOutputGeneric<GeneralDeviceResult>();
            if (IsInitMockMode)
            {
                result.Output = new GeneralDeviceResult() { Status = Common.Enums.StatusCode.OK, Timer = 123 };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(setTidTestSettingsInput, result, () =>
            {
                ExecuteDeviceCommandSetPattern(setTidTestSettingsInput, result, _testSetupCommandsInterpretator.ConvertTIDTestSettingsToCommandData);
            });

            return result;
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetAuxSettings(SetDataToDeviceSvcInput<AuxSettings> setAuxSettingsInput)
        {
            var result = new SvcOutputGeneric<GeneralDeviceResult>();
            if (IsInitMockMode)
            {
                result.Output = new GeneralDeviceResult { Status = StatusCode.OK, Timer = 123 };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(setAuxSettingsInput, result, () =>
            {
                ExecuteDeviceCommandSetPattern(setAuxSettingsInput, result, _testSetupCommandsInterpretator.ConvertAuxSettingsToCommandData);
            });

            return result;
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetEncoderSettings(SetDataToDeviceSvcInput<EncoderSettings> setEncoderSettingsInput)
        {
            var result = new SvcOutputGeneric<GeneralDeviceResult>();
            if (IsInitMockMode)
            {
                result.Output = new GeneralDeviceResult { Status = StatusCode.OK, Timer = 123 };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(setEncoderSettingsInput, result, () =>
            {
                ExecuteDeviceCommandSetPattern(setEncoderSettingsInput, result, _testSetupCommandsInterpretator.ConvertEncoderSettingsToCommandData);
            });

            return result;
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetPuncherSettings(SetDataToDeviceSvcInput<PunchSettings> setPuncherSettingsInput)
        {
            var result = new SvcOutputGeneric<GeneralDeviceResult>();
            if (IsInitMockMode)
            {
                result.Output = new GeneralDeviceResult { Status = StatusCode.OK, Timer = 123 };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(setPuncherSettingsInput, result, () =>
            {
                ExecuteDeviceCommandSetPattern(setPuncherSettingsInput, result, _extendedTestSetupCommadsInterpretator.ConvertPunchSettingsToCommandData);
            });

            return result;
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetHighSpeedTestModeTimer(SetDataToDeviceSvcInput<HighSpeedTestModeTimer> setHighSpeedTestModeTimerInput)
        {
            var result = new SvcOutputGeneric<GeneralDeviceResult>();
            if (IsInitMockMode)
            {
                result.Output = new GeneralDeviceResult { Status = StatusCode.OK, Timer = 123 };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(setHighSpeedTestModeTimerInput, result, () =>
            {
                ExecuteDeviceCommandSetPattern(setHighSpeedTestModeTimerInput, result, _gstCommandsInterpretator.ConvertHighSpeedTestModeTimerToCommandData);
            });

            return result;
        }

        public SvcOutputGeneric<EntityDeviceResult<HighSpeedTestModeTimer>> GetHighSpeedTestModeTimer(SvcInputGeneric<string> getHighSpeedTestModeTimerInput)
        {
            var result = new SvcOutputGeneric<EntityDeviceResult<HighSpeedTestModeTimer>>();
            if (IsInitMockMode)
            {
                byte sum = (byte)(getHighSpeedTestModeTimerInput.Input.Sum(c => (int)c) % 10);
                result.Output = new EntityDeviceResult<HighSpeedTestModeTimer>() { Entity = new HighSpeedTestModeTimer { DeviceType = HighSpeedTestDeviceType.GPIO, D1D0 = (ushort)(12 + sum) }, Status = StatusCode.OK, Timer = 123 };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(getHighSpeedTestModeTimerInput, result, () =>
            {
                ExecuteDeviceCommandGetPattern(getHighSpeedTestModeTimerInput.Input, result,
                    _gstCommandsInterpretator.GetHighSpeedGpioTestModeTimerCommandData,
                    _gstCommandsInterpretator.ConvertResponseDataToHighSpeedTestModeTimer);
            });

            return result;
        }

        private void ExecuteDeviceCommandSetPattern<TEntity>(
            SetDataToDeviceSvcInput<TEntity> setDataInput, SvcOutputGeneric<GeneralDeviceResult> result,
            Func<TEntity, CommandData> getCmdDataFunc)
        {
            DeviceIdentity device = _commandsProcessingContext.GetDeviceByMacAddr(setDataInput.DeviceMacAddr);
            if (device == null)
            {
                throw new BusinessLogicException(ErrorCode.DEVICE_NOT_FOUND);
            }
            CommandData outData = getCmdDataFunc(setDataInput.Input);
            DeviceOutgoingCommand outCmd = new DeviceOutgoingCommand() { DeviceIdentity = device, Data = outData };
            _commandsProcessingContext.Add(outCmd);
            _processingSvcFacade.SendDataToDevice(outCmd);
            DeviceIncommingCommand inpCmd = _waitForCommandResultService.WaitForResult(outCmd);
            GeneralDeviceResult procResult = _testModuleInterpretator.ConvertResponseDataToGeneralDeviceResult(inpCmd.Data);
            result.Output = procResult;
        }

        public SvcOutputGeneric<EntityDeviceResult<LaneDateTime>> GetDateTime(SvcInputGeneric<string> getDateTimeInput)
        {
            var result = new SvcOutputGeneric<EntityDeviceResult<LaneDateTime>>();
            bool initMockMode = bool.Parse(ConfigurationManager.AppSettings[InitMockMode]);
            if (initMockMode)
            {
                result.Output = new EntityDeviceResult<LaneDateTime>() { Entity = new LaneDateTime() { Day = 1, Hour = 2, Lane = 0, Minute = 4, Month = 1, Second = 6, Year = 17 }, Status = Common.Enums.StatusCode.OK, Timer = 123 };
                result.Output.Entity.Shuffle(getDateTimeInput.Input);
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(getDateTimeInput, result, () =>
            {
                ExecuteDeviceCommandGetPattern<LaneDateTime>(getDateTimeInput.Input, result,
                    _testModuleInterpretator.GetDateTimeCommandData, _testModuleInterpretator.ConvertResponseDataToLaneDateTime);
            });
            return result;
        }

        public SvcOutputGeneric<EntityDeviceResult<VersionResult>> GetVersion(SvcInputGeneric<string> getVersionInput)
        {
            var result = new SvcOutputGeneric<EntityDeviceResult<VersionResult>>();
            if (IsInitMockMode)
            {
                byte sum = (byte)(getVersionInput.Input.Sum(c => (int)c) % 10);
                result.Output = new EntityDeviceResult<VersionResult>() { Entity = new VersionResult() { Day = (byte)(1 + sum), Year = (byte)(10 + sum), Major = (byte)(1 + sum), Minor = (byte)(2 + sum), Month = (byte)(1 + sum) }, Status = StatusCode.OK, Timer = 123 };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(getVersionInput, result, () =>
            {
                ExecuteDeviceCommandGetPattern<VersionResult>(getVersionInput.Input, result,
                    _testModuleInterpretator.GetVersionCommandData, _testModuleInterpretator.ConvertResponseDataToVersionResult);
            });
            return result;
        }

        public SvcOutputGeneric<EntityDeviceResult<TestSettings>> GetTestSettings(SvcInputGeneric<string> getTestSettingsInput)
        {
            var result = new SvcOutputGeneric<EntityDeviceResult<TestSettings>>();
            if (IsInitMockMode)
            {
                byte sum = (byte)(getTestSettingsInput.Input.Sum(c => (int)c) % 10);
                result.Output = new EntityDeviceResult<TestSettings>
                {
                    Entity = new TestSettings
                    {
                        AntPort = AntPort.NoPort, ReadTimeout = (ushort)(123 + sum), Frequency1 = (uint)(23 + sum), Frequency2 = (uint)(34 + sum), Frequency3 = (uint)(45 + sum), ReadPower = (short)56, StartTagID = new byte[] { 1, 2, 3 }, TagClass = TagClass.EPC1Gen2, TestType = Test10Type.ForceKill, WritePower = 1, WriteTimeout = (ushort)(2 + sum), WriteType = WriteType.IncrementGivenTagID
                    },
                    Status = StatusCode.OK,
                    Timer = 345
                };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(null, result, () =>
            {
                ExecuteDeviceCommandGetPattern<TestSettings>(getTestSettingsInput.Input, result,
                    _getTestSetupCommandsInterpretator.GetTestSettingsCommandData,
                    _getTestSetupCommandsInterpretator.ConvertResponseDataToTestSettings);
            });

            return result;
        }

        public SvcOutputGeneric<EntityDeviceResult<MarkerSettings>> GetMarkerSettings(SvcInputGeneric<string> getMarkerSettingsInput)
        {
            var result = new SvcOutputGeneric<EntityDeviceResult<MarkerSettings>>();
            if (IsInitMockMode)
            {
                byte sum = (byte)(getMarkerSettingsInput.Input.Sum(c => c % 10));
                result.Output = new EntityDeviceResult<MarkerSettings>
                {
                    Entity = new MarkerSettings
                    {
                        Duration = (byte)(0 + sum), Enable = MarkerEnableMode.MarkBadLabels, Offset = (byte)(1 + sum), Position = (byte)(2 + sum)
                    },
                    Status = StatusCode.OK,
                    Timer = 123
                };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(getMarkerSettingsInput, result, () =>
            {
                ExecuteDeviceCommandGetPattern<MarkerSettings>(getMarkerSettingsInput.Input, result,
                    _getTestSetupCommandsInterpretator.GetMarkerSettingsCommandData,
                    _getTestSetupCommandsInterpretator.ConvertResponseDataToMarkerSettings);
            });

            return result;
        }

        public SvcOutputGeneric<EntityDeviceResult<TriggerInputSettings>> GetTriggerInputSettings(SvcInputGeneric<string> getTriggerInSettingsInput)
        {
            var result = new SvcOutputGeneric<EntityDeviceResult<TriggerInputSettings>>();
            if (IsInitMockMode)
            {
                byte sum = (byte)(getTriggerInSettingsInput.Input.Sum(c => c % 10));
                result.Output = new EntityDeviceResult<TriggerInputSettings>
                {
                    Entity = new TriggerInputSettings
                    {
                        Enable = EnableMode.Enable, DeafTime = (byte)(0 + sum), Debounce = (byte)(1 + sum), EdgeType = EdgeType.Rising, TestOffset = (byte)(2 + sum)
                    },
                    Status = StatusCode.OK,
                    Timer = 123
                };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(getTriggerInSettingsInput, result, () =>
            {
                ExecuteDeviceCommandGetPattern(getTriggerInSettingsInput.Input, result,
                    _getTestSetupCommandsInterpretator.GetTriggerInputSettingsCommandData,
                    _getTestSetupCommandsInterpretator.ConverResponseDataToTriggerInputSettings);
            });

            return result;
        }

        public SvcOutputGeneric<EntityDeviceResult<TesterSettings>> GetTesterSettings(SvcInputGeneric<string> getTesterSettingsInput)
        {
            var result = new SvcOutputGeneric<EntityDeviceResult<TesterSettings>>();
            if (IsInitMockMode)
            {
                byte sum = (byte)(getTesterSettingsInput.Input.Sum(c => c % 10));
                result.Output = new EntityDeviceResult<TesterSettings>
                {
                    Entity = new TesterSettings
                    {
                        Enable = EnableMode.Enable, Position = (byte)(0 + sum)
                    },
                    Status = StatusCode.OK,
                    Timer = 123
                };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(getTesterSettingsInput, result, () =>
            {
                ExecuteDeviceCommandGetPattern(getTesterSettingsInput.Input, result,
                    _getTestSetupCommandsInterpretator.GetTesterSettingsCommandData,
                    _getTestSetupCommandsInterpretator.ConvertResponseDataToTesterSettings);
            });

            return result;
        }

        public SvcOutputGeneric<EntityDeviceResult<TestStatistics>> GetTestStatistics(SvcInputGeneric<string> getTestStatisticsInput)
        {
            var result = new SvcOutputGeneric<EntityDeviceResult<TestStatistics>>();
            if (IsInitMockMode)
            {
                byte sum = (byte)(getTestStatisticsInput.Input.Sum(c => c % 10));
                result.Output = new EntityDeviceResult<TestStatistics>
                {
                    Entity = new TestStatistics
                    {
                        TestFailCount = (byte)(0 + sum), TestPassCount = (byte)(1 + sum), TriggerCount = (byte)(2 + sum)
                    },
                    Status = StatusCode.OK, Timer = 345
                };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(getTestStatisticsInput, result, () =>
            {
                ExecuteDeviceCommandGetPattern<TestStatistics>(getTestStatisticsInput.Input, result,
                    _getTestSetupCommandsInterpretator.GetTestStatisticsCommandData,
                    _getTestSetupCommandsInterpretator.ConvertResponseDataToTestStatistics);
            });

            return result;
        }

        public SvcOutputGeneric<EntityDeviceResult<AntennaSettings>> GetAntennaSettings(SvcInputGeneric<string> getAntennaSettingsInput)
        {
            var result = new SvcOutputGeneric<EntityDeviceResult<AntennaSettings>>();
            if (IsInitMockMode)
            {
                result.Output = new EntityDeviceResult<AntennaSettings>
                {
                    Entity = new AntennaSettings { AntPort = AntPort.NoPort },
                    Status = StatusCode.OK,
                    Timer = 234
                };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(getAntennaSettingsInput, result, () =>
            {
                ExecuteDeviceCommandGetPattern(getAntennaSettingsInput.Input, result,
                    _getTestSetupCommandsInterpretator.GetAntennaSettingsCommandData,
                    _getTestSetupCommandsInterpretator.ConvertResponseDataToAntennaSettings);
            });

            return result;
        }

        public SvcOutputGeneric<EntityDeviceResult<TagIDFilterSettings>> GetTagIdFilterSettings(SvcInputGeneric<string> getTagIdFilterInput)
        {
            var result = new SvcOutputGeneric<EntityDeviceResult<TagIDFilterSettings>>();
            if (IsInitMockMode)
            {
                byte sum = (byte)(getTagIdFilterInput.Input.Sum(c => (int)c) % 10);
                result.Output = new EntityDeviceResult<TagIDFilterSettings>() { Entity = new TagIDFilterSettings()
                {
                    NibbleEnable = new byte[] { 1, 2, 3 },
                    Options = (byte)(4 + sum),
                    TagIDFilter = new byte[] { 5, 6, 7 } },
                    Status = StatusCode.OK, Timer = 345
                };
                result.Output.Entity.NibbleEnable = result.Output.Entity.NibbleEnable.Select(b => (byte)(b + sum)).ToArray();
                result.Output.Entity.TagIDFilter = result.Output.Entity.TagIDFilter.Select(b => (byte)(b + sum)).ToArray();
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(getTagIdFilterInput, result, () =>
            {
                ExecuteDeviceCommandGetPattern<TagIDFilterSettings>(getTagIdFilterInput.Input, result,
                    _getTestSetupCommandsInterpretator.GetTagIdFilterSettingsCommandData, _getTestSetupCommandsInterpretator.ConvertResponseDataToTagIdFilterSettings);
            });
            return result;
        }

        public SvcOutputGeneric<EntityDeviceResult<AuxSettings>> GetAuxInSettings(SvcInputGeneric<string> getAuxSettingsInput)
        {
            var result = new SvcOutputGeneric<EntityDeviceResult<AuxSettings>>();
            if (IsInitMockMode)
            {
                byte sum = (byte)(getAuxSettingsInput.Input.Sum(c => c % 10));
                result.Output = new EntityDeviceResult<AuxSettings>
                {
                    Entity = new AuxSettings
                    {
                        EdgeType = EdgeType.Rising, Debounce = (byte)(0 + sum), DeafTime = (byte)(1 + sum), Function = AuxSettingsFunction.EnableTrigger, Option1 = (byte)(2 + sum), Option2 = (byte)(3 + sum)
                    },
                    Status = StatusCode.OK,
                    Timer = 123
                };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(getAuxSettingsInput, result, () =>
            {
                ExecuteDeviceCommandGetPattern(getAuxSettingsInput.Input, result,
                    _getTestSetupCommandsInterpretator.GetAuxSettingsCommandData,
                    _getTestSetupCommandsInterpretator.ConvertResponseDataToAuxSettings);
            });

            return result;
        }

        public SvcOutputGeneric<EntityDeviceResult<EncoderSettings>> GetEncoderSettings(SvcInputGeneric<string> getEncoderSettingsInput)
        {
            var result = new SvcOutputGeneric<EntityDeviceResult<EncoderSettings>>();
            if (IsInitMockMode)
            {
                byte sum = (byte)(getEncoderSettingsInput.Input.Sum(c => c % 10));
                result.Output = new EntityDeviceResult<EncoderSettings>
                {
                    Entity = new EncoderSettings
                    {
                        MarkerOffset = (ushort)(12 + sum), PunchFlight = (ushort)(23 + sum), PunchOffset = (ushort)(34 + sum), TesterOffset = (ushort)(45 + sum), TriggerFilterMin = (ushort)(56 + sum), TriggerFilterMax = (ushort)(67 + sum)
                    },
                    Status = StatusCode.OK,
                    Timer = 345
                };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(getEncoderSettingsInput, result, () =>
            {
                ExecuteDeviceCommandGetPattern(getEncoderSettingsInput.Input, result,
                    _getTestSetupCommandsInterpretator.GetEncoderSettingsCommandData,
                    _getTestSetupCommandsInterpretator.ConvertReponseDataToEncoderSettings);
            });

            return result;
        }

        public SvcOutputGeneric<EntityDeviceResult<PunchSettings>> GetPunchSettings(SvcInputGeneric<string> getPunchSettingsInput)
        {
            var result = new SvcOutputGeneric<EntityDeviceResult<PunchSettings>>();
            if (IsInitMockMode)
            {
                byte sum = (byte)(getPunchSettingsInput.Input.Sum(c => c % 10));
                result.Output = new EntityDeviceResult<PunchSettings>
                {
                    Entity = new PunchSettings
                    {
                        Enable = PunchEnableMode.PunchGood, Position = (byte)(0 + sum), Offset = (byte)(1 + sum), Duration = (byte)(2 + sum)
                    },
                    Status = StatusCode.OK,
                    Timer = 123
                };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(getPunchSettingsInput, result, () =>
            {
                ExecuteDeviceCommandGetPattern(getPunchSettingsInput.Input, result,
                    _extendedGetTestSetupCommandsInterpretator.GetPunchSettingsCommandData,
                    _extendedGetTestSetupCommandsInterpretator.ConvertResponseDataToPunchSettings);
            });

            return result;
        }

        public SvcOutputGeneric<EntityDeviceResult<SensitivityTestSettings>> GetSensitivityTestSettings(SvcInputGeneric<string> getSensitivitySettingsInput)
        {
            var result = new SvcOutputGeneric<EntityDeviceResult<SensitivityTestSettings>>();
            if (IsInitMockMode)
            {
                byte sum = (byte)(getSensitivitySettingsInput.Input.Sum(c => (int)c) % 10);
                result.Output = new EntityDeviceResult<SensitivityTestSettings>
                {
                    Entity = new SensitivityTestSettings
                    {
                        AntPort = AntPort.NoPort, Frequency = (uint)(123 + sum), MaxPower = (short)(23 + sum), MinPower = (short)(12 + sum), Options = (byte)(0 + sum) /*SensitivityTestOptions.EffectsResult*/, PassThreshold = (short)(2 + sum), ReadWriteMode = ReadWriteMode.Write, SearchDepth = (byte)(1 + sum), Timeout = (ushort)(321 + sum)
                    },
                    Status = StatusCode.OK, Timer = 345
                };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(getSensitivitySettingsInput, result, () =>
            {
                ExecuteDeviceCommandGetPattern<SensitivityTestSettings>(getSensitivitySettingsInput.Input, result,
                    _getTestSetupCommandsInterpretator.GetSensitivityTestSettingsCommandData,
                    _getTestSetupCommandsInterpretator.ConvertResponseDataToSensitivityTestSettings);
            });

            return result;
        }

        public SvcOutputGeneric<EntityDeviceResult<TIDTestSettings>> GetTIDTestSettings(SvcInputGeneric<string> getTidTestSettingsInput)
        {
            var result = new SvcOutputGeneric<EntityDeviceResult<TIDTestSettings>>();
            if (IsInitMockMode)
            {
                byte sum = (byte)(getTidTestSettingsInput.Input.Sum(c => (int)c) % 10);
                result.Output = new EntityDeviceResult<TIDTestSettings>
                {
                    Entity = new TIDTestSettings { Interval = (ushort)(32 + sum), Options = (byte)(12 + sum), ReadTimeout = (ushort)(21 + sum), TID = (uint)(43 + sum) },
                    Status = StatusCode.OK, Timer = 345
                };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(getTidTestSettingsInput, result, () =>
            {
                ExecuteDeviceCommandGetPattern<TIDTestSettings>(getTidTestSettingsInput.Input, result,
                    _getTestSetupCommandsInterpretator.GetTidTestSettingsCommandData,
                    _getTestSetupCommandsInterpretator.ConvertResponseDataToTIDTestSettings);
            });

            return result;
        }

        public SvcOutputGeneric<EntityDeviceResult<LastFaultResult>> GetLastFault(SvcInputGeneric<string> getLastFaultInput)
        {
            var result = new SvcOutputGeneric<EntityDeviceResult<LastFaultResult>>();
            if (IsInitMockMode)
            {
                byte sum = (byte)(getLastFaultInput.Input.Sum(c => (int)c) % 3);
                result.Output = new EntityDeviceResult<LastFaultResult>() { Entity = new LastFaultResult() { FaultCode = sum == 1 ? FaultCode.BIT_COMMS_FAULT : sum == 0 ? FaultCode.BIT_READER_FAULT : FaultCode.NO_FAULT }, Status = StatusCode.OK };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(getLastFaultInput, result, () =>
            {
                ExecuteDeviceCommandGetPattern<LastFaultResult>(getLastFaultInput.Input, result,
                    _testModuleInterpretator.GetLastFaultCommandData, _testModuleInterpretator.ConvertResponseDataToLastFaultResult);
            });
            return result;
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetFault(SetDataToDeviceSvcInput<FaultCode> setFaultInput)
        {
            var result = new SvcOutputGeneric<GeneralDeviceResult>();
            if (IsInitMockMode)
            {
                result.Output = new GeneralDeviceResult() { Status = Common.Enums.StatusCode.OK, Timer = 123 };
                return result;
            }

            base.RunCode(setFaultInput, result, () =>
            {
                ExecuteDeviceCommandSetPattern(setFaultInput, result, _testActionCommandsInterpretator.GetSetFaultCommandData);
            });

            return result;
        }

        public SvcOutputGeneric<GeneralDeviceResult> ClearLastFault(SvcInputGeneric<string> clearLastFaultInput)
        {
            var result = new SvcOutputGeneric<GeneralDeviceResult>();
            if (IsInitMockMode)
            {
                result.Output = new GeneralDeviceResult { Status = StatusCode.OK, Timer = 123 };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(clearLastFaultInput, result, () =>
            {
                DeviceIdentity device = _commandsProcessingContext.GetDeviceByMacAddr(clearLastFaultInput.Input);
                if (device == null)
                {
                    throw new BusinessLogicException(ErrorCode.DEVICE_NOT_FOUND);
                }
                CommandData outData = _testModuleInterpretator.GetClearLastFaultCommandData();
                DeviceOutgoingCommand outCmd = new DeviceOutgoingCommand() { DeviceIdentity = device, Data = outData };
                _commandsProcessingContext.Add(outCmd);
                _processingSvcFacade.SendDataToDevice(outCmd);
                DeviceIncommingCommand inpCmd = _waitForCommandResultService.WaitForResult(outCmd);
                GeneralDeviceResult procResult = _testModuleInterpretator.ConvertResponseDataToGeneralDeviceResult(inpCmd.Data);
                result.Output = procResult;
            });

            return result;
        }

        /// <summary>
        /// 0x02
        /// </summary>
        public SvcOutputGeneric<GeneralDeviceResult> Reset(SetDataToDeviceSvcInput<ResetSettings> resetInput)
        {
            var result = new SvcOutputGeneric<GeneralDeviceResult>();
            if (IsInitMockMode)
            {
                result.Output = new GeneralDeviceResult { Status = StatusCode.OK, Timer = 123 };
                Thread.Sleep(50);
                return result;
            }

            base.RunCode(resetInput, result, () =>
            {
                ExecuteDeviceCommandSetPattern(resetInput, result, _testModuleInterpretator.ConvertResetSettingsToCommandData);
            });
            return result;
        }

        private void ExecuteDeviceCommandGetPattern<TResultEntity>(string macAddr, SvcOutputGeneric<EntityDeviceResult<TResultEntity>> result,
            Func<CommandData> getCmdDataFunc, Func<ResponseData, TResultEntity> convertResToEntityFunc) where TResultEntity : new()
        {
            DeviceIdentity device = _commandsProcessingContext.GetDeviceByMacAddr(macAddr);
            if (device == null)
            {
                throw new BusinessLogicException(ErrorCode.DEVICE_NOT_FOUND);
            }
            CommandData outData = getCmdDataFunc();
            DeviceOutgoingCommand outCmd = new DeviceOutgoingCommand() { DeviceIdentity = device, Data = outData };
            _commandsProcessingContext.Add(outCmd);
            _processingSvcFacade.SendDataToDevice(outCmd);
            DeviceIncommingCommand inpCmd = _waitForCommandResultService.WaitForResult(outCmd);

            TResultEntity entity = default(TResultEntity);
            try
            {
                entity = convertResToEntityFunc(inpCmd.Data);
            }
            catch
            {
                throw new BusinessLogicException(ErrorCode.PROCESSING_RECEIVED_DATA_ERROR);
            }
            EntityDeviceResult<TResultEntity> procResult = _testModuleInterpretator.ConvertResponseDataToEntityDeviceResult<TResultEntity>(inpCmd.Data);
            procResult.Entity = entity;
            result.Output = procResult;
        }

        public StartTestingSvcOutput StartTesting(StartTestingSvcInput cmdInput)
        {
            // 1st. call start test for all readers - 0x50
            // 2nd move gpio to high test mode - 0x90
            // collect results and return them

            StartTestingSvcOutput result = new StartTestingSvcOutput();

            base.RunCode(cmdInput, result, () =>
            {
                List<DeviceIdentity> devices = _commandsProcessingContext.Devices;
                List<DeviceIdentity> readers = devices.Where(d => d.DeviceType == HighSpeedTestDeviceType.Reader).ToList();
                DeviceIdentity gpio = _commandsProcessingContext.GetGPIODevice();

                if (gpio == null || readers.Count == 0)
                {
                    throw new BusinessLogicException(ErrorCode.DEVICE_NOT_FOUND);
                }
                
                // executing start test commands...

                CommandData startRdTestCmd = _testActionCommandsInterpretator.GetStartTestingCommandData(cmdInput.TestMode);
                CommandData highModeGpioCmd = _gstCommandsInterpretator.GetHighSpeedGpioTestModeCommandData();

                List<DeviceOutgoingCommand> rdOutCmds = readers.Select(r => new DeviceOutgoingCommand() { DeviceIdentity = r, Data = startRdTestCmd }).ToList();
                DeviceOutgoingCommand gpioOutCmd = new DeviceOutgoingCommand() { DeviceIdentity = gpio, Data = highModeGpioCmd };

                _commandsProcessingContext.AddRange(rdOutCmds.ToArray()); // 1st - process readers
                foreach (DeviceOutgoingCommand outCmd in rdOutCmds)
                {
                    _processingSvcFacade.SendDataToDevice(outCmd); // send data asynchronously
                }

                foreach (DeviceOutgoingCommand outCmd in rdOutCmds) // now trying to get data
                {
                    try
                    {
                        DeviceIncommingCommand inpCmd = _waitForCommandResultService.WaitForResult(outCmd);
                        GeneralDeviceResult dres = _testActionCommandsInterpretator.ConvertResponseDataToGeneralDeviceResult(inpCmd.Data);
                        result.ReadersStartTestDeviceResults.Add(outCmd.DeviceIdentity.MacAddress, dres);
                    }
                    catch (TimeoutException)
                    {
                        result.ReadersStartTestSVCErrors.Add(outCmd.DeviceIdentity.MacAddress, new ErrorDetails(ErrorCode.DEVICE_WAITCMDRESULT_TIMEOUT_ERROR, null));
                    }
                }

                _commandsProcessingContext.Add(gpioOutCmd);
                _processingSvcFacade.SendDataToDevice(gpioOutCmd);
                try
                {
                    DeviceIncommingCommand inpGpioCmd = _waitForCommandResultService.WaitForResult(gpioOutCmd);
                    DeviceHighSpeedTestResult hsRes = _gstCommandsInterpretator.ConvertResponseDataToDeviceHighSpeedTestResult(inpGpioCmd.Data);
                    result.GpioHighSpeedDeviceResult = hsRes;
                }
                catch (TimeoutException)
                {
                    result.GpioHighSpeedSVCError = new ErrorDetails(ErrorCode.DEVICE_WAITCMDRESULT_TIMEOUT_ERROR, null);
                }

                // if all is ok then call c++ layer to start processing
                if (result.AllStarted)
                {
                    _processingSvcFacade.StartTest();
                    _commandsProcessingContext.TestState = CommandsProcessingMode.Running;
                    DBTest test = new DBTest() { AppSession = _commandsProcessingContext.AppSession.Id, Id = ObjectId.GenerateNewId(), Started = DateTime.Now, TestName = cmdInput.TestName };
                    _commandsProcessingContext.CurrentTest = test;
                    _dBTestRepository.Create(test);
                }
            });

            return result;
        }

        public StopTestingSvcOutput StopTesting()
        {
            StopTestingSvcOutput result = new StopTestingSvcOutput();

            base.RunCode(null, result, () =>
            {
                if (_commandsProcessingContext.AppMode == AppMode.Emulator)
                {
                    var client = new SimulatorSvcClient();
                    var simStopTestResult = client.StopTest();
                    if(!simStopTestResult.IsOk)
                    {
                        throw new BusinessLogicException(ErrorCode.SIM_STOP_TEST_ERROR);
                    }
                }

                // clear avery service c# state
                Console.WriteLine("clear avery service c# state..");

                _commandsProcessingContext.TestState = CommandsProcessingMode.NotRunningPending;
                Thread.Sleep(_commandsProcessingContext.CurrentDbUpdateFrequency * 2); // wain untill commands queues are clear
                _commandsProcessingContext.TestState = CommandsProcessingMode.NotRunning;  // TODO - not correct for device mode

                List<DeviceIdentity> devices = _commandsProcessingContext.Devices;
                List<DeviceIdentity> readers = devices.Where(d => d.DeviceType == HighSpeedTestDeviceType.Reader).ToList();
                DeviceIdentity gpio = _commandsProcessingContext.GetGPIODevice();

                if (gpio == null || readers.Count == 0)
                {
                    throw new BusinessLogicException(ErrorCode.DEVICE_NOT_FOUND);
                }

                CommandData stopRdTestCmd = _testActionCommandsInterpretator.GetStopTestingCommandData();
                CommandData highModeExitGpioCmd = _gstCommandsInterpretator.GetExitHighSpeedTestModeCommandData();

                List<DeviceOutgoingCommand> rdOutCmds = readers.Select(r => new DeviceOutgoingCommand() { DeviceIdentity = r, Data = stopRdTestCmd }).ToList();
                DeviceOutgoingCommand gpioOutCmd = new DeviceOutgoingCommand() { DeviceIdentity = gpio, Data = highModeExitGpioCmd };

                _commandsProcessingContext.AddRange(rdOutCmds.ToArray()); // 1st - process readers
                foreach (DeviceOutgoingCommand outCmd in rdOutCmds)
                {
                    _processingSvcFacade.SendDataToDevice(outCmd); // send data asynchronously
                }

                foreach (DeviceOutgoingCommand outCmd in rdOutCmds) // now trying to get data
                {
                    try
                    {
                        DeviceIncommingCommand inpCmd = _waitForCommandResultService.WaitForResult(outCmd);
                        GeneralDeviceResult dres = _testActionCommandsInterpretator.ConvertResponseDataToGeneralDeviceResult(inpCmd.Data);
                        result.ReadersStopTestDeviceResults.Add(outCmd.DeviceIdentity.MacAddress, dres);
                    }
                    catch (TimeoutException)
                    {
                        result.ReadersStopTestSVCErrors.Add(outCmd.DeviceIdentity.MacAddress, new ErrorDetails(ErrorCode.DEVICE_WAITCMDRESULT_TIMEOUT_ERROR, null));
                    }
                }

                _commandsProcessingContext.Add(gpioOutCmd);
                _processingSvcFacade.SendDataToDevice(gpioOutCmd);
                try
                {
                    DeviceIncommingCommand inpGpioCmd = _waitForCommandResultService.WaitForResult(gpioOutCmd);
                    DeviceHighSpeedTestResult hsRes = _gstCommandsInterpretator.ConvertResponseDataToDeviceHighSpeedTestResult(inpGpioCmd.Data);
                    result.GpioHighSpeedDeviceResult = hsRes;
                }
                catch (TimeoutException)
                {
                    result.GpioHighSpeedSVCError = new ErrorDetails(ErrorCode.DEVICE_WAITCMDRESULT_TIMEOUT_ERROR, null);
                }

                // if (result.AllStopped)
                {  //stop anyways
                    Console.WriteLine("_processingSvcFacade.StopTest();");
                    _processingSvcFacade.StopTest();

                    _commandsProcessingContext.TestState = CommandsProcessingMode.NotRunning;
                    _commandsProcessingContext.CurrentTest.Finished = DateTime.Now;
                    _commandsProcessingContext.UnsolicitedStats.Clear();
                    _dBTestRepository.Update(_commandsProcessingContext.CurrentTest);
                    _commandsProcessingContext.CurrentTest = null;
                }
            });
            return result;
        }

        public SvcOutputGeneric<DeviceConfig> PreInitialize(PreInitializeSvcInput inputAppMode)
        {
            var result = new SvcOutputGeneric<DeviceConfig>();

            base.RunCode(inputAppMode, result, () =>
            {
                bool initMockMode = bool.Parse(ConfigurationManager.AppSettings[InitMockMode]);
                _commandsProcessingContext.AppMode = inputAppMode.AppMode;

                DeviceConfig deviceConfig;
                if (initMockMode)
                {
                    var config = (DeviceConfigSection)ConfigurationManager.GetSection("devices");
                    deviceConfig = config.GetDeviceConfig();
                }
                else if (inputAppMode.AppMode == AppMode.Emulator)
                {
                    var simulatorClient = new SimulatorSvcClient();
                    SvcOutputGeneric<DeviceConfig> config = simulatorClient.Initialize(new SvcInputGeneric<int>() { Input = inputAppMode.NumberOfDeviceSetOnUI });
                    deviceConfig = config.Output;

                    if (deviceConfig == null)
                    {
                        throw new BusinessLogicException(ErrorCode.SIM_OBTAINING_DEVICECFG_ERROR);
                    }
                }
                else
                {
                    var config = (DeviceConfigSection)ConfigurationManager.GetSection("devices");
                    deviceConfig = config.GetDeviceConfig();
                }
                result.Output = deviceConfig;                
            });

            return result;
        }

        public CheckDevicesInstalledOutput CheckDevicesInitStatus()
        {
            var result = new CheckDevicesInstalledOutput();

            RunCode(null, result, () =>
            {
                bool initMockMode = bool.Parse(ConfigurationManager.AppSettings[InitMockMode]);
                if (initMockMode)
                {
                    if (attempt == 8)
                        result.Installed = true;
                    else
                        result.Installed = false;
                }
                else
                {
                    bool status = _processingSvcFacade.CheckDevicesInitStatus();
                    result.Installed = status;
                }
            });

            return result;
        }

        public SvcOutputBase Initialize(SvcInputGeneric<DeviceConfig> inputConfig)
        {
            var result = new SvcOutputBase();

            base.RunCode(inputConfig, result, () =>
            {
                bool initMockMode = bool.Parse(ConfigurationManager.AppSettings[InitMockMode]);
                if (initMockMode)
                    return;

                _processingSvcFacade.Initialize(inputConfig.Input);
            });

            return result;
        }

        public SvcOutputBase StartDeviceListening()
        {
            var result = new SvcOutputBase();

            base.RunCode(null, result, () =>
            {
                bool initMockMode = bool.Parse(ConfigurationManager.AppSettings[InitMockMode]);
                if (initMockMode)
                    return;

                _processingSvcFacade.StartDeviceListening();
            });

            return result;
        }

        static int attempt = 0;
        public SvcOutputGeneric<List<DeviceIdentity>> GetInitializedDevices()
        {
            var result = new SvcOutputGeneric<List<DeviceIdentity>>();

            base.RunCode(null, result, () =>
            {
                bool initMockMode = bool.Parse(ConfigurationManager.AppSettings[InitMockMode]);
                if (initMockMode)
                {
                    List<DeviceIdentity> devices = new List<DeviceIdentity>();
                    for (int i = 0; i < attempt; i++)
                    {
                        devices.Add(new DeviceIdentity()
                        {
                            DeviceType = i == 0 ? HighSpeedTestDeviceType.GPIO : HighSpeedTestDeviceType.Reader,
                            Lane = i - 1,
                            MacAddress = $"0x{i}123{i}",
                            ProductId = $"ProductId{i}",
                            VendorId = $"VendorId{i}"
                        });
                    }
                    if (attempt < 8)
                        attempt++;
                    result.Output = devices;
                }
                else
                {
                    List<DeviceIdentity> devices = _processingSvcFacade.GetDeviceListInfo();
                    devices.ForEach(d =>
                    {
                        d.Lane = 0;
                    });
                    result.Output = devices;
                }
            });

            return result;
        }

        public SvcOutputGeneric<List<DeviceIdentity>> InitializeStep2(InitializeStep2Input initInput)
        {
            var result = new SvcOutputGeneric<List<DeviceIdentity>>();

            base.RunCode(initInput, result, () =>
            {
                bool initMockMode = bool.Parse(ConfigurationManager.AppSettings[InitMockMode]);
                bool simulationMode = initInput.AppMode == AppMode.Emulator;


                List<int> laneNums = initInput.Devices.Where(d => d.DeviceType != HighSpeedTestDeviceType.GPIO && d.Lane != -1).Select(d => d.Lane).ToList();

                /*laneNums.Sort();
                if(laneNums.First() != 0)
                {
                    throw new BusinessLogicException(ErrorCode.LANE_NUM_START_INDEX_WRONG);
                }
                else if(laneNums.Distinct().Count() != laneNums.Count)
                {
                    throw new BusinessLogicException(ErrorCode.LANE_SEQUENCE_WRONG);
                }
                else if (laneNums.Last() != laneNums.Count-1)
                {
                    throw new BusinessLogicException(ErrorCode.LANE_SEQUENCE_WRONG);
                }*/

                if (laneNums.Distinct().Count() < laneNums.Count)
                {
                    throw new BusinessLogicException(ErrorCode.LANE_MUSTBE_UNIQUE);
                }


                List<DeviceIdentity> devicesToAccept = initInput.Devices.Where(d => d.DeviceType == HighSpeedTestDeviceType.GPIO || d.Lane >= 0).ToList();
                //_conveyorSettings.TotalLanes = devicesToAccept.Count(d => d.DeviceType == HighSpeedTestDeviceType.Reader);
                var appSession = new DBAppSession()
                {
					Name = initInput.AppSessionName,
                    Devices = devicesToAccept,
                    Id = ObjectId.GenerateNewId(),
                    AppMode = AppMode.Emulator,
                    StartDt = DateTime.Now
                };
	            this._commandsProcessingContext.AppSession = appSession;
	            _appSessionRepository.Create(appSession);
				
				if (initMockMode)
                {
                    this._commandsProcessingContext.Devices = devicesToAccept;
                }
                else if (!simulationMode)
                {
                    //_processingSvcFacade.SetConveyorSettings(_conveyorSettings);
                    _processingSvcFacade.InitializeStep2(devicesToAccept);
                    this._commandsProcessingContext.Devices = devicesToAccept;
                }
                else if (simulationMode)
                {
                    var client = new SimulatorSvcClient();
                    List<DeviceIdentity> devicesToExclude = initInput.Devices.Where(d => !devicesToAccept.Contains(d)).ToList();
                    client.ExcludeDevices(devicesToExclude);

                    //_processingSvcFacade.SetConveyorSettings(_conveyorSettings);
                    _processingSvcFacade.InitializeStep2(devicesToAccept);
                    this._commandsProcessingContext.Devices = devicesToAccept;
                }
                else
                {
                    throw new InvalidOperationException();
                }
                result.Output = devicesToAccept;
            });

            return result;
        }

        public SvcOutputGeneric<ConveyorShot> GetFirstTestSnapshot(SvcInputGeneric<ObjectId> inputTestId)
        {
            SvcOutputGeneric<ConveyorShot> result = new SvcOutputGeneric<ConveyorShot>();
            RunCode(inputTestId, result, () =>
            {
                ConveyorShotRepository repo = new ConveyorShotRepository(_mongoDatabase);
                List<ConveyorShot> shots = repo.Find(sh => sh.Id != ObjectId.Empty, sh => sh.PlanToCreateDt, true, 0, 1);
                result.Output = shots.FirstOrDefault();
            });
            return result;
        }

        public SvcOutputGeneric<List<ConveyorShot>> GetTopNSnapshotsStartingFromPlanToCreateDt(GetTopNSnapshotsStartingFromPlanCreateDtInput input)
        {
            SvcOutputGeneric<List<ConveyorShot>> result = new SvcOutputGeneric<List<ConveyorShot>>();
            RunCode(input, result, () =>
            {
                ConveyorShotRepository repo = new ConveyorShotRepository(_mongoDatabase);
                List<ConveyorShot> shots = repo.Find(sh => sh.PlanToCreateDt >= input.StartFromPlanDt, sh => sh.PlanToCreateDt, false, 0, input.N);
                result.Output = shots;
            });
            return result;
        }

        public SvcOutputGeneric<List<ConveyorShot>> GetShapshotsForTimeInterval(GetShapshotsForTimeIntervalSvcInput input)
        {
            SvcOutputGeneric<List<ConveyorShot>> result = new SvcOutputGeneric<List<ConveyorShot>>();
            RunCode(input, result, () =>
            {
                List<ConveyorShot> shots = new List<ConveyorShot>();
                ConveyorShotRepository repo = new ConveyorShotRepository(_mongoDatabase);
                result.Output = repo.Find(sh => sh.PlanToCreateDt >= input.Start && sh.PlanToCreateDt <= input.End);
            });
            return result;
        }

        public SvcOutputGeneric<List<DeviceCommandLogTransferItem>> GetDeviceDataLogItems(DeviceEntitySvcInput<DateTime> sinceInput)
        {
            var result = new SvcOutputGeneric<List<DeviceCommandLogTransferItem>>();

            RunCode(sinceInput, result, () =>
            {
                List<DeviceCommandLogTransferItem> resLogItems = new List<DeviceCommandLogTransferItem>();

                if (_commandsProcessingContext.Devices == null)
                {
                    result.Output = resLogItems;
                    return;
                }

                DateTime since = sinceInput.Input;

                List<DeviceIdentity> devicesToReview = new List<DeviceIdentity>();
                if (sinceInput.ForAllDevices)
                {
                    devicesToReview.AddRange(_commandsProcessingContext.Devices);
                }
                else
                {
                    devicesToReview.Add(_commandsProcessingContext.GetDeviceByMacAddr(sinceInput.DeviceMacAddr));
                }

                foreach (DeviceIdentity device in devicesToReview)
                {
                    DeviceCommandsRepository repo = new DeviceCommandsRepository(_mongoDatabase, device);
                    List<DBCollectionCommand> dbDeviceCmds = repo.Find(c => c.ReceiveDt >= sinceInput.Input && !c.IsUnsolicited);

                    var tmpLogItems = dbDeviceCmds.Select(dc => new DeviceCommandLogTransferItem() { DbId = dc.Id.ToString(), ReceiveDt = dc.ReceiveDt, Device = device, Direction = dc.Direction, HexData = dc.UserFriendlyCmdView }).ToList();
                    resLogItems.AddRange(tmpLogItems);
                }

                result.Output = resLogItems;
            });

            return result;
        }

        public SvcOutputGeneric<List<DeviceCommandLogTransferItem>> GetDeviceUnsolicitedDataLogItems(DeviceEntitySvcInput<Tuple<int, int>> limitInput)
        {
            var result = new SvcOutputGeneric<List<DeviceCommandLogTransferItem>>();

            RunCode(limitInput, result, () =>
            {
                List<DeviceCommandLogTransferItem> resLogItems = new List<DeviceCommandLogTransferItem>();

                List<DeviceIdentity> devicesToReview = new List<DeviceIdentity>();
                if (limitInput.ForAllDevices)
                {
                    devicesToReview.AddRange(_commandsProcessingContext.Devices);
                }
                else
                {
                    devicesToReview.Add(_commandsProcessingContext.GetDeviceByMacAddr(limitInput.DeviceMacAddr));
                }

                foreach (DeviceIdentity device in devicesToReview)
                {
                    DeviceCommandsRepository repo = new DeviceCommandsRepository(_mongoDatabase, device);
                    var dbDeviceCmds = repo.Find(c => c.IsUnsolicited, c => c.ReceiveDt, false, limitInput.Input.Item1, limitInput.Input.Item2);

                    var tmpLogItems = dbDeviceCmds.Select(dc => new DeviceCommandLogTransferItem() { ReceiveDt = dc.ReceiveDt, Device = device, Direction = dc.Direction, HexData = dc.UserFriendlyCmdView }).ToList();
                    resLogItems.AddRange(tmpLogItems);
                }
                result.Output = resLogItems;
            });

            return result;
        }

        public SvcOutputBase ReinstallDsf()
        {
            var result = new SvcOutputBase();

            RunCode(null, result, () =>
            {
                //	_processingSvcFacade.ResetState();
                _commandsProcessingContext.ClearCommandQueues();
                //_cmdLogQueue.Clear();

                var client = new SimulatorSvcClient();
                var res = client.ReinstallDsf();
                if (!res.IsOk)
                {
                    result.ErrorMessage = res.ErrorMessage;
                }
            });

            return result;
        }

        /*
        public SvcOutputBase ResetServicesState()
        {
            var result = new SvcOutputBase();

            base.RunCode(null, result, () =>
            {
                var client = new SimulatorSvcClient();
                var output = client.Reset();
                if (!output.IsOk)
                {
                    result.ErrorMessage = output.ErrorMessage;
                }

                //	_processingSvcFacade.ResetState();

                _commandsProcessingContext.ClearCommandQueues();
            });

            return result;
        }*/

        public CheckMongoStatusOutput CheckMongoServiceStatus()
        {
            var result = new CheckMongoStatusOutput();

            try
            {
                using (var serviceController = new ServiceController("MongoDB"))
                {
                    result.IsMongoDbStopped = serviceController.Status != ServiceControllerStatus.Running;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                result.ErrorMessage = ErrorMessageHelper.CreateError(ErrorCode.SVC_CHECK_MONGOSVC_ERROR);
            }

            return result;
        }

	    public SvcOutputGeneric<Dictionary<byte, int>> GetUnsolicitedStats()
        {
            SvcOutputGeneric<Dictionary<byte, int>> result = new SvcOutputGeneric<Dictionary<byte, int>>();
            result.Output = new Dictionary<byte, int>();

            RunCode(null, result, () =>
            {
                result.Output = _commandsProcessingContext.UnsolicitedStats.ToDictionary(d => d.Key, d2 => d2.Value);
            });
            return result;
        }

        public SvcOutputBase SetConveyorSettings(SvcInputGeneric<UIConveyorSettings> setInput)
        {
            SvcOutputBase result = new SvcOutputBase();

            RunCode(setInput, result, () =>
            {
                _processingSvcFacade.SetConveyorSettings(new ConveyorSettings()
                {
                    DistanceBetweenTagsInMm = setInput.Input.DistanceBetweenTagsInMm,
                    EncoderReaderTagsDistance = setInput.Input.EncoderReaderTagsDistance,
                    MarkerPuncherTagsDistance = setInput.Input.MarkerPuncherTagsDistance,
                    ReaderMarkerTagsDistance = setInput.Input.ReaderMarkerTagsDistance,
                    TagLengthInMm = setInput.Input.TagLengthInMm,
                    TagsCountPerOneLane = setInput.Input.TagsCountPerLane,
                    TotalLanesTemp = _commandsProcessingContext.Devices.Count(d => d.DeviceType == HighSpeedTestDeviceType.Reader)
                });

                if (_commandsProcessingContext.AppMode == AppMode.Emulator)
                {
                    var simulatorClient = new SimulatorSvcClient();
                    simulatorClient.SetSimulatorSettings(new SvcInputGeneric<SimulatorSettings>()
                    {
                        Input = new SimulatorSettings()
                        {
                            DistanceBetweenTagsInMm = setInput.Input.DistanceBetweenTagsInMm,
                            EncoderReaderTagsDistance = setInput.Input.EncoderReaderTagsDistance,
                            EncoderStepsPerTag = setInput.Input.EncoderStepsPerTag,
                            TagLengthInMm = setInput.Input.TagLengthInMm,
                            TestTagsNumber = setInput.Input.TestTagsNumber,
                            VelocityTagsPerMSec = (double)setInput.Input.VelocityTagsPerSec / 1000f
                        }
                    });
                }
            });
        
            return result;
        }
    }
}
