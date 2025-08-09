# OpenTelemetry .NET 微服務演示專案

這是一個基於 .NET 8 的微服務架構演示專案，整合了 OpenTelemetry 監控和追蹤功能，包含 5 個相互調用的服務。

## 🏗️ 專案架構

```
otel-dotnet-demo-otel-1.10.0/
├── service-a/          # 主要入口服務 (Port 5001)
├── service-b/          # 業務服務 B (Port 5002)
├── service-c/          # 業務服務 C (Port 5003)
├── service-d/          # 業務服務 D (Port 5004)
├── service-e/          # 測試服務 E (Port 5005)
├── docker-compose.yml  # Docker 編排配置
├── test-all.sh         # 自動化測試腳本
└── README.md           # 專案說明文件
```

## 🚀 服務說明

### Service A (🔴 紅色) - 主要入口服務
- **Port**: 5001
- **功能**: 作為主要入口點，可以調用其他所有服務
- **特殊行為**: 正常調用其他服務，無延遲或錯誤

### Service B (🟠 橘色) - 業務服務 B
- **Port**: 5002
- **功能**: 提供基礎業務功能
- **特殊行為**: 當收到來自 Service E 的請求時，會延遲 3-5 秒

### Service C (🔵 藍色) - 業務服務 C
- **Port**: 5003
- **功能**: 提供基礎業務功能
- **特殊行為**: 正常回應，無延遲

### Service D (🟣 紫色) - 業務服務 D
- **Port**: 5004
- **功能**: 提供基礎業務功能
- **特殊行為**: 當收到來自 Service E 的請求時，會拋出異常

### Service E (🟢 綠色) - 測試服務
- **Port**: 5005
- **功能**: 用於測試其他服務的特殊行為
- **特殊行為**: 調用其他服務時會帶上 `X-From: service-e` header

## 📡 API 規格

### Service A APIs

#### 1. 首頁
- **端點**: `GET /`
- **描述**: 服務狀態檢查
- **回應**: `Service A is running`

#### 2. 測試指標
- **端點**: `GET /api/test-metric`
- **描述**: 增加自定義指標計數
- **回應**: `Metric incremented`

#### 3. 調用 Service B
- **端點**: `GET /api/call-b`
- **描述**: 調用 Service B 的 hello 端點
- **回應**: `ServiceB said: Hello from ServiceB`

#### 4. 調用 Service C
- **端點**: `GET /api/call-c`
- **描述**: 調用 Service C 的 hello 端點
- **回應**: `ServiceC said: Hello from ServiceC`

#### 5. 調用 Service D
- **端點**: `GET /api/call-d`
- **描述**: 調用 Service D 的 hello 端點
- **回應**: `ServiceD said: Hello from ServiceD`

#### 6. 調用所有服務
- **端點**: `GET /api/call-all`
- **描述**: 同時調用 B、C、D 三個服務
- **回應**: JSON 格式包含所有服務的回應結果

### Service B APIs

#### 1. 首頁
- **端點**: `GET /`
- **描述**: 服務狀態檢查
- **回應**: `Service B is running`

#### 2. 測試指標
- **端點**: `GET /api/test-metric`
- **描述**: 增加自定義指標計數
- **回應**: `Service B metric incremented`

#### 3. Hello 端點
- **端點**: `GET /api/hello`
- **描述**: 基礎回應端點
- **特殊行為**: 
  - 正常請求: `Hello from ServiceB`
  - 來自 Service E 的請求: `Hello from ServiceB (delayed Xs for service-e)` (延遲 3-5 秒)

### Service C APIs

#### 1. 首頁
- **端點**: `GET /`
- **描述**: 服務狀態檢查
- **回應**: `Service C is running`

#### 2. 測試指標
- **端點**: `GET /api/test-metric`
- **描述**: 增加自定義指標計數
- **回應**: `Service C metric incremented`

#### 3. Hello 端點
- **端點**: `GET /api/hello`
- **描述**: 基礎回應端點
- **回應**: `Hello from ServiceC`

### Service D APIs

#### 1. 首頁
- **端點**: `GET /`
- **描述**: 服務狀態檢查
- **回應**: `Service D is running`

#### 2. 測試指標
- **端點**: `GET /api/test-metric`
- **描述**: 增加自定義指標計數
- **回應**: `Service D metric incremented`

#### 3. Hello 端點
- **端點**: `GET /api/hello`
- **描述**: 基礎回應端點
- **特殊行為**:
  - 正常請求: `Hello from ServiceD`
  - 來自 Service E 的請求: 拋出 `InvalidOperationException`

### Service E APIs

