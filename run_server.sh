source .env
docker build --build-arg BUILD_CONFIGURATION=Development -f Veiling.Server/Dockerfile -t veilingserver .
docker rm veiling &>/dev/null || true
docker run \
    -e ASPNETCORE_ENVIRONMENT=Development \
    -e ASPNETCORE_Kestrel__Certificates__Default__Password=$CERT_PASSWORD \
    -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/veiling.client.pfx \
    --name veiling \
    -p 7080:8081 -p 32274:8080 \
    -v $HOME/.aspnet/https:/https:ro \
    -t veilingserver
