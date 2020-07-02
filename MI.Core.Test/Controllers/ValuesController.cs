using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MI.Core.Runtime.Caching;
using MI.Core.Test.Service;
using Microsoft.AspNetCore.Mvc;

namespace MI.Core.Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ITestService _testService;
        private readonly ICacheManager _cacheManager;

        public ValuesController(ITestService testService, ICacheManager cacheManager)
        {
            _testService = testService;
            _cacheManager = cacheManager;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            //var result = _testService.GetTestResult();

            var cache = _cacheManager.GetCache<string, string>("test");
            var result = await cache.GetAsync("1", () => Task.FromResult("1"));
            return new string[] { result, result };
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
