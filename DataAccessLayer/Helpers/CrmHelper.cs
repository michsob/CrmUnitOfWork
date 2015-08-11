using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;

namespace DataAccessLayer.Helpers
{
    class CrmHelper
    {
        public static IEnumerable<Guid> GetGuidsAsEnumerable(ExecuteTransactionResponse executeTransactionResponse)
        {
            foreach (var responseItem in executeTransactionResponse.Responses)
            {
                yield return new Guid(responseItem.Results["id"].ToString());
            }

        }
    }
}
