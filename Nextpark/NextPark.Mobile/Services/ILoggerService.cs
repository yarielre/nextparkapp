using System;

namespace NextPark.Mobile.Services
{
    public interface ILoggerService
    {
        LoggerService LogVerboseException(Exception e, object sender);
        LoggerService ShowVerboseException(Exception e, object sender);
        LoggerService ThrowVerboseException(Exception e, object sender);
    }
}