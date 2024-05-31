# Changelog

# 0.0.6
New in this release

* Improvements
The native iOS Agent has been updated to version 7.4.12, bringing performance enhancements and bug fixes.

* New Features
A new backgroundReportingEnabled feature flag has been introduced to enable background reporting functionality. 
A new newEventSystemEnabled feature flag has been added to enable the new event system.

# 0.0.5
New in this release
- 
- Updated native iOS Agent: We've upgraded the native iOS agent to version 7.4.10, which includes performance improvements and bug fixes.


# 0.0.4
New in this release

- Added Offline Monitoring Feature: This new feature enables the preservation of harvest data that would otherwise be lost when the application lacks an internet connection. The stored harvests will be sent once the internet connection is re-established and the next harvest upload is successful.
- Introduced setMaxOfflineStorageSize API: This new API allows the user to determine the maximum volume of data that can be stored locally. This aids in better management and control of local data storage.
- Updated native iOS Agent: We've upgraded the native iOS agent to version 7.4.9, which includes performance improvements and bug fixes.
- Updated native Android Agent: We've also upgraded the native Android agent to version 7.3.0 bringing benefits like improved stability and enhanced features.


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