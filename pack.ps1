$version = [System.Reflection.Assembly]::LoadFile(".\src\Sitecore.React\bin\Release\Sitecore.React.dll").GetName().Version
$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content .\build\Sitecore.React.Nuget\Sitecore.React.Web.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File .\Build\Sitecore.React.Web.nuspec

& $root\NuGet\NuGet.exe pack .\Build\Sitecore.React.Nuget\Sitecore.React.nuspec
& $root\NuGet\NuGet.exe pack .\Build\Sitecore.React.Nuget\Sitecore.React.Web.nuspec
