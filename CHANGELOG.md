# Changelog

# 0.0.3
New in this release
- Added agent configuration for events, tracing, and FedRAMP compliance.
- Disabled default interaction tracing due to a crash.
- Resolved an issue causing app crashes when users utilized our plugin with a collection view.

# 0.0.2
New in this release
- Adds configurable request header instrumentation to network events The agent will now produce network event attributes for select header values if - the headers are detected on the request. The header names to instrument are passed into the agent when started.
- Updated the native Android agent to version 7.2.0.
- Updated the native iOS agent to version 7.4.8.

## 0.0.1
New Relic is proud to announce support for Xamarin!
This agent allows you to instrument Xamarin apps with help of native New Relic Android and iOS agents. The New Relic SDKs collect crashes, network traffic, and other information for hybrid apps using native components.

### New in this Release
* Capture C# exceptions
* Network Request tracking
* Distributed Tracing
* Capture interactions and the sequence in which they were created
* Pass user information to New Relic to track user sessions