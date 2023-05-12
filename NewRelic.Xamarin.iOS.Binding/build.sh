#!/usr/bin/env bash

msbuild -t:Clean,Build -p:Configuration=Release NewRelic.Xamarin.iOS.Binding.csproj
nuget pack newrelic-xamarin-ios.nuspec