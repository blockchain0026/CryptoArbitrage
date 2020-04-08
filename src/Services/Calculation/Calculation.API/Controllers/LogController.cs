using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoArbitrage.Services.Calculation.API.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Calculation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {

        private readonly ITestLogger _testLogger;
        public LogController(ITestLogger testLogger)
        {
            _testLogger = testLogger ?? throw new ArgumentNullException(nameof(testLogger));
        }

        // GET: api/Log
        [Route("GetLog")]
        [HttpGet]
        public async Task<IActionResult> GetLog()
        {

            return Ok(_testLogger.GetLog());
        }



        // POST: api/Log
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Log/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}