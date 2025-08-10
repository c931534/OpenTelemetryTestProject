using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Internal;

var builder = WebApplication.CreateBuilder(args);

// 加入 Controller 支援
builder.Services.AddControllers();

// 加入 HttpClient 支援（如果有需要呼叫外部服務）
builder.Services.AddHttpClient();

// 宣告自訂 metrics
var meter = new Meter("ServiceB.CustomMetrics");
var counter = meter.CreateCounter<int>("service_b_test_counter");
var requestCounter = meter.CreateCounter<int>("service_b_requests_total");

// 初始化 OpenTelemetry Metrics
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService("promotion-service"))
            .AddOtlpExporter(opt =>
            {
                opt.Endpoint = new Uri("http://collector:4317");
                opt.Protocol = OtlpExportProtocol.Grpc;
            });
    })
    .WithMetrics(metrics =>
    {
        metrics
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService("promotion-service")
                    .AddAttributes(new Dictionary<string, object>
                    {
                        ["service.instance.id"] = Guid.NewGuid().ToString(),
                        ["service.version"] = "1.0.0"
                    }))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            //.AddRuntimeInstrumentation()
            .AddMeter("ServiceB.CustomMetrics")
            .AddOtlpExporter(opt =>
            {
                opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                opt.Endpoint = new Uri("http://collector:4318/v1/metrics");
            });
    });

var app = builder.Build();

// 註冊 Controllers
app.MapControllers();

// 加一個 root 測試頁
app.MapGet("/", () => "Promotion Service is running");

// 添加測試 metrics 的端點
app.MapGet("/api/test-metric", () =>
{
    counter.Add(1, KeyValuePair.Create<string, object?>("label", "test"));
    return Results.Ok("Service B metric incremented");
});

app.Run();