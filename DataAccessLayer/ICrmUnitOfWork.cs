using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace DataAccessLayer
{
    public interface ICrmUnitOfWork
    {
        IOrganizationService OrganisationService { get; }

        void Add(Entity entity);
        void Update(Entity entity);
        void Delete(string entityName, Guid id);
        void Delete(EntityReference entityRef);
        void Commit();
        IEnumerable<Guid> CommitEnumerable();
    }
}
