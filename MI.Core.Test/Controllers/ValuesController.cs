using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MI.Core.Runtime.Caching;
using MI.Core.Test.Service;
using MI.Domain.Test;
using MI.Domain.Test.Interface.EntityFramework;
using MI.EF.Core.Env;
using MI.Library.Interface;
using MI.Library.Interface.Common;
using MI.Library.Interface.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IDbExecutorFactoryWithDbConfigType _dbExecutorFactory;
        private readonly ITestCurrentContextProvider _contextProvider;

        public ValuesController(ITestService testService, ICacheManager cacheManager, IResilientServiceClient resilientServiceClient, IDbExecutorFactoryWithDbConfigType dbExecutorFactory, ITestCurrentContextProvider contextProvider)
        {
            _testService = testService;
            _cacheManager = cacheManager;
            _resilientServiceClient = resilientServiceClient;
            _dbExecutorFactory = dbExecutorFactory;
            _contextProvider = contextProvider;
        }

        // GET api/values
        [HttpGet]
        public async Task<SlideShowImg> Get()
        {
            //依赖注入
            //var result = _testService.GetTestResult();

            //Redis缓存
            //var cache = _cacheManager.GetCache<string, string>("StockApi");
            //return await cache.GetAsync("HiCarEnjoys", () => _resilientServiceClient.RequestAsync($"{ApplicationUrls.PictureUrl}Picture/QueryPicture/Index", HttpVerb.Get));

            ////Dapper
            //var dbExecutor = _dbExecutorFactory.CreateExecutor(DbConfigType.MI);
            //string strCountSql = "select count(1) from SlideShowImg";
            //int totalCount = (int)await dbExecutor.ExecuteScalarAsync(strCountSql, null);
            //return totalCount.ToString();

            using (var context = _contextProvider.GetReadContext())
            {
                var result = await context.SlideShowImg.FirstOrDefaultAsync();
                return result;
            }
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
