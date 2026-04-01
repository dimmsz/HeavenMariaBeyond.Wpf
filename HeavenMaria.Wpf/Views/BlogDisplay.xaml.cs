using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace HeavenMaria.Wpf.Views
{
    /// <summary>
    /// Interaction logic for BlogDisplay
    /// </summary>
    public partial class BlogDisplay : UserControl
    {
        public BlogDisplay()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await webView.EnsureCoreWebView2Async();

            webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "local.test",
                @"Q:\HeavenNet\Diaries\",
                CoreWebView2HostResourceAccessKind.Allow
            );

            webView.CoreWebView2.WebResourceResponseReceived += (s, ev) =>
            {
                Debug.WriteLine($"WebResourceResponseReceived: {ev.Response.StatusCode} - {ev.Response.StatusCode}");
            };
        }
    }
}
