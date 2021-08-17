/KServer/Scripts/Common/kill_webserver.sh

cd /KServer/WebServer 
dotnet build ./WebServer.csproj -c Debug
dotnet run --launch-profile Business -p ./WebServer.csproj -c Debug >log.out 2>log.err &
