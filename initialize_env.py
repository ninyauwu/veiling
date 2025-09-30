import getpass
import subprocess
import platform
import os
import click
from pathlib import Path
from dotenv import dotenv_values
from dotenv import set_key
import socket

def can_connect(host, port=1433, timeout=4):
    try:
        with socket.create_connection((host, port), timeout=timeout):
            return True
    except OSError:
        return False

def create_certificate(cert_file, password):
    result = subprocess.run(["dotnet", "dev-certs", "https", "-ep", cert_file, "-p", password], capture_output=True, text=True)
    print (result.stdout)

# Setup path vars
is_windows = platform.system() == "Windows"
if is_windows:
    cert_dir = os.path.join(os.environ["USERPROFILE"], ".aspnet", "https")
else:
    cert_dir = os.path.expanduser("~/.aspnet/https")
cert_file = Path(cert_dir) / "veiling.client.pfx"
env_path = Path(".env")
env_vars = dotenv_values(env_path)

# Create SSL certificate if necessary
print("\n==Setting up SSL certificate==")
if cert_file.exists() & ("CERT_PASSWORD" in env_vars):
    if not click.confirm("Certificate already exists. Skip certificate creation?"):
        password = getpass.getpass("Enter a password for your (local) SSL certificate: ")
        set_key(env_path, "CERT_PASSWORD", password)
        create_certificate(cert_file, password)

# Connect to database
print("\n==Setting up database connection==")
connection_string = None
if not is_windows:
    result = subprocess.run(["grep nameserver /etc/resolv.conf | cut -d' ' -f2"], shell=True, capture_output=True, text=True)
    if result.returncode == 0:
        if can_connect(result.stdout.strip()):
            print(f"Sucessfully found SQL server at {result.stdout.strip()}:1433")
            connection_string = result.stdout.strip()
        else:
            print("Failed to connect to host bridge.")

if connection_string == None:
    if can_connect("localhost"):
        connection_string = "localhost"
        print("Successfully connected to localhost SQL server.")
    elif can_connect("127.0.0.1"):
        connection_string = "127.0.0.1"
        print("Successfully connected to localhost SQL server")
    else:
        print("Failed to connect to localhost SQL server.")
        print("Contact Nina, your SQL server might not be configured properly.")

if connection_string:
    set_key(env_path, "DB_SERVER", f"tcp:{connection_string},1433")
    set_key(env_path, "DB_NAME", "master")
    print()
    print("Please enter your SQL system admin name (will be saved locally):", end = " ")
    name = input()
    password = getpass.getpass("Please enter your SQL system admin password (will be saved locally): ")

    set_key(env_path, "DB_USERNAME", name)
    set_key(env_path, "DB_PASSWORD", password)

print(f".env file updated at {env_path.resolve()}")

