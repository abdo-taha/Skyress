# infrastructure

## migration
open terminal in this folder and run :
```sh
cd .\Shop.Infrastructure\
dotnet ef migrations add {MigrationName} -s ..\shop\Shop.API.csproj
```

## update database
```sh
 dotnet ef database update -s ..\shop\Shop.API.csproj
```