using CryptoArbitrage.Services.Execution.Domain.Model.Arbitrages;
using CryptoArbitrage.Services.Execution.Domain.Model.Orders;
using CryptoArbitrage.Services.Execution.Infrastructure;
using CryptoArbitrage.Services.Execution.WebAPI.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoArbitrage.Services.Execution.WebAPI.Infrastructure
{
    public class ExecutionContextSeed
    {
        public async Task SeedAsync(ExecutionContext context, IHostingEnvironment env, IOptions<ExecutionSettings> settings, ILogger<ExecutionContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(ExecutionContextSeed));

            await policy.ExecuteAsync(async () =>
            {


            });
            var useCustomizationData = settings.Value
              .UseCustomizationData;

            var contentRootPath = env.ContentRootPath;



            //context.Database.Migrate();


            if (!context.OrderStatus.Any())
            {
                context.OrderStatus.AddRange(useCustomizationData
                                        ? GetOrderStatusFromFile(contentRootPath, logger)
                                        : GetPredefinedOrderStatus());

                await context.SaveChangesAsync();
            }

            if (!context.OrderTypes.Any())
            {
                context.OrderTypes.AddRange(useCustomizationData
                                        ? GetOrderTypesFromFile(contentRootPath, logger)
                                        : GetPredefinedOrderTypes());
            }

            if (!context.SimpleArbitrageStatus.Any())
            {
                context.SimpleArbitrageStatus.AddRange(useCustomizationData
                                        ? GetSimpleArbitrageStatusFromFile(contentRootPath, logger)
                                        : GetPredefinedSimpleArbitrageStatus());
            }
            await context.SaveChangesAsync();

        }

        public static async Task SimpleSeedAsync(IApplicationBuilder applicationBuilder)
        {

            var context = (ExecutionContext)applicationBuilder
             .ApplicationServices.GetService(typeof(ExecutionContext));

            using (context)
            {
                context.Database.Migrate();
                if (!context.OrderStatus.Any())
                {
                    context.OrderStatus.AddRange(GetPredefinedOrderStatus());

                    await context.SaveChangesAsync();
                }

                if (!context.OrderTypes.Any())
                {
                    context.OrderTypes.AddRange(GetPredefinedOrderTypes());
                }

                if (!context.SimpleArbitrageStatus.Any())
                {
                    context.SimpleArbitrageStatus.AddRange(GetPredefinedSimpleArbitrageStatus());
                }
                await context.SaveChangesAsync();
            }
        }
        #region OrderStatus
        private IEnumerable<OrderStatus> GetOrderStatusFromFile(string contentRootPath, ILogger<ExecutionContextSeed> log)
        {
            string csvFileOrderStatus = Path.Combine(contentRootPath, "Setup", "OrderStatus.csv");

            if (!File.Exists(csvFileOrderStatus))
            {
                return GetPredefinedOrderStatus();
            }

            string[] csvheaders;

            try
            {
                string[] requiredHeaders = { "OrderStatus" };
                csvheaders = GetHeaders(requiredHeaders, csvFileOrderStatus);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return GetPredefinedOrderStatus();
            }

            int id = 1;
            return File.ReadAllLines(csvFileOrderStatus)
                                        .Skip(1) // skip header column
                                        .SelectTry(x => CreateOrderStatus(x, ref id))
                                        .OnCaughtException(ex => { log.LogError(ex.Message); return null; })
                                        .Where(x => x != null);
        }

        private OrderStatus CreateOrderStatus(string value, ref int id)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new Exception("Orderstatus is null or empty");
            }

            return new OrderStatus(id++, value.Trim('"').Trim());
        }

        private static IEnumerable<OrderStatus> GetPredefinedOrderStatus()
        {
            return new List<OrderStatus>()
            {
                OrderStatus.Started,
                OrderStatus.Submitted,
                OrderStatus.Rejected,
                OrderStatus.Created,
                OrderStatus.PartiallyFilled,
                OrderStatus.Filled,
                OrderStatus.Canceled
            };
        }
        #endregion


        #region OrderType
        private IEnumerable<OrderType> GetOrderTypesFromFile(string contentRootPath, ILogger<ExecutionContextSeed> log)
        {
            string csvFileOrderTypes = Path.Combine(contentRootPath, "Setup", "OrderTypes.csv");

            if (!File.Exists(csvFileOrderTypes))
            {
                return GetPredefinedOrderTypes();
            }

            string[] csvheaders;
            try
            {
                string[] requiredHeaders = { "OrderType" };
                csvheaders = GetHeaders(requiredHeaders, csvFileOrderTypes);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return GetPredefinedOrderTypes();
            }

            int id = 1;
            return File.ReadAllLines(csvFileOrderTypes)
                                        .Skip(1) // skip header row
                                        .SelectTry(x => CreateOrderTypes(x, ref id))
                                        .OnCaughtException(ex => { log.LogError(ex.Message); return null; })
                                        .Where(x => x != null);
        }

        private OrderType CreateOrderTypes(string value, ref int id)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new Exception("Ordertypes is null or empty");
            }

            return new OrderType(id++, value.Trim('"').Trim().ToLowerInvariant());
        }

        private static IEnumerable<OrderType> GetPredefinedOrderTypes()
        {
            return new List<OrderType>()
            {
                OrderType.BUY_LIMIT,
                OrderType.SELL_LIMIT
            };
        }
        #endregion


        #region SimpleArbitrageStatus
        private IEnumerable<SimpleArbitrageStatus> GetSimpleArbitrageStatusFromFile(string contentRootPath, ILogger<ExecutionContextSeed> log)
        {
            string csvFileSimpleArbitrageStatus = Path.Combine(contentRootPath, "Setup", "SimpleArbitrageStatus.csv");

            if (!File.Exists(csvFileSimpleArbitrageStatus))
            {
                return GetPredefinedSimpleArbitrageStatus();
            }

            string[] csvheaders;
            try
            {
                string[] requiredHeaders = { "SimpleArbitrageStatus" };
                csvheaders = GetHeaders(requiredHeaders, csvFileSimpleArbitrageStatus);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return GetPredefinedSimpleArbitrageStatus();
            }

            int id = 1;
            return File.ReadAllLines(csvFileSimpleArbitrageStatus)
                                        .Skip(1) // skip header row
                                        .SelectTry(x => CreateSimpleArbitrageStatus(x, ref id))
                                        .OnCaughtException(ex => { log.LogError(ex.Message); return null; })
                                        .Where(x => x != null);
        }

        private SimpleArbitrageStatus CreateSimpleArbitrageStatus(string value, ref int id)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new Exception("Simplearbitragestatus is null or empty");
            }

            return new SimpleArbitrageStatus(id++, value.Trim('"').Trim().ToLowerInvariant());
        }

        private static IEnumerable<SimpleArbitrageStatus> GetPredefinedSimpleArbitrageStatus()
        {
            return new List<SimpleArbitrageStatus>()
            {
                SimpleArbitrageStatus.Opened,
                SimpleArbitrageStatus.OrderPartiallyCreated,
                SimpleArbitrageStatus.OrderFullCreated,
                SimpleArbitrageStatus.OrderPartiallyFilled,
                SimpleArbitrageStatus.OrderFullFilled,
                SimpleArbitrageStatus.Closed
            };
        }
        #endregion


        private string[] GetHeaders(string[] requiredHeaders, string csvfile)
        {
            string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

            if (csvheaders.Count() != requiredHeaders.Count())
            {
                throw new Exception($"requiredHeader count '{ requiredHeaders.Count()}' is different then read header '{csvheaders.Count()}'");
            }

            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvheaders;
        }


        private Policy CreatePolicy(ILogger<ExecutionContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogTrace($"[{prefix}] Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retries}");
                    }
                );
        }
    }
}
