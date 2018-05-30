#Helper Functions
function Create-Folder {
    Param ([string]$path)
    if ((Test-Path $path) -eq $false) 
    {
        Write-Host $path +' doesn''t exist. Creating now..'
        New-Item -ItemType 'directory' -Path $path
    }
}

function Download-File{
    Param ([string]$src, [string] $dst)

    (New-Object System.Net.WebClient).DownloadFile($src,$dst)
    #Invoke-WebRequest $src -OutFile $dst
}

$setupFolder = 'D:\installs'
Create-Folder $setupFolder
Create-Folder $setupFolder'\sqlbi'
Create-Folder $setupFolder'\sqlbi\datasets'
Create-Folder $setupFolder'\sqlbi\installations'
$setupFolder = Join-Path -Path $setupFolder -ChildPath '\sqlbi\installations'
(Get-Content $setupFolder'\ConfigurationFile.ini').replace('USERNAMETBR', $env:computername+'\'+$env:username) | Set-Content $setupFolder\ConfigurationFile_local.ini
Copy-S3Object -BucketName sql-backups-demo -Key scripts/ConfigurationFile.ini -LocalFile $setupFolder'\ConfigurationFile.ini'  -Region 'us-east-1'

Download-File 'https://go.microsoft.com/fwlink/?LinkID=799012' $setupFolder\SQLServer2016-SSEI-Expr.exe
Download-File 'https://download.microsoft.com/download/3/1/D/31D734E0-BFE8-4C33-A9DE-2392808ADEE6/SSMS-Setup-ENU.exe' $setupFolder\SSMS-Setup-ENU.exe

(Get-Content $setupFolder'\ConfigurationFile.ini').replace('USERNAMETBR', $env:computername+'\'+$env:username) | Set-Content $setupFolder\ConfigurationFile_local.ini

$localconfigfilepath = $setupFolder + '\ConfigurationFile_local.ini'
$argulist1 = '/ConfigurationFile=' + $localconfigfilepath 
$argulist2 = '/MediaPath=' + $setupFolder
Write-Host 'Installing SQL Server..'
Start-Process -FilePath $setupFolder\SQLServer2016-SSEI-Expr.exe -ArgumentList $argulist1, $argulist2, '/IAcceptSqlServerLicenseTerms'  -Wait

Write-Host 'Installing SSMS..'
Start-Process -FilePath $setupFolder\SSMS-Setup-ENU.exe -ArgumentList '/install','/passive' -Wait

Add-PSSnapin SqlServerCmdletSnapin* -ErrorAction SilentlyContinue   
Import-Module SQLPS -WarningAction SilentlyContinue  

Write-Host 'Installation completed.'
