<#
   .SYNOPSIS
        Deletes files and directories (if empty) matching specified in configuration
        Json file

   .DESCRIPTION
        Deletes files and directories (if empty) matching specified in configuration
        Json file.

        The criteria for selecting the files to be deleted is based on a number of
        days from today and file extension

   .NOTES
        None

   .PARAMETER configFile
              Required. Json configuration file with entries specifying the
              the directories that need to be crawled and searched for stale files

   .OUTPUT
        Returns no zero value if script failed

   .EXAMPLE
        .\Purge-StaleFiles.ps1 -configFile config.json
#>


[CmdletBinding()]
param(
    [Parameter(Mandatory=$true,
               Position=0,
               HelpMessage='Provide a valid configuration file')]
    [ValidateScript({test-path $_})]
    [string] $configFile
)

function script:Remove-EmptyDirectories(
    [string] $path,
    [DateTime] $staleDate = [DateTime]::MaxValue)
{
  Trace-Info "Searching for empty directories..."
  $trace = $CR
  $deleteErrors = @()

  # Find directories that have no files and were created before the threshold date
  $directories = Get-ChildItem -Path $path -Recurse -Directory `
    | where { (Get-ChildItem -Path $_.FullName -Recurse -Force -File) -eq $null } `
    | where { $_.LastWriteTime -lt $staleDate }

  if($directories.Length -eq 0)
  {
    Trace-Info "Searching for empty directories... Done (no hits)"
    return
  }

  # Get directories listed by depth
  $directories = $directories | % { New-Object PSObject -Property @{ Object = $_ ; Depth = ($_.FullName.Split("\")).Count } } `
                | sort Depth -descending

  foreach ($directory in $directories)
  {
    $trace += "Deleting $($directory.Object.FullName) $CR"
    Remove-Item -Force -Recurse $($directory.Object.FullName) -ErrorAction SilentlyContinue -ErrorVariable +deleteErrors
  }

  Trace-Info $trace

  if ($deleteErrors)
  {
    $errorTrace = $deleteErrors | foreach { $_.ToString() } | Out-String
    Trace-Error "Problem while deleting files $CR $errorTrace"
  }

  Trace-Info "Searching for empty directories... Done"
}

function script:Remove-StaleFiles(
  [string] $path,
  [DateTime] $staleDate,
  [string[]] $extensions,
  [switch] $recurse)
{
  Trace-Info "Searching for stale files..."
  $trace = $CR
  $deleteErrors = @()

  $files = Get-ChildItem -Path $path -Include $extensions -Recurse:$recurse -File `
      | where { (($_.LastWriteTime -lt $staleDate) -and ($_.FullName -notlike "*\bin\*") -and ($_.FullName -notlike "*\CRX542\*") -and ($_.FullName -notlike "*\Development\*") -and 
      ($_.FullName -notlike "*\Innovation\*")) }
	    # This WHERE clause includes files older than a given date, and excludes any files under a folder named "bin".

  if($files.Length -eq 0)
  {
    Trace-Info "Searching for stale files... Done (no hits)"
    return
  }

  foreach ($file in $files)
  {
    $trace += "Deleting $file $CR"
    Remove-Item -Force $file -ErrorAction SilentlyContinue -ErrorVariable +deleteErrors
  }

  Trace-Info $trace

  if ($deleteErrors)
  {
    $errorTrace = $deleteErrors | foreach { $_.ToString() } | Out-String
    Trace-Error "Problem while deleting files $CR $errorTrace"
  }

  Trace-Info "Searching for stale files... Done"
}

function script:Process-ConfigEntries
{
  Trace-Info "Purging files..."
  $configEntries = (gc $configFile) -join "`n" | ConvertFrom-Json

  foreach ($entry in $configEntries)
  {
    Trace-Info "Processing with parameters`r`n`t$entry"
    $retentionDate = (get-date).AddDays($(-$entry.retentionDays))
	  
    Trace-Info "Stale Files $($entry.directoryPath) $retentionDate $($entry.fileExtensions) -Recurse:$($entry.recursive)"
    Remove-StaleFiles $($entry.directoryPath) $retentionDate $($entry.fileExtensions) -Recurse:$($entry.recursive)

    # if ( $($entry.removeEmptyDirectories) )
    # {
    #     Remove-EmptyDirectories $($entry.directoryPath) $retentionDate
    # }
  }

  Trace-Info "Purging... Done"
}

function script:Initialize-LoggingLibrary
{
    $script:modulePath = [IO.Path]::GetFullPath( (Convert-Path(Join-Path $script:librariesDirectory 'LibraryLogging.psm1') ) )

    if ( (test-path $script:modulePath) -eq $false )
    {
        throw "The LibraryLogging.psm1 not found"
    }

    Import-Module $script:modulePath -ArgumentList $SCRIPT_PATH -Force
    New-Logger "Purge-StaleFiles.ps1"
}

function script:Get-LibrariesPath
{
   $script:librariesDirectory = Join-Path $SCRIPT_PATH ..\Libraries

    if ((Test-Path $librariesDirectory) -eq $false)
    {
        $script:librariesDirectory = $SCRIPT_PATH
    }
}

function script:Initialize
{
    Get-LibrariesPath
    Initialize-LoggingLibrary
}

################################################################################
# SCRIPT BODY
################################################################################
$ErrorActionPreference = "Stop"
$CR = "`r`n"

# Get script's path
Set-Variable -Name SCRIPT_PATH -Value (Convert-Path(Split-Path (Resolve-Path $MyInvocation.MyCommand.Path))) -Scope local -ErrorAction SilentlyContinue
Initialize

try
{
  Process-ConfigEntries
}
catch [Exception]
{
    Trace-Critical $_.Exception
    Exit 1
}

Exit 0