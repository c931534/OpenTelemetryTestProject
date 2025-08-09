using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Collections.Generic;
using System;

[ApiController]
[Route("api")]
public class HelloController : ControllerBase
{
    private readonly Counter<int> _requestCounter;
    private readonly Counter<int> _exceptionCounter;

    public HelloController()
    {
        var meter = new Meter("ServiceD.CustomMetrics");
        _requestCounter = meter.CreateCounter<int>("service_d_hello_requests");
        _exceptionCounter = meter.CreateCounter<int>("service_d_exception_count");
    }

    [HttpGet("hello")]
    public IActionResult Hello()
    {
        _requestCounter.Add(1, KeyValuePair.Create<string, object?>("endpoint", "hello"));
        
        // 檢查是否來自 service-e
        if (Request.Headers.ContainsKey("X-From") && Request.Headers["X-From"].ToString() == "service-e")
        {
            _exceptionCounter.Add(1, KeyValuePair.Create<string, object?>("from", "service-e"));
            _exceptionCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "intentional"));
            
            throw new InvalidOperationException("ServiceD intentionally throwing exception for service-e request");
        }
        
        return Ok("Hello from ServiceD");
    }
} 