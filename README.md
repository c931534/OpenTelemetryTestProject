# OpenTelemetry .NET Demo

這是一個使用 OpenTelemetry 1.10.0 的 .NET 微服務架構演示專案，展示了如何在不同服務間收集和傳遞 telemetry 數據。

## 🎯 專案目的

### 主要目標
這個專案旨在演示和測試 **微服務架構的可觀測性 (Observability)** 解決方案，具體包括：

1. **OpenTelemetry 整合測試**: 驗證在 .NET 微服務中如何正確配置和整合 OpenTelemetry
2. **分散式追蹤 (Distributed Tracing)**: 展示服務間調用鏈路的追蹤能力
3. **指標監控 (Metrics)**: 演示關鍵業務指標的收集和監控
4. **日誌聚合 (Logs)**: 展示結構化日誌的收集和分析
5. **微服務架構監控**: 驗證多層服務架構的監控策略

### 測試架構類型
- **前台服務架構**: 會員端服務的監控和追蹤
- **後台服務架構**: 管理端服務的監控和追蹤  
- **業務服務架構**: 核心業務邏輯服務的監控和追蹤
- **服務網格監控**: 服務間依賴關係和調用鏈路的可視化

## 🏗️ 專案架構

### 整體架構圖
```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           OpenTelemetry 監控生態系統                        │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐  │
│  │   Grafana   │    │ Prometheus  │    │   Jaeger    │    │   Collector │  │
│  │   (3000)    │    │   (9090)    │    │   (16686)   │    │   (4318)    │  │
│  │   監控面板    │    │   指標存儲    │    │   追蹤查詢    │    │   數據收集    │  │
│  └─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘  │
│                                                                             │
├─────────────────────────────────────────────────────────────────────────────┤
│                             微服務應用層                                    │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │                        API 網關層                                    │   │
│  │  ┌─────────────┐                    ┌─────────────┐                  │   │
│  │  │  Service A  │                    │  Service E  │                  │   │
│  │  │   (5001)    │                    │   (5005)    │                  │   │
│  │  │   前台服務    │                    │   後台服務    │                  │   │
│  │  │  (會員介面)   │                    │  (管理介面)   │                  │   │
│  │  └─────────────┘                    └─────────────┘                  │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                                                             │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │                      業務服務層                                       │   │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐                  │   │
│  │  │  Service B  │  │  Service C  │  │  Service D  │                  │   │
│  │  │   (5002)    │  │   (5003)    │  │   (5004)    │                  │   │
│  │  │ Promotion   │  │  Payment    │  │ Third-party │                  │   │
│  │  │  (行銷服務)   │  │  (金流服務)   │  │  (第三方服務)  │                  │   │
│  │  └─────────────┘  └─────────────┘  └─────────────┘                  │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                                                             │
├─────────────────────────────────────────────────────────────────────────────┤
│                             基礎設施層                                     │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐  │
│  │   Docker    │    │   Docker    │    │   Docker    │    │   Docker    │  │
│  │   Compose   │    │   Network   │    │   Volumes   │    │   Health    │  │
│  │   編排管理    │    │   網路配置    │    │   數據持久    │    │   健康檢查    │  │
│  └─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘  │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 服務調用流程圖
```
會員操作流程:
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   會員      │───▶│  Service A  │───▶│  Service B  │───▶│  Collector  │
│  (前台)     │    │  (會員介面)   │    │ (Promotion) │    │  (數據收集)  │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
                           │                   │                   │
                           ▼                   ▼                   ▼
                    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
                    │  Service C  │    │  Service D  │    │  Prometheus │
                    │ (Payment)   │    │(Third-party)│    │  (指標存儲)  │
                    └─────────────┘    └─────────────┘    └─────────────┘

管理員操作流程:
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   管理員    │───▶│  Service E  │───▶│  Service B  │───▶│  Collector  │
│  (後台)     │    │  (管理介面)   │    │ (Promotion) │    │  (數據收集)  │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
                           │                   │                   │
                           ▼                   ▼                   ▼
                    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
                    │  Service C  │    │  Service D  │    │    Jaeger   │
                    │ (Payment)   │    │(Third-party)│    │  (追蹤查詢)  │
                    └─────────────┘    └─────────────┘    └─────────────┘
