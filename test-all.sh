#!/bin/bash

echo "🔍 開始測試所有 API..."
echo ""
echo "📋 服務對應顏色："
echo "🔴 紅色 = Service A (port 5001)"
echo "🟠 橘色 = Service B (port 5002)"
echo "🔵 藍色 = Service C (port 5003)"
echo "🟣 紫色 = Service D (port 5004)"
echo "🟢 綠色 = Service E (port 5005)"
echo ""

echo "🔴 GET http://localhost:5001/"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5001/

echo ""
echo "🔴 GET http://localhost:5001/api/test-metric"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5001/api/test-metric

echo ""
echo "🔴 GET http://localhost:5001/api/call-b"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5001/api/call-b

echo ""
echo "🔴 GET http://localhost:5001/api/call-c"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5001/api/call-c

echo ""
echo "🔴 GET http://localhost:5001/api/call-d"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5001/api/call-d

echo ""
echo "🔴 GET http://localhost:5001/api/call-all"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5001/api/call-all

echo ""
echo "🟠 GET http://localhost:5002/"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5002/

echo ""
echo "🟠 GET http://localhost:5002/api/test-metric"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5002/api/test-metric

echo ""
echo "🟠 GET http://localhost:5002/api/hello"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5002/api/hello

echo ""
echo "🔵 GET http://localhost:5003/"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5003/

echo ""
echo "🔵 GET http://localhost:5003/api/test-metric"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5003/api/test-metric

echo ""
echo "🔵 GET http://localhost:5003/api/hello"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5003/api/hello

echo ""
echo "🟣 GET http://localhost:5004/"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5004/

echo ""
echo "🟣 GET http://localhost:5004/api/test-metric"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5004/api/test-metric

echo ""
echo "🟣 GET http://localhost:5004/api/hello"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5004/api/hello

echo ""
echo "🟢 GET http://localhost:5005/"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5005/

echo ""
echo "🟢 GET http://localhost:5005/api/test-metric"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5005/api/test-metric

echo ""
echo "🟢 GET http://localhost:5005/api/call-b"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5005/api/call-b

echo ""
echo "🟢 GET http://localhost:5005/api/call-c"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5005/api/call-c

echo ""
echo "🟢 GET http://localhost:5005/api/call-d"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5005/api/call-d

echo ""
echo "🟢 GET http://localhost:5005/api/call-all"
curl -s -w "\n\n🔚 Response Code: %{http_code}\n" http://localhost:5005/api/call-all

echo ""
echo "✅ 所有 API 測試完成"