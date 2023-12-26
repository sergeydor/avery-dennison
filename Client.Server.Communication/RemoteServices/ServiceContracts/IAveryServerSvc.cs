using Client.Server.Communication.RemoteServices.Dtos.Input;
using Client.Server.Communication.RemoteServices.Dtos.Output;
using Common.Services.Input;
using Common.Services.Output;
using System.Collections.Generic;
using Common.Domain;
using Common.Domain.Device;
using Common.Domain.DeviceResults;
using Common.Domain.TestModuleCommands;
using Common.Domain.Conveyor;
using System;
using System.ServiceModel;
using Common.Domain.TestSetupCommands;
using Common.Domain.DeviceResults.TestModuleCommands;
using Common.Domain.ExtendedTestSetupCommands;
using Common.Domain.GSTCommands;
using Common.Enums;
using Client.Server.Communication.Domain;
using MongoDB.Bson;

namespace Client.Server.Communication.RemoteServices.ServiceContracts
{
    [ServiceContract]
    public interface IAveryServerSvc
    {
        [OperationContract]
        SvcOutputBase SetConveyorSettings(SvcInputGeneric<UIConveyorSettings> setInput);

        [OperationContract]
        SvcOutputGeneric<GeneralDeviceResult> Ping(SvcInputGeneric<string> pingInput);

		/// <summary>
		/// Set Date Time (0x08)
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<GeneralDeviceResult> SetDateTime(SetDataToDeviceSvcInput<LaneDateTime> setDateTimeInput);

        /// <summary>
        /// 0x1A
        /// </summary>
        [OperationContract]
        SvcOutputGeneric<GeneralDeviceResult> SetTagIdFilterSettings(SetDataToDeviceSvcInput<TagIDFilterSettings> setTagIdFilterInput);
        

        /// <summary>
        /// Get Date Time (0x09)
        /// </summary>
        [OperationContract]
	    SvcOutputGeneric<EntityDeviceResult<LaneDateTime>> GetDateTime(SvcInputGeneric<string> getDateTimeInput);

        /// <summary>
        /// 0x01
        /// </summary>
        [OperationContract]
        SvcOutputGeneric<EntityDeviceResult<VersionResult>> GetVersion(SvcInputGeneric<string> getVersionInput);

		/// <summary>
		/// 0x30
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<EntityDeviceResult<TestSettings>> GetTestSettings(SvcInputGeneric<string> getTestSettingsInput);

		/// <summary>
		/// 0x31
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<EntityDeviceResult<MarkerSettings>> GetMarkerSettings(SvcInputGeneric<string> getMarkerSettingsInput);

		/// <summary>
		/// 0x32
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<EntityDeviceResult<TriggerInputSettings>> GetTriggerInputSettings(SvcInputGeneric<string> getTriggerInSettingsInput);

		/// <summary>
		/// 0x34
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<EntityDeviceResult<TesterSettings>> GetTesterSettings(SvcInputGeneric<string> getTesterSettingsInput);

		/// <summary>
		/// 0x35
		/// </summary>
		[OperationContract]
		SvcOutputGeneric<EntityDeviceResult<TestStatistics>> GetTestStatistics(SvcInputGeneric<string> getTestStatisticsInput);

		/// <summary>
		/// 0x37
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<EntityDeviceResult<AntennaSettings>> GetAntennaSettings(SvcInputGeneric<string> getAntennaSettingsInput);

		/// <summary>
		/// 0x3A
		/// </summary>
		[OperationContract]
        SvcOutputGeneric<EntityDeviceResult<TagIDFilterSettings>> GetTagIdFilterSettings(SvcInputGeneric<string> getTagIdFilterInput);

		/// <summary>
		/// 0x3C
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<EntityDeviceResult<AuxSettings>> GetAuxInSettings(SvcInputGeneric<string> getAuxSettingsInput);

		/// <summary>
		/// 0x3D
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<EntityDeviceResult<EncoderSettings>> GetEncoderSettings(SvcInputGeneric<string> getEncoderSettingsInput);

		/// <summary>
		/// 0x40
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<EntityDeviceResult<PunchSettings>> GetPunchSettings(SvcInputGeneric<string> getPunchSettingsInput);

		/// <summary>
		/// 0x38
		/// </summary>
		[OperationContract]
		SvcOutputGeneric<EntityDeviceResult<SensitivityTestSettings>> GetSensitivityTestSettings(SvcInputGeneric<string> getSensitivitySettingsInput);

		/// <summary>
		/// 0x39
		/// </summary>
		[OperationContract]
		SvcOutputGeneric<EntityDeviceResult<TIDTestSettings>> GetTIDTestSettings(SvcInputGeneric<string> getTidTestSettingsInput);

        /// <summary>
        /// 0x03
        /// </summary>
        [OperationContract]
        SvcOutputGeneric<EntityDeviceResult<LastFaultResult>> GetLastFault(SvcInputGeneric<string> getLastFaultInput);

		/// <summary>
		/// 0x63
		/// </summary>
		[OperationContract]
		SvcOutputGeneric<GeneralDeviceResult> SetFault(SetDataToDeviceSvcInput<Common.Enums.FaultCode> setFaultInput);

	    /// <summary>
	    /// 0x6
	    /// </summary>
	    [OperationContract]
	    SvcOutputGeneric<GeneralDeviceResult> ClearLastFault(SvcInputGeneric<string> clearLastFaultInput);