```

### 服務角色說明

#### 上層服務 (Frontend/Backend)
- **Service A (前台服務 - 5001)**: 會員相關的 API 服務，提供會員功能介面
- **Service E (後台服務 - 5005)**: 內部管理人員的 API 服務，提供管理功能介面

#### 下層服務 (Business Services)
- **Service B (Promotion 服務 - 5002)**: 行銷活動、優惠券、折扣等服務
- **Service C (Payment 服務 - 5003)**: 金流、支付、退款等服務  
- **Service D (Third-party 服務 - 5004)**: 外部整合服務，如簡訊、郵件等

#### 監控服務
- **OpenTelemetry Collector (4318/9464)**: 收集和處理所有 telemetry 數據
- **Prometheus (9090)**: 存儲和查詢 metrics 數據
- **Grafana (3000)**: 數據可視化和監控面板
- **Jaeger (16686)**: 分散式追蹤查詢和可視化

## 🚀 專案啟動步驟

### 前置需求檢查
在開始之前，請確保您的系統已安裝以下軟體：

```bash
# 檢查 Docker 版本 (需要 Docker 20.10+)
docker --version

# 檢查 Docker Compose 版本 (需要 Docker Compose 2.0+)
docker-compose --version

# 檢查 .NET SDK 版本 (需要 .NET 8.0+)
dotnet --version
```

### 步驟 1: 下載專案
```bash
# 如果使用 Git 克隆
git clone <repository-url>
cd otel-dotnet-demo-otel-1.10.0

# 或者直接下載並解壓縮專案檔案
```

**這個步驟在做什麼**: 獲取專案原始碼和配置文件，為後續的服務啟動做準備。

**預期結果**: 進入專案目錄，看到包含 `docker-compose.yml`、`service-a/` 到 `service-e/` 等子目錄的專案結構。

### 步驟 2: 檢查專案結構
```bash
# 查看專案目錄結構
ls -la

# 確認以下關鍵檔案存在：
# - docker-compose.yml (Docker 編排配置)
# - otel-collector-config.yaml (OpenTelemetry 配置)
# - prometheus.yml (Prometheus 配置)
# - service-a/ 到 service-e/ (五個微服務)
```

**這個步驟在做什麼**: 驗證專案檔案完整性，確保所有必要的配置檔案和服務代碼都存在。

**預期結果**: 看到完整的專案檔案列表，確認所有必要檔案都存在。

### 步驟 3: 啟動所有服務
```bash
# 在專案根目錄執行
docker-compose up -d

# 這個命令會：
# - 構建所有微服務的 Docker 映像
# - 啟動 OpenTelemetry Collector
# - 啟動 Prometheus 和 Grafana
# - 啟動 Jaeger 追蹤系統
# - 啟動五個微服務 (A, B, C, D, E)
```

**這個步驟在做什麼**: 使用 Docker Compose 一次性啟動整個微服務架構，包括所有應用服務和監控工具。

**預期結果**: 
- 看到 Docker 映像構建過程
- 所有服務狀態顯示為 "Up"
- 沒有錯誤訊息

**觀察重點**: 注意是否有任何服務啟動失敗，特別關注 `collector`、`prometheus`、`grafana` 等監控服務的狀態。

### 步驟 4: 檢查服務狀態
```bash
# 查看所有服務的運行狀態
docker-compose ps

# 查看服務日誌 (可選)
docker-compose logs -f

# 檢查特定服務的日誌
docker-compose logs service-a
docker-compose logs collector
```

**這個步驟在做什麼**: 驗證所有服務是否正常啟動，檢查是否有錯誤或異常情況。

**預期結果**: 
- `docker-compose ps` 顯示所有服務狀態為 "Up"
- 健康檢查狀態為 "healthy"
- 日誌中沒有錯誤訊息

**觀察重點**: 
- 確認所有 8 個服務都在運行
- 檢查端口映射是否正確 (5001-5005, 9090, 3000, 16686, 4318, 9464)
- 觀察服務啟動順序和依賴關係

### 步驟 5: 等待服務就緒
```bash
# 等待所有服務健康檢查通過
# 這通常需要 1-2 分鐘時間

# 可以重複執行以下命令檢查狀態
docker-compose ps
```

**這個步驟在做什麼**: 等待所有服務完成初始化，確保健康檢查通過，服務可以正常接收請求。

**預期結果**: 所有服務的健康檢查狀態都顯示為 "healthy"。

**觀察重點**: 
- 服務狀態從 "starting" 變為 "Up"
- 健康檢查從 "unhealthy" 變為 "healthy"
- 沒有服務重啟或失敗

### 步驟 6: 測試服務功能
```bash
# 使用提供的測試腳本
chmod +x test-all.sh
./test-all.sh

