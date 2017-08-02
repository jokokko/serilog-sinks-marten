@echo off
cls
If Not Exist tools\gitlink\lib\net45\GitLink.exe nuget Install gitlink -Source "https://www.nuget.org/api/v2/" -OutputDirectory "tools" -ExcludeVersion
If Not Exist tools\FAKE\tools\fake.exe nuget.exe Install FAKE -Source "https://www.nuget.org/api/v2/" -OutputDirectory "tools" -ExcludeVersion
tools\FAKE\tools\fake.exe build.fsx %*
