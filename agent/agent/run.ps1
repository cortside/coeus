# PowerShell script to start agent-backend

$ErrorActionPreference = 'Stop'

Write-Host "Starting agent-backend..."
uvicorn main:app --host 0.0.0.0 --port 8000