# 或者手動測試各個服務
```

**這個步驟在做什麼**: 驗證所有微服務的 API 端點是否正常響應，確認服務間調用是否成功。

**預期結果**: 
- 腳本執行過程中看到每個 API 調用的響應
- 每個服務都返回 JSON 格式的響應
- 沒有 HTTP 錯誤 (如 404, 500 等)

**觀察重點**: 
- 前台服務 (5001) 的會員相關 API 響應
- 後台服務 (5005) 的管理相關 API 響應
- 下層業務服務 (5002-5004) 的基礎 API 響應

**具體響應示例**:
```json
// 前台服務響應示例
{
  "service": "Service A (前台服務)",
  "timestamp": "2024-01-01T12:00:00Z",
  "endpoint": "/api/member/promotion",
  "status": "success"
}

// 後台服務響應示例
{
  "service": "Service E (後台服務)",
  "timestamp": "2024-01-01T12:00:00Z",
  "endpoint": "/api/admin/promotion-management",
  "status": "success"
}
```

### 步驟 7: 觀察 OpenTelemetry 數據收集
```bash
# 執行測試腳本後，等待 30-60 秒讓數據傳輸完成
sleep 60

# 檢查 OpenTelemetry Collector 日誌
docker-compose logs collector | tail -20
```

**這個步驟在做什麼**: 確認 OpenTelemetry Collector 正在收集和處理 telemetry 數據。

**預期結果**: 
- Collector 日誌顯示正在接收和處理數據
- 看到來自各個服務的 metrics 和 traces 數據

**觀察重點**: 
- 日誌中是否有 "Received" 或 "Processed" 相關訊息
- 是否有來自不同服務的數據流
- 確認沒有錯誤或警告訊息

### 步驟 8: 查看 Prometheus 指標數據
```bash
# 在瀏覽器中打開 Prometheus
open http://localhost:9090

# 或者手動輸入地址: http://localhost:9090
```

**這個步驟在做什麼**: 通過 Prometheus 查看收集到的 metrics 數據，驗證 OpenTelemetry 整合是否成功。

**預期結果**: 
- 成功打開 Prometheus 查詢介面
- 在 "Status" → "Targets" 中看到所有服務的監控目標
- 在 "Graph" 中能夠查詢到 metrics

**具體操作步驟**:
1. 點擊 "Graph" 標籤
2. 在查詢框中輸入以下查詢語句，觀察結果：

```promql
# 查看所有可用的 metrics
{__name__=~".*"}

# 查看前台服務請求總數
frontend_requests_total

# 查看後台服務請求總數
backend_management_requests_total

# 查看服務響應時間
service_response_time_bucket

# 查看錯誤計數
error_count_total
```

**觀察重點**: 
- 確認能夠查詢到來自各個服務的 metrics
- 觀察 metrics 的標籤 (labels) 是否包含服務名稱、端點等信息
- 確認數據在持續更新

### 步驟 9: 登入 Grafana 並建立 Dashboard
```bash
# 在瀏覽器中打開 Grafana
open http://localhost:3000

# 或者手動輸入地址: http://localhost:3000
```

**這個步驟在做什麼**: 通過 Grafana 創建監控儀表板，可視化 metrics 數據。

**登入資訊**:
- **用戶名**: `admin`
- **密碼**: `admin`
- **首次登入**: 系統會要求修改密碼，可以保持 `admin/admin` 或設置新密碼

**具體操作步驟**:

#### 9.1 添加 Prometheus 數據源
1. 點擊左側選單的齒輪圖標 (⚙️) → "Data Sources"
2. 點擊 "Add data source"
3. 選擇 "Prometheus"
4. 配置數據源：
   - **Name**: `Prometheus`
   - **URL**: `http://prometheus:9090`
   - **Access**: `Server (default)`
5. 點擊 "Save & Test"，確認連接成功

#### 9.2 創建第一個 Dashboard
1. 點擊左側選單的 "+" 圖標 → "Dashboard"
2. 點擊 "Add new panel"
3. 在查詢編輯器中選擇 Prometheus 數據源
4. 添加以下查詢來創建面板：

**面板 1: 服務請求量監控**
```promql
# 前台服務請求速率
rate(frontend_requests_total[5m])

# 後台服務請求速率  
rate(backend_management_requests_total[5m])
```
- **Panel Title**: `服務請求量監控`
- **Visualization**: `Time series`
- **Legend**: `{{service_name}} - {{endpoint}}`

