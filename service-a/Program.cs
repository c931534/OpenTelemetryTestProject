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

// 添加 Controllers 支援
builder.Services.AddControllers();

// 註冊 named HttpClient
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("ServiceB", c =>
{
    c.BaseAddress = new Uri("http://service-b");
});
builder.Services.AddHttpClient("ServiceC", c =>
{
    c.BaseAddress = new Uri("http://service-c");
});
builder.Services.AddHttpClient("ServiceD", c =>
{
    c.BaseAddress = new Uri("http://service-d");
});

// 初始化 OpenTelemetry Metrics
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService("service-a"))
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
                    .AddService("service-a")
                    .AddAttributes(new Dictionary<string, object>
                    {
                        ["service.instance.id"] = Guid.NewGuid().ToString(),
                        ["service.version"] = "1.0.0"
                    }))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            //.AddRuntimeInstrumentation()
            .AddMeter("ServiceA.CustomMetrics")
            .AddOtlpExporter(opt =>
            {
                opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                opt.Endpoint = new Uri("http://collector:4318/v1/metrics");
            });
    });

// 宣告自訂 metrics
var meter = new Meter("ServiceA.CustomMetrics");
var counter = meter.CreateCounter<int>("service_a_test_counter");
var callBCounter = meter.CreateCounter<int>("service_a_call_b_requests");
var app = builder.Build();

// 使用 Controllers
app.MapControllers();

// ✅ 註冊路由
app.MapGet("/", () => "Service A is running");

app.MapGet("/api/test-metric", () =>
{
    counter.Add(1, KeyValuePair.Create<string, object?>("label", "test"));
    return Results.Ok("Metric incremented");
});

// ✅ 不要把這些寫在 `app.Run()` 之後
app.Run();