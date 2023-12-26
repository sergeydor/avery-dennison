using Client.UIApp.RemoteServices.Clients;
using Common.Services.Input;
using Common.Services.Output;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common.Domain;
using Common.Domain.Device;
using Common.Domain.TestModuleCommands;
using Client.UIApp.ViewModels;
using System.Collections.ObjectModel;
using Common.Domain.DeviceResults;
using Client.Server.Communication.RemoteServices.Dtos.Input;
using Client.UIApp.UIElements.LogView;
using Common.Enums;

namespace Client.Presentation
{
    /// <summary>
    /// Test Interaction logic with devices
    /// </summary>
    public partial class TempSettingsWindow : Window
    {
        private LogViewModel _logVm;

        public TempSettingsWindow(LogViewModel logVm)
        {
            InitializeComponent();
            _cbDevice.SelectionChanged += _cbDevice_SelectionChanged;
            _btPing.IsEnabled = _btSetDateTime.IsEnabled = _btGetDateTime.IsEnabled = false;

            _vLog.DataContext = _logVm = logVm;  //new LogViewModel();
        }

        private void _cbDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool deviceSelected = _cbDevice.SelectedItem != null;
            _btPing.IsEnabled = deviceSelected;
            _btSetDateTime.IsEnabled = deviceSelected;
            _btGetDateTime.IsEnabled = deviceSelected;
        }

        public void Initialize(List<DeviceIdentityViewModel> devicesViewModels)
        {
            _cbDevice.Items.Clear();
            foreach(DeviceIdentityViewModel device in devicesViewModels)
            {
                _cbDevice.Items.Add(device);
            }
            _cbDevice.SelectedIndex = 0;
        }
                
        private void _btPing_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var deviceVm = _cbDevice.SelectedItem as DeviceIdentityViewModel;
                if (deviceVm == null)
                {
                    System.Diagnostics.Debug.Assert(false);
                    return;
                }

                var input = new SvcInputGeneric<string>() { Input = deviceVm.DeviceIdentity.MacAddress };                
                _logVm.AppendMessage("Call Ping to Device - " + deviceVm.ToString() + "...");
                var client = new AverySvcClient();
                SvcOutputGeneric<GeneralDeviceResult> response = client.Ping(input);
                _logVm.AppendMessage("Call Ping to Device - " + deviceVm.ToString() + "..." + response.Output.Status);
            }
            catch(Exception ex)
            {
                _logVm.AppendWarning(ex.GetType() + ex.Message);
            }            
        }

	    private void _setDateTime_OnClick(object sender, RoutedEventArgs e)
	    {
			try
			{
                var deviceVm = _cbDevice.SelectedItem as DeviceIdentityViewModel;
                if (deviceVm == null)
                {
                    System.Diagnostics.Debug.Assert(false);
                    return;
                }

                var date = _mainCalendar.SelectedDate ?? DateTime.Now;
				var dateTime = new LaneDateTime
				{
					Lane = (byte)deviceVm.DeviceIdentity.Lane,
					Year = (byte)(date.Year - 2000),
					Month = (byte)date.Month,
					Day = (byte)date.Day,
					Hour = (byte)date.Hour,
					Minute = (byte)date.Minute,
					Second = (byte)date.Second
				};
				var input = new SetDataToDeviceSvcInput<LaneDateTime> { Input = dateTime, DeviceMacAddr = deviceVm.DeviceIdentity.MacAddress };
                _logVm.AppendMessage("Call SetDateTime_0x08 to Device - " + deviceVm.ToString() + "...");
                var client = new AverySvcClient();
			    var response = client.SetDateTime(input);
                _logVm.AppendMessage("Call SetDateTime_0x08 to Device - " + deviceVm.ToString() + "..." + response.Output.Status);
            }
		    catch (Exception ex)
		    {
                _logVm.AppendWarning(ex.GetType() + ex.Message);
		    }
	    }

	    private void _getDateTime_OnClick(object sender, RoutedEventArgs e)
	    {
		    try
		    {
                var deviceVm = _cbDevice.SelectedItem as DeviceIdentityViewModel;
                if (deviceVm == null)
                {
                    System.Diagnostics.Debug.Assert(false);
                    return;
                }

                _logVm.AppendMessage("Call GetDateTime to Device - " + deviceVm.ToString() + "...");
                var client = new AverySvcClient();
                SvcInputGeneric<string> input = new SvcInputGeneric<string>() { Input = deviceVm.DeviceIdentity.MacAddress };
                var response = client.GetDateTime(input);
			    var output = response.Output?.Entity ?? new LaneDateTime();
				var dateTime = new DateTime(CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(output.Year), output.Month, output.Day, output.Hour, output.Minute, output.Second);
                _logVm.AppendMessage("Call GetDateTime to Device - " + deviceVm.ToString() + "..." + response.Output.Status);
            }
		    catch (Exception ex)
		    {
                _logVm.AppendWarning(ex.GetType() + ex.Message);
			}
	    }

		private void _reinstallDsf_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var client = new AverySvcClient();
				client.ReinstallDsf();
			}
			catch (Exception ex)
			{
				_logVm.AppendWarning(ex.GetType() + ex.Message);
			}

			this.Close();
		}

		private void _startHighSpeedTest_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var client = new AverySvcClient();
				var input = new StartTestingSvcInput {TestMode = TestMode.HSTest};
				client.StartTesting(input);
			}
			catch (Exception ex)
			{
				_logVm.AppendWarning(ex.GetType() + ex.Message);
			}
		}

	    private void _stopTesting_OnClick(object sender, RoutedEventArgs e)
	    {
		    try
		    {
				var client = new AverySvcClient();
				client.StopTesting();
			}
		    catch (Exception ex)
		    {
				_logVm.AppendWarning(ex.GetType() + ex.Message);
			}
	    }
    }
}
