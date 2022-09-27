using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using CloudNative.CloudEvents;
using CompanyName.Shared.Events.CloudEvents;

namespace CompanyName.SharedFunctionApp
{
    public class EventFunctions
    {
        private readonly ICloudEventsSubscriberService _subscriber;

        public EventFunctions()
        {
            _subscriber = new CompanyName.Shared.Events.CloudEvents.CloudEventsSubscriberService();
        }
        [FunctionName(nameof(TestValidation))]
        public async Task<HttpResponseMessage> TestValidation(
            [HttpTrigger(AuthorizationLevel.Anonymous, "options", Route = "download")] HttpRequestMessage req,
            ILogger log)
        {
            try
            {
                var response = await _subscriber.HandleSubscriptionValidationEvent(req);
                return response;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Unhandled exception");
                return await Task.FromResult(req.CreateResponse(new BadRequestObjectResult(ex.Message)));
            }
        }
        [FunctionName(nameof(TestSubscription))]
        public IActionResult TestSubscription(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "download")] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var (evt, userid, id) = _subscriber.DeconstructEventGridMessage(req);

            return new OkObjectResult(evt);
        }
    }
}
