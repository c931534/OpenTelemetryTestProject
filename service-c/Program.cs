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
var meter = new Meter("ServiceC.CustomMetrics");
var counter = meter.CreateCounter<int>("service_c_test_counter");
var requestCounter = meter.CreateCounter<int>("service_c_requests_total");

// 加入 OpenTelemetry：Metrics + Tracing
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService("service-c")
                    .AddAttributes(new Dictionary<string, object>
                    {
                        ["service.instance.id"] = Guid.NewGuid().ToString(),
                        ["service.version"] = "1.0.0"
                    }))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            //.AddRuntimeInstrumentation()
            .AddMeter("ServiceC.CustomMetrics")
            .AddOtlpExporter(opt =>
            {
                opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                opt.Endpoint = new Uri("http://collector:4318/v1/metrics");
            });
    })
    .WithTracing(tracing =>
    {
        tracing
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService("service-c"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(opt =>
            {
                opt.Protocol = OtlpExportProtocol.Grpc;
                opt.Endpoint = new Uri("http://collector:4317");
            });
    });

var app = builder.Build();

// 註冊 Controllers
app.MapControllers();

// 加一個 root 測試頁
app.MapGet("/", () => "Service C is running");

// 添加測試 metrics 的端點
app.MapGet("/api/test-metric", () =>
{
    counter.Add(1, KeyValuePair.Create<string, object?>("label", "test"));
    return Results.Ok("Service C metric incremented");
});

app.Run(); 