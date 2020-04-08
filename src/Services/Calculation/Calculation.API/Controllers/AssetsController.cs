using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoArbitrage.Services.Calculation.API.ApiResponses.Assets;
using CryptoArbitrage.Services.Calculation.Domain.Model.Exchanges;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CryptoArbitrage.Services.Calculation.API.Controllers
{
    [Route("api/[controller]")]
    public class AssetsController : Controller
    {
        private readonly IExchangeRepository _exchangeRepository;

        public AssetsController(IExchangeRepository exchangeRepository)
        {
            this._exchangeRepository = exchangeRepository ?? throw new ArgumentNullException(nameof(exchangeRepository));
        }

        // GET: api/<controller>
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<ExchangeAssetsInfoApiResponse>>> GetAll()
        {
            try
            {
                var exchanges = new List<ExchangeAssetsInfoApiResponse>();
                for (int i = 0; i <= 5; i++)
                {
                    var assets = new List<AssetApiResponse>();

                    var exchange = await _exchangeRepository.GetAsync(i);
                    if (exchange != null)
                    {
                        foreach (var asset in exchange.ExchangeAssets)
                        {
                            assets.Add(new AssetApiResponse
                            {
                                Symbol = asset.Symbol,
                                Balance = asset.GetAvailableBalances()
                            });
                        }
                        exchanges.Add(new ExchangeAssetsInfoApiResponse
                        {
                            ExchangeId = exchange.ExchangeId,
                            Assets = assets.AsEnumerable()
                        });
                    }
                    else
                    {
                        throw new KeyNotFoundException("Failed to fetch assets info for ExchangeId: " + i.ToString());
                    }
                }


                return Ok(exchanges.AsEnumerable());
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
