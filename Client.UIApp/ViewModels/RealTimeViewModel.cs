using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.UIApp.UIElements.LogView;
using GalaSoft.MvvmLight.Command;
using Common.Services.Output;
using System.Windows;
using Client.Server.Communication.RemoteServices.Dtos.Output;
using Client.UIApp.UIElements.ConveyorSettingsView;
using Client.Server.Communication.Domain;
using Common.Services.Input;
using Common.Domain.DeviceResults;
using Client.Server.Communication.RemoteServices.Dtos.Input;
using Common.Domain.TestModuleCommands;

namespace Client.UIApp.ViewModels
{
    public class RealTimeViewModel : ViewModelCommonBase
    {
        public RelayCommand StartTestCommand { get; set; }
        public RelayCommand StopTestCommand { get; set; }

        //public string TestName { get; set; }

        public string Captions { get; set; } = string.Empty;
        public string Counts { get; set; } = string.Empty;
            

        public bool IsRunning
        {
            get
            {
                return DevicesContext.Instance.TestMode == Common.Enums.CommandsProcessingMode.Running;
            }
        }

        public ConveyorSettingsViewModel ConveyorSettingsViewModel { get; set; } = new ConveyorSettingsViewModel();

        public RealTimeViewModel(LogViewModel logViewModel) : base(logViewModel)
        {
            StartTestCommand = new RelayCommand(StartTestCommandHandler, StartTestCommandHandlerCanExecute);
            StopTestCommand = new RelayCommand(StopTestCommandHandler, StopTestCommandHandlerCanExecute);
            Task.Run(() => LoopUnsolicitedStats());
        }

        private async void LoopUnsolicitedStats()
        {
            while(true)
            {
                SvcOutputGeneric<Dictionary<byte, int>> res = SafeCall(Client => Client.GetUnsolicitedStats());

                if(!res.IsOk)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() => base.LogViewModel.AppendWarning("Error while receiving unsolicited statistics"));
                    return;
                }

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Captions = string.Empty;
                    Counts = string.Empty;
                    foreach (var kv in res.Output)
                    {
                        Captions += BitConverter.ToString(new byte[] { kv.Key }) + Environment.NewLine;
                        Counts += kv.Value.ToString() + Environment.NewLine;
                    }

                    if (string.IsNullOrEmpty(Captions))
                        Captions = "XX";
                    if (string.IsNullOrEmpty(Counts))
                        Counts = "YY";

