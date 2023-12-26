using Client.Server.Communication.RemoteServices.ServiceContracts;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using Common.Services.Input;
using Common.Services.Output;
using Client.Server.Communication.RemoteServices.Dtos.Input;
using Client.Server.Communication.RemoteServices.Dtos.Output;
using Common.Domain.Device;
using Common.Domain.DeviceResults;
using Common.Domain.TestModuleCommands;
using Common.Domain.Conveyor;
using Common.Domain.DeviceResults.TestModuleCommands;
using Common.Domain.ExtendedTestSetupCommands;
using Common.Domain.GSTCommands;
using Common.Domain.TestSetupCommands;
using Common.Enums;
using Client.Server.Communication.Domain;

namespace Client.UIApp.RemoteServices.Clients
{
    public class AverySvcClient : ClientBase<IAveryServerSvc>, IAveryServerSvc
    {
        public SvcOutputGeneric<GeneralDeviceResult> SetTestSettings(SetDataToDeviceSvcInput<TestSettings> setTestSettingsInput)
        {
            return Channel.SetTestSettings(setTestSettingsInput);
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetMarkerSettings(SetDataToDeviceSvcInput<MarkerSettings> setMarkerSettingsInput)
        {
            return Channel.SetMarkerSettings(setMarkerSettingsInput);
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetTriggerInputSettings(SetDataToDeviceSvcInput<TriggerInputSettings> setTriggerInSettingsInput)
        {
            return Channel.SetTriggerInputSettings(setTriggerInSettingsInput);
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetReaderSettings(SetDataToDeviceSvcInput<TesterSettings> setTesterSettingsInput)
        {
            return Channel.SetReaderSettings(setTesterSettingsInput);
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetTestStatistics(SetDataToDeviceSvcInput<TestStatistics> setTestStatisticsInput)
        {
            return Channel.SetTestStatistics(setTestStatisticsInput);
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetAntennaSettings(SetDataToDeviceSvcInput<AntennaSettings> setAntennaSettingsInput)
        {
            return Channel.SetAntennaSettings(setAntennaSettingsInput);
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetSensitivityTestSettings(SetDataToDeviceSvcInput<SensitivityTestSettings> setSensitibityTestSettingsInput)
        {
            return Channel.SetSensitivityTestSettings(setSensitibityTestSettingsInput);
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetTIDTestSettings(SetDataToDeviceSvcInput<TIDTestSettings> setTidTestSettingsInput)
        {
            return Channel.SetTIDTestSettings(setTidTestSettingsInput);
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetAuxSettings(SetDataToDeviceSvcInput<AuxSettings> setAuxSettingsInput)
        {
            return Channel.SetAuxSettings(setAuxSettingsInput);
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetEncoderSettings(SetDataToDeviceSvcInput<EncoderSettings> setEncoderSettingsInput)
        {
            return Channel.SetEncoderSettings(setEncoderSettingsInput);
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetPuncherSettings(SetDataToDeviceSvcInput<PunchSettings> setPuncherSettingsInput)
        {
            return Channel.SetPuncherSettings(setPuncherSettingsInput);
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetHighSpeedTestModeTimer(SetDataToDeviceSvcInput<HighSpeedTestModeTimer> setHighSpeedTestModeTimerInput)
        {
            return Channel.SetHighSpeedTestModeTimer(setHighSpeedTestModeTimerInput);
        }

        public SvcOutputGeneric<EntityDeviceResult<HighSpeedTestModeTimer>> GetHighSpeedTestModeTimer(SvcInputGeneric<string> getHighSpeedTestModeTimerInput)
        {
            return Channel.GetHighSpeedTestModeTimer(getHighSpeedTestModeTimerInput);
        }

        public StartTestingSvcOutput StartTesting(StartTestingSvcInput cmdInput)
        {
            return Channel.StartTesting(cmdInput);
        }

        public StopTestingSvcOutput StopTesting()
        {
            return Channel.StopTesting();
        }

        public SvcOutputGeneric<DeviceConfig> PreInitialize(PreInitializeSvcInput inputAppMode)
        {
            return Channel.PreInitialize(inputAppMode);
        }

        public CheckDevicesInstalledOutput CheckDevicesInitStatus()
        {
            return Channel.CheckDevicesInitStatus();
        }

        public SvcOutputBase Initialize(SvcInputGeneric<DeviceConfig> inputConfig)
        {
            return Channel.Initialize(inputConfig);
        }

        public SvcOutputBase StartDeviceListening()
        {
            return Channel.StartDeviceListening();
        }

        public SvcOutputGeneric<List<DeviceIdentity>> GetInitializedDevices()
        {
            return Channel.GetInitializedDevices();
        }

        public SvcOutputGeneric<List<DeviceIdentity>> InitializeStep2(InitializeStep2Input initInput)
        {
            return Channel.InitializeStep2(initInput);
        }

        public SvcOutputGeneric<GeneralDeviceResult> Ping(SvcInputGeneric<string> pingInput)
        {
            return Channel.Ping(pingInput);
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetDateTime(SetDataToDeviceSvcInput<LaneDateTime> setDateTimeInput)
        {
            return Channel.SetDateTime(setDateTimeInput);
        }

        public SvcOutputGeneric<EntityDeviceResult<LaneDateTime>> GetDateTime(SvcInputGeneric<string> getDateTimeInput)
        {
            return Channel.GetDateTime(getDateTimeInput);
        }

        public SvcOutputGeneric<GeneralDeviceResult> ClearLastFault(SvcInputGeneric<string> clearLastFaultInput)
        {
            return Channel.ClearLastFault(clearLastFaultInput);
        }

        public SvcOutputGeneric<List<ConveyorShot>> GetShapshotsForTimeInterval(GetShapshotsForTimeIntervalSvcInput input)
        {
            return Channel.GetShapshotsForTimeInterval(input);
        }

        public SvcOutputGeneric<List<DeviceCommandLogTransferItem>> GetDeviceDataLogItems(DeviceEntitySvcInput<DateTime> sinceInput)
        {
            return Channel.GetDeviceDataLogItems(sinceInput);
        }

        public SvcOutputGeneric<List<DeviceCommandLogTransferItem>> GetDeviceUnsolicitedDataLogItems(DeviceEntitySvcInput<Tuple<int, int>> limitInput)
        {
            return Channel.GetDeviceUnsolicitedDataLogItems(limitInput);
        }

        public SvcOutputBase ReinstallDsf()
        {
            return Channel.ReinstallDsf();
        }

        //public SvcOutputBase ResetServicesState()
        //{
        //    return Channel.ResetServicesState();
        //}

	    public CheckMongoStatusOutput CheckMongoServiceStatus()
	    {
		    return Channel.CheckMongoServiceStatus();
	    }

	    public SvcOutputGeneric<GeneralDeviceResult> SetTagIdFilterSettings(SetDataToDeviceSvcInput<TagIDFilterSettings> setTagIdFilterInput)
        {
            return Channel.SetTagIdFilterSettings(setTagIdFilterInput);
        }

        public SvcOutputGeneric<EntityDeviceResult<VersionResult>> GetVersion(SvcInputGeneric<string> getVersionInput)
        {
            return Channel.GetVersion(getVersionInput);
        }

        public SvcOutputGeneric<EntityDeviceResult<TestSettings>> GetTestSettings(SvcInputGeneric<string> getTestSettingsInput)
        {
            return Channel.GetTestSettings(getTestSettingsInput);
        }

        public SvcOutputGeneric<EntityDeviceResult<MarkerSettings>> GetMarkerSettings(SvcInputGeneric<string> getMarkerSettingsInput)
        {
            return Channel.GetMarkerSettings(getMarkerSettingsInput);
        }

        public SvcOutputGeneric<EntityDeviceResult<TriggerInputSettings>> GetTriggerInputSettings(SvcInputGeneric<string> getTriggerInSettingsInput)
        {
            return Channel.GetTriggerInputSettings(getTriggerInSettingsInput);
        }

        public SvcOutputGeneric<EntityDeviceResult<TesterSettings>> GetTesterSettings(SvcInputGeneric<string> getTesterSettingsInput)
        {
            return Channel.GetTesterSettings(getTesterSettingsInput);
        }

        public SvcOutputGeneric<EntityDeviceResult<TestStatistics>> GetTestStatistics(SvcInputGeneric<string> getTestStatisticsInput)
        {
            return Channel.GetTestStatistics(getTestStatisticsInput);
        }

        public SvcOutputGeneric<EntityDeviceResult<AntennaSettings>> GetAntennaSettings(SvcInputGeneric<string> getAntennaSettingsInput)
        {
            return Channel.GetAntennaSettings(getAntennaSettingsInput);
        }

        public SvcOutputGeneric<EntityDeviceResult<TagIDFilterSettings>> GetTagIdFilterSettings(SvcInputGeneric<string> getTagIdFilterInput)
        {
            return Channel.GetTagIdFilterSettings(getTagIdFilterInput);
        }

        public SvcOutputGeneric<EntityDeviceResult<AuxSettings>> GetAuxInSettings(SvcInputGeneric<string> getAuxSettingsInput)
        {
            return Channel.GetAuxInSettings(getAuxSettingsInput);
        }

        public SvcOutputGeneric<EntityDeviceResult<EncoderSettings>> GetEncoderSettings(SvcInputGeneric<string> getEncoderSettingsInput)
        {
            return Channel.GetEncoderSettings(getEncoderSettingsInput);
        }

        public SvcOutputGeneric<EntityDeviceResult<PunchSettings>> GetPunchSettings(SvcInputGeneric<string> getPunchSettingsInput)
        {
            return Channel.GetPunchSettings(getPunchSettingsInput);
        }

        public SvcOutputGeneric<EntityDeviceResult<SensitivityTestSettings>> GetSensitivityTestSettings(SvcInputGeneric<string> getSensitivitySettingsInput)
        {
            return Channel.GetSensitivityTestSettings(getSensitivitySettingsInput);
        }

        public SvcOutputGeneric<EntityDeviceResult<TIDTestSettings>> GetTIDTestSettings(SvcInputGeneric<string> getTidTestSettingsInput)
        {
            return Channel.GetTIDTestSettings(getTidTestSettingsInput);
        }

        public SvcOutputGeneric<EntityDeviceResult<LastFaultResult>> GetLastFault(SvcInputGeneric<string> getLastFaultInput)
        {
            return Channel.GetLastFault(getLastFaultInput);
        }

        public SvcOutputGeneric<GeneralDeviceResult> SetFault(SetDataToDeviceSvcInput<Common.Enums.FaultCode> setFaultInput)
        {
            return Channel.SetFault(setFaultInput);
        }

        public SvcOutputGeneric<GeneralDeviceResult> Reset(SetDataToDeviceSvcInput<ResetSettings> resetInput)
        {
            return Channel.Reset(resetInput);
        }

        public SvcOutputGeneric<Dictionary<byte, int>> GetUnsolicitedStats()
        {
            return Channel.GetUnsolicitedStats();
        }

        public SvcOutputBase SetConveyorSettings(SvcInputGeneric<UIConveyorSettings> setInput)
        {
            return Channel.SetConveyorSettings(setInput);
        }

        public SvcOutputGeneric<ConveyorShot> GetFirstTestSnapshot(SvcInputGeneric<global::MongoDB.Bson.ObjectId> inputTestId)
        {
            return Channel.GetFirstTestSnapshot(inputTestId);
        }

        public SvcOutputGeneric<List<ConveyorShot>> GetTopNSnapshotsStartingFromPlanToCreateDt(GetTopNSnapshotsStartingFromPlanCreateDtInput input)
        {
            return Channel.GetTopNSnapshotsStartingFromPlanToCreateDt(input);
        }
    }
}
