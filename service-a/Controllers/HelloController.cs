using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api")]
public class HelloController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly Meter Meter = new("ServiceA.CustomMetrics");
    private static readonly Counter<int> CallBErrorCounter = Meter.CreateCounter<int>("service_a_callb_error_count");
    private static readonly Counter<int> CallCErrorCounter = Meter.CreateCounter<int>("service_a_callc_error_count");
    private static readonly Counter<int> CallDErrorCounter = Meter.CreateCounter<int>("service_a_calld_error_count");
    
    public HelloController(IHttpClientFactory factory)
    {
        _httpClientFactory = factory;
    }

    [HttpGet("call-b")]
    public async Task<IActionResult> CallB()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ServiceB");
            var result = await client.GetStringAsync("/api/hello");
            return Ok($"ServiceB said: {result}");
        }
        catch (HttpRequestException ex)
        {
            CallBErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            return StatusCode(502, $"ServiceB unreachable: {ex.Message}");
        }
        catch (System.Exception ex)
        {
            CallBErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "other"));
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    [HttpGet("call-c")]
    public async Task<IActionResult> CallC()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ServiceC");
            var result = await client.GetStringAsync("/api/hello");
            return Ok($"ServiceC said: {result}");
        }
        catch (HttpRequestException ex)
        {
            CallCErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            return StatusCode(502, $"ServiceC unreachable: {ex.Message}");
        }
        catch (System.Exception ex)
        {
            CallCErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "other"));
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    [HttpGet("call-d")]
    public async Task<IActionResult> CallD()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ServiceD");
            var result = await client.GetStringAsync("/api/hello");
            return Ok($"ServiceD said: {result}");
        }
        catch (HttpRequestException ex)
        {
            CallDErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            return StatusCode(502, $"ServiceD unreachable: {ex.Message}");
        }
        catch (System.Exception ex)
        {
            CallDErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "other"));
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    [HttpGet("call-all")]
    public async Task<IActionResult> CallAll()
    {
        var results = new List<string>();
        
        // 調用 ServiceB
        try
        {
            var clientB = _httpClientFactory.CreateClient("ServiceB");
            var resultB = await clientB.GetStringAsync("/api/hello");
            results.Add($"ServiceB: {resultB}");
        }
        catch (HttpRequestException ex)
        {
            CallBErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            results.Add($"ServiceB: Error - {ex.Message}");
        }
        
        // 調用 ServiceC
        try
        {
            var clientC = _httpClientFactory.CreateClient("ServiceC");
            var resultC = await clientC.GetStringAsync("/api/hello");
            results.Add($"ServiceC: {resultC}");
        }
        catch (HttpRequestException ex)
        {
            CallCErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            results.Add($"ServiceC: Error - {ex.Message}");
        }
        
        // 調用 ServiceD
        try
        {
            var clientD = _httpClientFactory.CreateClient("ServiceD");
            var resultD = await clientD.GetStringAsync("/api/hello");
            results.Add($"ServiceD: {resultD}");
        }
        catch (HttpRequestException ex)
        {
            CallDErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            results.Add($"ServiceD: Error - {ex.Message}");
        }
        
        return Ok(new { message = "All services called", results });
    }
}
