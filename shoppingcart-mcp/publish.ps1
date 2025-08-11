#docker compose down

#mkdir -p //freenas/work/k8s-share/acme

#mkdir -p //freenas/work/k8s-share/acme/otel
Copy-Item otel-collector-config.yaml //freenas/work/k8s-share/acme/otel/otel-collector-config.yaml -Force

#mkdir -p //freenas/work/k8s-share/acme/prometheus
Copy-Item prometheus.yml //freenas/work/k8s-share/acme/prometheus/prometheus.yml -Force

Copy-Item .\grafana //freenas/work/k8s-share/acme/grafana -Recurse -Force

docker compose up --force-recreate --remove-orphans -d --build

docker compose ps
#docker compose logs -f

curl -f http://kehlstein:13133