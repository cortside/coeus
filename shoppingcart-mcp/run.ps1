#export UVICORN_PORT="8080"
$port = 8080
netstat -ano | grep $port

uvicorn main:app --port $port
