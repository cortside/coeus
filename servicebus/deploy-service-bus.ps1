[CmdletBinding()]
Param 
([Parameter(Mandatory = $true)][String]$config,
[Parameter(Mandatory = $true)][string] $namespace,
[Parameter(Mandatory = $true)][string] $resourcegroup,
[Parameter(Mandatory = $false)][Switch] $Force)
<#

.SYNOPSIS
This script will update azure service bus namespace

.DESCRIPTION
This script uses a json configuration file to update an azure
service bus namespace to match the configuration.  Use exportSbConfig
script to generate json configuration based on an existing namespace.

.PARAMETER config
json configuration file.  in the following format
{
    "namespaceAuthorizationRules": [
        {
            "name": "ruleName",
            "rights": [
                "Send",
                "Manage",
                "Listen"
                ]
        }
    ],
    "queues": [
        {
            "name": "queueName",
            "maxSizeInMegabytes": 1024,
            "enableBatchedOperations": false,
            "enableDuplicateDetection": false,
            "duplicateDetectionHistoryTimeWindow": "PT10M",
            "lockDuration": "PT1M",
            "rules": [
                {
                    "name": "ruleName",
                    "rights": [
                        "Send",
                        "Manage",
                        "Listen"
                        ]
                }
            ]
        }
    ],
    "topics": [
        {
            "name": "topicEventName",
            "maxSizeInMegabytes": 1024,
            "enableBatchedOperations": false,
            "supportOrdering": false,
            "enableDuplicateDetection": false,
            "duplicateDetectionHistoryTimeWindow": "PT10M",
            "subscriptions": {
                "name": "topic.subscription",
                "deadLetteringOnFilterEvaluationExceptions": false,
                "enableBatchedOperations": false,
                "forwardTo": "forwarding.queue",
                "lockDuration": "PT1M"
            },
            "rules": [
                {
                    "name": "ruleName",
                    "rights": [
                        "Send",
                        "Manage",
                        "Listen"
                        ]
                }
            ]
        }
    ]
}

.EXAMPLE
./deploy-service-bus -config config.json -namespace namespace -resourcegroup resource-group

.EXAMPLE
./deploy-service-bus -config config.json -namespace namespace -Force
to remove configured topics, queues and rules not defined in the configuration file

.NOTES
- Interactive login to azure required.  Script uses permissions of user running the script.

