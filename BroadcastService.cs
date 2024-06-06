using System.Text.Json;
using BerkutSignalrBroadcaster.Models;
using BerkutSignalrBroadcaster.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BerkutSignalrBroadcaster
{
    public class BroadcastService(ILogger<BroadcastService> logger, IOptionsSnapshot<BroadcastServiceOptions> options)
    {
        private readonly ILogger<BroadcastService> _logger = logger;
        private readonly BroadcastServiceOptions _options = options.Value;

        [Function("devicestatus")]
        [SignalROutput(HubName = "lampstatus")]
        public async Task<SignalRMessageAction?> ReportDeviceStatus([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            var deviceData = await req.ReadFromJsonAsync<DeviceData>();

            if (deviceData is null || deviceData.Status is null)
                throw new ArgumentException("Cannot read device data");

            var lampStatus = deviceData.Status.FirstOrDefault(status => _options.LampStatusCodes.Contains(status.Code));

            return lampStatus is null
                ? default
                : new SignalRMessageAction("lampstatuschanged", [new LampStatusMessage(
                        ((JsonElement)lampStatus.Value).GetBoolean()
                    )]);
        }

        [Function("negotiate")]
        public static IActionResult Negotiate([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
        [SignalRConnectionInfoInput(HubName = "lampstatus")] string connectionInfo) =>
            new OkObjectResult(connectionInfo);
    }
}
