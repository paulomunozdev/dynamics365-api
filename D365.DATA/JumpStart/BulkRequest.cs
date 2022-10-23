using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System.Linq;

namespace JumpStart
{
    public class BulkRequestData
    {
        private IOrganizationService service { get; set; }

        private int pageNumber { get; set; }

        public BulkRequestData(IOrganizationService service)
        {
            this.service = service;
            _requestCollection = new List<OrganizationRequest>();
        }

        int recordsPerPage = 1000;

        private List<OrganizationRequest> _requestCollection { get; set; }

        public int GetRequestsCount()
        {
            return _requestCollection.Count;
        }

        public void AddCreateRequest(Entity entity)
        {
            CreateRequest createRequest = new CreateRequest { Target = entity };
            _requestCollection.Add(createRequest);
        }

        public void AddUpdateRequest(Entity entity)
        {
            UpdateRequest updateRequest = new UpdateRequest { Target = entity };
            _requestCollection.Add(updateRequest);
        }

        public void AddDeleteRequest(string entityName, Guid entityId)
        {
            DeleteRequest deleteRequest = new DeleteRequest();
            deleteRequest.Target = new EntityReference(entityName, entityId);

            _requestCollection.Add(deleteRequest);
        }
        
        public void AddRequest(OrganizationRequest request)
        {
            _requestCollection.Add(request);
        }

        public void ExecuteRequest()
        {
            var _multipleRequest = new ExecuteMultipleRequest();

            _multipleRequest.Settings = new ExecuteMultipleSettings();

            _multipleRequest.Settings.ContinueOnError = true;
            _multipleRequest.Settings.ReturnResponses = true;

            int totalPages = (int)Math.Ceiling(_requestCollection.Count() * 1.00/recordsPerPage);

            for (pageNumber = 0; pageNumber < totalPages; pageNumber++)
            {
                var currentRequestCollection = new OrganizationRequestCollection();

                currentRequestCollection.AddRange(_requestCollection.Skip(recordsPerPage * pageNumber).Take(recordsPerPage));

                _multipleRequest.Requests = currentRequestCollection;

                ExecuteMultipleResponse requestResponses = (ExecuteMultipleResponse)service.Execute(_multipleRequest);

                foreach (var response in requestResponses.Responses)
                {
                    if (response.Fault != null)
                    {
                        JumpStart.Log.FileLog.Write(String.Format("A fault has been produced on request {0}, with the following message: {1}", _multipleRequest.Requests.ElementAt(response.RequestIndex).RequestName, response.Fault.Message));
                    }
                }
            }
        }

        public ExecuteMultipleResponseItemCollection ExecuteRequestWithResponse()
        {
            var response = new ExecuteMultipleResponseItemCollection();
            var _multipleRequest = new ExecuteMultipleRequest();

            _multipleRequest.Settings = new ExecuteMultipleSettings();

            _multipleRequest.Settings.ContinueOnError = true;
            _multipleRequest.Settings.ReturnResponses = true;

            int totalPages = (int)Math.Ceiling(_requestCollection.Count() * 1.00 / recordsPerPage);

            for (pageNumber = 0; pageNumber < totalPages; pageNumber++)
            {
                var currentRequestCollection = new OrganizationRequestCollection();

                currentRequestCollection.AddRange(_requestCollection.Skip(recordsPerPage * pageNumber).Take(recordsPerPage));

                _multipleRequest.Requests = currentRequestCollection;

                ExecuteMultipleResponse requestResponses = (ExecuteMultipleResponse)service.Execute(_multipleRequest);
                response.AddRange(requestResponses.Responses);
            }
            return response;
        }
    }
}
