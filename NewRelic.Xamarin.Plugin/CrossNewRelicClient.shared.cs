/*
 * Copyright (c) 2023-present New Relic Corporation. All rights reserved.
 * SPDX-License-Identifier: Apache-2.0 
 */

using System;
namespace Plugin.NewRelicClient
{
    /// <summary>
    /// Cross NewRelicClient
    /// </summary>
    public static class CrossNewRelicClient
    {
        static Lazy<INewRelicClientManager> implementation = new Lazy<INewRelicClientManager>(() => CreateNewRelicClient(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets if the plugin is supported on the current platform.
        /// </summary>
        public static bool IsSupported => implementation.Value == null ? false : true;
        


        /// <summary>
        /// Current plugin implementation to use
        /// </summary>
        public static INewRelicClientManager Current
        {
            get
            {
                INewRelicClientManager ret = implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }

                
                return ret;
            }
        }

        static INewRelicClientManager CreateNewRelicClient()
        {
#if  NETSTANDARD2_0
            return null;
#else
            return new NewRelicClientManager();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly() =>
            new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");

    }
}
