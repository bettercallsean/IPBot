$scriptDirectory = Get-Location

Set-Location "IPBot"
Publish-Container "ipbot"

Set-Location $scriptDirectory

Set-Location "IPBot.API"
Publish-Container "ipbot-api"

ssh $env:SERVER_NAME 'cd ~/IPBot && docker compose pull && docker-compose up -d --no-build'

Set-Location $scriptDirectory