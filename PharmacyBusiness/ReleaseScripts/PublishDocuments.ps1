#############################################################################################
# Converts the XML docs from source into Markdown files, 
# combines them with the docs folder at the root of the repository,
# and publishes them to the wiki
#############################################################################################

### 
# Define Variables
###
$adoOrganizationUri         = $env:System_TeamFoundationCollectionUri       # https://wegmans.VisualStudio.com
$artifactAlias              = $env:RELEASE_PRIMARYARTIFACTSOURCEALIAS       # The alias of the primary artifact source
$artifactsDirectory         = $env:System_ArtifactsDirectory                # The directory to which artifacts are downloaded during deployment of a release.
$nugetToolPath              = $env:NugetExeToolPath                         # Path to nuget.exe as installed by the Nuget Installer Task
$projectId                  = $env:System_TeamProjectId
$projectName                = $env:System_TeamProject                       # Pharmacy
$releaseRequestedForEmail   = $env:Release_RequestedForEmail                # jane.doe@wegmans.com
$releaseRequestedForName    = $env:Release_RequestedFor                     # Jane Doe
$releaseWebURL              = $env:Release_ReleaseWebURL                    # Url to this release
$repositoryName             = $env:Build_Repository_Name                    # PharmacyBusinesss
$systemAccessToken          = $env:System_AccessToken
$tempDirectory              = $env:Agent_TempDirectory                      # A temporary folder that is cleaned after each pipeline job

$docArtifact                = "$artifactsDirectory/$artifactAlias/Docs/"
$wikiPath                   = Join-Path $tempDirectory $(New-Guid)
$docsFolder                 = "$wikiPath\Repositories\$repositoryName"
$xmlDocMarkdownPath         = "$tempDirectory\XmlDocMarkdown\Tools\XmlDocMarkdown.exe"

# dlls to create documentation for
$dlls = @(
  "$artifactsDirectory/$artifactAlias/ETL/PharmacyBusiness/bin/RX.PharmacyBusiness.ETL.dll"
)

# Define the wiki Url
$wikiRepositoryUri = "$adoOrganizationUri$projectId/_git/$projectName.wiki"
$secureWikiRepositoryUri = New-Object System.UriBuilder ($wikiRepositoryUri)
$secureWikiRepositoryUri.UserName = "OAuth"
$secureWikiRepositoryUri.Password = $systemAccessToken
$secureWikiRepositoryUri = $secureWikiRepositoryUri.ToString()

###
# Validate and Prep
###
if (!(Test-Path $xmlDocMarkdownPath))
{
    # XmlDocMarkdown.exe is needed to create the documents.
    # Since it is missing we need to download it.
    # To download it, we need nuget
    if ("$nugetToolPath" -eq "") 
    { 
        throw "The Nuget Installer Task must be used before running this script so that XmlDocMarkdown can be installed." 
    }
    else
    {
        nuget install XmlDocMarkdown -o $tempDirectory -ExcludeVersion -NonInteractive
        if ($LASTEXITCODE) { throw "Failed to install XmlDocMarkdown." }
    }
}

###
# Publish the docs
###

# Clone the wiki
git clone --depth 1 --branch wikiMaster --single-branch --progress $secureWikiRepositoryUri $wikiPath
if ($LASTEXITCODE) {
    throw "Could not clone git repo: $wikiRepositoryUri."
}

# Remove the docsFolder and everything in it to start fresh
Remove-Item $docsFolder -Recurse -Force -ErrorAction SilentlyContinue

# Populate the Docs folder from the dlls
foreach ($dll in $dlls)
{
  Copy-Item "$dll.config" -Destination "$xmlDocMarkdownPath.config" -ErrorAction SilentlyContinue
  & $xmlDocMarkdownPath $dll $docsFolder  --obsolete
}

# Create the .order files
Get-ChildItem $docsFolder -Directory | Sort-Object BaseName | ForEach-Object {
    $_.Name
} | Out-File -FilePath "$docsFolder\.order"

Get-ChildItem $docsFolder -Directory -Recurse | ForEach-Object {
    Get-ChildItem $_.FullName -Exclude .order | Sort-Object BaseName | ForEach-Object {
        $_.Name
    } | Out-File -FilePath "$($_.FullName)\.order"
}

# Copy all of the files from the docs artifact to the wiki location
Get-ChildItem $docArtifact | ForEach-Object { Copy-Item $_.FullName -Destination $docsFolder -Recurse -ErrorAction Stop }


##############################
function Convert-FolderContentToMarkdownTableOfContents{    
    param (
        [string]$BaseFolder,
        [string]$BaseURL,
        [string]$FiletypeFilter,
        [int]$Indent = 0
    )

    if ($Indent -eq 5) { return ""}
    
    $nl = [System.Environment]::NewLine
    $TOC = ""
    
    $repoFolderStructure = Get-ChildItem -Path $BaseFolder -Directory 
    
    foreach ($dir in ($repoFolderStructure | Sort-Object -Property Name)) {
        $repoStructure = Get-ChildItem -Path $dir.FullName -Filter $FiletypeFilter
    
        $TOC += (" " * $Indent*4) +  "* $($dir.Name) $nl"
    
        foreach ($md in ($repoStructure | Sort-Object -Property Name)) {
            $suffix = "/$($dir.Name)"
            $fileName = $($md.Name.TrimEnd($md.Extension))
            $TOC += (" " * ($Indent+1)*4) + "* [$fileName]($([uri]::EscapeUriString(""$baseURL$suffix/$($md.BaseName)"")))$nl"
        }

        $TOC += Convert-FolderContentToMarkdownTableOfContents -BaseFolder ($BaseFolder + "\$($dir.Name)")  -BaseURL ($BaseURL + "/$($dir.Name)") -FiletypeFilter $FiletypeFilter -Indent ($Indent+1)
    }
    
    return $TOC
}
##############################


# Regenerate the index file
Push-Location $wikiPath -ErrorAction Stop
$index = "#Pharmacy`n---`n"
$index += Convert-FolderContentToMarkdownTableOfContents -BaseFolder ".\Repositories" -BaseURL "/Repositories" -FiletypeFilter "*.md" 
$index | Out-File -FilePath "$($wikiPath)/Repositories/Index.md"

Write-Host 'Update git configuration'
git config user.name $releaseRequestedForName
git config user.email $releaseRequestedForEmail


# Commit and push changes
if (@(git status --porcelain).Count -eq 0) {
    Write-Host 'No wiki doc changes detected'
} else {
    Write-Host 'Publishing wiki doc changes'
    
    git add -A .
    if ($LASTEXITCODE) {
        throw "Could not add changes to git repo: $wikiPath"
    }

    git commit -m "Release:  $releaseWebURL"
    if ($LASTEXITCODE) {
        throw "Could not commit changes to git repo: $wikiPath"
    }

    git push
    if ($LASTEXITCODE) {
        throw "Could not push changes to git repo: $wikiRepositoryUri"
    }
}

Pop-Location