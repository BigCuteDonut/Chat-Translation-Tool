/*
    Chat Translation Tool
    Copyright (C) 2020 Johanna Sierak

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using CefSharp;
using CefSharp.OffScreen;
using System;
using System.IO;
using System.Threading;

namespace TranslateTool
{

    public class Browser
    {
        private static CefSettings settings;
        public ChromiumWebBrowser Page { get; private set; }
        public RequestContext RequestContext { get; private set; }
        private ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        static Browser()
        {
            settings = new CefSettings()
            {
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache"),
            };
            settings.CefCommandLineArgs.Add("process-per-tab", "1");
            settings.DisableGpuAcceleration();
            CefSharpSettings.ShutdownOnExit = true;
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
        }

        public Browser()
        {
            RequestContext = new RequestContext();
            Page = new ChromiumWebBrowser("", null, RequestContext);
            PageInitialize();
        }

        public void OpenUrl(string url)
        {
            try
            {
                Page.LoadingStateChanged += PageLoadingStateChanged;
                if (Page.IsBrowserInitialized)
                {
                    Page.Load(url);

                    bool isSignalled = manualResetEvent.WaitOne(TimeSpan.FromSeconds(60));
                    manualResetEvent.Reset();

                    if (!isSignalled)
                    {
                        Page.Stop();
                    }
                }
            }
            catch (ObjectDisposedException)
            {
            }
            Page.LoadingStateChanged -= PageLoadingStateChanged;
        }

        private void PageLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                manualResetEvent.Set();
            }
        }

        public void PageInitialize()
        {
            SpinWait.SpinUntil(() => Page.IsBrowserInitialized);
        }
    }
}
