function Restore-Database
{
    # Create Data locations
    $path = "D:\DATA\"
    If(!(test-path $path))
        {
            New-Item -ItemType Directory -Force -Path $path
        }
    #Load Assemblies Required for Restore
    [reflection.assembly]::LoadWithPartialName("Microsoft.SqlServer.Smo")
    [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoExtended") | Out-Null
    [Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.ConnectionInfo") | Out-Null
    [Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoEnum") | Out-Null
    #Create server object and set timeout to 0 to avoid restore time out
    $server = New-Object ("Microsoft.SqlServer.Management.Smo.Server") $dbInstance
    $server.ConnectionContext.StatementTimeout = 0
    $MachineName = (Get-WmiObject -Class Win32_ComputerSystem -Property Name).Name + '\SQLEXPRESS'
    $RelocateData = New-Object Microsoft.SqlServer.Management.Smo.RelocateFile("AdventureWorks_Data", "D:\DATA\AdventureWorks_Data.mdf") 
    $RelocateLog = New-Object Microsoft.SqlServer.Management.Smo.RelocateFile("AdventureWorks_Log", "D:\DATA\AdventureWorks_Log.ldf") 
    $file = New-Object Microsoft.SqlServer.Management.Smo.RelocateFile($RelocateData,$RelocateLog) 
    $myarr=@($RelocateData,$RelocateLog)
    #Restore Database
    $backupfile1 = "D:\Backups\Adventureworks1.bak"
    $backupfile2 = "D:\Backups\Adventureworks2.bak"
    $backupfile3 = "D:\Backups\Adventureworks3.bak"
    Restore-SqlDatabase -ServerInstance $MachineName -Database AdventureWorks -BackupFile $backupfile1,$backupfile2,$backupfile3 -RelocateFile $myarr
}   

# This Function Will Query A Dependancy Service and wait until the timer expires OR for the service to start. 
function QueryService { param($Service,$timer1) 
    $success = "" 
    write-host "Waiting on $Service Service..."      
    # Create a for loop to INC a timer Every Second 
   for ($b=1; $b -lt $timer1; $b++) { 
            $servicestat = get-service $Service 
            $status = $servicestat.status 
            $b2 = $timer1 - $b 
 
            # Determine the Percent Complete for the seconds. 
            $percent = $b * (100 / $timer1) 
             
            # Display the progress on the Screen 
            Write-Progress -Activity "Waiting on $Service Service..." -PercentComplete $percent -CurrentOperation "$b2 Seconds Remaining" -Status "Current WMI Status: $status" 
             
            # Determine if the Process is Running. If not, reloop. If so exit loop. 
            if ($status -eq "Running") { 
                write-host "$Service Service Started Successfully: $status in $b Seconds"  
                [int]$b = $timer1     
                    $success = "yes" 
                 
                # Tells the Loop to Stop Incrementing as the Service is running 
                Write-Progress -Activity "Completed" -Status "Current $Service Status: $status in $b Seconds" -Completed  
                 
            } 
         
        # Start-Sleep is available for the write-progress. Its value is in seconds. 
        start-sleep 1 
 
    }  
    # The script will now stop as the above loop has meet its time criteria and the success is not set to yes.time has ex-pired. 
    if ($success -ne "yes") {  
        # Stop the Script 
       BREAK 
    }  
    Else
    {
       #Restore Database
        Restore-Database
    }
} 

$region = "us-east-1"
# The name of your S3 Bucket
$bucket = "sql-backups-demo"
# The folder in your bucket to copy, including trailing slash. Leave blank to copy the entire bucket
$keyPrefix = "SQLServer/Backups/"
# Create Download locations
$path = "D:\Backups\"
If(!(test-path $path))
    {
        New-Item -ItemType Directory -Force -Path $path
    }

#Download Database from S3
$objects = Get-S3Object -BucketName $bucket -KeyPrefix $keyPrefix -Region $region

foreach($object in $objects) {
    $localFileName = $object.Key -replace $keyPrefix, ''
    if ($localFileName -ne '') {
        $localFilePath = Join-Path $path $localFileName
        Copy-S3Object -BucketName $bucket -Key $object.Key -LocalFile $localFilePath -Region $region
    }
}

#Begin Restore Process, check if SQL is up and running first give it 2 hours to standup
QueryService 'MSSQL$SQLEXPRESS' "7200"