**面板 2: 服務響應時間監控**
```promql
# 95% 響應時間
histogram_quantile(0.95, rate(service_response_time_bucket[5m]))

# 平均響應時間
rate(service_response_time_sum[5m]) / rate(service_response_time_count[5m])
```
- **Panel Title**: `服務響應時間監控`
- **Visualization**: `Time series`
- **Y-axis**: `milliseconds`

**面板 3: 錯誤率監控**
```promql
# 錯誤率
rate(error_count_total[5m]) / rate(requests_total[5m]) * 100
```
- **Panel Title**: `服務錯誤率監控`
- **Visualization**: `Time series`
- **Y-axis**: `percentage`

#### 9.3 保存 Dashboard
1. 點擊右上角的 "Save" 按鈕
2. 輸入 Dashboard 名稱：`微服務監控儀表板`
3. 選擇文件夾：`General`
4. 點擊 "Save"

**觀察重點**: 
- 確認能夠成功連接到 Prometheus 數據源
- 驗證查詢能夠返回數據
- 觀察圖表是否正確顯示時間序列數據
- 確認數據在實時更新

### 步驟 10: 查看 Jaeger 追蹤數據
```bash
# 在瀏覽器中打開 Jaeger
open http://localhost:16686

# 或者手動輸入地址: http://localhost:16686
```

**這個步驟在做什麼**: 通過 Jaeger 查看分散式追蹤數據，分析服務調用鏈路。

**具體操作步驟**:
1. 在 "Service" 下拉選單中選擇要查看的服務 (如 `service-a`)
2. 點擊 "Find Traces" 按鈕
3. 觀察返回的追蹤記錄

**觀察重點**: 
- 確認能夠看到來自各個服務的追蹤數據
- 觀察服務間的調用關係和依賴鏈路
- 查看請求的響應時間和狀態
- 分析是否有錯誤或異常的調用鏈路

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

### 監控工具使用說明

#### Prometheus (http://localhost:9090)
- **用途**: 查詢和分析 metrics 數據
- **常用查詢**:
  - `rate(frontend_requests_total[5m])` - 前台服務請求速率
  - `rate(backend_management_requests_total[5m])` - 後台服務請求速率
  - `histogram_quantile(0.95, service_response_time_bucket)` - 95% 響應時間

**操作建議**:
1. 在 "Graph" 頁面測試不同的查詢語句
2. 使用 "Table" 視圖查看詳細的標籤信息
3. 在 "Status" → "Targets" 中檢查監控目標狀態
4. 使用 "Alerts" 功能設置告警規則

#### Grafana (http://localhost:3000)
- **用途**: 創建監控儀表板，可視化 metrics 數據
- **預設帳號**: admin/admin
- **建議儀表板**:
  - 服務請求量監控
  - 服務響應時間監控
  - 錯誤率監控
  - 服務依賴關係圖

**Dashboard 建立建議**:
1. **服務概覽面板**: 顯示所有服務的關鍵指標
2. **性能監控面板**: 響應時間、吞吐量、錯誤率
3. **業務指標面板**: 根據業務邏輯自定義指標
4. **告警面板**: 顯示當前告警狀態和歷史記錄

**進階功能**:
- 使用 Variables 創建動態查詢
- 設置 Dashboard 自動刷新
- 創建多個 Dashboard 用於不同用途
- 設置 Dashboard 權限和分享

#### Jaeger (http://localhost:16686)
- **用途**: 查看分散式追蹤數據，分析服務調用鏈路
- **功能**:
  - 追蹤查詢和過濾
  - 調用鏈路可視化
  - 性能分析
  - 錯誤診斷

**操作建議**:
1. 使用 "Service" 和 "Operation" 過濾器快速找到相關追蹤
2. 分析調用鏈路的響應時間分布
3. 檢查錯誤和異常的追蹤記錄
4. 使用 "Dependencies" 視圖了解服務依賴關係

## 🔧 配置說明

### OpenTelemetry 配置
- 使用 OTLP 協議傳輸數據
- 支援 Metrics、Traces 和 Logs
- 可配置的採樣率和緩衝區大小

### 服務配置
- 每個服務都有獨立的 OpenTelemetry 配置
- 支援自定義 metrics 和 traces
- 可配置的服務名稱和版本

### Docker 配置
- 所有服務都使用 Docker 容器化部署
- 支援健康檢查和自動重啟
- 配置了服務間依賴關係

