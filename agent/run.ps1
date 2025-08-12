# PowerShell script to start both agent-backend and chat-frontend

$ErrorActionPreference = 'Stop'

Write-Host "Starting agent-backend..."
Start-Process -NoNewWindow -FilePath "pwsh" -ArgumentList "-NoExit", "-Command", "cd ./agent-backend; uvicorn main:app --host 0.0.0.0 --port 8000" | Out-Null

Start-Sleep -Seconds 3

Write-Host "Starting chat-frontend..."
Start-Process -NoNewWindow -FilePath "pwsh" -ArgumentList "-NoExit", "-Command", "cd ./chat-frontend; npm run dev" | Out-Null

Write-Host "Both agent-backend and chat-frontend have been started in new PowerShell windows."
