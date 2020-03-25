// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Components;
using Interop = Microsoft.AspNetCore.Components.Web.BrowserNavigationManagerInterop;

namespace Microsoft.AspNetCore.Components.WebAssembly.Services
{
    /// <summary>
    /// Default client-side implementation of <see cref="NavigationManager"/>.
    /// </summary>
    internal class WebAssemblyNavigationManager : NavigationManager
    {
        /// <summary>
        /// Gets the instance of <see cref="WebAssemblyNavigationManager"/>.
        /// </summary>
        public static readonly WebAssemblyNavigationManager Instance = new WebAssemblyNavigationManager();

        // For simplicity we force public consumption of the BrowserNavigationManager through
        // a singleton. Only a single instance can be updated by the browser through
        // interop. We can construct instances for testing.
        internal WebAssemblyNavigationManager()
        {
        }

        protected override void EnsureInitialized()
        {
            // As described in the comment block above, BrowserNavigationManager is only for
            // client-side (Mono) use, so it's OK to rely on synchronicity here.
            var baseUri = DefaultWebAssemblyJSRuntime.Instance.Invoke<string>(Interop.GetBaseUri);
            var uri = DefaultWebAssemblyJSRuntime.Instance.Invoke<string>(Interop.GetLocationHref);
            Initialize(baseUri, uri);
        }

        public void SetLocation(string uri, bool isInterceptedLink)
        {
            Uri = uri;
            NotifyLocationChanged(isInterceptedLink);
        }

        /// <inheritdoc />
        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            DefaultWebAssemblyJSRuntime.Instance.Invoke<object>(Interop.NavigateTo, uri, forceLoad);
        }
    }
}
