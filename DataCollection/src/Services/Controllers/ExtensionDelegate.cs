using FligthNode.Common.Api.Controllers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightNode.DataCollection.Services.Controllers
{
    /// <summary>
    /// Contains delegates pointing to extension methods, which can be overridden for unit test purposes.
    /// </summary>
    public static class ExtensionDelegate
    {

        public static Func<LoggingController, int> LookupUserIdDelegate;

        public static int LookupUserId(this LoggingController controller)
        {
            return LookupUserIdDelegate(controller);
        }



        static ExtensionDelegate()
        {
            Init();
        }




        public static void Init()
        {
            LookupUserIdDelegate = (LoggingController controller) =>
            {
                return controller.User.Identity.GetUserId<int>();
            };
        }

    }
}
