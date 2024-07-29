/*
 * Copyright (c) 2023-present New Relic Corporation. All rights reserved.
 * SPDX-License-Identifier: Apache-2.0 
 */

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using Foundation;
using UIKit;
using NRIosAgent = NewRelicXamarinIOS.NewRelic;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using static CoreFoundation.DispatchQueue;

namespace NewRelic.Xamarin.Plugin
{
    /// <summary>
    /// Implementation for NewRelicClient
    /// </summary>
	/// 
	public class NewRelicClientManager : NSObject, INewRelicClientManager
    {


        private bool isStarted;
        private bool _isUncaughtExceptionHandled;
        private Dictionary<LogLevel, NewRelicXamarinIOS.NRLogLevels> logLevelDict = new Dictionary<LogLevel, NewRelicXamarinIOS.NRLogLevels>()
        {
            { LogLevel.ERROR, NewRelicXamarinIOS.NRLogLevels.Error },
            { LogLevel.WARNING, NewRelicXamarinIOS.NRLogLevels.Warning },
            { LogLevel.INFO, NewRelicXamarinIOS.NRLogLevels.Info },
            { LogLevel.VERBOSE, NewRelicXamarinIOS.NRLogLevels.Verbose },
            { LogLevel.AUDIT, NewRelicXamarinIOS.NRLogLevels.Audit }
        };

        private Dictionary<NetworkFailure, NewRelicXamarinIOS.NRNetworkFailureCode> networkFailureDict = new Dictionary<NetworkFailure, NewRelicXamarinIOS.NRNetworkFailureCode>()
        {
            { NetworkFailure.Unknown, NewRelicXamarinIOS.NRNetworkFailureCode.Unknown },
            { NetworkFailure.BadURL, NewRelicXamarinIOS.NRNetworkFailureCode.BadURL },
            { NetworkFailure.TimedOut, NewRelicXamarinIOS.NRNetworkFailureCode.TimedOut },
            { NetworkFailure.CannotConnectToHost, NewRelicXamarinIOS.NRNetworkFailureCode.CannotConnectToHost },
            { NetworkFailure.DNSLookupFailed, NewRelicXamarinIOS.NRNetworkFailureCode.DNSLookupFailed },
            { NetworkFailure.BadServerResponse, NewRelicXamarinIOS.NRNetworkFailureCode.BadServerResponse },
            { NetworkFailure.SecureConnectionFailed, NewRelicXamarinIOS.NRNetworkFailureCode.SecureConnectionFailed }
        };

        private Dictionary<MetricUnit, string> metricUnitDict = new Dictionary<MetricUnit, string>()
        {
            { MetricUnit.PERCENT, "%" },
            { MetricUnit.BYTES, "bytes" },
            { MetricUnit.SECONDS, "sec" },
            { MetricUnit.BYTES_PER_SECOND, "bytes/second" },
            { MetricUnit.OPERATIONS, "op" }
        };


        public NewRelicClientManager()
        {
            isStarted = false;

        }

        public void Start(string applicationToken, AgentStartConfiguration agentConfig = null)
        {

            if (agentConfig == null)
            {
                agentConfig = new AgentStartConfiguration();
            }

            NRIosAgent.EnableCrashReporting(agentConfig.crashReportingEnabled);
            NRIosAgent.SetPlatform(NewRelicXamarinIOS.NRMAApplicationPlatform.Xamarin);
            NRIosAgent.SetPlatformVersion("1.0.0");

            if (agentConfig.fedRampEnabled)
            {
                NRIosAgent.EnableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.FedRampEnabled);
            }

