dotnet build src
dotnet pack src --include-symbols -p:SymbolPackageFormat=snupkg --configuration debug -o ((get-location).Path + '\artifacts') /property:Version=1.0-local
dotnet tool uninstall amqptools --global
dotnet tool install --global AmqpTools --version 1.0-local --configfile (Join-Path $PSScriptRoot nuget.local.config)