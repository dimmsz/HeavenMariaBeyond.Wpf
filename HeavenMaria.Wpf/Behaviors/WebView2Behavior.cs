using System.Windows;
using Microsoft.Web.WebView2.Wpf;

namespace HeavenMaria.Wpf.Behaviors
{
    public class WebView2Behavior
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html",
            typeof(string),
            typeof(WebView2Behavior),
            new PropertyMetadata(OnHtmlChanged));

        public static string GetHtml(WebView2 webView)
        {
            return (string)webView.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebView2 webView, string value)
        {
            webView.SetValue(HtmlProperty, value);
        }

        private static async void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WebView2 webView && e.NewValue is string html)
            {
                await webView.EnsureCoreWebView2Async();
                webView.CoreWebView2.NavigateToString(html ?? "&nbsp;");
            }
        }
    }
}
