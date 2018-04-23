// Developer Express Code Central Example:
// How to implement CRUD operations using XtraGrid and WCF Data Services
// 
// This example is similar to the http://www.devexpress.com/scid=E3866 example and
// demonstrates how to implement CRUD operations using XtraGrid and WCF Data
// Services.
// 
// This example works with the standard SQL Northwind database.
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E4365

using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;
using WcfService1;

namespace NorthwindWcfService
{
    public class NorthwindWcfDataService : DataService<NorthwindEntities>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            // TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.
            // Examples:
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            // config.SetServiceOperationAccessRule("MyServiceOperation", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
        }
    }
}
