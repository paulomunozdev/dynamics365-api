using D365.MODEL.Response;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365.LOGIC
{
    public class AccountLogic : DAL.CRM
    {
        #region CTOR
        public AccountLogic() : base("account") { }
        public AccountLogic(IOrganizationService svc) : base("account", svc) { }

        public AccountLogic(IOrganizationService svc, ITracingService trace) : base("account", svc, trace) { }

        #endregion CTOR

        #region Logic
        public async Task<AccountInfoResponseModel> GetAccountById(string requestId)
        {
            var _responseModel = new AccountInfoResponseModel();
            try
            {
                _responseModel = await Task.Run(() => ReceiveModel(requestId));
                if (_responseModel.Success == false)
                {
                    throw new Exception(_responseModel.Message);
                }
            }
            catch (Exception ex)
            {
                _responseModel.Success = false;
                _responseModel.Message = ex.Message;
            }
            return _responseModel;
        }
        public async Task<AccountsResponseModel> GetAllaccounts()
        {
            var _responseModel = new AccountsResponseModel();
            try
            {
                _responseModel = await Task.Run(() => ReceiveModel());
                if (_responseModel.Success == false)
                {
                    throw new Exception(_responseModel.Message);
                }
            }
            catch (Exception ex)
            {
                _responseModel.Success = false;
                _responseModel.Message = ex.Message;
            }
            return _responseModel;
        }

        private AccountsResponseModel ReceiveModel()
        {
            var _responseModel = new AccountsResponseModel();
            _responseModel.Success = false;
            var _accounts = GetAllActiveAccounts();
            if (_accounts.Count > 0)
            {
                _responseModel.Success = true;
                _responseModel.Accounts = _accounts;
            }
            else
            {
                _responseModel.Message = "No active accounts found";
            }

            return _responseModel;
        }

        public AccountInfoResponseModel ReceiveModel(string requestId)
        {
            var _responseModel = new AccountInfoResponseModel();
            _responseModel.Success = false;
            var _account = GetByCustomId("accountnumber", requestId, new string[] { "accountid", "name" });
            if(_account != null)
            {
                _responseModel.Success = true;
                _responseModel.Name = EntityAttibuteString(_account, "name");
                _responseModel.Id = _account.Id;
            }
            else
            {
                _responseModel.Message = "The account was not found";
            }
                
            return _responseModel;
        }

        #endregion Logic

        #region Get
        public List<Entity> GetAllActiveAccounts()
        {
            var returnList = new List<Entity>();

            var query = GetQuery();
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.ColumnSet.AddColumn("name");
            query.ColumnSet.AddColumn("accountid");
            PagingHelper(query, (ec) => { returnList.AddRange(ec.Entities); });

            return returnList;
        }

        
        #endregion Get

        #region Create
        #endregion Create

        #region Update

        #endregion Update
    }
}
