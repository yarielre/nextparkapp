using NextPark.Mobile.Extensions;
using System;

namespace NextPark.Mobile.Services
{
    public class LoggerService : ILoggerService
    {
        public readonly IDialogService _dialogService;

        public LoggerService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public LoggerService LogVerboseException(Exception e, object sender)
        {
           
            if (e == null || sender == null)
            {
                return this;
            }
            //TODO: Log verbose exception;
            return this;
        }
        public LoggerService ShowVerboseException(Exception e, object sender)
        {

            if (e == null || sender == null)
            {
                return this;
            }

            var verboseMessage = e.GetVerboseException(sender).Message;

            _dialogService.ShowErrorAlert(verboseMessage);

            return this;
        }
        public LoggerService ThrowVerboseException(Exception e, object sender)
        {

            if (e == null || sender == null)
            {
                return this;
            }

            throw e.GetVerboseException(sender);
        }
    }
}
