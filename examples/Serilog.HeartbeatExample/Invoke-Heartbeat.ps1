$args = @{
    Uri = "http://localhost:64344/api/heartbeat"
    Method = "Get"
    Headers = @{ "DiagnosticsAPIKey"="Secret" }
}

Invoke-RestMethod @args -UseBasicParsing

