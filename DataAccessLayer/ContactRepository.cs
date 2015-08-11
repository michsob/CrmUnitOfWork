using CrmEntities;
using System;

namespace DataAccessLayer
{
    public class ContactRepository : IRepository
    {
        private readonly TestOrganizationContext _context;
        private readonly ICrmUnitOfWork _crmUnitOfWork;
        
        public ContactRepository(ICrmUnitOfWork crmUnitOfWork)
        {
            if (crmUnitOfWork == null)  
                throw new ArgumentNullException("crmUnitOfWork");

            _crmUnitOfWork = crmUnitOfWork;
            _context = new TestOrganizationContext(_crmUnitOfWork.OrganisationService);
        }

        public void Add(string firstname, string lastname, string email)
        {
            var contact = new Contact
            {
                FirstName = firstname,
                LastName = lastname,
                EMailAddress1 = email
            };

            _crmUnitOfWork.Add(contact);
        }

        public void Update(Contact contact)
        {
            _crmUnitOfWork.Update(contact);
        }

        public void Delete(Guid contactId)
        {
            _crmUnitOfWork.Delete(Contact.EntityLogicalName, contactId);
        }
    }
}
