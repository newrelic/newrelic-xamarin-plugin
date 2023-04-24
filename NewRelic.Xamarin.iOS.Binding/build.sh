#!/usr/bin/env bash

msbuild -t:Clean,Build -p:Configuration=Release NewReic.iOS.csproj
nuget pack newrelic-xamarin-ios.nuspec