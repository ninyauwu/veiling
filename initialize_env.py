import getpass
import subprocess
import platform
import os
from pathlib import Path

if platform.system() == "Windows":
    cert_dir = os.path.join(os.environ["USERPROFILE"], ".aspnet", "https")
else:
    cert_dir = os.path.expanduser("~/.aspnet/https")

# Ask user for password
password = getpass.getpass("Enter your certificate password: ")

# Path to .env
env_path = Path(".env")

# Append or create .env
with env_path.open("a") as f:
    f.write(f"CERT_PASSWORD={password}\n")

print(f".env file updated at {env_path.resolve()}")

result = subprocess.run(["dotnet", "dev-certs", "https", "-ep", cert_dir + "/veiling.client.pfx", "-p", password], capture_output=True, text=True)

print(result.stdout)
