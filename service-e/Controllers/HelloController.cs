using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;

[ApiController]
[Route("api/admin")]
public class HelloController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly Meter Meter = new("ServiceE.CustomMetrics");
    private static readonly Counter<int> CallBErrorCounter = Meter.CreateCounter<int>("backend_promotion_management_error_count");
    private static readonly Counter<int> CallCErrorCounter = Meter.CreateCounter<int>("backend_payment_management_error_count");
    private static readonly Counter<int> CallDErrorCounter = Meter.CreateCounter<int>("backend_thirdparty_management_error_count");
    
    public HelloController(IHttpClientFactory factory)
    {
        _httpClientFactory = factory;
    }

    [HttpGet("promotion-management")]
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
                return Ok($"Promotion Management: {result}");
            }
            else
            {
                CallBErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
                return StatusCode((int)response.StatusCode, $"Promotion Management error: {result}");
            }
        }
        catch (HttpRequestException ex)
        {
            CallBErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            return StatusCode(502, $"Promotion Management unreachable: {ex.Message}");
        }
        catch (System.Exception ex)
        {
            CallBErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "other"));
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    [HttpGet("payment-management")]
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
                return Ok($"Payment Management: {result}");
            }
            else
            {
                CallCErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
                return StatusCode((int)response.StatusCode, $"Payment Management error: {result}");
            }
        }
        catch (HttpRequestException ex)
        {
            CallCErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            return StatusCode(502, $"Payment Management unreachable: {ex.Message}");
        }
        catch (System.Exception ex)
        {
            CallCErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "other"));
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    [HttpGet("thirdparty-management")]
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
                return Ok($"Third-party Management: {result}");
            }
            else
            {
                CallDErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
                return StatusCode((int)response.StatusCode, $"Third-party Management error: {result}");
            }
        }
        catch (HttpRequestException ex)
        {
            CallDErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            return StatusCode(502, $"Third-party Management unreachable: {ex.Message}");
        }
        catch (System.Exception ex)
        {
            CallDErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "other"));
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    [HttpGet("all-management")]
    public async Task<IActionResult> CallAll()
    {
        var results = new List<string>();
        
        // 調用 Promotion Management
        try
        {
            var clientB = _httpClientFactory.CreateClient("ServiceB");
            var requestB = new HttpRequestMessage(HttpMethod.Get, "/api/hello");
            requestB.Headers.Add("X-From", "service-e");
            
            var responseB = await clientB.SendAsync(requestB);
            var resultB = await responseB.Content.ReadAsStringAsync();
            
            if (responseB.IsSuccessStatusCode)
            {
                results.Add($"Promotion Management: {resultB}");
            }
            else
            {
                CallBErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
                results.Add($"Promotion Management: Error - {resultB}");
            }
        }
        catch (HttpRequestException ex)
        {
            CallBErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            results.Add($"Promotion Management: Error - {ex.Message}");
        }
        
        // 調用 Payment Management
        try
        {
            var clientC = _httpClientFactory.CreateClient("ServiceC");
            var requestC = new HttpRequestMessage(HttpMethod.Get, "/api/hello");
            requestC.Headers.Add("X-From", "service-e");
            
            var responseC = await clientC.SendAsync(requestC);
            var resultC = await responseC.Content.ReadAsStringAsync();
            
            if (responseC.IsSuccessStatusCode)
            {
                results.Add($"Payment Management: {resultC}");
            }
            else
            {
                CallCErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
                results.Add($"Payment Management: Error - {resultC}");
            }
        }
        catch (HttpRequestException ex)
        {
            CallCErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            results.Add($"Payment Management: Error - {ex.Message}");
        }
        
        // 調用 Third-party Management
        try
        {
            var clientD = _httpClientFactory.CreateClient("ServiceD");
            var requestD = new HttpRequestMessage(HttpMethod.Get, "/api/hello");
            requestD.Headers.Add("X-From", "service-e");
            
            var responseD = await clientD.SendAsync(requestD);
            var resultD = await responseD.Content.ReadAsStringAsync();
            
            if (responseD.IsSuccessStatusCode)
            {
                results.Add($"Third-party Management: {resultD}");
            }
            else
            {
                CallDErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
                results.Add($"Third-party Management: Error - {resultD}");
            }
        }
        catch (HttpRequestException ex)
        {
            CallDErrorCounter.Add(1, KeyValuePair.Create<string, object?>("error_type", "http"));
            results.Add($"Third-party Management: Error - {ex.Message}");
        }
        
        return Ok(new { message = "All management services called from Backend", results });
    }
} 