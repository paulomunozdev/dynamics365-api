using D365.LOGIC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace D365.API.Controllers
{
    public class AccountController : ApiController
    {
        private readonly AccountLogic _accountLogic;

        public AccountController(AccountLogic accountLogic)
        {
            _accountLogic = accountLogic;
        }
        readonly AccountLogic accountLogic = new AccountLogic();
        public AccountController()
        {
            _accountLogic = accountLogic;
        }
        [Route("api/account/GetAccountById")]
        [HttpGet]

        public async Task<IHttpActionResult> GetAccountByIdAsync(string requestId)
        {
            var _response = await _accountLogic.GetAccountById(requestId);
            return Ok(_response);
        }
        [Route("api/account/GetAllaccounts")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllaccountsAsync()
        {
            var _response = await _accountLogic.GetAllaccounts();
            return Ok(_response);
        }
    }
}
