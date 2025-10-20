# init-db.ps1

Write-Host "Starting containers in detached mode..."
docker-compose up -d --build

Write-Host "Waiting 20 seconds for SQL Server to be fully ready..."
Start-Sleep -Seconds 20

Write-Host "Executing SQL initialization script..."
docker exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "P@ssw0rd123" -C -i /docker-entrypoint-initdb.d/ALittleLeaf.sql

Write-Host "✅ Database initialization complete. Attaching to logs..."
docker-compose logs -f