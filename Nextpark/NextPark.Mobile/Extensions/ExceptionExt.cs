using System;

namespace NextPark.Mobile.Extensions
{
    public static class ExceptionExt
    {
        public static Exception ThrowVerboseException(this Exception e, object sender) {
            
            //TODO: Setup logger;

            if (sender == null) {
                return e;
            }

            Type senderType = sender.GetType();
            string exVerboseMessage = string.Format("Sender class: {0}; Sender method: {1}; Sender namespace: {2}; ErrorMessage: {3}",
                senderType.Name, System.Reflection.MethodBase.GetCurrentMethod().Name, senderType.Namespace, e.Message);

            return new Exception(exVerboseMessage, e);
        }
    }
}
