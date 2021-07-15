using Rollbar;
using System;
using System.IO;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;

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
