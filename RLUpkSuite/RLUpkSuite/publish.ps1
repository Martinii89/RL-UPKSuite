
$version = $args[0]

# build / publish your app
dotnet publish -c Release -o ".\publish" 

# find Squirrel.exe path and add an alias
Set-Alias Squirrel ($env:USERPROFILE + "\.nuget\packages\clowd.squirrel\2.11.1\tools\Squirrel.exe");

# download currently live version
#Squirrel http-down --url "https://the.place/you-host/updates"

# build new version and delta updates.
Squirrel pack --framework net8,vcredist143-x86 --packId "RlUpkSuite" --packVersion $version --packAuthors "Martinn" --packDir ".\publish" --icon ".\Resources\AppIcon.ico"