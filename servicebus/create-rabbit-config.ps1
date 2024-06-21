Param
(
    [Parameter(Mandatory = $false)][string]$filePath = "$PSScriptRoot/config.json"
)

$global:sb_que_names = [System.Collections.ArrayList]::new()
$global:sb_exchange_names = [System.Collections.ArrayList]::new()
$global:rabbit_bindings = [PSCustomObject] @{}

function  Begin {    

    $exists = Test-Path -Path $filePath -PathType Leaf

    if ($exists) {
        $global:sb_json = Get-Content $filePath | ConvertFrom-Json
        Get-Rabbit-QueNames 
        Get-Rabbit-ExchangeNames 
        Build-Rabbit-Config
    }
    else {
        Write-Host "File not found. Please ensure the file $filePath exists."
    }
}

function Get-Rabbit-QueNames {
    ForEach ($item in $global:sb_json.queues) {
        $rabbit_que_name = $item.name
        $global:sb_que_names.Add($rabbit_que_name)
    }
}

function Get-Rabbit-ExchangeNames {
    ForEach ($item in $global:sb_json.topics) {
        $rabbit_exchange_name = $item.name.split('.')[0] # -split ".", 1
        if ($global:sb_exchange_names -notcontains $rabbit_exchange_name) {
            $global:sb_exchange_names.Add($rabbit_exchange_name)
        }
    }
}

function Build-Rabbit-Config {

    $RabbitHashTable = [ordered]@{
        rabbit_version    = "3.6.15"
        users             = @(
            @{
                name              = "admin"
                password_hash     = "9X8QnCA+uDWKwxV+7WeE0IA4eqR32gtjjgikdiJYhqfQskmk"
                hashing_algorithm = "rabbit_password_hashing_sha256"
                tags              = "administrator"
            }
        )
        vhosts            = @(
            @{
                name = "/"
            }
        )
        permissions       = @(
            @{
                user      = "admin"
                vhost     = "/"
                configure = ".*"
                write     = ".*"
                read      = ".*"
            }
        )
        parameters        = @()
        global_parameters = @(
            @{
                name  = "cluster_name"
                value = "rabbit@rabbitmq"
            }
        )
        policies          = @()
        queues            = @(
            foreach ($queName in $global:sb_que_names) {
                @{
                    name        = $queName
                    vhost       = "/"
                    durable     = "true"
                    auto_delete = "false"
                    arguments   = "{}"
                }    
            }
        )
        exchanges         = @(
            foreach ($exchangeName in $global:sb_exchange_names ) {
                @{
                    name        = $exchangeName
                    vhost       = "/"
                    type        = "topic"
                    durable     = "true"
                    auto_delete = "false"
                    internal    = "false"
                    arguments   = "{}"
                }    
            }

        )
        bindings          = @(
            ForEach ($topic in $global:sb_json.topics) {
                $topicName = $topic.name.split(".") 
                ForEach ($sub in $topic.subscriptions) {
                    [ordered]@{
                        source           = $topicName[0]
                        vhost            = "/"
                        destination      = $sub.forwardTo
                        destination_type = "queue"
                        routing_key      = $topicName[1]
                        arguments        = "{}"
                    } 
                }
            }
        )
    }

    $rabbitFilepath = $PSScriptRoot + '\rabbitmq\rabbit_configuration.json'
    $jsonData = ($RabbitHashTable | ConvertTo-Json)
    $jsonData = $jsonData -replace '\"{}\"', '{}'

    $jsonData = $jsonData -replace '\"true\"', 'true'
    $jsonData = $jsonData -replace '\"false\"', 'false'
    
    ($jsonData | Set-Content $rabbitFilepath)
    
}

Begin