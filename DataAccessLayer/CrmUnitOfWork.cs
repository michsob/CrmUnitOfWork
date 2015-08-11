using DataAccessLayer.Helpers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataAccessLayer
{
    public class CrmUnitOfWork : ICrmUnitOfWork
    {
        private readonly IOrganizationService _service;
        private readonly int _batchSize;

        public IOrganizationService OrganisationService
        {
            get { return _service; }
        }

        private List<CreateRequest> Added { get; set; }
        private List<UpdateRequest> Modified { get; set; }
        private List<DeleteRequest> Deleted { get; set; }

        public CrmUnitOfWork(IOrganizationService service, int batchSize = 100)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            _service = service;
            _batchSize = batchSize;

            Added = new List<CreateRequest>();
            Modified = new List<UpdateRequest>();
            Deleted = new List<DeleteRequest>();
        }

        public void Add(Entity entity)
        {
            Added.Add(
                new CreateRequest
                {
                    Target = entity
                });
        }

        public void Update(Entity entity)
        {
            Modified.Add(
                new UpdateRequest
                {
                    Target = entity
                });
        }

        public void Delete(string entityLogicalName, Guid id)
        {
            Deleted.Add(
                new DeleteRequest
                {
                    Target = new EntityReference(entityLogicalName, id)
                });
        }

        public void Delete(EntityReference entityRef)
        {
            Deleted.Add(
                new DeleteRequest
                {
                    Target = entityRef
                });
        }

        public void Commit()
        {
            var requests = new OrganizationRequestCollection();
            var transactionRequest = new ExecuteTransactionRequest
            {
                Requests = new OrganizationRequestCollection()
            };

            Added.ForEach(requests.Add);
            Modified.ForEach(requests.Add);
            Deleted.ForEach(requests.Add);

            Added.Clear();
            Modified.Clear();
            Deleted.Clear();

            try
            {
                for (int i = 0; i < requests.Count; i++)
                {
                    if ((i + 1) % _batchSize != 0)
                    {
                        transactionRequest.Requests.Add(requests[i]);
                    }
                    else
                    {
                        transactionRequest.Requests.Add(requests[i]);
                        var response = _service.Execute(transactionRequest);

                        transactionRequest = new ExecuteTransactionRequest
                        {
                            Requests = new OrganizationRequestCollection()
                        };
                    }

                    if ((i == requests.Count - 1) && transactionRequest.Requests.Count > 0)
                    {
                        var response = _service.Execute(transactionRequest);
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> fault)
            {
                if (fault.Detail.ErrorDetails.Contains("MaxBatchSize"))
                {
                    int maxBatchSize = Convert.ToInt32(fault.Detail.ErrorDetails["MaxBatchSize"]);
                    if (maxBatchSize < transactionRequest.Requests.Count)
                    {
                        var errMsg =
                            string.Format(
                                "The input request collection contains {0} requests, which exceeds the maximum allowed {1}",
                                transactionRequest.Requests.Count, maxBatchSize);
                        throw new InvalidOperationException(errMsg, fault);
                    }
                }

                throw;
            }
        }

        public IEnumerable<Guid> CommitEnumerable()
        {
            var requests = new OrganizationRequestCollection();
            var response = new List<Guid>();
            var transactionRequest = new ExecuteTransactionRequest
            {
                Requests = new OrganizationRequestCollection()
            };

            Added.ForEach(requests.Add);
            Modified.ForEach(requests.Add);
            Deleted.ForEach(requests.Add);

            Added.Clear();
            Modified.Clear();
            Deleted.Clear();

            try
            {
                for (int i = 0; i < requests.Count; i++)
                {
                    if ((i + 1) % _batchSize != 0)
                    {
                        transactionRequest.Requests.Add(requests[i]);
                    }
                    else
                    {

                        transactionRequest.Requests.Add(requests[i]);
                        var multipleResponse = (ExecuteTransactionResponse)_service.Execute(transactionRequest);
                        response.AddRange(CrmHelper.GetGuidsAsEnumerable(multipleResponse));

                        transactionRequest = new ExecuteTransactionRequest
                        {
                            Requests = new OrganizationRequestCollection()
                        };
                    }

                    if ((i == requests.Count - 1) && transactionRequest.Requests.Count > 0)
                    {
                        var multipleResponse = (ExecuteTransactionResponse)_service.Execute(transactionRequest);
                        response.AddRange(CrmHelper.GetGuidsAsEnumerable(multipleResponse));
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> fault)
            {
                if (fault.Detail.ErrorDetails.Contains("MaxBatchSize"))
                {
                    int maxBatchSize = Convert.ToInt32(fault.Detail.ErrorDetails["MaxBatchSize"]);
                    if (maxBatchSize < transactionRequest.Requests.Count)
                    {
                        var errMsg =
                            string.Format(
                                "The input request collection contains {0} requests, which exceeds the maximum allowed {1}",
                                transactionRequest.Requests.Count, maxBatchSize);
                        throw new InvalidOperationException(errMsg, fault);
                    }
                }

                throw;
            }

            return response;
        }
    }
}
