using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MI.Core.Runtime.Caching;
using MI.Core.Test.Service;
using MI.Library.Interface;
using MI.Library.Interface.Common;
using Microsoft.AspNetCore.Mvc;
using ServiceClient;

namespace MI.Core.Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ITestService _testService;
        private readonly ICacheManager _cacheManager;
        private readonly IResilientServiceClient _resilientServiceClient;

        public ValuesController(ITestService testService, ICacheManager cacheManager, IResilientServiceClient resilientServiceClient)
        {
            _testService = testService;
            _cacheManager = cacheManager;
            _resilientServiceClient = resilientServiceClient;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            //var result = _testService.GetTestResult();
            var cache = _cacheManager.GetCache<string, string>("StockApi");
            return await cache.GetAsync("HiCarEnjoys", () => _resilientServiceClient.RequestAsync($"{ApplicationUrls.PictureUrl}Picture/QueryPicture/Index", HttpVerb.Get));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
