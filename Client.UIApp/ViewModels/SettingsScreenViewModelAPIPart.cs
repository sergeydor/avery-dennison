using Client.Server.Communication.RemoteServices.Dtos.Input;
using Client.UIApp.UIElements.BasicConfigurationView;
using Client.UIApp.ViewModels;
using Common.Domain.DeviceResults;
using Common.Domain.DeviceResults.TestModuleCommands;
using Common.Domain.ExtendedTestSetupCommands;
using Common.Domain.GSTCommands;
using Common.Domain.TestModuleCommands;
using Common.Domain.TestSetupCommands;
using Common.Enums;
using Common.Enums.GSTCommands;
using Common.Services.Input;
using Common.Services.Output;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Client.UIApp.UIElements
{
    public partial class SettingsScreenViewModel
    {
        public event EventHandler SetAllSettingsEvent;
        public event EventHandler GetAllSettingsEvent;

        private async void ExecuteGetCommandsCommandHandler()
        {
            try
            {
                IsBusy = true;

                // executing get lane date for all devices

                var dateTimeByDeviceDict = new Dictionary<DeviceIdentityViewModel, LaneDateTime>();

                foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Devices)
                {
                    string msg = $"Getting date time (0x09) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<EntityDeviceResult<LaneDateTime>>>(client =>
                    {
                        return client.GetDateTime(new SvcInputGeneric<string>() { Input = deviceVm.DeviceIdentity.MacAddress });
                    });
                    AddSvcCallLog(msg, getSvcRes);

                    if (getSvcRes == null || !getSvcRes.IsOk || getSvcRes.Output.Status != StatusCode.OK)
                    {
                        dateTimeByDeviceDict.Add(deviceVm, new LaneDateTime()); // an error occured and it's written in logview
                        continue;
                    }
                    else
                    {
                        dateTimeByDeviceDict.Add(deviceVm, getSvcRes.Output.Entity);
                    }
                }
                this.BasicConfigurationViewModel.UpdateLaneDateTimeSource(dateTimeByDeviceDict);

                // executing get version for all devices

                var versionByDeviceDict = new Dictionary<DeviceIdentityViewModel, VersionResult>();

                foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Devices)
                {
                    string msg = $"Getting version (0x01) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<EntityDeviceResult<VersionResult>>>(client =>
                    {
                        return client.GetVersion(new SvcInputGeneric<string>() { Input = deviceVm.DeviceIdentity.MacAddress });
                    });
                    AddSvcCallLog(msg, getSvcRes);

                    if (getSvcRes == null || !getSvcRes.IsOk || getSvcRes.Output.Status != StatusCode.OK)
                    {
                        versionByDeviceDict.Add(deviceVm, new VersionResult()); // an error occured and it's written in logview
                        continue;
                    }
                    else
                    {
                        versionByDeviceDict.Add(deviceVm, getSvcRes.Output.Entity);
                    }
                }
                this.BasicConfigurationViewModel.UpdateVersionsSource(versionByDeviceDict);

                // executing get last fault for all devices

                var lastFaultByDeviceDict = new Dictionary<DeviceIdentityViewModel, LastFaultResult>();

                foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Devices)
                {
                    string msg = $"Getting last fault (0x03) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<EntityDeviceResult<LastFaultResult>>>(client =>
                    {
                        return client.GetLastFault(new SvcInputGeneric<string>() { Input = deviceVm.DeviceIdentity.MacAddress });
                    });
                    AddSvcCallLog(msg, getSvcRes);

                    if (getSvcRes == null || !getSvcRes.IsOk || getSvcRes.Output.Status != StatusCode.OK)
                    {
                        lastFaultByDeviceDict.Add(deviceVm, new LastFaultResult()); // an error occured and it's written in logview
                        continue;
                    }
                    else
                    {
                        lastFaultByDeviceDict.Add(deviceVm, getSvcRes.Output.Entity);
                    }
                }
                this.BasicConfigurationViewModel.UpdateLastFaultsSource(lastFaultByDeviceDict);

                this.SelectedDeviceInTestModuleCofiguration = null;

                // getting Test Module Cofiguration

                // executing Test Settings flags & test statistics for the first reader in collection
                {
                    DeviceIdentityViewModel firstReader = DevicesContext.Instance.Readers.First();
                    string msg = $"Executing Get Test Settings (0x30) to obtain TestType flags, considered the first reader in collection {firstReader.ToString()}...";

                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<EntityDeviceResult<TestSettings>>>(client =>
                    {
                        return client.GetTestSettings(new SvcInputGeneric<string>() { Input = firstReader.DeviceIdentity.MacAddress });
                    });
                    AddSvcCallLog(msg, getSvcRes);

                    if (getSvcRes == null || !getSvcRes.IsOk || getSvcRes.Output.Status != StatusCode.OK)
                    {
                        this.TestModeFlagsViewModel.UpdateViewModel(Test10Type.NotSet);
                    }
                    else
                    {
                        this.TestModeFlagsViewModel.UpdateViewModel(getSvcRes.Output.Entity.TestType);
                    }
                }

                {
                    DeviceIdentityViewModel firstReader = DevicesContext.Instance.Readers.First();
                    string msg = $"Executing Get Test Statistics (0x35), considered the first reader in collection {firstReader.ToString()}...";

                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<EntityDeviceResult<TestStatistics>>>(client =>
                    {
                        return client.GetTestStatistics(new SvcInputGeneric<string>() { Input = firstReader.DeviceIdentity.MacAddress });
                    });
                    AddSvcCallLog(msg, getSvcRes);

                    if (getSvcRes == null || !getSvcRes.IsOk || getSvcRes.Output.Status != StatusCode.OK)
                    {
                        this.SetStatisticsViewModel.UpdateViewModel(new TestStatistics());
                    }
                    else
                    {
                        this.SetStatisticsViewModel.UpdateViewModel(getSvcRes.Output.Entity);
                    }
                }

                foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                {
                    string msg = $"Executing Get TID Test Settings (0x39) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<EntityDeviceResult<TIDTestSettings>>>(client =>
                    {
                        return client.GetTIDTestSettings(new SvcInputGeneric<string>() { Input = deviceVm.DeviceIdentity.MacAddress });
                    });
                    AddSvcCallLog(msg, getSvcRes);
                    SetValueToDict(_tidSettingsDict, getSvcRes, deviceVm);
                }

                foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                {
                    string msg = $"Executing Get Tag ID Filter Settings (0x3A) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<EntityDeviceResult<TagIDFilterSettings>>>(client =>
                    {
                        return client.GetTagIdFilterSettings(new SvcInputGeneric<string>() { Input = deviceVm.DeviceIdentity.MacAddress });
                    });
                    AddSvcCallLog(msg, getSvcRes);
                    SetValueToDict(_tagIDFilterSettingsDict, getSvcRes, deviceVm);
                }

                foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                {
                    string msg = $"Executing Get Sensitivity Test Settings (0x38) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<EntityDeviceResult<SensitivityTestSettings>>>(client =>
                    {
                        return client.GetSensitivityTestSettings(new SvcInputGeneric<string>() { Input = deviceVm.DeviceIdentity.MacAddress });
                    });
                    AddSvcCallLog(msg, getSvcRes);
                    SetValueToDict(_sensSettingsDict, getSvcRes, deviceVm);
                }

                foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                {
                    string msg = $"Executing Get Test Settings (0x30) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<EntityDeviceResult<TestSettings>>>(client =>
                    {
                        return client.GetTestSettings(new SvcInputGeneric<string>() { Input = deviceVm.DeviceIdentity.MacAddress });
                    });
                    AddSvcCallLog(msg, getSvcRes);
                    SetValueToDict(_confRWSettingsDict, getSvcRes, deviceVm);
                }

                // now  getting hardware configuration

                foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                {
                    string msg = $"Executing Get Tester Settings (0x34) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<EntityDeviceResult<TesterSettings>>>(client =>
                    {
                        return client.GetTesterSettings(new SvcInputGeneric<string>() { Input = deviceVm.DeviceIdentity.MacAddress });
                    });
                    AddSvcCallLog(msg, getSvcRes);
                    SetValueToDict(_readerSettingsDict, getSvcRes, deviceVm);
                }

                foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                {
                    string msg = $"Executing Get Antenna Settings (0x37) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<EntityDeviceResult<AntennaSettings>>>(client =>
                    {
                        return client.GetAntennaSettings(new SvcInputGeneric<string>() { Input = deviceVm.DeviceIdentity.MacAddress });
                    });
                    AddSvcCallLog(msg, getSvcRes);
                    SetValueToDict(_antennaSettingsDict, getSvcRes, deviceVm);
                }

                foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                {
                    string msg = $"Executing Get Marker Settings (0x31) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<EntityDeviceResult<MarkerSettings>>>(client =>
                    {
                        return client.GetMarkerSettings(new SvcInputGeneric<string>() { Input = deviceVm.DeviceIdentity.MacAddress });
                    });
                    AddSvcCallLog(msg, getSvcRes);
                    SetValueToDict(_markerSettingsDict, getSvcRes, deviceVm);
                }

                foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                {
                    string msg = $"Executing Get Punch Settings (0x40) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<EntityDeviceResult<PunchSettings>>>(client =>
                    {
                        return client.GetPunchSettings(new SvcInputGeneric<string>() { Input = deviceVm.DeviceIdentity.MacAddress });
                    });
                    AddSvcCallLog(msg, getSvcRes);
                    SetValueToDict(_puncherSettingsDict, getSvcRes, deviceVm);
                }

                foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                {
                    string msg = $"Executing Get Trigger Input Settings (0x32) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<EntityDeviceResult<TriggerInputSettings>>>(client =>
                    {
                        return client.GetTriggerInputSettings(new SvcInputGeneric<string>() { Input = deviceVm.DeviceIdentity.MacAddress });
                    });
                    AddSvcCallLog(msg, getSvcRes);
                    SetValueToDict(_triggerInputSettingsDict, getSvcRes, deviceVm);
                }

                foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                {
                    string msg = $"Executing Get Aux In Settings (0x3C) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<EntityDeviceResult<AuxSettings>>>(client =>
                    {
                        return client.GetAuxInSettings(new SvcInputGeneric<string>() { Input = deviceVm.DeviceIdentity.MacAddress });
                    });
                    AddSvcCallLog(msg, getSvcRes);
                    SetValueToDict(_auxInSettingsDict, getSvcRes, deviceVm);
                }

                foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                {
                    string msg = $"Executing Get Encoder In Settings (0x3D) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<EntityDeviceResult<EncoderSettings>>>(client =>
                    {
                        return client.GetEncoderSettings(new SvcInputGeneric<string>() { Input = deviceVm.DeviceIdentity.MacAddress });
                    });
                    AddSvcCallLog(msg, getSvcRes);
                    SetValueToDict(_encoderSettingsDict, getSvcRes, deviceVm);
                }

                // getting GPIO settings
                {
                    DeviceIdentityViewModel gpio = DevicesContext.Instance.Devices.FirstOrDefault(d => d.DeviceIdentity.DeviceType == HighSpeedTestDeviceType.GPIO);
                    if (gpio != null)
                    {
                        string msg = $"Executing Get High Speed GPIO Test Mode Timer (0x93) for {gpio.ToString()}...";

                        var getSvcRes = await SafeCallAsync<SvcOutputGeneric<EntityDeviceResult<HighSpeedTestModeTimer>>>(client =>
                        {
                            return client.GetHighSpeedTestModeTimer(new SvcInputGeneric<string>() { Input = gpio.DeviceIdentity.MacAddress });
                        });
                        AddSvcCallLog(msg, getSvcRes);

                        if (getSvcRes == null || !getSvcRes.IsOk || getSvcRes.Output.Status != StatusCode.OK)
                        {
                            this.GPIOSettingsViewModel.UpdateViewModel(new HighSpeedTestModeTimer());
                        }
                        else
                        {
                            this.GPIOSettingsViewModel.UpdateViewModel(getSvcRes.Output.Entity);
                        }
                    }
                }

                if (GetAllSettingsEvent != null)
                {
                    GetAllSettingsEvent(this, null);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        private bool SetValueToDict<TEntity>(Dictionary<DeviceIdentityViewModel, TEntity> dict, SvcOutputGeneric<EntityDeviceResult<TEntity>> svcOutput, DeviceIdentityViewModel deviceVm) where TEntity : new()
        {
            if (svcOutput == null || !svcOutput.IsOk || svcOutput.Output.Status != StatusCode.OK)
            {
                dict[deviceVm] = new TEntity();
                return false;
            }
            else
            {
                dict[deviceVm] = svcOutput.Output.Entity;
            }
            return true;
        }

        private async void SaveAllSettingsCommandHandler()
        {
            try
            {
                IsBusy = true;

                // executing set lane date for all devices

                var dateTimeByDeviceDict = new Dictionary<DeviceIdentityViewModel, LaneDateTime>();

              //  if (BasicConfigurationViewModel.SelectedDevice != null)
                {
                    foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Devices)
                    {
                        LaneDateTime entityToSet = GetValueFromDict(BasicConfigurationViewModel.DateTimeByDeviceDict, BasicConfigurationViewModel.SelectedDevice, deviceVm);
                        if (deviceVm.DeviceIdentity.DeviceType == HighSpeedTestDeviceType.Reader)
                        {
                            entityToSet.Lane = Convert.ToByte(deviceVm.DeviceIdentity.Lane);
                        }
                        string msg = $"Set date time (0x08) for device {deviceVm.ToString()}...";
                        base.LogViewModel.AppendMessage(msg);
                        var getSvcRes = await SafeCallAsync<SvcOutputGeneric<GeneralDeviceResult>>(client =>
                        {
                            return client.SetDateTime(new SetDataToDeviceSvcInput<LaneDateTime>() { DeviceMacAddr = deviceVm.DeviceIdentity.MacAddress, Input = entityToSet });
                        });
                        AddSvcCallLog(msg, getSvcRes);
                    }
                }

              //  if (SelectedDeviceInTestModuleCofiguration != null)
                {
                    foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                    {
                        TIDTestSettings set = GetValueFromDict(_tidSettingsDict, SelectedDeviceInTestModuleCofiguration, deviceVm);
                        string msg = $"Set TID settings (0x19) for device {deviceVm.ToString()}...";
                        base.LogViewModel.AppendMessage(msg);
                        var getSvcRes = await SafeCallAsync<SvcOutputGeneric<GeneralDeviceResult>>(client =>
                        {
                            return client.SetTIDTestSettings(new SetDataToDeviceSvcInput<TIDTestSettings>() { DeviceMacAddr = deviceVm.DeviceIdentity.MacAddress, Input = set });
                        });
                        AddSvcCallLog(msg, getSvcRes);
                    }

                    foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                    {
                        TagIDFilterSettings set = GetValueFromDict(_tagIDFilterSettingsDict, SelectedDeviceInTestModuleCofiguration, deviceVm);
                        string msg = $"Set Tag ID Filter settings (0x1A) for device {deviceVm.ToString()}...";
                        base.LogViewModel.AppendMessage(msg);
                        var getSvcRes = await SafeCallAsync<SvcOutputGeneric<GeneralDeviceResult>>(client =>
                        {
                            return client.SetTagIdFilterSettings(new SetDataToDeviceSvcInput<TagIDFilterSettings>() { DeviceMacAddr = deviceVm.DeviceIdentity.MacAddress, Input = set });
                        });
                        AddSvcCallLog(msg, getSvcRes);
                    }

                    foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                    {
                        SensitivityTestSettings set = GetValueFromDict(_sensSettingsDict, SelectedDeviceInTestModuleCofiguration, deviceVm);
                        string msg = $"Set Sensitivity Filter settings (0x18) for device {deviceVm.ToString()}...";
                        base.LogViewModel.AppendMessage(msg);
                        var getSvcRes = await SafeCallAsync<SvcOutputGeneric<GeneralDeviceResult>>(client =>
                        {
                            return client.SetSensitivityTestSettings(new SetDataToDeviceSvcInput<SensitivityTestSettings>() { DeviceMacAddr = deviceVm.DeviceIdentity.MacAddress, Input = set });
                        });
                        AddSvcCallLog(msg, getSvcRes);
                    }

                    foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                    {
                        TestSettings set = GetValueFromDict(_confRWSettingsDict, SelectedDeviceInTestModuleCofiguration, deviceVm);
                        set.TestType = this.TestModeFlagsViewModel.TargetFlagsEnum;
                        string msg = $"Set test settings (0x10) for device {deviceVm.ToString()}...";
                        base.LogViewModel.AppendMessage(msg);
                        var getSvcRes = await SafeCallAsync<SvcOutputGeneric<GeneralDeviceResult>>(client =>
                        {
                            return client.SetTestSettings(new SetDataToDeviceSvcInput<TestSettings>() { DeviceMacAddr = deviceVm.DeviceIdentity.MacAddress, Input = set });
                        });
                        AddSvcCallLog(msg, getSvcRes);
                    }
                }

                // test statistics copying to all readers
                foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                {
                    TestStatistics stat = this.SetStatisticsViewModel.TestStatistics;
                    string msg = $"Set test statistics (0x15) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<GeneralDeviceResult>>(client =>
                    {
                        return client.SetTestStatistics(new SetDataToDeviceSvcInput<TestStatistics>() { DeviceMacAddr = deviceVm.DeviceIdentity.MacAddress, Input = stat });
                    });
                    AddSvcCallLog(msg, getSvcRes);
                }

               // if (SelectedDeviceInHardwareCofiguration != null)
                {
                    foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                    {
                        TesterSettings set = GetValueFromDict(_readerSettingsDict, SelectedDeviceInHardwareCofiguration, deviceVm);
                        string msg = $"Set Tester Settings (0x14) for device {deviceVm.ToString()}...";
                        base.LogViewModel.AppendMessage(msg);
                        var getSvcRes = await SafeCallAsync<SvcOutputGeneric<GeneralDeviceResult>>(client =>
                        {
                            return client.SetReaderSettings(new SetDataToDeviceSvcInput<TesterSettings>() { DeviceMacAddr = deviceVm.DeviceIdentity.MacAddress, Input = set });
                        });
                        AddSvcCallLog(msg, getSvcRes);
                    }

                    foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                    {
                        AntennaSettings set = GetValueFromDict(_antennaSettingsDict, SelectedDeviceInHardwareCofiguration, deviceVm);
                        string msg = $"Set Antenna Settings (0x17) for device {deviceVm.ToString()}...";
                        base.LogViewModel.AppendMessage(msg);
                        var getSvcRes = await SafeCallAsync<SvcOutputGeneric<GeneralDeviceResult>>(client =>
                        {
                            return client.SetAntennaSettings(new SetDataToDeviceSvcInput<AntennaSettings>() { DeviceMacAddr = deviceVm.DeviceIdentity.MacAddress, Input = set });
                        });
                        AddSvcCallLog(msg, getSvcRes);
                    }

                    foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                    {
                        MarkerSettings set = GetValueFromDict(_markerSettingsDict, SelectedDeviceInHardwareCofiguration, deviceVm);
                        string msg = $"Set Marker Settings (0x11) for device {deviceVm.ToString()}...";
                        base.LogViewModel.AppendMessage(msg);
                        var getSvcRes = await SafeCallAsync<SvcOutputGeneric<GeneralDeviceResult>>(client =>
                        {
                            return client.SetMarkerSettings(new SetDataToDeviceSvcInput<MarkerSettings>() { DeviceMacAddr = deviceVm.DeviceIdentity.MacAddress, Input = set });
                        });
                        AddSvcCallLog(msg, getSvcRes);
                    }

                    foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                    {
                        PunchSettings set = GetValueFromDict(_puncherSettingsDict, SelectedDeviceInHardwareCofiguration, deviceVm);
                        string msg = $"Set Punch Settings (0x20) for device {deviceVm.ToString()}...";
                        base.LogViewModel.AppendMessage(msg);
                        var getSvcRes = await SafeCallAsync<SvcOutputGeneric<GeneralDeviceResult>>(client =>
                        {
                            return client.SetPuncherSettings(new SetDataToDeviceSvcInput<PunchSettings>() { DeviceMacAddr = deviceVm.DeviceIdentity.MacAddress, Input = set });
                        });
                        AddSvcCallLog(msg, getSvcRes);
                    }

                    foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                    {
                        TriggerInputSettings set = GetValueFromDict(_triggerInputSettingsDict, SelectedDeviceInHardwareCofiguration, deviceVm);
                        string msg = $"Set Trigger In Settings (0x12) for device {deviceVm.ToString()}...";
                        base.LogViewModel.AppendMessage(msg);
                        var getSvcRes = await SafeCallAsync<SvcOutputGeneric<GeneralDeviceResult>>(client =>
                        {
                            return client.SetTriggerInputSettings(new SetDataToDeviceSvcInput<TriggerInputSettings>() { DeviceMacAddr = deviceVm.DeviceIdentity.MacAddress, Input = set });
                        });
                        AddSvcCallLog(msg, getSvcRes);
                    }

                    foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                    {
                        AuxSettings set = GetValueFromDict(_auxInSettingsDict, SelectedDeviceInHardwareCofiguration, deviceVm);
                        string msg = $"Set Aux In Settings (0x1C) for device {deviceVm.ToString()}...";
                        base.LogViewModel.AppendMessage(msg);
                        var getSvcRes = await SafeCallAsync<SvcOutputGeneric<GeneralDeviceResult>>(client =>
                        {
                            return client.SetAuxSettings(new SetDataToDeviceSvcInput<AuxSettings>() { DeviceMacAddr = deviceVm.DeviceIdentity.MacAddress, Input = set });
                        });
                        AddSvcCallLog(msg, getSvcRes);
                    }

                    foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Readers)
                    {
                        EncoderSettings set = GetValueFromDict(_encoderSettingsDict, SelectedDeviceInHardwareCofiguration, deviceVm);
                        string msg = $"Set Encoder In Settings (0x1D) for device {deviceVm.ToString()}...";
                        base.LogViewModel.AppendMessage(msg);
                        var getSvcRes = await SafeCallAsync<SvcOutputGeneric<GeneralDeviceResult>>(client =>
                        {
                            return client.SetEncoderSettings(new SetDataToDeviceSvcInput<EncoderSettings>() { DeviceMacAddr = deviceVm.DeviceIdentity.MacAddress, Input = set });
                        });
                        AddSvcCallLog(msg, getSvcRes);
                    }
                }

                if(DevicesContext.Instance.GPIODeviceVm != null)
                {
                    HighSpeedTestModeTimer set = this.GPIOSettingsViewModel.HighSpeedTestModeTimer;
                    set.DeviceType = HighSpeedTestDeviceType.GPIO;
                    DeviceIdentityViewModel gpioDeviceVm = DevicesContext.Instance.GPIODeviceVm;
                    string msg = $"Set High Speed GPIO Test Mode Timer (0x92) for device {gpioDeviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<GeneralDeviceResult>>(client =>
                    {
                        return client.SetHighSpeedTestModeTimer(new SetDataToDeviceSvcInput<HighSpeedTestModeTimer>() { DeviceMacAddr = gpioDeviceVm.DeviceIdentity.MacAddress,  Input = set });
                    });
                    AddSvcCallLog(msg, getSvcRes);
                }

                if(SetAllSettingsEvent != null)
                {
                    SetAllSettingsEvent(this, null);
                }

                UserStepContext.Instance.CompleteSetSettingsStep();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private TEntity GetValueFromDict<TEntity>(Dictionary<DeviceIdentityViewModel, TEntity> dict, DeviceIdentityViewModel selectedDevice, DeviceIdentityViewModel currentDevice) where TEntity : class
        {
            if(selectedDevice == null)
            {
                return dict.Keys.ToList().Contains(DevicesContext.Instance.AllReadersItem) ?
                    dict[DevicesContext.Instance.AllReadersItem] :
                    dict.Keys.ToList().Contains(DevicesContext.Instance.AllDevicesItem) ?
                    dict[DevicesContext.Instance.AllDevicesItem] : null;
            }
            if (selectedDevice.Type == DeviceIdentityViewModelType.AllReadersItem)
            {
                return dict[DevicesContext.Instance.AllReadersItem];
            }
            if (selectedDevice.Type == DeviceIdentityViewModelType.AllDevicesItem)
            {
                return dict[DevicesContext.Instance.AllDevicesItem];
            }
            else
            {
                return dict[currentDevice];
            }
        }

        private async void ClearLastFaultCommandHandler(DeviceIdentityViewModel selectedDevice)
        {
            try
            {
                IsBusy = true;
                List<DeviceIdentityViewModel> deviceVms = new List<DeviceIdentityViewModel>();
                if (selectedDevice == DevicesContext.Instance.AllDevicesItem)
                {
                    deviceVms.AddRange(DevicesContext.Instance.Devices);
                }
                else
                {
                    deviceVms.Add(selectedDevice);
                }

                foreach (DeviceIdentityViewModel deviceVm in deviceVms)
                {
                    string msg = $"Executing Clear Last Fault (0x06) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<GeneralDeviceResult>>(client =>
                    {
                        return client.ClearLastFault(new SvcInputGeneric<string>() { Input = deviceVm.DeviceIdentity.MacAddress });
                    });
                    AddSvcCallLog(msg, getSvcRes);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        //private bool ClearLastFaultCommandHandlerCanExecute(DeviceIdentityViewModel selectedDevice)
        //{
        //    if (selectedDevice == null)
        //        return false;
        //    return true;
        //}

        private async void PingCommandHandler(DeviceIdentityViewModel selectedDevice)
        {
            try
            {
                IsBusy = true;
                List<DeviceIdentityViewModel> deviceVms = new List<DeviceIdentityViewModel>();
                if (selectedDevice == DevicesContext.Instance.AllDevicesItem)
                {
                    deviceVms.AddRange(DevicesContext.Instance.Devices);
                }
                else
                {
                    deviceVms.Add(selectedDevice);
                }

                foreach (DeviceIdentityViewModel deviceVm in deviceVms)
                {
                    string msg = $"Executing Ping (0x00) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<GeneralDeviceResult>>(client =>
                    {
                        return client.Ping(new SvcInputGeneric<string>() { Input = deviceVm.DeviceIdentity.MacAddress });
                    });
                    AddSvcCallLog(msg, getSvcRes);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        //private bool PingCommandHandlerCanExecute(DeviceIdentityViewModel selectedDevice)
        //{
        //    if (selectedDevice == null)
        //        return false;
        //    return true;
        //}

        private async void ResetCommandHandler(DeviceIdentityViewModel selectedDevice)
        {
            try
            {
                IsBusy = true;
                List<DeviceIdentityViewModel> deviceVms = new List<DeviceIdentityViewModel>();
                if (selectedDevice == DevicesContext.Instance.AllDevicesItem)
                {
                    deviceVms.AddRange(DevicesContext.Instance.Devices);
                }
                else
                {
                    deviceVms.Add(selectedDevice);
                }

                foreach (DeviceIdentityViewModel deviceVm in deviceVms)
                {
                    string msg = $"Executing Reset (0x02) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcRes = await SafeCallAsync<SvcOutputGeneric<GeneralDeviceResult>>(client =>
                    {
                        return client.Reset(new SetDataToDeviceSvcInput<ResetSettings>()
                        {
                            DeviceMacAddr = deviceVm.DeviceIdentity.MacAddress,
                            Input = new ResetSettings() { Type = BasicConfigurationViewModel.InputResetType }
                        });
                    });
                    AddSvcCallLog(msg, getSvcRes);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        //private bool ResetCommandHandlerCanExecute(DeviceIdentityViewModel selectedDevice)
        //{
        //    if (selectedDevice == null)
        //        return false;
        //    return true;
        //}
    }
}
