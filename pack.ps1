$root = (split-path -parent $MyInvocation.MyCommand.Definition)
$version = [System.Reflection.Assembly]::LoadFile("$($root)\src\Sitecore.React\bin\Sitecore.React.dll").GetName().Version
$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build)

Write-Host "Setting .nuspec version tag to $versionStr"

& $root\NuGet\NuGet.exe pack $root\Build\Sitecore.React.Nuget\Sitecore.React.nuspec -Version $versionStr
& $root\NuGet\NuGet.exe pack $root\Build\Sitecore.React.Nuget\Sitecore.React.Web.nuspec -Version $versionStr
