using D365.MODEL.Response;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365.MODEL.Response
{
    public class AccountsResponseModel : ResponseModel
    {
        public List<Entity> Accounts { get; set; }

        public AccountsResponseModel()
        {

        }

        public AccountsResponseModel(AccountsResponseModel model) : this()
        {
            this.Accounts = model.Accounts;
            this.Success = model.Success;
            this.Message = model.Message;
        }

        public AccountsResponseModel(List<Entity> accounts, bool sucess, string message) : this()
        {
            this.Accounts = accounts;
            this.Success = sucess;
            this.Message = message;
        }
    }
}
