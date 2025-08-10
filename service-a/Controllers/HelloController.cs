using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/member")]
public class HelloController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly Meter Meter = new("ServiceA.CustomMetrics");
    private static readonly Counter<int> CallBErrorCounter = Meter.CreateCounter<int>("frontend_promotion_error_count");
    private static readonly Counter<int> CallCErrorCounter = Meter.CreateCounter<int>("frontend_payment_error_count");
    private static readonly Counter<int> CallDErrorCounter = Meter.CreateCounter<int>("frontend_thirdparty_error_count");
    
    public HelloController(IHttpClientFactory factory)
    {
        _httpClientFactory = factory;
    }

    [HttpGet("promotion")]
    public async Task<IActionResult> CallB()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ServiceB");
            var result = await client.GetStringAsync("/api/hello");
            return Ok($"Promotion Service: {result}");
        }
        catch (HttpRequestException ex)
        {
            CallBErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            return StatusCode(502, $"Promotion Service unreachable: {ex.Message}");
        }
        catch (System.Exception ex)
        {
            CallBErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "other"));
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    [HttpGet("payment")]
    public async Task<IActionResult> CallC()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ServiceC");
            var result = await client.GetStringAsync("/api/hello");
            return Ok($"Payment Service: {result}");
        }
        catch (HttpRequestException ex)
        {
            CallCErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            return StatusCode(502, $"Payment Service unreachable: {ex.Message}");
        }
        catch (System.Exception ex)
        {
            CallCErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "other"));
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    [HttpGet("thirdparty")]
    public async Task<IActionResult> CallD()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ServiceD");
            var result = await client.GetStringAsync("/api/hello");
            return Ok($"Third-party Service: {result}");
        }
        catch (HttpRequestException ex)
        {
            CallDErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            return StatusCode(502, $"Third-party Service unreachable: {ex.Message}");
        }
        catch (System.Exception ex)
        {
            CallDErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "other"));
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    [HttpGet("all-services")]
    public async Task<IActionResult> CallAll()
    {
        var results = new List<string>();
        
        // 調用 Promotion Service
        try
        {
            var clientB = _httpClientFactory.CreateClient("ServiceB");
            var resultB = await clientB.GetStringAsync("/api/hello");
            results.Add($"Promotion Service: {resultB}");
        }
        catch (HttpRequestException ex)
        {
            CallBErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            results.Add($"Promotion Service: Error - {ex.Message}");
        }
        
        // 調用 Payment Service
        try
        {
            var clientC = _httpClientFactory.CreateClient("ServiceC");
            var resultC = await clientC.GetStringAsync("/api/hello");
            results.Add($"Payment Service: {resultC}");
        }
        catch (HttpRequestException ex)
        {
            CallCErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            results.Add($"Payment Service: Error - {ex.Message}");
        }
        
        // 調用 Third-party Service
        try
        {
            var clientD = _httpClientFactory.CreateClient("ServiceD");
            var resultD = await clientD.GetStringAsync("/api/hello");
            results.Add($"Third-party Service: {resultD}");
        }
        catch (HttpRequestException ex)
        {
            CallDErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            results.Add($"Third-party Service: Error - {ex.Message}");
        }
        
        return Ok(new { message = "All services called from Frontend", results });
    }
}