#>
# azure cli returns timestamp in python timedelta duration format "[{D} day[s], ][H]H:MM:SS[.UUUUUU]" when querying.
# It expects it in ISO8601 format when creating or updating.
# The following two functions are to be able to compare those values.
function Convert-ISO8601ToTimespan {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true,
            ValueFromPipeline = $true,
            ValueFromPipelineByPropertyName = $true)]
        [string]$Duration
    )
    try {
        $response = [Xml.XmlConvert]::ToTimeSpan($Duration)
        return $response
    }
    catch {
        return New-TimeSpan
    }
}
function Convert-StringToTimespan {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true,
            ValueFromPipeline = $true,
            ValueFromPipelineByPropertyName = $true)]
        [string]$Duration
    )
    $result = New-TimeSpan
    if ($Duration -as [TimeSpan]) {
        $result = [TimeSpan]$Duration
    }
    else {
        $days = (($Duration -Split (','))[0] -Split (' '))[0]
        $time = ($Duration -Split (','))[-1]
        if ($time -as [timespan]) {
            try {
                $daysts = New-TimeSpan -Days $days
                $timets = [timespan]$time
                $result = $daysts + $timets
            }
            catch {
                $result = New-TimeSpan
            }
        }
    }
    return $result
}
function GetDefaultISO860Timespan {
    return [Xml.XmlConvert]::ToString([TimeSpan]::Parse("366:0:0:0.0"))
}
function Login {
    $needLogin = $true
    Try {
        $content = az account list 2>$null
        if ($content) {
            $content = $content | ConvertFrom-Json
            $needLogin = ([string]::IsNullOrEmpty($content.isDefault))
        } 
    }
    Catch {
        if ($_ -like "*Login-AzureRmAccount to login*") {
            $needLogin = $true
        }
        else {
            throw
        }
    }
    if ($needLogin) {
        az login --only-show-errors | Out-Null
    }
}
function CheckResourceGroup {
    $resourceGroupExists = az group show --name $resourcegroup 2>$null
    if (-Not $resourceGroupExists) {
        Write-Output "creating resource group $resourceGroup"
        az group create --location westus --name $resourcegroup --only-show-errors | Out-Null
    }
    else {
        Write-Verbose "Resource group $resourceGroup already exists"
    }
}
function CheckNameSpace {
    $namespaceExists = az servicebus namespace exists --name $namespace --query nameAvailable 2>$null
    if (-Not $namespaceExists.Equals("false")) {
        Write-Output "creating namespace $namespace"
        az servicebus namespace create --resource-group $resourceGroup --name $namespace --location westus --sku Premium --only-show-errors | Out-Null
    }
    else {
        Write-Verbose "namespace $namespace already exists."
    }
}
function DifferentRights {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true)]
        [String]$rights1,
        [Parameter(Mandatory = $true)]
        [String]$rights2
    )
    if ([string]::IsNullOrEmpty($rights1)) {
        $array1 = @()
    }
    else {
        $array1 = $rights1 -Split (' ')
    }
    if ([string]::IsNullOrEmpty($rights2)) {
        $array2 = @()
    }
    else {
        $array2 = $rights2 -Split (' ')
    }
    return [String]::IsNullOrEmpty((Compare-Object -ReferenceObject $array1 -DifferenceObject $array2))
}
function CheckAuthorizationRules {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true,
            ValueFromPipeline = $true,
            ValueFromPipelineByPropertyName = $true)]
        [AllowNull()]
        [System.Object[]]$rules,
        [Parameter(Mandatory = $false,
            ValueFromPipelineByPropertyName = $true)]
        [String]$topic,
        [Parameter(Mandatory = $false,
            ValueFromPipelineByPropertyName = $true)]
        [string]$queue
    )
    $ruleFlags = ''
    $ruleType = 'namespace'
    if ($topic) {
        $ruleFlags += " --topic-name $topic"
        $ruleType = 'topic'
    }
    if ($queue) {
        $ruleFlags += " --queue-name $queue"
        $ruleType = 'queue'
    }
    if ($rules -and $rules.Count -gt 0) {
        foreach ($rule in $rules) {
            if (!([String]::IsNullOrEmpty($rule.name))) {
                $cmd = "az servicebus $ruleType authorization-rule show --resource-group $resourcegroup --namespace-name $namespace $ruleFlags --name $($rule.name) | ConvertFrom-Json"
                Write-Debug $cmd
                $existingRule = Invoke-Expression $cmd 2>$null
                if ([string]::IsNullOrEmpty($existingrule)) {
                    Write-Output "creating $ruleType rule $($rule.name)..."
                    $cmd2 = "az servicebus $ruleType authorization-rule create --resource-group $resourcegroup --namespace-name $namespace $ruleFlags --name $($rule.name) --rights $($rule.rights) --only-show-errors"
                    Write-Debug $cmd2
                    Invoke-Expression $cmd2 | Out-Null
                }
                else {
                    Write-Verbose "$ruleType rule $($rule.name) already exists."
                    $runUpdate = $false
                    $rights1 = ($rule.rights  | Out-String)
                    $rights2 = ($existingRule.rights  | Out-String)
                    $updateSettings = " --rights $($rule.rights)"
                    if (-Not(DifferentRights -rights1 $rights1 -rights2 $rights2)) {
                        Write-Output "updating rights for $ruleType rule $($rule.name) from $($rights) to $($rule.rights)."
                        $runUpdate = $true
                    }
                    if ($runUpdate) {
                        $cmd2 = "az servicebus $ruleType authorization-rule update --resource-group $resourcegroup --namespace-name $namespace $ruleFlags --name $($rule.name) $updateSettings"
                        Write-Debug $cmd2
                        Invoke-Expression $cmd2 | Out-Null
                    }
                }
            }
        }
    }
    if ($Force) {
        Write-Verbose "Checking for removed rules..."
        $cmd = "az servicebus $ruleType authorization-rule list --resource-group $resourcegroup --namespace-name $namespace $ruleFlags| ConvertFrom-Json"
        Write-Debug $cmd
        $definedrules = Invoke-Expression $cmd 2>$null
        foreach ($rule in $definedrules) {
            if ((-Not $rules -or $rule.name -notin $rules.name) -and ($rule.name -ne "RootManageSharedAccessKey")) {
                Write-Output "removing $ruleType rule $($rule.name)..."
                $cmd2 = "az servicebus $ruleType authorization-rule delete --resource-group $resourcegroup --namespace-name $namespace $ruleFlags --name $($rule.name) --only-show-errors"
                Write-Debug $cmd2
                Invoke-Expression $cmd2 | Out-Null
            }
        }
    }
}
function CheckQueues {
    param ([System.Object[]]$queues)
    foreach ($queue in $queues) {
        if ($queue.lockDuration) {
            $lockDuration = $queue.lockDuration
        }
        else {
            $lockDuration = [Xml.XmlConvert]::ToString((New-TimeSpan -Minutes 1))
        }
        if ($queue.duplicateDetectionHistoryTimeWindow) {
            $duplicateDetectionHistoryTimeWindow = $queue.duplicateDetectionHistoryTimeWindow
        }
        else {
            $duplicateDetectionHistoryTimeWindow = [Xml.XmlConvert]::ToString((New-TimeSpan -Minutes 10))
        }
        if ($queue.defaultMessageTimeToLive) {
            $defaultMessageTimeToLive = $queue.defaultMessageTimeToLive
        }
        else {
            $defaultMessageTimeToLive = GetDefaultISO860Timespan
        }
        if ($queue.autoDeleteOnIdle) {
            $autoDeleteOnIdle = $queue.autoDeleteOnIdle
        }
        else {
            $autoDeleteOnIdle = GetDefaultISO860Timespan
        }
        $updateSettings = " --max-size $($queue.maxSizeInMegabytes)"
        $updateSettings += " --enable-batched-operations $($queue.enableBatchedOperations)"
        $updateSettings += " --duplicate-detection $($queue.enableDuplicateDetection)"
        $updateSettings += " --lock-duration $lockDuration"
        $updateSettings += " --duplicate-detection-history-time-window $duplicateDetectionHistoryTimeWindow"
        $updateSettings += " --default-message-time-to-live $defaultMessageTimeToLive"
        $updateSettings += " --auto-delete-on-idle $autoDeleteOnIdle"
        $cmd = "az servicebus queue show --resource-group $resourcegroup --namespace-name $namespace --name $($queue.name) | ConvertFrom-Json"
        Write-Debug $cmd
        $existingQueue = Invoke-Expression $cmd 2>$null
        if ([string]::IsNullOrEmpty($existingQueue)) {
            Write-Output "creating queue $($queue.name)..."
            $cmd2 = "az servicebus queue create --resource-group $resourcegroup --namespace-name $namespace --name $($queue.name)$updateSettings"
            Write-Debug $cmd2
            Invoke-Expression $cmd2 | Out-Null
        }
        else {
            Write-Verbose "queue $($queue.name) already exists."
            $runUpdate = $false
            if ($existingQueue.maxSizeInMegabytes -ne $queue.maxSizeInMegabytes) {
                Write-Output "updating max size for queue $($queue.name) from $($existingQueue.maxSizeInMegabytes) to $($queue.maxSizeInMegabytes)."
                $runUpdate = $true
            }
            # if ($existingQueue.enableBatchedOperations -ine $queue.enableBatchedOperations) {
            #     Write-Output "updating batch operation flag for queue $($queue.name) from $($existingQueue.enableBatchedOperations) to $($queue.enableBatchedOperations)."
            #     $runUpdate = $true
            # }
            if (TimeSpansAreNotEqual -existing $existingQueue.lockDuration -config $lockDuration) {
                Write-Output "updating lock duration for queue $($queue.name) from $($existingQueue.lockDuration) to $lockDuration."
                $runUpdate = $true
            }
            if (TimeSpansAreNotEqual -existing $existingQueue.duplicateDetectionHistoryTimeWindow -config $duplicateDetectionHistoryTimeWindow) {
                Write-Output "updating duplicate detection history time window for queue $($queue.name) from $($existingQueue.duplicateDetectionHistoryTimeWindow) to $duplicateDetectionHistoryTimeWindow."
                $runUpdate = $true
            }
            if (TimeSpansAreNotEqual -existing $existingQueue.defaultMessageTimeToLive -config $defaultMessageTimeToLive) {
                Write-Output "updating default message time to live for queue $($queue.name) from $($existingQueue.defaultMessageTimeToLive)to $defaultMessageTimeToLive."
                $runUpdate = $true
            }
            if ($runUpdate) {
                $cmd2 = "az servicebus queue update --resource-group $resourcegroup --namespace-name $namespace --name $($queue.name) $updateSettings"
                Write-Debug $cmd2
                Invoke-Expression $cmd2 | Out-Null
            }
        }
        CheckAuthorizationRules -queue $queue.name -rules $queue.rules
    }
    if ($Force) {
        Write-Verbose "Checking for removed queues..."
        $cmd = "az servicebus queue list --resource-group $resourcegroup --namespace-name $namespace | ConvertFrom-Json"
        Write-Debug $cmd
        $definedQueues = Invoke-Expression $cmd 2>$null
        foreach ($queue in $definedQueues) {
            if ($queue.name -notin $queues.name) {
                Write-Output "removing queue $($queue.name)..."
                $cmd2 = "az servicebus queue delete --resource-group $resourcegroup --namespace-name $namespace --name $($queue.name)"
                Write-Debug $cmd2
                Invoke-Expression $cmd2 | Out-null
            }
        }
    }
}
function CheckTopics {
    param ([System.Object[]]$topics)
    foreach ($topic in $topics) {
        if ($topic.duplicateDetectionHistoryTimeWindow) {
            $duplicateDetectionHistoryTimeWindow = $topic.duplicateDetectionHistoryTimeWindow
        }
        else {
            $duplicateDetectionHistoryTimeWindow = [Xml.XmlConvert]::ToString((New-TimeSpan -Minutes 10))
        }
        if ($topic.defaultMessageTimeToLive) {
            $defaultMessageTimeToLive = $topic.defaultMessageTimeToLive
        }
        else {
            $defaultMessageTimeToLive = GetDefaultISO860Timespan
        }
        if ($queue.autoDeleteOnIdle) {
            $autoDeleteOnIdle = $queue.autoDeleteOnIdle
        }
        else {
            $autoDeleteOnIdle = GetDefaultISO860Timespan
        }
        $updateSettings = " --max-size $($topic.maxSizeInMegabytes)"
        $updateSettings += " --enable-batched-operations $($topic.enableBatchedOperations)"
        $updateSettings += " --duplicate-detection $($topic.enableDuplicateDetection)"
        $updateSettings += " --enable-ordering $($topic.supportOrdering)"
        $updateSettings += " --duplicate-detection-history-time-window $duplicateDetectionHistoryTimeWindow"
        $updateSettings += " --default-message-time-to-live $defaultMessageTimeToLive"
        $updateSettings += " --auto-delete-on-idle $autoDeleteOnIdle"
        $cmd = "az servicebus topic show --resource-group $resourcegroup --namespace-name $namespace --name $($topic.name) | ConvertFrom-Json"
        Write-Debug $cmd
        $existingtopic = Invoke-Expression $cmd 2>$null
        if ([string]::IsNullOrEmpty($existingtopic)) {
            Write-Output "creating topic $($topic.name)"
            $cmd2 = "az servicebus topic create --resource-group $resourcegroup --namespace-name $namespace --name $($topic.name)$updateSettings"
            Write-Debug $cmd2
            Invoke-Expression $cmd2 | Out-Null
        }
        else {
            Write-Verbose "topic $($topic.name) already exists."
            $runUpdate = $false
            if ($existingtopic.maxSizeInMegabytes -ne $topic.maxSizeInMegabytes) {
                Write-Output "updating max size for topic $($topic.name) from $($existingtopic.maxSizeInMegabytes) to $($topic.maxSizeInMegabytes)."
                $runUpdate = $true
            }
            # if ($existingtopic.enableBatchedOperations -ine $topic.enableBatchedOperations) {
            #     Write-Output "updating batch operation flag for topic $($topic.name) from $($existingtopic.enableBatchedOperations) to $($topic.enableBatchedOperations)."
            #     $runUpdate = $true
            # }
            # if ($existingtopic.supportOrdering -ine $topic.supportOrdering) {
            #     Write-Output "updating support ordering flag for topic $($topic.name) from $($existingtopic.supportOrdering) to $($topic.supportOrdering)."
            #     $runUpdate = $true
            # }
            if (TimeSpansAreNotEqual -existing $existingtopic.duplicateDetectionHistoryTimeWindow -config $duplicateDetectionHistoryTimeWindow) {
                Write-Output "updating duplicate detection history time window for topic $($topic.name) from $($existingtopic.duplicateDetectionHistoryTimeWindow) to $duplicateDetectionHistoryTimeWindow."
                $runUpdate = $true
            }
            if (TimeSpansAreNotEqual -existing $existingtopic.defaultMessageTimeToLive -config $defaultMessageTimeToLive) {
                Write-Output "updating defaultMessageTimeToLive for topic $($topic.name) from $($existingtopic.defaultMessageTimeToLive) to $defaultMessageTimeToLive."
                $runUpdate = $true
            }
            if ($runUpdate) {
                $cmd2 = "az servicebus topic update --resource-group $resourcegroup --namespace-name $namespace --name $($topic.name) $updateSettings"
                Write-Debug $cmd2
                Invoke-Expression $cmd2 | Out-Null
            }
        }
        CheckSubscriptions -topic $topic
        CheckAuthorizationRules -topic $topic.name -rules $topic.rules
    }
    if ($Force) {
        Write-Verbose "Checking for removed topics..."
        $cmd = "az servicebus topic list --resource-group $resourcegroup --namespace-name $namespace | ConvertFrom-Json"
        Write-Debug $cmd
        $definedTopics = Invoke-Expression $cmd 2>$null
        foreach ($topic in $definedTopics) {
            if ($topic.name -notin $topics.name) {
                Write-Output "removing topic $($topic.name)..."
                $cmd2 = "az servicebus topic delete --resource-group $resourcegroup --namespace-name $namespace --name $($topic.name)"
                Write-Debug $cmd2
                Invoke-Expression $cmd2 | Out-Null
            }
        }
    }
}
function TimeSpansAreNotEqual {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true)]
        [String]$existing,
        [Parameter(Mandatory = $true)]
        [string]$config
    )
    $tsExisting = Convert-StringToTimespan($existing)
    $tsConfig = Convert-ISO8601ToTimespan($config)
    if ($tsExisting -ne $tsConfig) {
        return $true
    }
    return $false
}
function CheckSubscriptions {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [System.Object]$topic
    )
    if ($topic.subscriptions) {
        $subscriptions = $topic | select -ExpandProperty subscriptions
        foreach ($subscription in $subscriptions) {
            if ($subscription.deadLetteringOnFilterEvaluationExceptions) {
                $deadLetteringOnFilterEvaluationExceptions = $subscription.deadLetteringOnFilterEvaluationExceptions
            }
            else {
                $deadLetteringOnFilterEvaluationExceptions = $false
            }
            if ($subscription.lockDuration) {
                $lockDuration = $subscription.lockDuration
            }
            else {
                $lockDuration = [Xml.XmlConvert]::ToString((New-TimeSpan -Minutes 1))
            }
            if ($subscription.defaultMessageTimeToLive) {
                $defaultMessageTimeToLive = $subscription.defaultMessageTimeToLive
            }
            else {
                $defaultMessageTimeToLive = GetDefaultISO860Timespan
            }
            if ($queue.autoDeleteOnIdle) {
                $autoDeleteOnIdle = $queue.autoDeleteOnIdle
            }
            else {
                $autoDeleteOnIdle = GetDefaultISO860Timespan
            }
            $updateSettings = " --enable-batched-operations $($subscription.enableBatchedOperations)"
            $updateSettings += " --dead-letter-on-filter-exceptions $deadLetteringOnFilterEvaluationExceptions"
            $updateSettings += " --forward-to $($subscription.forwardTo)"
            $updateSettings += " --lock-duration $($lockDuration)"
            $updateSettings += " --default-message-time-to-live $($defaultMessageTimeToLive)"
            $updateSettings += " --auto-delete-on-idle $autoDeleteOnIdle"
            $cmd = "az servicebus topic subscription show --resource-group $resourcegroup --namespace-name $namespace --topic-name $($topic.name) --name $($subscription.name) | ConvertFrom-Json"
            Write-Debug $cmd
            $existingsubscription = Invoke-Expression $cmd 2> $null
            if ([string]::IsNullOrEmpty($existingsubscription)) {
                Write-Output "creating subscription $($subscription.name)"
                $cmd2 = "az servicebus topic subscription create --resource-group $resourcegroup --namespace-name $namespace --topic-name $($topic.name) --name $($subscription.name)$updateSettings"
                Write-Debug $cmd2
                Invoke-Expression $cmd2 | Out-Null
            }
            else {
                Write-Verbose "subscription $($subscription.name) already exists."
                $runUpdate = $false
                if ($deadLetteringOnFilterEvaluationExceptions -ne $existingsubscription.deadLetteringOnFilterEvaluationExceptions) {
                    Write-Output "updating dead letter of filter exception flag for subscription $($subscription.name) from $($existingsubscription.deadLetteringOnFilterEvaluationExceptions) to $($deadLetteringOnFilterEvaluationExceptions)."
                    $runUpdate = $true
                }
                # if ($existingsubscription.enableBatchedOperations -ine $subscription.enableBatchedOperations) {
                #     Write-Output "updating batch operation flag for subscription $($subscription.name) from $($existingsubscription.enableBatchedOperations) to $($subscription.enableBatchedOperations)."
                #     $runUpdate = $true
                # }
                if (TimeSpansAreNotEqual -existing $existingSubscription.lockDuration -config $lockDuration) {
                    Write-Output "updating lock duration for subscription $($subscription.name) from $($existingsubscription.lockDuration) to $($lockDuration)."
                    $runUpdate = $true
                }
                if (TimeSpansAreNotEqual -existing $existingSubscription.defaultMessageTimeToLive -config $defaultMessageTimeToLive) {
                    Write-Output "updating lock duration for subscription $($subscription.name) from $($existingsubscription.defaultMessageTimeToLive) to $($defaultMessageTimeToLive)."
                    $runUpdate = $true
                }
                if ($existingsubscription.forwardTo -ine $subscription.forwardTo) {
                    Write-Output "updating forward to queue for subscription $($subscription.name) from $($existingsubscription.forwardTo) to $($subscription.forwardTo)."
                    $runUpdate = $true
                }
                if ($runUpdate) {
                    $cmd2 = "az servicebus topic subscription update --resource-group $resourcegroup --namespace-name $namespace --topic-name $($topic.name) --name $($subscription.name)$updateSettings"
                    Write-Debug $cmd2
                    Invoke-Expression $cmd2 | Out-Null
                }
            }
        }
    }
    if ($Force) {
        Write-Verbose "Checking for removed subscriptions..."
        $cmd = "az servicebus topic subscription list --resource-group $resourcegroup --namespace-name $namespace --topic-name $($topic.name) | ConvertFrom-Json"
        Write-Debug $cmd
        $definedSubscriptions = Invoke-Expression $cmd 2>$null
        foreach ($subscription in $definedSubscriptions) {
            if (-Not $subscriptions -or $subscription.name -notin $subscriptions.name) {
                Write-Output "removing subscription $($subscription.name)..."
                $cmd2 = "az servicebus topic subscription delete --resource-group $resourcegroup --namespace-name $namespace --topic-name $($topic.name) --name $($subscription.name)"
                Write-Debug $cmd2
                Invoke-Expression $cmd2
            }
        }
    }
}
try {
    Login
    CheckResourceGroup
    CheckNameSpace
    if (Test-Path $config) {
        try {
            Write-Verbose "Reading configuration file $config"
            Write-Verbose "Parsing config"
            $sbconfig = (Get-Content $config | ConvertFrom-Json -ErrorAction Stop)
            $validConfig = $true
        }
        catch {
            Write-Verbose "An error occurred:"
            Write-Verbose $_
            $validConfig = $false
        }
        if ($validConfig) {
            CheckAuthorizationRules -rules $sbconfig.namespaceAuthorizationRules
            CheckQueues -queues $sbconfig.queues
            CheckTopics -topics $sbconfig.topics
        }
        else {
            Write-Error "Configuration file $config content could not be parsed."
        }
    }
    else {
        Write-Error "Configuration file $config was not found." 
    }
}
catch {
    Write-Output "Script did not finish successfully $($_.Exception.Message)"
}
finally {
    Remove-Variable -Name resourcegroup -Scope script 2>$1 | Out-Null
    Remove-Variable -Name namespace -Scope script 2>$1 | Out-Null
    Remove-Variable -Name sbconfig 2>$1 | Out-Null
}
