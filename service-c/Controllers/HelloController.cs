using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Collections.Generic;

[ApiController]
[Route("api")]
public class HelloController : ControllerBase
{
    private readonly Counter<int> _requestCounter;

    public HelloController()
    {
        var meter = new Meter("ServiceC.CustomMetrics");
        _requestCounter = meter.CreateCounter<int>("service_c_hello_requests");
    }

    [HttpGet("hello")]
    public IActionResult Hello()
    {
        _requestCounter.Add(1, KeyValuePair.Create<string, object?>("endpoint", "hello"));
        return Ok("Hello from ServiceC");
    }
} 