                    RaisePropertyChanged(() => Captions);
                    RaisePropertyChanged(() => Counts);
                });
                await Task.Delay(1000);
            }
        }

        private async void StartTestCommandHandler()
        {
            try
            {
                IsBusy = true;

                string msg = "Applying conveyor settings...";
                base.LogViewModel.AppendMessage(msg);
                var getSvcRes = await SafeCallAsync(client =>
                {
                    return client.SetConveyorSettings(new SvcInputGeneric<UIConveyorSettings>() { Input = ConveyorSettingsViewModel.ConveyorSettings });
                });
                AddSvcCallLog(msg, getSvcRes);
                if (!getSvcRes.IsOk)
                {
                    base.LogViewModel.AppendWarning("Test has not been started due to error in 'Applying conveyor settings'");
                    return;
                }

                // reset timers

                foreach (DeviceIdentityViewModel deviceVm in DevicesContext.Instance.Devices)
                {
                    msg = $"Executing Timer Reset (0x02) for device {deviceVm.ToString()}...";
                    base.LogViewModel.AppendMessage(msg);
                    var getSvcResetRes = await SafeCallAsync<SvcOutputGeneric<GeneralDeviceResult>>(client =>
                    {
                        return client.Reset(new SetDataToDeviceSvcInput<ResetSettings>()
                        {
                            DeviceMacAddr = deviceVm.DeviceIdentity.MacAddress,
                            Input = new ResetSettings() { Type = Common.Enums.InputResetType.TimerReset }
                        });
                    });
                    AddSvcCallLog(msg, getSvcResetRes);

                    if (!getSvcResetRes.IsOk)
                    {
                        base.LogViewModel.AppendWarning("Test has not been started due to error processing Reset (0x02) command");
                        return;
                    }
                }

				//if (string.IsNullOrEmpty(DevicesContext.Instance.TestName))
				//{
    //                DevicesContext.Instance.TestName = $"Test - {DateTime.Now.ToString("G")}";					
				//}

				//DevicesContext.Instance.Title = string.Format(TitleFormat, DevicesContext.Instance.AppSessionName, TestName);

				msg = "Starting Test...";
                base.LogViewModel.AppendMessage(msg);
                StartTestingSvcOutput outp = await SafeCallAsync(
                    client => 
                        client.StartTesting(new Server.Communication.RemoteServices.Dtos.Input.StartTestingSvcInput() { TestMode = Common.Enums.TestMode.StandardTest, TestName = DevicesContext.Instance.TestName ?? $"Test - {DateTime.Now.ToString("G")}" })
                    );
                AddSvcCallLog(msg, outp);
                if (!outp.IsOk)
                {
                    return;
                }

                if (outp.AllStarted)
                {
                    LogViewModel.AppendMessages("All devices started succesfully", "Avery.Svc is moving to TestRunning state");
                    DevicesContext.Instance.TestMode = Common.Enums.CommandsProcessingMode.Running;
                    RaisePropertyChanged(() => IsRunning);

                    Captions = "XX";
                    Counts = "YY";
                }
                else
                {
                    StringBuilder bld = new StringBuilder();
                    if (outp.ReadersStartTestSVCErrors.Count > 0)
                    {
                        foreach (var kv in outp.ReadersStartTestSVCErrors)
                        {
                            bld.AppendLine("Reader, MacAddr: " + kv.Key);
                        }
                    }
                    if (outp.GpioHighSpeedSVCError != null)
                    {
                        bld.AppendLine("GPIO, MacAddr: " + DevicesContext.Instance.GPIODeviceVm.DeviceIdentity.MacAddress);
                    }
                    LogViewModel.AppendWarning("Some devices were not started correctly. Avery.Svc is NOT moving to TestRunning state. See logs for details.");
                    if (bld.Length > 0)
                    {
                        LogViewModel.AppendWarning("These devices results were not received (timeout error)" + Environment.NewLine + bld.ToString());
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool StartTestCommandHandlerCanExecute()
        {
            return true;
        }

        private async void StopTestCommandHandler()
        {
            try
            {
                IsBusy = true;

                StopTestingSvcOutput outp = await SafeCallAsync(client => client.StopTesting());
                AddSvcCallLog("", outp);

				//DevicesContext.Instance.Title = string.Format(TitleFormat, DevicesContext.Instance.AppSessionName, NotDefined);

                if (outp.AllStopped)
                {
                    LogViewModel.AppendMessages("All devices stopped succesfully", "Avery.Svc is moving to TestNotRunning state");
                    DevicesContext.Instance.TestMode = Common.Enums.CommandsProcessingMode.NotRunning;
                    RaisePropertyChanged(() => IsRunning);
                }
                else
                {
                    StringBuilder bld = new StringBuilder();
                    if (outp.ReadersStopTestSVCErrors.Count > 0)
                    {
                        foreach (var kv in outp.ReadersStopTestSVCErrors)
                        {
                            bld.AppendLine("Reader, MacAddr: " + kv.Key);
                        }
                    }
                    if (outp.GpioHighSpeedSVCError != null)
                    {
                        bld.AppendLine("GPIO, MacAddr: " + DevicesContext.Instance.GPIODeviceVm.DeviceIdentity.MacAddress);
                    }
                    LogViewModel.AppendWarning("Some devices were not stopped correctly. Avery.Svc is moving to TestNotRunning state anyway. See logs for details.");
                    if (bld.Length > 0)
                    {
                        LogViewModel.AppendWarning("Timeout error. Those devices results were not received: " + Environment.NewLine + bld.ToString());
                    }

                    DevicesContext.Instance.TestMode = Common.Enums.CommandsProcessingMode.NotRunning;
                    RaisePropertyChanged(() => IsRunning);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool StopTestCommandHandlerCanExecute()
        {
            return true;
        }

    }
}
