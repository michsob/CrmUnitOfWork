using Microsoft.Xrm.Sdk.Client;
using System;
using System.Reflection;
using System.ServiceModel.Description;

namespace DataAccessLayer.Connection
{
    public interface ICrmConnection
    {
        OrganizationServiceContext GetContext();
        OrganizationServiceProxy ServiceProxy { get; }
    }

    public class CrmConnection : ICrmConnection
    {
        private readonly ClientCredentials _client;
        private readonly string _organisationServiceUrl;

        public CrmConnection(string orgUrl, string username, string password)
        {
            if (string.IsNullOrEmpty(orgUrl))
                throw new NullReferenceException("OrganisationServiceUrl");
            if (string.IsNullOrEmpty(username))
                throw new NullReferenceException("Username");
            if (string.IsNullOrEmpty(password))
                throw new NullReferenceException("Password");

            this._organisationServiceUrl = orgUrl;
            this._client = new ClientCredentials();
            this._client.UserName.UserName = username;
            this._client.UserName.Password = password;

        }

        private OrganizationServiceProxy _serviceProxy;

        public OrganizationServiceProxy ServiceProxy
        {
            get
            {
                if (_serviceProxy == null)
                {
                    Uri uri = new Uri(this._organisationServiceUrl);
                    _serviceProxy = new OrganizationServiceProxy(uri, null, _client, null);
                    _serviceProxy.ServiceConfiguration.CurrentServiceEndpoint.Behaviors.Add(
                        new ProxyTypesBehavior(Assembly.GetExecutingAssembly()));
                    return _serviceProxy;
                }
                else
                {
                    return _serviceProxy;
                }
            }
        }

        public OrganizationServiceContext GetContext()
        {
            return new OrganizationServiceContext(ServiceProxy);
        }
    }
}