            if (agentConfig.offlineStorageEnabled)
            {
                NRIosAgent.EnableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.OfflineStorage);
            } else
            {
                NRIosAgent.DisableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.OfflineStorage);
            }

            if (agentConfig.newEventSystemEnabled)
            {
                NRIosAgent.EnableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.NewEventSystem);
            } else
            {
                NRIosAgent.DisableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.NewEventSystem);
            }

            if (agentConfig.backgroundReportingEnabled)
            {
                NRIosAgent.EnableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.BackgroundReporting);
            } else
            {
                NRIosAgent.DisableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.BackgroundReporting);
            }

            if (!agentConfig.networkErrorRequestEnabled)
            {
                NRIosAgent.DisableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.RequestErrorEvents);
            }

            if (!agentConfig.networkRequestEnabled)
            {
                NRIosAgent.DisableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.NetworkRequestEvents);
            }

            if (!agentConfig.interactionTracingEnabled)
            {
                NRIosAgent.DisableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.InteractionTracing);
                NRIosAgent.DisableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.DefaultInteractions);
            }

            if (!agentConfig.webViewInstrumentation)
            {
                NRIosAgent.DisableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.WebViewInstrumentation);
            }

            if (agentConfig.fedRampEnabled)
            {
                NRIosAgent.EnableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.FedRampEnabled);
            }



            NewRelicXamarinIOS.NRLogger.SetLogLevels((uint)logLevelDict[agentConfig.logLevel]);
            if (!agentConfig.loggingEnabled)
            {
                NewRelicXamarinIOS.NRLogger.SetLogLevels((uint)NewRelicXamarinIOS.NRLogLevels.None);
            }
            Mono.Runtime.RemoveSignalHandlers();

            if (agentConfig.collectorAddress.Equals("DEFAULT") && agentConfig.crashCollectorAddress.Equals("DEFAULT"))
            {
                NRIosAgent.StartWithApplicationToken(applicationToken);
            }
            else
            {
                string collectorAddress = agentConfig.collectorAddress.Equals("DEFAULT") ?
                    "mobile-collector.newrelic.com" : agentConfig.collectorAddress;
                string crashCollectorAddress = agentConfig.crashCollectorAddress.Equals("DEFAULT") ?
                    "mobile-crash.newrelic.com" : agentConfig.crashCollectorAddress;
                NRIosAgent.StartWithApplicationToken(applicationToken, collectorAddress, crashCollectorAddress);
            }
            Mono.Runtime.RemoveSignalHandlers();

        }

        public void CrashNow(string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                NRIosAgent.CrashNow();
            }
            else
            {
                NRIosAgent.CrashNow(message);
            }
        }

        public string CurrentSessionId()
        {
            return NRIosAgent.CurrentSessionId;
        }

        public void EndInteraction(string interactionId)
        {
            NRIosAgent.StopCurrentInteraction(interactionId);
        }

        public bool IncrementAttribute(string name, float value = 1)
        {
            return NRIosAgent.IncrementAttribute(name, value);
        }

        public void NoticeHttpTransaction(string url, string httpMethod, int statusCode, long startTime, long endTime, long bytesSent, long bytesReceived, string responseBody)
        {
            NRIosAgent.NoticeNetworkRequestForURL(Foundation.NSUrl.FromString(url),
                httpMethod,
                startTime,
                endTime,
                new NSDictionary(),
                statusCode,
                (nuint)bytesSent,
                (nuint)bytesReceived,
                new NSData(),
                null,
                null);
            return;
        }

        public void NoticeNetworkFailure(string url, string httpMethod, long startTime, long endTime, NetworkFailure failure)
        {
            NRIosAgent.NoticeNetworkFailureForURL(NSUrl.FromString(url), httpMethod, (double)startTime, (double)endTime, (int)networkFailureDict[failure]);
            return;
        }

        public bool RecordBreadcrumb(string name, Dictionary<string, object> attributes)
        {
            return NRIosAgent.RecordBreadcrumb(name, ConvertAttributesToNSDictionary(attributes));

        }

        public bool RecordCustomEvent(string eventType, string eventName, Dictionary<string, object> attributes)
        {
            return NRIosAgent.RecordCustomEvent(eventType, eventName, ConvertAttributesToNSDictionary(attributes));
        }

        public void RecordMetric(string name, string category)
        {
            NRIosAgent.RecordMetricWithName(name, category);
            return;
        }

        public void RecordMetric(string name, string category, double value)
        {
            NRIosAgent.RecordMetricWithName(name, category, Foundation.NSNumber.FromDouble(value));
            return;
        }

        public void RecordMetric(string name, string category, double value, MetricUnit countUnit, MetricUnit valueUnit)
        {
            NRIosAgent.RecordMetricWithName(name, category, Foundation.NSNumber.FromDouble(value), metricUnitDict[valueUnit], metricUnitDict[countUnit]);
            return;
        }

        public bool RemoveAllAttributes()
        {
            return NRIosAgent.RemoveAllAttributes;
        }

        public bool RemoveAttribute(string name)
        {
            return NRIosAgent.RemoveAttribute(name);
        }

        public bool SetAttribute(string name, string value)
        {
            return NRIosAgent.SetAttribute(name, Foundation.NSObject.FromObject(value));
        }

        public bool SetAttribute(string name, double value)
        {
            return NRIosAgent.SetAttribute(name, Foundation.NSObject.FromObject(value));
        }

        public bool SetAttribute(string name, bool value)
        {
            return NRIosAgent.SetAttribute(name, Foundation.NSObject.FromObject(value));
        }

        public void SetMaxEventBufferTime(int maxBufferTimeInSec)
        {
            NRIosAgent.SetMaxEventBufferTime((uint)maxBufferTimeInSec);
            return;
        }

        public void SetMaxEventPoolSize(int maxPoolSize)
        {
            NRIosAgent.SetMaxEventPoolSize((uint)maxPoolSize);
            return;
        }

        public bool SetUserId(string userId)
        {
            return NRIosAgent.SetUserId(userId);
        }

        public string StartInteraction(string interactionName)
        {
            return NRIosAgent.StartInteractionWithName(interactionName);
        }

        public void AnalyticsEventEnabled(bool enabled)
        {
            // This is an Android-only function
            return;
        }

        public void NetworkRequestEnabled(bool enabled)
        {
            if (enabled)
            {
                NRIosAgent.EnableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.NetworkRequestEvents);
            }
            else
            {
                NRIosAgent.DisableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.NetworkRequestEvents);
            }
            return;
        }

        public void NetworkErrorRequestEnabled(bool enabled)
        {
            if (enabled)
            {
                NRIosAgent.EnableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.RequestErrorEvents);
            }
            else
            {
                NRIosAgent.DisableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.RequestErrorEvents);
            }
            return;
        }

        public void HttpResponseBodyCaptureEnabled(bool enabled)
        {
            if (enabled)
            {
                NRIosAgent.EnableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.HttpResponseBodyCapture);
            }
            else
            {
                NRIosAgent.DisableFeatures(NewRelicXamarinIOS.NRMAFeatureFlags.HttpResponseBodyCapture);
            }
            return;
        }


        public HttpMessageHandler GetHttpMessageHandler()
        {
            return new NSUrlSessionHandler();
        }

        public void RecordException(Exception exception)

        {
            RecordException(exception, new Dictionary<string, object>());
        }

        public void HandleUncaughtException(bool shouldThrowFormattedException = true)
        {

            if (!_isUncaughtExceptionHandled)
            {
                _isUncaughtExceptionHandled = true;

                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    if (e.ExceptionObject is Exception exception)
                    {
                        RecordException(exception);
                    }
                };
            }
        }

        public void TrackShellNavigatedEvents()
        {
            Shell.Current.Navigated += (sender, e) =>
            {
                Dictionary<string, object> attr = new Dictionary<string, object>();
                if (e.Previous != null)
                {
                    attr.Add("Previous", e.Previous.Location.ToString());
                }
                attr.Add("Current", e.Current.Location.ToString());
                attr.Add("Source", e.Source.ToString());
                this.RecordBreadcrumb("ShellNavigated", attr);
            };
        }

        public void Shutdown()
        {
            NRIosAgent.Shutdown();
            return;
        }

        public void addHTTPHeadersTrackingFor(List<string> headers)
        {
            NRIosAgent.AddHTTPHeaderTrackingFor(headers.ToArray());
        }

        public List<string> getHTTPHeadersTrackingFor()
        {
            throw new NotImplementedException();
        }

        public void SetMaxOfflineStorageSize(int megabytes)
        {
            NRIosAgent.SetMaxOfflineStorageSize((uint)megabytes);
            return;
        }

        public void LogInfo(String message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                NRIosAgent.LogInfo(message);
            }
            else
            {
                Console.WriteLine("Info: Message is empty or null.");
            }
        }

        public void LogWarning(String message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                NRIosAgent.LogWarning(message);
            }
            else
            {
                Console.WriteLine("Warning: Message is empty or null.");
            }
        }

        public void LogDebug(String message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                NRIosAgent.LogDebug(message);
            }
            else
            {
                Console.WriteLine("Debug: Message is empty or null.");
            }
        }

        public void LogVerbose(String message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                NRIosAgent.LogVerbose(message);
            }
            else
            {
                Console.WriteLine("Verbose: Message is empty or null.");
            }
        }

        public void LogError(String message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                NRIosAgent.LogError(message);
            }
            else
            {
                Console.WriteLine("Error: Message is empty or null.");
            }
        }

        public void Log(LogLevel level, String message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Dictionary<string, object> attributes = new Dictionary<string, object>();
                attributes.Add("Message", message);
                attributes.Add("logLevel", level.ToString());

                NRIosAgent.LogAll(ConvertAttributesToNSDictionary(attributes));
            }
            else
            {
                Console.WriteLine($"Log Level {level}: Message is empty or null.");
            }
        }



        public void LogAttributes(Dictionary<string, object> attributes)
        {
            if (attributes != null && attributes.Count > 0)
            {
                NRIosAgent.LogAttributes(ConvertAttributesToNSDictionary(attributes));
            }
            else
            {
                Console.WriteLine("Attributes are empty or null.");
            }
        }

        public Foundation.NSMutableDictionary ConvertAttributesToNSDictionary(Dictionary<string, object> attributes)
        {
            Foundation.NSMutableDictionary NSDict = new Foundation.NSMutableDictionary();
            foreach (KeyValuePair<string, object> entry in attributes)
            {
                NSDict.Add(Foundation.NSObject.FromObject(entry.Key), Foundation.NSObject.FromObject(entry.Value));
            }
            return NSDict;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void RecordException(Exception exception, Dictionary<string, object> attributes)
        {
            List<StackFrame> stackFrames = StackTraceParser.Parse(exception.StackTrace).ToList();
            var _stackFramesArray = new NSMutableArray();
            foreach (StackFrame length in stackFrames)
            {
                var stackFrameKeys = new object[] { "file", "line", "method", "class" };
                var stackFrameObjects = new object[] { length.FileName, length.LineNumber, length.MethodName, length.ClassName };
                NSDictionary dictionary = NSDictionary.FromObjectsAndKeys(stackFrameObjects, stackFrameKeys);
                _stackFramesArray.Add(dictionary);
            }

            var errorKeys = new object[] { "name", "reason", "cause", "fatal", "stackTraceElements","attributes"};

            var errorObjects = new object[] { exception.Message, exception.Message, exception.Message, false, _stackFramesArray, ConvertAttributesToNSDictionary(attributes)};
            NSDictionary NSDict = NSDictionary.FromObjectsAndKeys(errorObjects, errorKeys);


            NRIosAgent.RecordHandledExceptionWithStackTrace(NSDict);
        }
    }
}

