using System;
using System.Configuration;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Client;
using System.ServiceModel.Description;
using System.Net;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;

namespace JumpStartDATA
{
    public static class CrmConnectionFactory
    {
        private static object lockObjService = new object();
        private static volatile IOrganizationService serviceInstance;
        private static OrganizationWebProxyClient sdkService;
        private static IOrganizationService svc;
        private static IOrganizationService service
        {
            get
            {
                if (serviceInstance == null)
                {
                    lock (lockObjService)
                    {
                        if (serviceInstance == null)
                        {
                            serviceInstance = GetNewOrganizationService();
                        }
                    }
                }
                return serviceInstance;
            }
        }

        public static IOrganizationService GetOrganizationService()
        {
            return service;
        }

        public static IOrganizationService GetNewOrganizationServiceClientIdClientSecret(string crmUrl, string crmClientId, string crmClientSecret)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                CrmServiceClient client = null;
                IOrganizationService _orgService = null;
                client = new CrmServiceClient($@"AuthType=ClientSecret;url={crmUrl};ClientId={crmClientId};ClientSecret={crmClientSecret}");
                if (client.IsReady)
                {
                    _orgService = (IOrganizationService)client.OrganizationWebProxyClient != null ? (IOrganizationService)client.OrganizationWebProxyClient : (IOrganizationService)client.OrganizationServiceProxy;
                }



                return _orgService;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static IOrganizationService GetNewOrganizationService()
        {
            try
            {
                var Url = ConfigurationManager.AppSettings["crmConnectionUrl"];
                var username = ConfigurationManager.AppSettings["crmConnectionUsername"];
                var password = ConfigurationManager.AppSettings["crmConnectionPassword"];
                string clientId = ConfigurationManager.AppSettings["crmClientId"];
                string appKey = ConfigurationManager.AppSettings["crmClientSecret"];



                var _orgService = GetNewOrganizationServiceClientIdClientSecret(Url, clientId, appKey);



                return _orgService;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region oldconnection

        //public static IOrganizationService GetNewOrganizationService()
        //{
        //    var url = ConfigurationManager.AppSettings["crmConnectionUrl"];
        //    var username = ConfigurationManager.AppSettings["crmConnectionUsername"];
        //    var password = ConfigurationManager.AppSettings["crmConnectionPassword"];
        //    var timeout = ConfigurationManager.AppSettings["crmConnectionTimeout"];
        //    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

        //    if (String.IsNullOrWhiteSpace(url) || String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
        //    {
        //        throw new Exception("AppConfig not available or Connection String not configured.");
        //    }

        //    var uri = new Uri($"{ url }/XRMServices/2011/Organization.svc");

        //    var cred = new ClientCredentials();

        //    cred.UserName.UserName = username;
        //    cred.UserName.Password = password;

        //    var svc = new OrganizationServiceProxy(uri, null, cred, null);
        //    svc.Timeout = new TimeSpan(15, 15, 0);
        //    return svc;
        //} 
        //public static IOrganizationService GetNewOrganizationService(string url, string username, string password)
        //{

        //    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

        //    if (String.IsNullOrWhiteSpace(url) || String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
        //    {
        //        throw new Exception("AppConfig not available or Connection String not configured.");
        //    }

        //    var uri = new Uri($"{ url }/XRMServices/2011/Organization.svc");

        //    var cred = new ClientCredentials();

        //    cred.UserName.UserName = username;
        //    cred.UserName.Password = password;

        //    var svc = new OrganizationServiceProxy(uri, null, cred, null);
        //    svc.Timeout = new TimeSpan(15, 15, 0);
        //    serviceInstance = svc;
        //    return svc;
        //}
        #endregion

        public static Guid GetUser()
        {
            WhoAmIRequest req = new WhoAmIRequest();

            WhoAmIResponse res = (WhoAmIResponse)service.Execute(req);

            return res.UserId;
        }
    }
}
