# Infrastructure
## Migration
Open terminal in this folder and run:
```sh
cd .\Skyress.Infrastructure\
dotnet ef migrations add {MigrationName} -s ..\Skyress\Skyress.API.csproj
```

## update database
```sh
 dotnet ef database update -s ..\Skyress\Skyress.API.csproj
```