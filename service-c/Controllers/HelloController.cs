using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("api")]
public class HelloController : ControllerBase
{
    private readonly Counter<int> _requestCounter;
    private readonly Counter<int> _delayCounter;

    public HelloController()
    {
        var meter = new Meter("ServiceC.CustomMetrics");
        _requestCounter = meter.CreateCounter<int>("payment_service_requests_total");
        _delayCounter = meter.CreateCounter<int>("payment_service_delay_count");
    }

    [HttpGet("hello")]
    public async Task<IActionResult> Hello()
    {
        _requestCounter.Add(1, KeyValuePair.Create<string, object?>("endpoint", "hello"));
        
        // 檢查是否來自 service-e (後台管理)
        if (Request.Headers.ContainsKey("X-From") && Request.Headers["X-From"].ToString() == "service-e")
        {
            var random = new Random();
            var delaySeconds = random.Next(3, 6);
            await Task.Delay(delaySeconds * 1000);
            
            _delayCounter.Add(1, KeyValuePair.Create<string, object?>("from", "backend_admin"));
            _delayCounter.Add(1, KeyValuePair.Create<string, object?>("delay_seconds", delaySeconds));
            
            return Ok($"Hello from Payment Service (delayed {delaySeconds}s for backend admin)");
        }
        
        return Ok("Hello from Payment Service");
    }
} 