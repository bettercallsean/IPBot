$scriptDirectory = Get-Location

Set-Location "IPBot"
dotnet publish --os linux --arch arm64 /t:PublishContainer -c Release -p:ContainerFamily=jammy-chiseled
Push-Container("ipbot")

Set-Location $scriptDirectory

Set-Location "IPBot.API"
dotnet publish --os linux --arch arm64 /t:PublishContainer -c Release -p:ContainerFamily=jammy-chiseled
Push-Container("ipbot-api")

ssh pi 'cd ~/src/IPBot && docker compose pull && docker-compose up -d --no-build'
