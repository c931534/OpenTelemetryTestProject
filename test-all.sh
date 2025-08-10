#!/bin/bash

echo "🚀 開始測試所有服務..."

# 前台服務測試 (Service A)
echo "📱 測試前台服務 (Service A - 會員服務)"
echo "  - 測試 Promotion 服務..."
curl -s http://localhost:5001/api/member/promotion
echo ""
echo "  - 測試 Payment 服務..."
curl -s http://localhost:5001/api/member/payment
echo ""
echo "  - 測試 Third-party 服務..."
curl -s http://localhost:5001/api/member/thirdparty
echo ""
echo "  - 測試所有服務..."
curl -s http://localhost:5001/api/member/all-services
echo ""

echo ""

# 後台服務測試 (Service E)
echo "🖥️  測試後台服務 (Service E - 管理員服務)"
echo "  - 測試 Promotion 管理..."
curl -s http://localhost:5005/api/admin/promotion-management
echo ""
echo "  - 測試 Payment 管理..."
curl -s http://localhost:5005/api/admin/payment-management
echo ""
echo "  - 測試 Third-party 管理..."
curl -s http://localhost:5005/api/admin/thirdparty-management
echo ""
echo "  - 測試所有管理服務..."
curl -s http://localhost:5005/api/admin/all-management
echo ""

echo ""

# 直接測試下層服務
echo "🔧 直接測試下層服務"
echo "  - 測試 Promotion 服務 (Service B)..."
curl -s http://localhost:5002/api/hello
echo ""
echo "  - 測試 Payment 服務 (Service C)..."
curl -s http://localhost:5003/api/hello
echo ""
echo "  - 測試 Third-party 服務 (Service D)..."
curl -s http://localhost:5004/api/hello
echo ""

echo ""
echo "✅ 所有服務測試完成！"
echo ""
echo "📊 現在可以查看監控數據："
echo "  - Prometheus: http://localhost:9090"
echo "  - Grafana: http://localhost:3000 (admin/admin)"
echo "  - Jaeger: http://localhost:16686"