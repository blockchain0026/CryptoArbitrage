using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.Calculation.API.Infrastructure;
using CryptoArbitrage.Services.Calculation.API.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Calculation.API.IntegrationEvents.EventHandling
{
    public class ProfitRoomFoundedIntegrationEventHandler : IIntegrationEventHandler<ProfitRoomFoundedIntegrationEvent>
    {
        //private string _testLog = string.Empty;
        private readonly ITestLogger _testLogger;
        public ProfitRoomFoundedIntegrationEventHandler(ITestLogger testLogger)
        {
            _testLogger = testLogger ?? throw new ArgumentNullException(nameof(testLogger));
        }

        public async Task Handle(ProfitRoomFoundedIntegrationEvent @event)
        {
            return;
            await Task.Run(() =>
            {
                // Set a variable to the My Documents path.
                string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Create a string array with the additional lines of text
                /*string[] lines = {
                    "Time:" + DateTime.UtcNow.ToShortTimeString(),
                    "Market:" + @event.BuyOrderBaseCurrency + @event.BuyOrderQuoteCurrency,
                    "Buy From:" + "ExchangeId => " + @event.BuyFrom,
                    "Buy Price:" + @event.BuyOrderPrice,
                    "Buy Amount:" + @event.BuyOrderAmounts,
                    "Sell To:" + "ExchangeId => " + @event.SellTo,
                    "Sell Price:" + @event.SellOrderPrice,
                    "Sell Amount:" + @event.SellOrderAmounts,
                    "Estimated Profits" + @event.EstimatedProfits
                };*/
                string log =  "Time:" + DateTime.Now.AddHours(8).ToLongTimeString() + "\n"
                            + "Market:" + @event.BuyOrderBaseCurrency + @event.BuyOrderQuoteCurrency + "\n"
                            + "Buy From:" + "ExchangeId => " + @event.BuyFrom + "\n"
                            + "Buy Price:" + @event.BuyOrderPrice + "\n"
                            + "Buy Amount:" + @event.BuyOrderAmounts + "\n"
                            + "Sell To:" + "ExchangeId => " + @event.SellTo + "\n"
                            + "Sell Price:" + @event.SellOrderPrice + "\n"
                            + "Sell Amount:" + @event.SellOrderAmounts + "\n"
                            + "Estimated Profits:" + @event.EstimatedProfits + "\n";
                this._testLogger.AddLog(log);
                Console.WriteLine(log);



            });

        }
    }
}
