$scriptDirectory = Get-Location

Set-Location "IPBot/IPBot"
Publish-Container "ipbot"

Set-Location $scriptDirectory

Set-Location "IPBot/IPBot.API"
Publish-Container "ipbot-api"

ssh $env:SERVER_NAME 'cd ~/IPBot && docker compose pull && docker compose up -d'

Set-Location $scriptDirectory