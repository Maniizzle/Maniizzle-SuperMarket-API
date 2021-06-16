using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Supermarket.API.Domain.Model;
using Supermarket.API.Domain.Services;
using Supermarket.API.Dtos;
using Supermarket.API.Services;

namespace Supermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly ICacheService cacheService;

        public CacheController(ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {

            return new string[] { "value1", "value2" };
        }

        [HttpGet("get/{key}")]
        public async  Task<ActionResult<string>> Get(string key)
        {
           var value=await cacheService.GetCacheValueAsync(key);
            return Ok(value);
        }

        [HttpPost("set")]
        public async Task<IActionResult> Post([FromBody] CacheDto value)
        {
             await cacheService.SetCacheValueAsync(value.Key,value.Value);
            return Ok();

        }

       
    }
}