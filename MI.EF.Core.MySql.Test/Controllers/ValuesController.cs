﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MI.EF.Core.Env;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MI.EF.Core.MySql.Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IEnvironmentHandler<MIContext> env;
        private readonly IServiceProvider serviceProvider;

        public ValuesController(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.env = serviceProvider.GetRequiredService<IEnvironmentHandler<MIContext>>();
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SlideShowImg>>> Get()
        {
            //var result = await _context.SlideShowImg.ToListAsync();
            //return result;

            var result = await env.Db.SingleQueryAsync<SlideShowImg>(a => a.Id == 1);

            //await env.Db.UpdateAsync<SlideShowImg>(result, a => { a.PushHome = false; });

            await env.Db.UpdateAsync(1, new UpdateProperty<SlideShowImg, bool>(a => a.pushHome, false));

            return await env.Db.QueryAsync<SlideShowImg>(a => true); ;
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
