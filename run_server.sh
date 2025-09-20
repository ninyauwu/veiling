docker build -f Veiling.Server/Dockerfile -t veilingserver .
docker rm veiling &>/dev/null || true
docker run --name veiling -p 32274:8080 -t veilingserver
