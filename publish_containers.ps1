$scriptDirectory = Get-Location

Set-Location "IPBot"
dotnet publish --os linux --arch arm64 /t:PublishContainer -c Release -p:ContainerFamily=jammy-chiseled
docker tag ipbot raspberrypi.local:9298/ipbot
docker push raspberrypi.local:9298/ipbot

Set-Location $scriptDirectory

Set-Location "IPBot.API"
dotnet publish --os linux --arch arm64 /t:PublishContainer -c Release -p:ContainerFamily=jammy-chiseled
docker tag ipbot-api raspberrypi.local:9298/ipbot-api
docker push raspberrypi.local:9298/ipbot-api

ssh pi 'cd ~/src/IPBot && docker compose pull && docker-compose up -d --no-build'
