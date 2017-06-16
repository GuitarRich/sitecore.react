$root = (split-path -parent $MyInvocation.MyCommand.Definition)
$version = [System.Reflection.Assembly]::LoadFile("$($root)\src\Sitecore.React\bin\Release\Sitecore.React.dll").GetName().Version
$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\build\Sitecore.React.Nuget\Sitecore.React.Web.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\Build\Sitecore.React.Web.nuspec

& $root\NuGet\NuGet.exe pack $root\Build\Sitecore.React.Nuget\Sitecore.React.nuspec
& $root\NuGet\NuGet.exe pack $root\Build\Sitecore.React.Nuget\Sitecore.React.Web.nuspec
