using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365.LOGIC
{
    public class ExampleLogic : DAL.CRM
    {
        #region CTOR
        public ExampleLogic() : base("entityname") { }
        public ExampleLogic(IOrganizationService svc) : base("entityname", svc) { }

        public ExampleLogic(IOrganizationService svc, ITracingService trace) : base("entityname", svc, trace) { }
        #endregion CTOR

        #region Logic

        #endregion Logic

        #region Get

        #endregion Get

        #region Create

        #endregion Create

        #region Update

        #endregion Update
    }
}
