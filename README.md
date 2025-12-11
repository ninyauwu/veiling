Veilingwebsite voor jem_id. Gebouwd met behulp van React en ASP.NET.
Ook een veiler worden? Ga naar https://www.jem-id.nl/

To run tests and get a view of test coverage, execute these commands in the root of the project: 
```cpp
cd C:\Users\stefa\RiderProjects\veiling

Remove-Item Veiling.Server.Test\TestResults -Recurse -ErrorAction SilentlyContinue
Remove-Item coveragereport -Recurse -ErrorAction SilentlyContinue

dotnet test --collect:"XPlat Code Coverage"

$coverageFile = Get-ChildItem -Path "Veiling.Server.Test\TestResults" -Recurse -Filter "coverage.cobertura.xml" | Select-Object -First 1

reportgenerator -reports:"$($coverageFile.FullName)" -targetdir:"coveragereport" -reporttypes:Html

start coveragereport/index.html
```