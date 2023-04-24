msbuild -t:Clean,Build -p:Configuration=Release android-agent.csproj
nuget pack newrelic-xamarin-android.nuspec