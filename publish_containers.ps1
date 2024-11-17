$scriptDirectory = Get-Location

Set-Location "IPBot"
Publish-Container "ipbot"

Set-Location $scriptDirectory

Set-Location "IPBot.API"
Publish-Container "ipbot-api"

ssh pi 'cd ~/src/IPBot && docker compose pull && docker-compose up -d --no-build'
