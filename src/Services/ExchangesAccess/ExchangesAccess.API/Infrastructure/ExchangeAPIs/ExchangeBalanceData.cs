using CryptoArbitrage.EventBus.Abstractions;
using CryptoArbitrage.Services.ExchangesAccess.API.Application.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs
{
    public class ExchangeBalanceData : IExchangeBalanceData
    {
        private readonly IEventBus _eventBus;
        private Timer _pushDataTimer;

        private Dictionary<string, BalanceData> _bittrex;
        private Dictionary<string, BalanceData> _binance;
        private Dictionary<string, BalanceData> _kraken;
        private Dictionary<string, BalanceData> _bitstamp;
        private Dictionary<string, BalanceData> _poloniex;
        private Dictionary<string, BalanceData> _kucoin;
        /*public IEnumerable<BalanceData> Bittrex => this._bittrex.AsEnumerable();
        public IEnumerable<BalanceData> Binance => this._binance.AsEnumerable();
        public IEnumerable<BalanceData> Kraken => this._kraken.AsEnumerable();
        public IEnumerable<BalanceData> Bitstamp => this._bitstamp.AsEnumerable();
        public IEnumerable<BalanceData> Poloniex => this._poloniex.AsEnumerable();*/

        public ExchangeBalanceData(IEventBus eventBus)
        {
            _eventBus = eventBus;

            this._bittrex = new Dictionary<string, BalanceData>();
            this._binance = new Dictionary<string, BalanceData>();
            this._kraken = new Dictionary<string, BalanceData>();
            this._bitstamp = new Dictionary<string, BalanceData>();
            this._poloniex = new Dictionary<string, BalanceData>();
            this._kucoin = new Dictionary<string, BalanceData>();
            //KeepPushingBalanceData();
        }


        public async Task WriteBalanceData(string exchangeId, string symbol, decimal totalBalance, decimal availableBalance, decimal pendingBalance)
        {
            await Task.Run(() =>
            {
                if (exchangeId == "0")
                {
                    var assetKey = symbol.ToUpper();
                    lock (this._bittrex)
                    {
                        BalanceData balanceData;


                        if (!this._bittrex.TryGetValue(assetKey, out balanceData))
                        {
                            this._bittrex.Add(
                                assetKey,
                                new BalanceData { AssetSymbol = assetKey, TotalBalance = totalBalance, AvailableBalance = availableBalance, PendingBalance = pendingBalance });
                        }
                        else
                        {
                            balanceData.TotalBalance = totalBalance;
                            balanceData.AvailableBalance = availableBalance;
                            balanceData.PendingBalance = pendingBalance;

                            this._bittrex[assetKey] = balanceData;
                        }

                        this._eventBus.Publish(new ExchangeAssetInfoUpdatedIntegrationEvent(
                            Int32.Parse(exchangeId),
                            symbol,
                            totalBalance,
                            availableBalance,
                            pendingBalance
                            ));
                    }
                }
                else if (exchangeId == "1")
                {
                    var assetKey = symbol.ToUpper();
                    lock (this._binance)
                    {
                        BalanceData balanceData;


                        if (!this._binance.TryGetValue(assetKey, out balanceData))
                        {
                            this._binance.Add(
                                assetKey,
                                new BalanceData { AssetSymbol = assetKey, TotalBalance = totalBalance, AvailableBalance = availableBalance, PendingBalance = pendingBalance });
                        }
                        else
                        {
                            balanceData.TotalBalance = totalBalance;
                            balanceData.AvailableBalance = availableBalance;
                            balanceData.PendingBalance = pendingBalance;

                            this._binance[assetKey] = balanceData;
                        }

                        this._eventBus.Publish(new ExchangeAssetInfoUpdatedIntegrationEvent(
                            Int32.Parse(exchangeId),
                            symbol,
                            totalBalance,
                            availableBalance,
                            pendingBalance
                            ));
                    }


                }
                else if (exchangeId == "2")
                {
                    var assetKey = symbol.ToUpper();
                    lock (this._kraken)
                    {
                        BalanceData balanceData;


                        if (!this._kraken.TryGetValue(assetKey, out balanceData))
                        {
                            this._kraken.Add(
                                assetKey,
                                new BalanceData { AssetSymbol = assetKey, TotalBalance = totalBalance, AvailableBalance = availableBalance, PendingBalance = pendingBalance });
                        }
                        else
                        {
                            balanceData.TotalBalance = totalBalance;
                            balanceData.AvailableBalance = availableBalance;
                            balanceData.PendingBalance = pendingBalance;

                            this._kraken[assetKey] = balanceData;
                        }

                        this._eventBus.Publish(new ExchangeAssetInfoUpdatedIntegrationEvent(
                            Int32.Parse(exchangeId),
                            symbol,
                            totalBalance,
                            availableBalance,
                            pendingBalance
                            ));
                    }


                }
                else if (exchangeId == "3")
                {
                    var assetKey = symbol.ToUpper();
                    lock (this._bitstamp)
                    {
                        BalanceData balanceData;


                        if (!this._bitstamp.TryGetValue(assetKey, out balanceData))
                        {
                            this._bitstamp.Add(
                                assetKey,
                                new BalanceData { AssetSymbol = assetKey, TotalBalance = totalBalance, AvailableBalance = availableBalance, PendingBalance = pendingBalance });
                        }
                        else
                        {
                            balanceData.TotalBalance = totalBalance;
                            balanceData.AvailableBalance = availableBalance;
                            balanceData.PendingBalance = pendingBalance;

                            this._bitstamp[assetKey] = balanceData;
                        }

                        this._eventBus.Publish(new ExchangeAssetInfoUpdatedIntegrationEvent(
                            Int32.Parse(exchangeId),
                            symbol,
                            totalBalance,
                            availableBalance,
                            pendingBalance
                            ));
                    }

                }
                else if (exchangeId == "4")
                {
                    var assetKey = symbol.ToUpper();
                    lock (this._poloniex)
                    {
                        BalanceData balanceData;


                        if (!this._poloniex.TryGetValue(assetKey, out balanceData))
                        {
                            this._poloniex.Add(
                                assetKey,
                                new BalanceData { AssetSymbol = assetKey, TotalBalance = totalBalance, AvailableBalance = availableBalance, PendingBalance = pendingBalance });
                        }
                        else
                        {
                            balanceData.TotalBalance = totalBalance;
                            balanceData.AvailableBalance = availableBalance;
                            balanceData.PendingBalance = pendingBalance;

                            this._poloniex[assetKey] = balanceData;
                        }

                        this._eventBus.Publish(new ExchangeAssetInfoUpdatedIntegrationEvent(
                            Int32.Parse(exchangeId),
                            symbol,
                            totalBalance,
                            availableBalance,
                            pendingBalance
                            ));
                    }
                }
                else if (exchangeId == "5")
                {
                    var assetKey = symbol.ToUpper();
                    lock (this._kucoin)
                    {
                        BalanceData balanceData;


                        if (!this._kucoin.TryGetValue(assetKey, out balanceData))
                        {
                            this._kucoin.Add(
                                assetKey,
                                new BalanceData { AssetSymbol = assetKey, TotalBalance = totalBalance, AvailableBalance = availableBalance, PendingBalance = pendingBalance });
                        }
                        else
                        {
                            balanceData.TotalBalance = totalBalance;
                            balanceData.AvailableBalance = availableBalance;
                            balanceData.PendingBalance = pendingBalance;

                            this._kucoin[assetKey] = balanceData;
                        }

                        this._eventBus.Publish(new ExchangeAssetInfoUpdatedIntegrationEvent(
                            Int32.Parse(exchangeId),
                            symbol,
                            totalBalance,
                            availableBalance,
                            pendingBalance
                            ));
                    }
                }
            });
        }


        public void KeepPushingBalanceData()
        {
            _pushDataTimer = new System.Threading.Timer(
                e => PushBalanceData(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(1));

        }

        private void PushBalanceData()
        {
            lock (this._bittrex)
            {
                foreach (var asset in this._bittrex.Values)
                {
                    this._eventBus.Publish(new ExchangeAssetInfoUpdatedIntegrationEvent(
                        0,
                        asset.AssetSymbol,
                        asset.TotalBalance,
                        asset.AvailableBalance,
                        asset.PendingBalance
                        ));
                }
            }
            lock (this._binance)
            {
                foreach (var asset in this._binance.Values)
                {
                    this._eventBus.Publish(new ExchangeAssetInfoUpdatedIntegrationEvent(
                        1,
                        asset.AssetSymbol,
                        asset.TotalBalance,
                        asset.AvailableBalance,
                        asset.PendingBalance
                        ));
                }
            }
            lock (this._kraken)
            {
                foreach (var asset in this._kraken.Values)
                {
                    this._eventBus.Publish(new ExchangeAssetInfoUpdatedIntegrationEvent(
                        2,
                        asset.AssetSymbol,
                        asset.TotalBalance,
                        asset.AvailableBalance,
                        asset.PendingBalance
                        ));
                }
            }
            lock (this._bitstamp)
            {
                foreach (var asset in this._bitstamp.Values)
                {
                    this._eventBus.Publish(new ExchangeAssetInfoUpdatedIntegrationEvent(
                        3,
                        asset.AssetSymbol,
                        asset.TotalBalance,
                        asset.AvailableBalance,
                        asset.PendingBalance
                        ));
                }
            }
            lock (this._poloniex)
            {
                foreach (var asset in this._poloniex.Values)
                {
                    this._eventBus.Publish(new ExchangeAssetInfoUpdatedIntegrationEvent(
                        4,
                        asset.AssetSymbol,
                        asset.TotalBalance,
                        asset.AvailableBalance,
                        asset.PendingBalance
                        ));
                }
            }
        }

        public class BalanceData
        {
            public string AssetSymbol { get; set; }
            public decimal TotalBalance { get; set; }
            public decimal AvailableBalance { get; set; }
            public decimal PendingBalance { get; set; }

        }
    }
}