#### 1. 首頁
- **端點**: `GET /`
- **描述**: 服務狀態檢查
- **回應**: `Service E is running`

#### 2. 測試指標
- **端點**: `GET /api/test-metric`
- **描述**: 增加自定義指標計數
- **回應**: `Metric incremented`

#### 3. 調用 Service B
- **端點**: `GET /api/call-b`
- **描述**: 調用 Service B (會延遲 3-5 秒)
- **Header**: `X-From: service-e`
- **回應**: `ServiceB said: Hello from ServiceB (delayed Xs for service-e)`

#### 4. 調用 Service C
- **端點**: `GET /api/call-c`
- **描述**: 調用 Service C (正常回應)
- **Header**: `X-From: service-e`
- **回應**: `ServiceC said: Hello from ServiceC`

#### 5. 調用 Service D
- **端點**: `GET /api/call-d`
- **描述**: 調用 Service D (會拋出異常)
- **Header**: `X-From: service-e`
- **回應**: HTTP 500 錯誤

#### 6. 調用所有服務
- **端點**: `GET /api/call-all`
- **描述**: 同時調用 B、C、D 三個服務
- **Header**: `X-From: service-e`
- **回應**: JSON 格式包含所有服務的回應結果

## 🛠️ 技術棧

- **.NET 8**: 主要開發框架
- **ASP.NET Core**: Web API 框架
- **OpenTelemetry 1.10.0**: 監控和追蹤
- **Docker**: 容器化部署
- **Docker Compose**: 服務編排

## 📊 監控功能

