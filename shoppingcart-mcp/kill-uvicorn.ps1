# Kill all uvicorn and python processes (use with caution)
Get-Process uvicorn, python -ErrorAction SilentlyContinue | Stop-Process -Force
