using CryptoArbitrage.Services.ExchangesAccess.API.Application.Queries;
using CryptoArbitrage.Services.ExchangesAccess.API.Infrastructure.ExchangeAPIs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NJsonSchema.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.ExchangesAccess.API.Application.Utilitys
{
    public class ExchangeResponseWrapper
    {

        /* public const string RESPONSE_GETMARKET = "response_getmarket";
         public const string RESPONSE_GETOPENORDER = "response_getorderbook";
         public const string RESPONSE_GETBALANCES = "response_getbalances";
         public const string RESPONSE_BUYORDER = "response_buyorder";
         public const string RESPONSE_SELLORDER = "response_sellorder";
         public const string RESPONSE_CANCELORDER = "response_cancelorder";*/



        public async static Task<ExchangeApiResponse<ApiResponse>> ParseResponse(string exchangeId, ExchangeApiMethodEnum method, HttpResponseMessage httpResponse)
        {
            string errorMessage = null;
            HttpStatusCode statusCode = httpResponse.StatusCode;

            if (!httpResponse.IsSuccessStatusCode)
            {
                errorMessage = httpResponse.ReasonPhrase;
                method = 0;
            }



            var exchangeResponse = new ExchangeApiResponse<ApiResponse>();
            var formObject = await httpResponse.Content.ReadAsStringAsync();

            JObject res;
            if (!formObject.StartsWith("["))
            {
                res = JObject.Parse(formObject);
                if (res["success"] != null)
                {
                    if (!Boolean.Parse(res["success"].ToString()))
                    {
                        errorMessage = res["message"].ToString();
                        method = 0;
                    }
                }
            }



            switch (method)
            {
                case ExchangeApiMethodEnum.GetMarket:
                    exchangeResponse.Result = GetOrderBookResponse(exchangeId, formObject);
                    break;
                case ExchangeApiMethodEnum.GetBalances:
                    exchangeResponse.Result = GetBalancesResponse(exchangeId, formObject);
                    break;
                case ExchangeApiMethodEnum.GetOpenOrders:
                    exchangeResponse.Result = GetOpenOrdersResponse(exchangeId, formObject);
                    break;
                case ExchangeApiMethodEnum.PlaceBuyOrder:
                    exchangeResponse.Result = PlaceBuyOrderResponse(exchangeId, formObject);
                    break;
                case ExchangeApiMethodEnum.PlaceSellOrder:
                    exchangeResponse.Result = PlaceSellOrderResponse(exchangeId, formObject);
                    break;
                case ExchangeApiMethodEnum.CancelOrder:
                    exchangeResponse.Result = CancelOrderResponse(exchangeId, formObject);
                    break;
                default:
                    break;
            }



            exchangeResponse.StatusCode = statusCode;
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            exchangeResponse.Timestamp = dt;
            exchangeResponse.ErrorMessage = errorMessage;
            //exchangeResponse.Size = responseContent.ToString().Length;

            return exchangeResponse;
        }


        public static OrderBookViewModel GetOrderBookResponse(string exchangeId, string formObject)
        {
            JObject jsonResult = JObject.Parse(formObject);
            OrderBookViewModel orderBook = null;
            var asksList = new List<OrderPriceAndQuantityViewModel>();
            var bidsList = new List<OrderPriceAndQuantityViewModel>();

            if (exchangeId == "0")
            {
                var bidsJson = jsonResult["result"]["buy"];
                var asksJson = jsonResult["result"]["sell"];



                //Alternate Property Name.
                var jsonResolver = new PropertyRenameAndIgnoreSerializerContractResolver();
                jsonResolver.RenameProperty(typeof(OrderPriceAndQuantityViewModel), "price", "rate");
                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.ContractResolver = jsonResolver;

                asksList = JsonConvert.DeserializeObject<List<OrderPriceAndQuantityViewModel>>(asksJson.ToString(), serializerSettings);
                bidsList = JsonConvert.DeserializeObject<List<OrderPriceAndQuantityViewModel>>(bidsJson.ToString(), serializerSettings);
            }
            else if (exchangeId == "1")
            {
                var bidsJson = jsonResult["bids"];
                var asksJson = jsonResult["asks"];


                foreach (var order in asksJson)
                {
                    asksList.Add(
                        new OrderPriceAndQuantityViewModel
                        {
                            price = Decimal.Parse(order[0].ToString()),
                            quantity = Decimal.Parse(order[1].ToString())
                        }
                        );
                }
                foreach (var order in bidsJson)
                {
                    bidsList.Add(
                        new OrderPriceAndQuantityViewModel
                        {
                            price = Decimal.Parse(order[0].ToString()),
                            quantity = Decimal.Parse(order[1].ToString())
                        }
                        );
                }

            }
            else if (exchangeId == "2")
            {
                var data = jsonResult["result"].First.First;
                var bidsJson = data["bids"];
                var asksJson = data["asks"];


                foreach (var order in asksJson)
                {
                    asksList.Add(
                        new OrderPriceAndQuantityViewModel
                        {
                            price = Decimal.Parse(order[0].ToString()),
                            quantity = Decimal.Parse(order[1].ToString())
                        }
                        );
                }
                foreach (var order in bidsJson)
                {
                    bidsList.Add(
                        new OrderPriceAndQuantityViewModel
                        {
                            price = Decimal.Parse(order[0].ToString()),
                            quantity = Decimal.Parse(order[1].ToString())
                        }
                        );
                }

            }
            else if (exchangeId == "3")
            {
                var bidsJson = jsonResult["bids"];
                var asksJson = jsonResult["asks"];


                foreach (var order in asksJson)
                {
                    asksList.Add(
                        new OrderPriceAndQuantityViewModel
                        {
                            price = Decimal.Parse(order[0].ToString()),
                            quantity = Decimal.Parse(order[1].ToString())
                        }
                        );
                    if (asksList.Count >= 100)
                        break;
                }
                foreach (var order in bidsJson)
                {
                    bidsList.Add(
                        new OrderPriceAndQuantityViewModel
                        {
                            price = Decimal.Parse(order[0].ToString()),
                            quantity = Decimal.Parse(order[1].ToString())
                        }
                        );
                    if (bidsList.Count >= 100)
                        break;
                }

            }



            orderBook = new OrderBookViewModel
            {
                bids = bidsList,
                asks = asksList

            };

            return orderBook;

        }


        private static GetOpenOrderViewModel GetOpenOrdersResponse(string exchangeId, string formObject)
        {

            GetOpenOrderViewModel orders = null;
            var ordersList = new List<OrderViewModel>();



            if (exchangeId == "0")
            {
                JObject jsonResult = JObject.Parse(formObject);
                var ordersJson = jsonResult["result"];


                foreach (var order in ordersJson)
                {
                    var orderToAdd = new OrderViewModel
                    {
                        orderId = order["OrderUuid"].ToString(),
                        baseCurrency = order["Exchange"].ToString().Substring(4),
                        quoteCurrency = order["Exchange"].ToString().Substring(0, 3),
                        quantityTotal = order["Quantity"].ToString(),
                        quantityExecuted = (Double.Parse(order["Quantity"].ToString()) - Double.Parse(order["QuantityRemaining"].ToString())).ToString(),
                        openedTime = order["Opened"].ToString(),
                        closedTime = order["Closed"].ToString(),
                        canceled = order["CancelInitiated"].ToString()
                    };

                    ordersList.Add(orderToAdd);
                }

            }
            else if (exchangeId == "1")
            {
                JObject jsonResult = JObject.Parse("{\"result\":" + formObject + "}");

                var ordersJson = jsonResult["result"];

                foreach (var order in ordersJson)
                {
                    double commission = 0;
                    if (order["fills"] != null)
                    {
                        foreach (var f in order["fills"])
                        {
                            commission += Double.Parse(f["commission"].ToString());
                        }
                    }

                    var orderToAdd = new OrderViewModel
                    {
                        orderId = order["orderId"].ToString(),
                        baseCurrency = order["symbol"].ToString().Substring(0, 3),
                        quoteCurrency = order["symbol"].ToString().Substring(3),
                        quantityTotal = order["origQty"].ToString(),
                        quantityExecuted = order["executedQty"].ToString(),
                        openedTime = order["time"].ToString(),
                        closedTime = order["status"].ToString() == "FILLED" ? null : order["updateTime"].ToString(),
                        canceled = order["status"].ToString() == "CANCELLED" ? null : order["updateTime"].ToString(),
                        commissionPaid = commission.ToString()
                    };

                    ordersList.Add(orderToAdd);
                }

            }
            else if (exchangeId == "2")
            {
                JObject jsonResult = JObject.Parse(formObject);
                var ordersJson = jsonResult["result"]["open"];
                var obj = JObject.Parse(ordersJson.ToString());



                foreach (var order in obj)
                {
                    var orderToAdd = new OrderViewModel
                    {
                        orderId = order.Key,
                        baseCurrency = order.Value["descr"]["pair"].ToString().Substring(0, 3),
                        quoteCurrency = order.Value["descr"]["pair"].ToString().Substring(3),
                        quantityTotal = order.Value["vol"].ToString(),
                        quantityExecuted = order.Value["vol_exec"].ToString(),
                        openedTime = order.Value["opentm"].ToString(),
                        closedTime = order.Value["descr"]["close"].ToString(),
                        canceled = order.Value["status"].ToString() == "canceled" ? "true" : "false",
                        commissionPaid = order.Value["fee"].ToString(),
                    };

                    ordersList.Add(orderToAdd);
                }
            }
            else if (exchangeId == "3")
            {
                JObject jsonResult = JObject.Parse("{\"result\":" + formObject + "}");
                var ordersJson = jsonResult["result"];

                foreach (var order in ordersJson)
                {
                    var orderToAdd = new OrderViewModel
                    {
                        orderId = order["id"].ToString(),
                        baseCurrency = order["currency_pair"].ToString().Substring(0, 3),
                        quoteCurrency = order["currency_pair"].ToString().Substring(4),
                        quantityTotal = order["amount"].ToString(),
                        quantityExecuted = null,
                        openedTime = order["datetime"].ToString(),
                        closedTime = null,
                        canceled = null
                    };

                    ordersList.Add(orderToAdd);
                }
            }
            else if (exchangeId == "5")
            {
                JObject jsonResult = JObject.Parse("{\"result\":" + formObject + "}");
                var ordersJson = jsonResult["result"];

                foreach (var order in ordersJson)
                {
                    var orderToAdd = new OrderViewModel
                    {
                        orderId = order["id"].ToString(),
                        baseCurrency = order["currency_pair"].ToString().Substring(0, 3),
                        quoteCurrency = order["currency_pair"].ToString().Substring(4),
                        quantityTotal = order["amount"].ToString(),
                        quantityExecuted = null,
                        openedTime = order["datetime"].ToString(),
                        closedTime = null,
                        canceled = null
                    };

                    ordersList.Add(orderToAdd);
                }
            }


            if (orders == null)
            {
                orders = new GetOpenOrderViewModel
                {
                    orders = ordersList
                };

            }


            return orders;

        }

        private static BalanceViewModel GetBalancesResponse(string exchangeId, string formObject)
        {
            JObject jsonResult = JObject.Parse(formObject);
            BalanceViewModel balances = null;
            var balanceDetailsList = new List<BalanceDetailViewModel>();

            if (exchangeId == "0")
            {

                var balancesJson = jsonResult["result"];

                balanceDetailsList = JsonConvert.DeserializeObject<List<BalanceDetailViewModel>>(balancesJson.ToString());

            }
            else if (exchangeId == "1")
            {
                var balancesJson = jsonResult["balances"];

                foreach (var detail in balancesJson)
                {
                    var totalBalance = Double.Parse(detail["free"].ToString()) + Double.Parse(detail["locked"].ToString());

                    if (totalBalance == 0)
                        continue;

                    balanceDetailsList.Add(
                        new BalanceDetailViewModel
                        {
                            currency = detail["asset"].ToString(),
                            balance = String.Format("{0:F8}", totalBalance),
                            available = detail["free"].ToString(),
                            pending = detail["locked"].ToString()
                        }
                        );
                }
            }
            else if (exchangeId == "2")
            {
                var balancesJson = jsonResult["result"];
                var obj = JObject.Parse(balancesJson.ToString());


                foreach (var asset in obj)
                {


                    balanceDetailsList.Add(
                        new BalanceDetailViewModel
                        {
                            currency = asset.Key.Substring(1),
                            balance = String.Format("{0:F8}", asset.Value.ToString()),
                            /*available = detail["free"].ToString(),
                            pending = detail["locked"].ToString()*/
                        }
                        );
                }

            }
            else if (exchangeId == "3")
            {
                var balancesJson = jsonResult;


                foreach (var detail in balancesJson)
                {
                    if (Double.Parse(detail.Value.ToString()) == 0)
                    {
                        continue;
                    }

                    if (detail.Key.Contains("available", StringComparison.InvariantCulture))
                    {

                        var key = detail.Key.Substring(0, 3);
                        var asset = balanceDetailsList.Where(c => c.currency == key).SingleOrDefault();
                        if (asset == null)
                        {
                            balanceDetailsList.Add(new BalanceDetailViewModel
                            {
                                currency = key,
                                balance = "0",

                                available = detail.Value.ToString(),
                                pending = "0"
                            });
                        }
                        else
                        {
                            asset.available = detail.Value.ToString();
                        }
                    }
                    else if (detail.Key.Contains("balance", StringComparison.InvariantCulture))
                    {
                        var key = detail.Key.Substring(0, 3);
                        var asset = balanceDetailsList.Where(c => c.currency == key).SingleOrDefault();
                        if (asset == null)
                        {
                            balanceDetailsList.Add(new BalanceDetailViewModel
                            {
                                currency = key,
                                balance = detail.Value.ToString(),
                                available = "0",
                                pending = "0",
                            });
                        }
                        else
                        {
                            asset.balance = detail.Value.ToString();
                        }
                    }
                    else if (detail.Key.Contains("reserved"))
                    {
                        var key = detail.Key.Substring(0, 3);
                        var asset = balanceDetailsList.Where(c => c.currency == key).SingleOrDefault();
                        if (asset == null)
                        {
                            balanceDetailsList.Add(new BalanceDetailViewModel
                            {
                                currency = key,
                                balance = "0",
                                available = "0",
                                pending = detail.Value.ToString()
                            });
                        }
                        else
                        {
                            asset.pending = detail.Value.ToString();
                        }
                    }

                    /*balanceDetailsList.Add(
                        new BalanceDetailViewModel
                        {
                            currency = detail["detail"].ToString(),
                            balance = detail[1].ToString(),
                            available = detail[2].ToString(),
                            pending = detail[3].ToString()
                        }
                        );*/
                }

            }
            else if (exchangeId == "4")
            {
                var balancesJson = jsonResult["result"];
                var obj = JObject.Parse(balancesJson.ToString());


                foreach (var asset in obj)
                {


                    balanceDetailsList.Add(
                        new BalanceDetailViewModel
                        {
                            currency = asset.Key.Substring(1),
                            balance = String.Format("{0:F8}", asset.Value.ToString()),
                            /*available = detail["free"].ToString(),
                            pending = detail["locked"].ToString()*/
                        }
                        );
                }
            }



            balances = new BalanceViewModel
            {
                pairs = balanceDetailsList
            };

            return balances;

        }







        private static PlaceBuyOrderViewModel PlaceBuyOrderResponse(string exchangeId, string formObject)
        {
            JObject jsonResult = JObject.Parse(formObject);
            PlaceBuyOrderViewModel buyOrder = null;
            string orderId = string.Empty;

            if (exchangeId == "0")
            {
                var buyOrderJson = jsonResult["result"];

                orderId = buyOrderJson["uuid"].ToString();
            }
            else if (exchangeId == "1")
            {
                var buyOrderJson = jsonResult["orderId"];

                orderId = buyOrderJson.ToString();
            }
            else if (exchangeId == "2")
            {
                var buyOrderJson = jsonResult["result"]["txid"][0];

                orderId = buyOrderJson.ToString();
            }
            else if (exchangeId == "3")
            {
                var buyOrderJson = jsonResult["id"];

                orderId = buyOrderJson.ToString();
            }

            buyOrder = new PlaceBuyOrderViewModel
            {
                orderId = orderId
            };

            return buyOrder;
        }



        private static PlaceSellOrderViewModel PlaceSellOrderResponse(string exchangeId, string formObject)
        {
            JObject jsonResult = JObject.Parse(formObject);
            PlaceSellOrderViewModel sellOrder = null;
            string orderId = string.Empty;

            if (exchangeId == "0")
            {
                var sellOrderJson = jsonResult["result"];

                orderId = sellOrderJson["uuid"].ToString();
            }
            else if (exchangeId == "1")
            {
                var sellOrderJson = jsonResult["orderId"];

                orderId = sellOrderJson.ToString();
            }
            else if (exchangeId == "2")
            {
                var sellOrderJson = jsonResult["result"]["txid"][0];

                orderId = sellOrderJson.ToString();
            }
            else if (exchangeId == "3")
            {
                var sellOrderJson = jsonResult["id"];

                orderId = sellOrderJson.ToString();
            }

            sellOrder = new PlaceSellOrderViewModel
            {
                orderId = orderId
            };

            return sellOrder;

        }



        private static CancelOrderViewModel CancelOrderResponse(string exchangeId, string formObject)
        {
            JObject jsonResult = JObject.Parse(formObject);
            CancelOrderViewModel cancelledOrder = null;
            bool cancelled = false;

            if (exchangeId == "0")
            {
                //if(jsonResult["Success"]==)
                cancelled = true;
            }
            else if (exchangeId == "1")
            {

                cancelled = true;
            }
            else if (exchangeId == "2")
            {

                cancelled = Int32.Parse(jsonResult["result"]["count"].ToString()) > 0 ? true : false;
            }
            else if (exchangeId == "3")
            {
                cancelled = true;

            }



            cancelledOrder = new CancelOrderViewModel
            {
                orderCancelled = cancelled
            };

            return cancelledOrder;

        }


        private bool IsResponseValid(HttpResponseMessage response)
        {
            if ((response != null) && (response.StatusCode == HttpStatusCode.OK))
                return true;
            return false;
        }



    }


    public class CustomDataContractResolver : DefaultContractResolver
    {
        public static readonly CustomDataContractResolver Instance = new CustomDataContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (property.DeclaringType == typeof(OrderPriceAndQuantityViewModel))
            {
                if (property.PropertyName.Equals("Rate", StringComparison.OrdinalIgnoreCase))
                {
                    property.PropertyName = "Price";
                }
            }
            return property;
        }
    }
}
