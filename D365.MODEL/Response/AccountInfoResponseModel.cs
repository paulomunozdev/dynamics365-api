using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace D365.MODEL.Response
{
    public class AccountInfoResponseModel: ResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public AccountInfoResponseModel()
        {

        }

        public AccountInfoResponseModel(AccountInfoResponseModel model) : this()
        {
            this.Id = model.Id;
            this.Name = model.Name;
            this.Success = model.Success;
            this.Message = model.Message;
        }

        public AccountInfoResponseModel(Guid id, string name, bool sucess, string message) : this()
        {
            this.Id = id;
            this.Name = name;
            this.Success = sucess;
            this.Message = message;
        }
    }
}
