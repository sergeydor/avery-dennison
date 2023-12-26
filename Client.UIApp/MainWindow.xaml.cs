using Client.Server.Communication.ServiceContracts.Dtos.Input;
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

namespace Client.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        StringBuilder _bldTest = new StringBuilder();

        private void _btPing_Click(object sender, RoutedEventArgs e)
        {/*
            try
            {
                var input = new PingSvcInput() { InputArrayParam = new List<int>() { 5, 6, 7 }, InputDateParam = DateTime.Now.AddDays(-1) };
                _bldTest.AppendLine("Input(PingSvcInput) " + input);
                _bldTest.AppendLine("Call Ping...");
                var client = new AverySvcClient();
                SvcOutputGeneric<List<PingTestDomainObject>> response = client.Ping(input);
                _bldTest.AppendLine("Output(SvcOutputGeneric<T>) " + response);
            }
            catch(Exception ex)
            {
                _bldTest.AppendLine(ex.GetType() + ex.Message);
            }            
            _tbLog.Text = _bldTest.ToString();
            _bldTest.AppendLine();*/
        }

	    private void _setDateTime_OnClick(object sender, RoutedEventArgs e)
	    {/*
			try
			{
				var date = _mainCalendar.SelectedDate ?? DateTime.Now;
				var dateTime = new LaneDateTime
				{
					Lane = 0x1,
					Year = (byte)date.Year,
					Month = (byte)date.Month,
					Day = (byte)date.Day,
					Hour = (byte)date.Hour,
					Minute = (byte)date.Minute,
					Second = (byte)date.Second
				};
				var input = new SvcInputGeneric<LaneDateTime> {Input = dateTime};
				_bldTest.AppendLine("Set Date Time 0x08...");
				var client = new AverySvcClient();
			    var response = client.SetDateTime(input);
			    _bldTest.AppendLine("Output (set date time) " + response);
		    }
		    catch (Exception ex)
		    {
			    _bldTest.AppendLine(ex.GetType() + ex.Message);
		    }
		    _tbLog.Text = _bldTest.ToString();
		    _bldTest.AppendLine();*/
	    }

	    private void _getDateTime_OnClick(object sender, RoutedEventArgs e)
	    {/*
		    try
		    {
			    _bldTest.AppendLine("Get Date Time 0x09...");
			    var client = new AverySvcClient();
			    var response = client.GetDateTime();
			    var output = response.Output;
				var dateTime = new DateTime(CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(output.Year), output.Month, output.Day, output.Hour, output.Minute, output.Second);
			    _bldTest.AppendLine("Output (get date time) " + dateTime.ToString());
		    }
		    catch (Exception ex)
		    {
				_bldTest.AppendLine(ex.GetType() + ex.Message);
			}
		    _tbLog.Text = _bldTest.ToString();
		    _bldTest.AppendLine();*/
	    }

	    private void _initialize_OnClick(object sender, RoutedEventArgs e)
	    {
		    try
		    {
			    _bldTest.AppendLine("Devices initialization...");
				var client = new AverySvcClient();
			    bool success = client.Initialize().Output;
			    if (!success)
			    {
				    _bldTest.AppendLine("[ERROR] Something went wrong while initializing (STEP 1) devices...");
				    _tbLog.Text = _bldTest.ToString();
					return;
			    }
			    
				List<DeviceIdentity> devices = client.GetInitializedDevices().Output;

			    for (int i = 0; i < devices.Count; i++)
			    {
				    var device = devices[i];
				    device.Lane = i;
			    }

			    success = client.InitializeStep2(new SvcInputGeneric<List<DeviceIdentity>> {Input = devices}).Output;

				if (!success)
				{
					_bldTest.AppendLine("[ERROR] Something went wrong while initializing (STEP 2) devices...");
					_tbLog.Text = _bldTest.ToString();
					return;
				}

				_bldTest.AppendLine("Devices are initialized successfully");
			}
		    catch (Exception ex)
		    {
			    _bldTest.AppendLine(ex.GetType() + ex.Message);
		    }
			_tbLog.Text = _bldTest.ToString();
			_bldTest.AppendLine();
		}

	    private void _deinitialize_OnClick(object sender, RoutedEventArgs e)
	    {
			try
			{
				_bldTest.AppendLine("Devices deinitialization...");
				var client = new AverySvcClient();
				client.Deinitialize();
			}
			catch (Exception ex)
			{
				_bldTest.AppendLine(ex.GetType() + ex.Message);
			}
			_tbLog.Text = _bldTest.ToString();
			_bldTest.AppendLine();
		}

	    private void _createVirtDev_OnClick(object sender, RoutedEventArgs e)
	    {
		    try
		    {
				var client = new AverySvcClient();
			    client.CreateVirtualDevices();
		    }
		    catch (Exception)
		    {
			    
			    throw;
		    }
	    }
    }
}
