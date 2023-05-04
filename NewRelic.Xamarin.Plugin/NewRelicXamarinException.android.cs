/*
 * Copyright (c) 2023-present New Relic Corporation. All rights reserved.
 * SPDX-License-Identifier: Apache-2.0 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Java.Lang;
namespace Plugin.NewRelicClient
{

    internal class NewRelicXamarinException : Java.Lang.Exception
    {
        public NewRelicXamarinException(string message, StackTraceElement[] stackTrace) : base(message)
        {
            SetStackTrace(stackTrace);
        }

        public static NewRelicXamarinException Create(System.Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));

            var message = $"{exception.GetType()}: {exception.Message}";

            var stackTrace = StackTraceParser.Parse(exception)
                .Select(frame => new StackTraceElement(frame.ClassName, frame.MethodName, frame.FileName, frame.LineNumber))
                .ToArray();

            return new NewRelicXamarinException(message, stackTrace);
        }
    }
}

