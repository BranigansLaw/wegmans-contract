###
# Copies all folders within the TenTen artifact, completely replacing them if they already exist
###

### 
# Define Variables
###
$artifactAlias              = $env:RELEASE_PRIMARYARTIFACTSOURCEALIAS       # The alias of the primary artifact source
$artifactsDirectory         = $env:System_ArtifactsDirectory                # The directory to which artifacts are downloaded during deployment of a release.
$configExtension            = $env:ConfigExtension
$cpsDatabase                = $env:CpsDatabase
$netezzaDatabase            = $env:NetezzaDatabase
$netezzaId                  = $env:NetezzaId
$netezzaDb                  = $env:NetezzaDb

$srcRoot                    = "$artifactsDirectory/$artifactAlias/TenTen"
$destRoot                   = "X:\Batch"
$configFolder               = "$srcRoot/PharmacyBusiness/bin"

###
# Replace Config
###
Write-Host "Replacing '$configFolder\Wegmans.InterfaceEngine.exe.config' with Wegmans.InterfaceEngine.exe.config.$configExtension"
Copy-Item "$configFolder\Wegmans.InterfaceEngine.exe.config.$configExtension" -Destination "$configFolder\Wegmans.InterfaceEngine.exe.config"

###
# Copy files
###

# Find/replace passwords within config file:
Write-Host "Find and replace Azure Secrets in config file."
$oldConfigContent = Get-Content -Path "$srcRoot\PharmacyBusiness\bin\Wegmans.InterfaceEngine.exe.config"
$newConfigContent = $oldConfigContent -replace 'AZURE_CPS_WEG_BATCH_SECRET',$cpsDatabase
$newConfigContent = $newConfigContent -replace 'AZURE_NETZ_WEG_BATCH_ID',$netezzaId
$newConfigContent = $newConfigContent -replace 'AZURE_NETZ_WEG_BATCH_SECRET',$netezzaDatabase
$newConfigContent = $newConfigContent -replace 'AZURE_NETZ_WEG_BATCH_DB',$netezzaDb
$newConfigContent | Set-Content -Path "$srcRoot\PharmacyBusiness\bin\Wegmans.InterfaceEngine.exe.config"

# Loop over all the folders in $srcRoot
$folders = Get-ChildItem -Directory $srcRoot 

foreach ($folder in $folders) {
  # Find the new destination for the BIN folder
  $dest = (Join-Path  $destRoot $folder)
  $bin = (Join-Path  $dest "bin")
  
  # Remove the BIN folder if it exists
  if (Test-Path "$bin") {
    Write-Host "Removing: $bin"
    Remove-Item -Force "$bin" -Recurse 
  }

  # Make other folders if not exists
  if (!(Test-Path "$dest\Input")) {
    New-Item "$dest\Input" -ItemType "directory" -ErrorAction SilentlyContinue | Out-Null
  }
  if (!(Test-Path "$dest\Output")) {
    New-Item "$dest\Output" -ItemType "directory" -ErrorAction SilentlyContinue | Out-Null
  }
  if (!(Test-Path "$dest\Archive")) {
    New-Item "$dest\Archive" -ItemType "directory" -ErrorAction SilentlyContinue | Out-Null
  }
  if (!(Test-Path "$dest\Reject")) {
    New-Item "$dest\Reject" -ItemType "directory" -ErrorAction SilentlyContinue | Out-Null
  } 

  # Copy the folder to the destination
  Write-Host "Copying '$($folder.FullName)\bin' -> '$bin'"
  Copy-Item "$($folder.FullName)\bin" -Destination $bin -Recurse 

}