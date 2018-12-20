$args = @{
    Uri = "http://localhost:64252/api/heartbeat"
    Method = "Get"
    Headers = @{ "DiagnosticsAPIKey"="Secret" }
}

Invoke-RestMethod @args -UseBasicParsing

