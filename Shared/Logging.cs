using Rollbar;
using System;


namespace HVMDash.Shared
{
    public class Logging
    {
        public static void ErrorLogging(Exception ex, string rollbarToken)
        {
            RollbarLocator.RollbarInstance.Configure(new RollbarConfig(rollbarToken));
            RollbarLocator.RollbarInstance.Error(ex);
        }

    }
}
