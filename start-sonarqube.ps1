docker rm sonarqube --force
docker run -d -v /var/local/sonarqube/extensions/plugins:/opt/sonarqube/extensions/plugins --name sonarqube -e SONARQUBE_JDBC_USERNAME=engineering@eb-engineering -e SONARQUBE_JDBC_PASSWORD=0nlineAdmin! -e SONARQUBE_JDBC_URL="jdbc:sqlserver://eb-engineering.database.windows.net:1433;database=sonarqube;" -p 9000:9000 -p 9092:9092 sonarqube:7.1-alpine
docker logs sonarqube --follow
