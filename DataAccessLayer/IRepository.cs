using CrmEntities;
using System;

namespace DataAccessLayer
{
    public interface IRepository
    {
        void Add(string firstname, string lastname, string email);
        void Update(Contact contact);
        void Delete(Guid contactId);
    }
}