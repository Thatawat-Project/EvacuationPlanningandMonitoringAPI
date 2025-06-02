#!/bin/bash

# แสดงข้อความใน log
echo [+] Starting WireGuard VPN...

# เปิด VPN จาก config ไฟล์ wg0.conf
wg-quick up /etc/wireguard/wg0.conf

# รอ VPN เสถียรก่อนเรียก Web API
echo [+] Waiting for VPN to be ready...
sleep 3
ping -c 2 10.0.0.1

# รัน Web API
echo [+] Starting ASP.NET Core Web API...
exec dotnet EvacuationPlanningandMonitoringAPI.dll