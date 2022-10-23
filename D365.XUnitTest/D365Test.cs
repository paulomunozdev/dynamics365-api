using D365.LOGIC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using Xunit;

namespace D365.XUnitTest
{
    public class D365Test
    {
        #region TestConnection
        public static bool TestConnection()
        {
            var req = new Microsoft.Crm.Sdk.Messages.WhoAmIRequest();

            try
            {
                var svc = JumpStartDATA.CrmConnectionFactory.GetOrganizationService();
                var res = svc.Execute(req);

                return true;
            }
            catch
            {
                return false;
            }
        }

        [Fact]
        public void TestConexion()
        {            
            if (TestConnection())
            {
                Xunit.Assert.Fail("OK");
            }
            else
            {
                Xunit.Assert.Fail("Error");
                return;
            }
        }
        #endregion
    }
}
