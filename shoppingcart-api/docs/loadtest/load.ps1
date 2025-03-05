$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
$headers.Add("Content-Type", "application/json")
$headers.Add("Authorization", "Bearer 32335B921D74BC0C15951CBB60DC640284FBB6DA61BCE160AE621033CA9FA62D")

$body = @"
{
  `"customer`": {
        `"firstName`": `"Jack`",
        `"lastName`": `"Doe`",
        `"email`": `"jack.doe@gmail.com`"
  },
  `"address`": {
    `"street`": `"1234 Main St`",
    `"city`": `"Salt Lake City`",
    `"state`": `"UT`",
    `"country`": `"USA`",
    `"zipCode`": `"84123`"
  },
  `"items`": [
      { 
          `"sku`": `"123`", 
          `"quantity`": 2
      },
      { 
          `"sku`": `"234`", 
          `"quantity`": 1
      }
  ]
}
"@

$startTime = Get-Date
$stopTime = (Get-Date).AddMinutes(5)
$iterations = 0

while ((Get-Date) -lt $stopTime) {
    # Your code to be executed repeatedly
    Write-Host "Looping..."
    $response = Invoke-RestMethod 'http://localhost:5000/api/v1/orders' -Method 'POST' -Headers $headers -Body $body
	$iterations += 1
}

$stopTime = Get-Date
Write-Host "Loop finished."

Write-Output "Start Time: $startTime"
Write-Output "Stop Time: $stopTime"
Write-Output "Duration: $($stopTime - $startTime)"
Write-Output "Iterations: $iterations"

