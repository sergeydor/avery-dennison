using Client.UIApp.RemoteServices.Clients;
using Client.UIApp.UIElements.LogView;
using Common.Domain.DeviceResults;
using Common.Infrastructure.ErrorHandling.Enums;
using Common.Infrastructure.ErrorHandling.Helpers;
using Common.Infrastructure.ErrorHandling.Output;
using Common.Services.Output;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Client.Presentation;

namespace Client.UIApp.ViewModels
{
    public class ViewModelCommonBase : ViewModelBase
    {
	    //protected const string TitleFormat = "Avery Dennison - App Session Name = {0} - Current Test Name = {1}";
	    //protected const string NotDefined = "Not Defined";

		public ViewModelCommonBase(LogViewModel logViewModel)
        {
            this.LogViewModel = logViewModel;
		}

        public virtual string Title { get; set; }
		public LogViewModel LogViewModel { get; set; }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged<bool>(() => IsBusy);
            }
        }

        protected void AddSvcCallLog(string msg, SvcOutputBase res)
        {
            if (res == null)
            {
                return;
            }
            if (res.IsOk)
            {
                LogViewModel.AppendMessage(msg + res.ToString());
            }
            else
            {
                LogViewModel.AppendWarning(msg + res.ToString());
            }
        }

        protected void AddSvcCallLog<TEntity>(string msg, SvcOutputGeneric<EntityDeviceResult<TEntity>> res)
        {
            if(res == null)
            {
                return;
            }
            if (res.IsOk && res.Output.Status == Common.Enums.StatusCode.OK)
            {
                LogViewModel.AppendMessage(msg + res.ToString());
            }
            else
            {
                LogViewModel.AppendWarning(msg + res.ToString());
            }
        }

        protected void AddSvcCallLog(string msg, SvcOutputGeneric<GeneralDeviceResult> res)
        {
            if (res == null)
            {
                return;
            }
            if (res.IsOk && res.Output.Status == Common.Enums.StatusCode.OK)
            {
                LogViewModel.AppendMessage(msg + res.ToString());
            }
            else
            {
                LogViewModel.AppendWarning(msg + res.ToString());
            }
        }

        protected TRes SafeCall<TRes>(Func<AverySvcClient, TRes> func) where TRes : SvcOutputBase, new()
        {
            AverySvcClient client = null;
            try
            {
                client = new AverySvcClient();
                TRes res = func(client);                
                return res;
            }
            catch(TimeoutException)
            {
                var ret = new TRes();
                ret.ErrorMessage = ErrorMessageHelper.CreateError(ErrorCode.SVC_TIMEOUT_EXCEPTION);
                return ret as TRes;
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.InvokeAsync(() => 
                    LogViewModel.AppendWarning(ex.GetType().Name + ex.Message));

                var ret = new TRes();
                ret.ErrorMessage = ErrorMessageHelper.CreateError(ErrorCode.SVC_CALLSVC_RESULT_ISNULL);
                return ret as TRes;
            }
            finally
            {
                client.Abort();
            }
        }

        protected Task<TRes> SafeCallAsync<TRes>(Func<AverySvcClient, TRes> func) where TRes : SvcOutputBase, new()
        {
            return Task<TRes>.Run(() =>
            {
                AverySvcClient client = null;
                try
                {
                    client = new AverySvcClient();
                    TRes res = func(client);
                    return res;
                }
                catch (TimeoutException)
                {
                    var ret = new TRes();
                    ret.ErrorMessage = ErrorMessageHelper.CreateError(ErrorCode.SVC_TIMEOUT_EXCEPTION);
                    return ret as TRes;
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.InvokeAsync(() =>
                        LogViewModel.AppendWarning(ex.GetType().Name + ex.Message));

                    var ret = new TRes();
                    ret.ErrorMessage = ErrorMessageHelper.CreateError(ErrorCode.SVC_CALLSVC_RESULT_ISNULL);
                    return ret as TRes;
                }
                finally
                {
                    client.Abort();
                }
            });
        }

        protected void SafeCall(Action<AverySvcClient> func) 
        {
            AverySvcClient client = null;
            try
            {
                client = new AverySvcClient();
                func(client);
            }           
            catch (Exception ex)
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                    LogViewModel.AppendWarning(ex.GetType().Name + ex.Message));
            }
            finally
            {
                client.Abort();
            }
        }

       
        public virtual void Refresh()
        {
        }
    }
}