## 📁 專案結構
```
otel-dotnet-demo-otel-1.10.0/
├── service-a/                    # 前台服務 (會員) - 端口 5001
│   ├── Controllers/             # API 控制器
│   ├── Program.cs               # 應用程式入口點
│   ├── ServiceA.csproj          # 專案檔案
│   └── Dockerfile               # Docker 映像配置
├── service-e/                    # 後台服務 (管理員) - 端口 5005
│   ├── Controllers/             # API 控制器
│   ├── Program.cs               # 應用程式入口點
│   ├── ServiceE.csproj          # 專案檔案
│   └── Dockerfile               # Docker 映像配置
├── service-b/                    # Promotion 服務 - 端口 5002
├── service-c/                    # Payment 服務 - 端口 5003
├── service-d/                    # Third-party 服務 - 端口 5004
├── otel-collector-config.yaml   # OpenTelemetry Collector 配置
├── prometheus.yml               # Prometheus 配置
├── docker-compose.yml           # Docker 編排配置
├── test-all.sh                  # 服務測試腳本
└── README.md                    # 專案說明文件
```

## 🧪 測試和驗證

### 自動化測試
```bash
# 執行完整測試腳本
./test-all.sh

# 腳本會測試：
# 1. 前台服務的所有 API 端點
# 2. 後台服務的所有 API 端點
# 3. 下層業務服務的可用性
# 4. 服務間調用的成功性
```

**測試後觀察重點**:
1. **Prometheus 數據**: 等待 1-2 分鐘後查看是否有新的 metrics 數據
2. **Collector 日誌**: 確認正在接收和處理數據
3. **服務狀態**: 確認所有服務仍在正常運行

### 手動測試
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

# 直接測試下層服務
curl http://localhost:5002/api/hello
curl http://localhost:5003/api/hello
curl http://localhost:5004/api/hello
```

### 預期結果
- 所有 API 端點都應該返回 JSON 響應
- 服務間調用應該成功執行
- 監控工具應該顯示相關的 metrics 和 traces
- 沒有錯誤或異常情況

## 🚨 故障排除

### 常見問題

#### 1. 服務啟動失敗
```bash
# 檢查 Docker 日誌
docker-compose logs <service-name>

# 檢查服務狀態
docker-compose ps

# 重新啟動服務
docker-compose restart <service-name>
```

#### 2. 端口衝突
```bash
# 檢查端口使用情況
lsof -i :5001
lsof -i :5002
lsof -i :5003
lsof -i :5004
lsof -i :5005

# 停止衝突的服務或修改端口配置
```

#### 3. 監控工具無法訪問
```bash
# 檢查服務是否正常運行
docker-compose ps

# 檢查防火牆設置
# 確保本地端口沒有被阻擋
```

#### 4. 服務間調用失敗
```bash
# 檢查網路配置
docker network ls
docker network inspect <network-name>

# 檢查服務依賴關係
docker-compose config
```

#### 5. Prometheus 沒有數據
```bash
# 檢查 Collector 是否正常運行
docker-compose logs collector

# 確認服務正在發送數據
docker-compose logs service-a | grep -i "telemetry"

# 等待數據傳輸完成 (通常需要 1-2 分鐘)
```

#### 6. Grafana 無法連接 Prometheus
```bash
# 檢查 Prometheus 服務狀態
docker-compose ps prometheus

# 確認數據源 URL 正確: http://prometheus:9090
# 注意使用容器名稱而不是 localhost
```

### 清理和重置
```bash
# 停止所有服務
docker-compose down

# 清理所有容器和網路
docker-compose down --volumes --remove-orphans

# 重新構建和啟動
docker-compose up -d --build
```

## 🎉 總結

這個專案展示了如何在 .NET 微服務架構中整合 OpenTelemetry，提供完整的可觀測性解決方案。通過實際的業務場景模擬，開發者和運維人員可以更好地理解微服務監控的重要性和實作方式。

### 學習重點
1. **OpenTelemetry 整合**: 了解如何在 .NET 應用程式中配置和使用 OpenTelemetry
2. **微服務監控**: 掌握多服務架構的監控策略和工具使用
3. **分散式追蹤**: 理解服務間調用鏈路的追蹤和分析
4. **指標收集**: 學習如何收集和分析關鍵業務指標
5. **監控工具**: 熟悉 Prometheus、Grafana、Jaeger 等監控工具的使用

### 下一步
- 嘗試修改服務配置，觀察監控數據的變化
- 創建自定義的 Grafana 儀表板
- 添加更多的業務邏輯和監控指標
- 探索 OpenTelemetry 的高級功能，如採樣、過濾等

通過這個專案，您將能夠建立對微服務可觀測性的深入理解，為實際專案中的監控和故障排除提供寶貴的經驗。
