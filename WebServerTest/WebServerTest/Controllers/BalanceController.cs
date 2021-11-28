using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WebServerTest.Models;

namespace WebServerTest.Controllers
{
    public class AmountRequestBody
    {
        [JsonPropertyName("account_id")]
        public string accountId { get; set; }

        [JsonPropertyName("amount")]
        public int? amount { get; set; }
    }

    public class BalanceResponse
    {
        [JsonPropertyName("balance")]
        public int balance { get; set; } = 0;
    }

    public class BalanceController
    {
        BalanceModel BalanceModel = new BalanceModel();

        public BalanceController()
        {

        }

        internal async System.Threading.Tasks.Task GetBalanceAsync(HttpContext context)
        {
            string accountId = context.GetRouteValue("id") as string;
            BalanceResponse response = new BalanceResponse();
            var amount = BalanceModel.GetAmount(accountId);

            if (!amount.HasValue)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Account not found");
                return;
            }
            else
            {
                response.balance = amount.Value;
            }

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        internal async System.Threading.Tasks.Task AddAmountAsync(HttpContext context)
        {
            if (context.Request.ContentType != "application/json")
            {
                context.Response.StatusCode = 415;
                await context.Response.WriteAsync("Endpoint only accepts JSON content");
                return;
            }

            using (var reader = new StreamReader(context.Request.Body))
            {
                string body = await reader.ReadToEndAsync();
                AmountRequestBody data = new AmountRequestBody();

                try
                {
                    data = JsonSerializer.Deserialize<AmountRequestBody>(body);
                }
                catch(Exception ex)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync(ex.Message);
                    return;
                }

                if (data.accountId == null || data.accountId.Length == 0)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Endpoint requires account_id in body.");
                    return;
                }

                if (data.amount == null)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Endpoint requires balance in body.");
                    return;
                }

                BalanceResponse response = new BalanceResponse();
                response.balance = BalanceModel.AddAmount(data.accountId, data.amount.Value);

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }

        }
    }
}
