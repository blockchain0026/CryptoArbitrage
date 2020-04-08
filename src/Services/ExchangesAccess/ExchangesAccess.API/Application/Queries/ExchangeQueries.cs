using CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Application.Queries
{
    public class ExchangeQueries
       : IExchangeQueries
    {
        private string _connectionString = string.Empty;
        private IExchangeMarketData _marketData;
        public ExchangeQueries(IExchangeMarketData exchangeMarketData)
        {
            //_connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
            _marketData = exchangeMarketData;
        }

        public async Task<ExchangeViewModel> GetExchangeAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var result = await connection.QueryAsync<dynamic>(
                   @"select e.[Id] as exchangeid,e.name as name, e.url as url,
                        FROM exchangesaccess.Exchanges e
                        LEFT JOIN exchangesaccess.APICommands er ON e.Id = er.exchangeid
                        LEFT JOIN exchangesaccess.APIResponses er ON e.Id = er.exchangeid 
                        WHERE e.Id=@id"
                        , new { id }
                    );

                if (result.AsList().Count == 0)
                    throw new KeyNotFoundException();

                return MapAPICommandsAndResponses(result);
            }
        }


        public async Task<OrderBookViewModel> GetMarketAsync(int exchangeId, string baseCurrency, string quoteCurrency)
        {
            OrderBookViewModel orderbook;

            orderbook = await Task.Run(() =>
            {
                if (String.IsNullOrEmpty(baseCurrency) || String.IsNullOrEmpty(quoteCurrency))
                    throw new ArgumentNullException("Market isn't provided.");
                if (exchangeId.ToString() == "0")
                {
                    if (quoteCurrency.ToUpper() == "USD")
                    {
                        quoteCurrency = "USDT";
                    }

                    return _marketData.GetMarketData(exchangeId.ToString(), baseCurrency.ToUpper(), quoteCurrency.ToUpper());
                }
                else if (exchangeId.ToString() == "1")
                {
                    if (quoteCurrency.ToUpper() == "USD")
                    {
                        quoteCurrency = "USDT";
                    }

                    return _marketData.GetMarketData(exchangeId.ToString(), baseCurrency.ToUpper(), quoteCurrency.ToUpper());
                }
                else if (exchangeId.ToString() == "3")
                {
                    return _marketData.GetMarketData(exchangeId.ToString(), baseCurrency.ToUpper(), quoteCurrency.ToUpper());
                }
                else if (exchangeId.ToString() == "4")
                {
                    return _marketData.GetMarketData(exchangeId.ToString(), baseCurrency.ToUpper(), quoteCurrency.ToUpper());
                }
                else if (exchangeId.ToString() == "5")
                {
                    if (quoteCurrency.ToUpper() == "USD")
                    {
                        quoteCurrency = "USDT";
                    }
                    return _marketData.GetMarketData(exchangeId.ToString(), baseCurrency.ToUpper(), quoteCurrency.ToUpper());
                }
                return null;
            });

            return orderbook;
        }


        private ExchangeViewModel MapAPICommandsAndResponses(dynamic result)
        {
            var exchangeViewModel = new ExchangeViewModel
            {
                exchangeid = result[0].exchangeid,
                name = result[0].name,
                url = result[0].url,
                apicommands = new List<APICommandViewModel>(),
                apiresponses = new List<APIResponseViewModel>()
            };
            foreach (dynamic command in result)
            {
                var apicommand = new APICommandViewModel
                {
                    apicommandtypeid = command.apicommandtypeid
                };
                exchangeViewModel.apicommands.Add(apicommand);
            }
            foreach (dynamic response in result)
            {
                var apiresponse = new APIResponseViewModel
                {
                    apiresponsetypeid = response.apiresponsetypeid
                };
                exchangeViewModel.apicommands.Add(response);
            }
            return exchangeViewModel;
        }
    }
}
