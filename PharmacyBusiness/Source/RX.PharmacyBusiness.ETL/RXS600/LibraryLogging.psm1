param
(
  [Parameter(Mandatory=$true)]
  [string]$currentPath
)

function script:Initialize-Log
{
    param
    (
      [Parameter(Mandatory=$true)]
      [string] $currentPath
    )

    $configFile = Join-Path($currentPath) "log4net.cfg.xml"
    $assembly = Join-Path($currentPath) "..\Libraries\log4net.dll"
    $logPath = Join-Path($currentPath) "..\Logs"

    if ( (test-path $logPath) -eq $false )
    {
        $logPath = $currentPath
    }

    if ( (test-path $assembly) -eq $false)
    {
        $assembly = Join-Path $currentPath "log4net.dll"

        if ( (test-path $assembly) -eq $false)
        {
            throw "[log4net.dll] assembly not found: $assembly"
        }
        
        copy $assembly $env:TEMP -force
        $assembly = Join-Path $env:TEMP "log4net.dll"        
    }
    
    $assembly = Convert-Path $assembly

    [Void][System.Reflection.Assembly]::LoadFile($assembly)

    if ( (test-path $configFile) -eq $false) 
    {
        throw "[$configFile] logging config file not found"
    }
       
	$configFile = Convert-Path $configFile 
    $configFile = new-object System.IO.FileInfo( $configFile );   
    [log4net.GlobalContext]::Properties["LogFolder"] = $logPath	
    $xmlConfigurator = [log4net.Config.XmlConfigurator]::Configure($configFile);   

    $global:LogManager = [log4net.LogManager] 
    $global:logger = $LogManager::GetLogger((Get-ChildItem $MyInvocation.ScriptName).Name);	
}

function Trace-Info
{
    param
    (
      [Parameter(Mandatory=$true)]
      [string] $message
    )
    
    $logger.Info($message);
}

function Trace-Warn
{
    param
    (
      [Parameter(Mandatory=$true)]
      [string] $message
    )
    
    $logger.Warn($message);
}

function Trace-Error
{
    param
    (
      [Parameter(Mandatory=$true)]
      [string] $message
    )
    
    $logger.Error($message);
}

function Trace-Critical
{
    param
    (
      [Parameter(Mandatory=$true)]
      [Exception] $exception
    )
    $logger.Fatal($exception);
}

function New-Logger
{
    param
    (
      [Parameter(Mandatory=$true)]
      [string]$loggerName
    )
    
    $global:logger = $LogManager::GetLogger($loggerName);
}

Initialize-Log $currentPath