#$Parent = 'D:\MySuperCoolMusic'  # CHANGE THIS TO YOUR DESIRED FOLDER!!! #
$Dirs = Get-ChildItem -Path ./src -directory -Recurse

foreach ($item in $Dirs) {   
    if ((get-childitem $item.Fullname | measure-object | select -ExpandProperty count) -eq '0') {
    write-verbose -Message "The folder $($item.Fullname) is empty" -Verbose

    remove-item $item.Fullname -Verbose
    }
}