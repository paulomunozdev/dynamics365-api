using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365.MODEL.Response
{
    public class ResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public ResponseModel()
        {
            this.Success = false;
            this.Message = string.Empty;
        }

        public ResponseModel(ResponseModel model) : this()
        {
            this.Success = model.Success;
            this.Message = model.Message;
        }

        public ResponseModel(bool sucess, string message) : this()
        {
            this.Success = sucess;
            this.Message = message;
        }
    }
}
