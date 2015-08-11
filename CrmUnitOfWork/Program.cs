using DataAccessLayer;
using DataAccessLayer.Connection;
using System;
using System.Configuration;
using System.Linq;

namespace CrmUnitOfWorkApp
{
    class Program
    {
        static void Main(string[] args)
        {
            CrmConnection crmConnection = new CrmConnection(
                ConfigurationManager.AppSettings["OrganisationServiceUrl"],
                ConfigurationManager.AppSettings["Username"],
                ConfigurationManager.AppSettings["Password"]);
            
            var crmUnitOfWork = new CrmUnitOfWork(crmConnection.ServiceProxy);
            var repository = new ContactRepository(crmUnitOfWork);

            repository.Add("test1", "contact", "email1@company.com");
            repository.Add("test2", "contact", "email1@company.com");

            crmUnitOfWork.Commit();

            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
