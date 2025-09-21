# -------------------------------
# Load environment variables from .env
# -------------------------------
$envFile = ".env"
if (Test-Path $envFile) {
    Get-Content $envFile | ForEach-Object {
        if ($_ -match "^\s*([^#][^=]+)=(.+)$") {
            $name  = $matches[1].Trim()
            $value = $matches[2].Trim()
            [System.Environment]::SetEnvironmentVariable($name, $value)
        }
    }
}

# -------------------------------
# Build Docker image
# -------------------------------
docker build --build-arg BUILD_CONFIGURATION=Development -f Veiling.Server/Dockerfile -t veilingserver .

# -------------------------------
# Remove existing container if it exists
# -------------------------------
if (docker ps -a -q -f name=veiling) {
    docker rm veiling | Out-Null
}

# -------------------------------
# Run Docker container
# -------------------------------
docker run `
    -e "ASPNETCORE_ENVIRONMENT=Development" `
    -e "ASPNETCORE_Kestrel__Certificates__Default__Password=$env:CERT_PASSWORD" `
    -e "ASPNETCORE_Kestrel__Certificates__Default__Path=/https/veiling.client.pfx" `
    --name veiling `
    -p 7080:8081 -p 32274:8080 `
    -v "$env:USERPROFILE\.aspnet\https:/https:ro" `
    -t veilingserver