### OpenTelemetry 配置
- **Metrics**: 透過 HTTP/Protobuf 發送到 collector:4318
- **Traces**: 透過 gRPC 發送到 collector:4317
- **Jaeger**: 分散式追蹤 UI (http://localhost:16686)
- **Prometheus**: 指標收集 (http://localhost:9090)
- **Grafana**: 監控儀表板 (http://localhost:3000)

### 自定義指標
每個服務都包含以下自定義指標：
- `service_[a-e]_test_counter`: 測試計數器
- `service_[a-e]_hello_requests`: Hello 端點請求計數
- `service_[a-e]_call_[b/c/d]_error_count`: 調用錯誤計數
- `service_b_delay_count`: Service B 延遲計數
- `service_d_exception_count`: Service D 異常計數

## 🚀 快速開始

### 1. 啟動所有服務
```bash
docker-compose up -d
```

### 2. 執行自動化測試
```bash
./test-all.sh
```

### 3. 訪問監控介面
- **Jaeger**: http://localhost:16686
- **Prometheus**: http://localhost:9090
- **Grafana**: http://localhost:3000 (admin/admin)

## 🔧 開發指南

### 新增服務
1. 創建新的服務目錄
2. 複製現有服務的結構
3. 修改 `Program.cs` 中的服務名稱
4. 更新 `docker-compose.yml`
5. 更新 `test-all.sh`

### 修改 API 行為
- 在對應的 `Controllers/HelloController.cs` 中修改邏輯
- 使用 `Request.Headers["X-From"]` 檢查請求來源
- 添加相應的監控指標

## 📝 注意事項

1. **服務依賴**: Service A 和 E 依賴於 B、C、D 服務
2. **特殊行為**: 只有來自 Service E 的請求會觸發特殊行為
3. **監控**: 所有服務都配置了完整的 OpenTelemetry 監控
4. **健康檢查**: 所有服務都配置了 Docker 健康檢查

## ✅ 驗證與觀測指南（可直接複製貼上）

### 1) 一鍵觸發所有 API（產生資料）

```bash
./test-all.sh
```

或手動各服務觸發一次測試指標（可多次執行累積數據）：
```bash
curl -s http://localhost:5001/api/test-metric
curl -s http://localhost:5002/api/test-metric
curl -s http://localhost:5003/api/test-metric
curl -s http://localhost:5004/api/test-metric
curl -s http://localhost:5005/api/test-metric
```

小提醒：.NET OTel 預設 metrics 出口有彙整/間隔（常見 60 秒）。觸發後等 30~60 秒再查詢 Prometheus/Grafana。

---

### 2) Collector 與 Prometheus 快速自檢

- 檢查 Collector 是否有暴露 Prometheus 指標：
```bash
curl -sS http://localhost:9464/metrics | head -n 80
```

- 檢查 Prometheus 是否有抓 Collector：
  - 打開瀏覽器 `http://localhost:9090/targets`
  - 看到 `otel-collector (collector:9464)` 為 UP 即正常

- 在 Prometheus UI 直接查詢（`http://localhost:9090` → Graph）：
  - 這些查詢可直接複製貼上：
```promql
# A 的測試計數器（速率）
rate(etta_service_a_test_counter[1m])

# B 的 hello 請求量（依 endpoint 維度）
sum by (endpoint) (increase(etta_service_b_hello_requests[5m]))

# B 因 E 觸發的延遲次數（依 delay_seconds 分組）
sum by (delay_seconds) (increase(etta_service_b_delay_count[5m]))

# C 的 hello 請求量（總和）
sum(increase(etta_service_c_hello_requests[5m]))

# D 因 E 觸發的例外次數（依 from 標籤分組）
sum by (from) (increase(etta_service_d_exception_count[5m]))
```

---

### 3) Grafana 設定（一步步照做）

1. 打開 `http://localhost:3000`（預設帳密 `admin / admin`）
2. 新增資料來源（Data sources）
   - 選 `Prometheus`
   - URL 填：`http://prometheus:9090`
   - 按 `Save & Test` 應顯示 Data source is working
3. 匯入儀表板（Dashboard）
   - 左側 `Dashboards` → `New` → `Import`
   - 將下方 JSON 全部複製貼上 → `Load` → 指定剛剛的 Prometheus Data source → `Import`

儀表板 JSON（可直接複製貼上）：
```json
{
  "annotations": {"list": []},
  "editable": true,
  "fiscalYearStartMonth": 0,
  "graphTooltip": 0,
  "panels": [
    {
      "type": "timeseries",
      "title": "Service A test counter (rate)",
      "gridPos": {"h": 8, "w": 24, "x": 0, "y": 0},
      "targets": [
        {"expr": "rate(etta_service_a_test_counter[1m])", "legendFormat": "test_counter"}
      ]
    },
    {
      "type": "timeseries",
      "title": "Service B delay count by delay_seconds",
      "gridPos": {"h": 8, "w": 24, "x": 0, "y": 8},
      "targets": [
        {"expr": "sum by (delay_seconds) (increase(etta_service_b_delay_count[5m]))", "legendFormat": "{{delay_seconds}}s"}
      ]
    },
    {
      "type": "timeseries",
      "title": "Service D exceptions (by from)",
      "gridPos": {"h": 8, "w": 24, "x": 0, "y": 16},
      "targets": [
        {"expr": "sum by (from) (increase(etta_service_d_exception_count[5m]))", "legendFormat": "from {{from}}"}
      ]
    },
    {
      "type": "timeseries",
      "title": "Hello requests (B/C/D)",
      "gridPos": {"h": 8, "w": 24, "x": 0, "y": 24},
      "targets": [
        {"expr": "sum by (endpoint) (increase(etta_service_b_hello_requests[5m]))", "legendFormat": "B {{endpoint}}"},
        {"expr": "sum(increase(etta_service_c_hello_requests[5m]))", "legendFormat": "C total"},
        {"expr": "sum(increase(etta_service_d_hello_requests[5m]))", "legendFormat": "D total"}
      ]
    }
  ],
  "refresh": "5s",
  "schemaVersion": 38,
  "style": "dark",
  "tags": ["otel", "demo"],
  "templating": {"list": []},
  "time": {"from": "now-15m", "to": "now"},
  "timezone": "browser",
  "title": "OTel .NET Demo - Metrics Overview",
  "version": 0
}
```

---

### 4) 產生穩定流量（方便觀測圖表變化）

- 連續打 A 的測試計數器：
```bash
for i in {1..60}; do curl -s http://localhost:5001/api/test-metric > /dev/null; sleep 1; done
```

- 連續觸發 E→B（B 會延遲 3~5 秒）：
```bash
for i in {1..20}; do curl -s http://localhost:5005/api/call-b > /dev/null; sleep 2; done
```

- 連續觸發 E→D（D 會拋例外）：
```bash
for i in {1..10}; do curl -s -o /dev/null -w "%{http_code}\n" http://localhost:5005/api/call-d; sleep 2; done
```

---

### 5) 常見問題（Troubleshooting）

- 在服務日誌看到 `POST http://collector:4318/ ... 404`：
  - 已修正：Metrics HTTP 端點需為 `http://collector:4318/v1/metrics`（本專案已調整）。
- 觸發後 Prometheus 沒資料：
  - 等 30~60 秒（metrics 批次/彙整/抓取間隔）。
  - 到 `http://localhost:9090/targets` 確認 `otel-collector` 目標為 UP。
  - `curl http://localhost:9464/metrics` 應該要有輸出。
- 想縮短等待時間：
  - 可將匯出間隔設短（例如 5 秒）。若需要，我可以幫你把各服務的匯出間隔調整為 5 秒。
