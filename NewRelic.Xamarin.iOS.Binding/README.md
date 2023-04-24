# NewRelic Xamarin IOS

NewRelic iOS SDK binding for Xamarin

[![NuGet][nuget-img]][nuget-link]

[nuget-img]: https://img.shields.io/badge/nuget-1.0.0-blue.svg
[nuget-link]: https://www.nuget.org/packages/NewRelic.iOS.XamarinBinding

## How to Build

### NewRelic iOS 7.4.1 (Feb 22, 2023)
```
sh bootstrapper.sh
```

Add --registrar:static as additional mtouch arguments on iOS Build dialog for your iOS application
Sometimes adding --registrar:static -cxx -gcc_flags -dead_strip flags is required.

