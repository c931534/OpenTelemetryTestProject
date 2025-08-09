using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;

[ApiController]
[Route("api")]
public class HelloController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly Meter Meter = new("ServiceE.CustomMetrics");
    private static readonly Counter<int> CallBErrorCounter = Meter.CreateCounter<int>("service_e_callb_error_count");
    private static readonly Counter<int> CallCErrorCounter = Meter.CreateCounter<int>("service_e_callc_error_count");
    private static readonly Counter<int> CallDErrorCounter = Meter.CreateCounter<int>("service_e_calld_error_count");
    
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
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/hello");
            request.Headers.Add("X-From", "service-e");
            
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                return Ok($"ServiceB said: {result}");
            }
            else
            {
                CallBErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
                return StatusCode((int)response.StatusCode, $"ServiceB error: {result}");
            }
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
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/hello");
            request.Headers.Add("X-From", "service-e");
            
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                return Ok($"ServiceC said: {result}");
            }
            else
            {
                CallCErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
                return StatusCode((int)response.StatusCode, $"ServiceC error: {result}");
            }
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
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/hello");
            request.Headers.Add("X-From", "service-e");
            
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                return Ok($"ServiceD said: {result}");
            }
            else
            {
                CallDErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
                return StatusCode((int)response.StatusCode, $"ServiceD error: {result}");
            }
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
            var requestB = new HttpRequestMessage(HttpMethod.Get, "/api/hello");
            requestB.Headers.Add("X-From", "service-e");
            
            var responseB = await clientB.SendAsync(requestB);
            var resultB = await responseB.Content.ReadAsStringAsync();
            
            if (responseB.IsSuccessStatusCode)
            {
                results.Add($"ServiceB: {resultB}");
            }
            else
            {
                CallBErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
                results.Add($"ServiceB: Error - {resultB}");
            }
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
            var requestC = new HttpRequestMessage(HttpMethod.Get, "/api/hello");
            requestC.Headers.Add("X-From", "service-e");
            
            var responseC = await clientC.SendAsync(requestC);
            var resultC = await responseC.Content.ReadAsStringAsync();
            
            if (responseC.IsSuccessStatusCode)
            {
                results.Add($"ServiceC: {resultC}");
            }
            else
            {
                CallCErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
                results.Add($"ServiceC: Error - {resultC}");
            }
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
            var requestD = new HttpRequestMessage(HttpMethod.Get, "/api/hello");
            requestD.Headers.Add("X-From", "service-e");
            
            var responseD = await clientD.SendAsync(requestD);
            var resultD = await responseD.Content.ReadAsStringAsync();
            
            if (responseD.IsSuccessStatusCode)
            {
                results.Add($"ServiceD: {resultD}");
            }
            else
            {
                CallDErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
                results.Add($"ServiceD: Error - {resultD}");
            }
        }
        catch (HttpRequestException ex)
        {
            CallDErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            results.Add($"ServiceD: Error - {ex.Message}");
        }
        
        return Ok(new { message = "All services called", results });
    }
} 