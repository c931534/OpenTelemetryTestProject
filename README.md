# OpenTelemetry .NET Demo

這是一個使用 OpenTelemetry 1.10.0 的 .NET 微服務架構演示專案，展示了如何在不同服務間收集和傳遞 telemetry 數據。

## 🏗️ 架構概述

### 服務架構
```
┌─────────────────┐    ┌─────────────────┐
│   Frontend      │    │    Backend      │
│   Service A     │    │   Service E     │
│  (會員服務)      │    │  (管理員服務)    │
└─────────┬───────┘    └─────────┬───────┘
          │                      │
          │                      │
          ▼                      ▼
    ┌─────────────────────────────────────┐
    │           Business Services         │
    │  ┌─────────┐ ┌─────────┐ ┌─────────┐ │
    │  │Service B│ │Service C│ │Service D│ │
    │  │Promotion│ │Payment  │ │Third-   │ │
    │  │(行銷)   │ │(金流)   │ │party    │ │
    │  │         │ │         │ │(第三方)  │ │
    │  └─────────┘ └─────────┘ └─────────┘ │
    └─────────────────────────────────────┘
```

### 服務角色說明

#### 上層服務 (Frontend/Backend)
- **Service A (前台服務)**: 會員相關的 API 服務，提供會員功能介面
- **Service E (後台服務)**: 內部管理人員的 API 服務，提供管理功能介面

#### 下層服務 (Business Services)
- **Service B (Promotion 服務)**: 行銷活動、優惠券、折扣等服務
- **Service C (Payment 服務)**: 金流、支付、退款等服務  
- **Service D (Third-party 服務)**: 外部整合服務，如簡訊、郵件等

### 服務調用流程
1. **前台流程**: 會員透過 Service A 調用下層業務服務
2. **後台流程**: 管理員透過 Service E 調用下層業務服務進行管理
3. **業務服務**: 下層服務處理具體業務邏輯，並回傳結果

## 🎯 專案 Demo 目標

### 1. OpenTelemetry 整合展示
- 展示如何在 .NET 微服務中整合 OpenTelemetry
- 演示 Metrics、Traces 和 Logs 的收集
- 展示服務間調用的追蹤能力

### 2. 微服務架構監控
- 展示多層服務架構的監控方式
- 演示前台和後台服務的不同監控維度
- 展示業務服務的性能監控

### 3. 實際業務場景模擬
- 模擬真實的會員和管理員操作場景
- 展示不同服務類型的監控需求
- 演示服務延遲和錯誤的監控

### 4. 監控數據分析
- 提供 Prometheus 和 Grafana 的監控面板
- 展示不同維度的 metrics 分析
- 演示服務依賴關係的可視化

## 🚀 快速開始

### 前置需求
- Docker 和 Docker Compose
- .NET 8.0 SDK

### 啟動服務
```bash
# 啟動所有服務
docker-compose up -d

# 查看服務狀態
docker-compose ps
```

### 測試 API
```bash
# 前台服務測試
curl http://localhost:5001/api/member/promotion
curl http://localhost:5001/api/member/payment
curl http://localhost:5001/api/member/thirdparty
curl http://localhost:5001/api/member/all-services

# 後台服務測試
curl http://localhost:5005/api/admin/promotion-management
curl http://localhost:5005/api/admin/payment-management
curl http://localhost:5005/api/admin/thirdparty-management
curl http://localhost:5005/api/admin/all-management
```

## 📊 監控和指標

### Metrics 命名規範
- **前台服務**: `frontend_*_requests_total`, `frontend_*_error_count`
- **後台服務**: `backend_*_management_requests_total`, `backend_*_management_error_count`
- **業務服務**: `*_service_requests_total`, `*_service_delay_count`

### 監控維度
- `service_type`: 服務類型 (frontend, backend, promotion, payment, thirdparty)
- `request_source`: 請求來源 (member, admin)
- `endpoint`: API 端點
- `error_type`: 錯誤類型

### 監控工具
- **OpenTelemetry Collector**: 收集和處理 telemetry 數據
- **Prometheus**: 存儲和查詢 metrics
- **Grafana**: 數據可視化和監控面板

## 🔧 配置說明

### OpenTelemetry 配置
- 使用 OTLP 協議傳輸數據
- 支援 Metrics、Traces 和 Logs
- 可配置的採樣率和緩衝區大小

### 服務配置
- 每個服務都有獨立的 OpenTelemetry 配置
- 支援自定義 metrics 和 traces
- 可配置的服務名稱和版本

## 📁 專案結構
```
otel-dotnet-demo-otel-1.10.0/
├── service-a/          # 前台服務 (會員)
├── service-e/          # 後台服務 (管理員)
├── service-b/          # Promotion 服務
├── service-c/          # Payment 服務
├── service-d/          # Third-party 服務
├── otel-collector-config.yaml  # OpenTelemetry 配置
├── prometheus.yml      # Prometheus 配置
└── docker-compose.yml  # Docker 編排配置
```

## 🎉 總結

這個專案展示了如何在 .NET 微服務架構中整合 OpenTelemetry，提供完整的可觀測性解決方案。通過實際的業務場景模擬，開發者和運維人員可以更好地理解微服務監控的重要性和實作方式。
