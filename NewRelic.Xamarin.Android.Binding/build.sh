msbuild -t:Clean,Build -p:Configuration=Release NewRelic.Xamarin.Android.Binding.csproj
nuget pack newrelic-xamarin-android.nuspec