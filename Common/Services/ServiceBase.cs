using Common.Infrastructure.ErrorHandling.Enums;
using Common.Infrastructure.ErrorHandling.Exceptions;
using Common.Infrastructure.ErrorHandling.Helpers;
using Common.Infrastructure.ErrorHandling.Output;
using Common.Infrastructure.Logging;
using Common.Services.Input;
using Common.Services.Output;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Common.Infrastructure.Constants;

namespace Common.Services
{
    public class ServiceBase
    {
        protected Logger _logger;

        public ServiceBase(Logger logger)
        {
            _logger = logger;
        }

        protected virtual string ModuleName
        {
            get { return this.GetType().Name; }
        }

        public bool DebugExceptions
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["DebugExceptions"] ?? "false");
            }
        }

        protected async Task RunCodeAsync(SvcInputBase request, SvcOutputBase response, Func<Task> action, [CallerMemberName] string callerMemberName = null)
        {
            LogMethodStarted(callerMemberName, request);
            try
            {
                await action();
            }
            catch (BusinessLogicException ex)
            {
                HandleBusinessLogicException(callerMemberName, request, response, ex);
                return;
            }
            catch (Exception generalException)
            {
                HandleException(callerMemberName, generalException, request, response);
                return;
            }
            LogMethodFinished(callerMemberName, request, response);
        }

        protected void RunCode(SvcInputBase request, SvcOutputBase response, Action action, [CallerMemberName] string callerMemberName = null)
        {
            LogMethodStarted(callerMemberName, request);
            try
            {
                action();
            }
            catch (BusinessLogicException ex)
            {
                HandleBusinessLogicException(callerMemberName, request, response, ex);
                return;
            }
            catch (Exception generalException)
            {
                HandleException(callerMemberName, generalException, request, response);
                return;
            }
            LogMethodFinished(callerMemberName, request, response);
        }
        
        protected void LogMethodStarted(string methodName, SvcInputBase request, string callerMemberName = null)
        {
            _logger.LogInfo("Started", request, methodName, this.ModuleName);
        }

        protected void LogMethodFinished(string methodName, SvcInputBase request, SvcOutputBase response)
        {
            _logger.LogInfo("Finished", response, methodName, this.ModuleName);
        }

        protected void HandleBusinessLogicException(string methodName, SvcInputBase request, SvcOutputBase response, BusinessLogicException ex)
        {
            response.ErrorMessage = ex.GetErrorDetails();
            _logger.LogError(response.ErrorMessage.ErrorText, request, methodName, this.ModuleName);
        }

        protected void HandleException(string methodName, Exception ex, SvcInputBase request, SvcOutputBase responce)
        {
            var message = DebugExceptions
                              ? ex.ToString()
                              : ErrorMessageHelper.GetErrorMessage(ErrorCode.UNDEFINED_ERROR);
            responce.ErrorMessage = new ErrorDetails(ErrorCode.UNDEFINED_ERROR, message);

            var messages = new List<string>();

            var exc = ex;
            do
            {
                messages.Add(exc.Message);
                exc = exc.InnerException;
            }
            while (exc != null);

            messages.Add("CALL STACK");
            messages.Add(ex.StackTrace);

            _logger.LogError(string.Join(Environment.NewLine, messages), request, methodName, ModuleName);
        }
    }
}
