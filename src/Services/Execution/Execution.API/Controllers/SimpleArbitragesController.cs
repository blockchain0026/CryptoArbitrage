using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages;
using CryptoArbitrage.Services.Execution.Infrastructure;
using CryptoArbitrage.Services.Execution.API.Responses.SimpleArbitrages;
using CryptoArbitrage.Services.Execution.API.Extensions;

namespace Execution.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimpleArbitragesController : ControllerBase
    {
        private readonly ExecutionContext _context;

        public SimpleArbitragesController(ExecutionContext context)
        {
            _context = context;
        }

        // GET: api/SimpleArbitrages
        [HttpGet]
        public async Task<IEnumerable<SimpleArbitrageResponse>> GetSimpleArbitrages()
        {
            var result = new List<SimpleArbitrageResponse>();
            foreach (var arbitrage in _context.SimpleArbitrages)
            {

                await _context.Entry(arbitrage)
                    .Reference(i => i.Status).LoadAsync();

                result.Add(arbitrage.ToSimpleArbitrageResponse());
            }
            return result.AsEnumerable();
        }

        // GET: api/SimpleArbitrages/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSimpleArbitrage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var simpleArbitrage = await _context.SimpleArbitrages.FindAsync(id);

            if (simpleArbitrage == null)
            {
                return NotFound();
            }

            return Ok(simpleArbitrage);
        }

        // PUT: api/SimpleArbitrages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSimpleArbitrage([FromRoute] int id, [FromBody] SimpleArbitrage simpleArbitrage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != simpleArbitrage.Id)
            {
                return BadRequest();
            }

            _context.Entry(simpleArbitrage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SimpleArbitrageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/SimpleArbitrages
        [HttpPost]
        public async Task<IActionResult> PostSimpleArbitrage([FromBody] SimpleArbitrage simpleArbitrage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.SimpleArbitrages.Add(simpleArbitrage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSimpleArbitrage", new { id = simpleArbitrage.Id }, simpleArbitrage);
        }

        // DELETE: api/SimpleArbitrages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSimpleArbitrage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var simpleArbitrage = await _context.SimpleArbitrages.FindAsync(id);
            if (simpleArbitrage == null)
            {
                return NotFound();
            }

            _context.SimpleArbitrages.Remove(simpleArbitrage);
            await _context.SaveChangesAsync();

            return Ok(simpleArbitrage);
        }

        private bool SimpleArbitrageExists(int id)
        {
            return _context.SimpleArbitrages.Any(e => e.Id == id);
        }
    }
}