        /// <summary>
        /// 0x02
        /// </summary>
        [OperationContract]
        SvcOutputGeneric<GeneralDeviceResult> Reset(SetDataToDeviceSvcInput<ResetSettings> resetInput);

        [OperationContract]
        SvcOutputGeneric<ConveyorShot> GetFirstTestSnapshot(SvcInputGeneric<ObjectId> inputTestId);

        [OperationContract]
        SvcOutputGeneric<List<ConveyorShot>> GetTopNSnapshotsStartingFromPlanToCreateDt(GetTopNSnapshotsStartingFromPlanCreateDtInput input);

        [OperationContract]
        SvcOutputGeneric<List<ConveyorShot>> GetShapshotsForTimeInterval(GetShapshotsForTimeIntervalSvcInput input);

        /// <summary>
        /// 0x50, 0x90
        /// </summary>
        /// <param name="cmdInput"></param>
        /// <returns></returns>
        [OperationContract]
        StartTestingSvcOutput StartTesting(StartTestingSvcInput cmdInput);

        /// <summary>
        /// 0x51
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        StopTestingSvcOutput StopTesting();

        /// <summary>
		/// 0x10
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<GeneralDeviceResult> SetTestSettings(SetDataToDeviceSvcInput<TestSettings> setTestSettingsInput);

		/// <summary>
		/// 0x11
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<GeneralDeviceResult> SetMarkerSettings(SetDataToDeviceSvcInput<MarkerSettings> setMarkerSettingsInput);

		/// <summary>
		/// 0x12
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<GeneralDeviceResult> SetTriggerInputSettings(SetDataToDeviceSvcInput<TriggerInputSettings> setTriggerInSettingsInput);

		/// <summary>
		/// 0x14
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<GeneralDeviceResult> SetReaderSettings(SetDataToDeviceSvcInput<TesterSettings> setTesterSettingsInput);

		/// <summary>
		/// 0x15
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<GeneralDeviceResult> SetTestStatistics(SetDataToDeviceSvcInput<TestStatistics> setTestStatisticsInput);

		/// <summary>
		/// 0x17
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<GeneralDeviceResult> SetAntennaSettings(SetDataToDeviceSvcInput<AntennaSettings> setAntennaSettingsInput);

		/// <summary>
		/// 0x18
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<GeneralDeviceResult> SetSensitivityTestSettings(SetDataToDeviceSvcInput<SensitivityTestSettings> setSensitibityTestSettingsInput);

		/// <summary>
		/// 0x19
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<GeneralDeviceResult> SetTIDTestSettings(SetDataToDeviceSvcInput<TIDTestSettings> setTidTestSettingsInput);

		/// <summary>
		/// 0x1C
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<GeneralDeviceResult> SetAuxSettings(SetDataToDeviceSvcInput<AuxSettings> setAuxSettingsInput);

		/// <summary>
		/// 0x1D
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<GeneralDeviceResult> SetEncoderSettings(SetDataToDeviceSvcInput<EncoderSettings> setEncoderSettingsInput);

		/// <summary>
		/// 0x20
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<GeneralDeviceResult> SetPuncherSettings(SetDataToDeviceSvcInput<PunchSettings> setPuncherSettingsInput);

		/// <summary>
		/// 0x92
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<GeneralDeviceResult> SetHighSpeedTestModeTimer(SetDataToDeviceSvcInput<HighSpeedTestModeTimer> setHighSpeedTestModeTimerInput);

		/// <summary>
		/// 0x93
		/// </summary>
		[OperationContract]
	    SvcOutputGeneric<EntityDeviceResult<HighSpeedTestModeTimer>> GetHighSpeedTestModeTimer(SvcInputGeneric<string> getHighSpeedTestModeTimerInput);

	    [OperationContract]
	    SvcOutputGeneric<DeviceConfig> PreInitialize(PreInitializeSvcInput inputAppMode);

	    [OperationContract]
		CheckDevicesInstalledOutput CheckDevicesInitStatus();

		[OperationContract]
		SvcOutputBase Initialize(SvcInputGeneric<DeviceConfig> inputConfig);

	    [OperationContract]
	    SvcOutputBase StartDeviceListening();

		[OperationContract]
	    SvcOutputGeneric<List<DeviceIdentity>> GetInitializedDevices();

		[OperationContract]
        SvcOutputGeneric<List<DeviceIdentity>> InitializeStep2(InitializeStep2Input initInput);

        [OperationContract]
        SvcOutputGeneric<List<DeviceCommandLogTransferItem>> GetDeviceDataLogItems(DeviceEntitySvcInput<DateTime> sinceInput);

	    [OperationContract]
	    SvcOutputGeneric<List<DeviceCommandLogTransferItem>> GetDeviceUnsolicitedDataLogItems(DeviceEntitySvcInput<Tuple<int, int>> limitInput);

        [OperationContract]
        SvcOutputGeneric<Dictionary<byte, int>> GetUnsolicitedStats();

        [OperationContract]
	    SvcOutputBase ReinstallDsf();

	    //[OperationContract]
	    //SvcOutputBase ResetServicesState();

	    [OperationContract]
	    CheckMongoStatusOutput CheckMongoServiceStatus();
    }
}
