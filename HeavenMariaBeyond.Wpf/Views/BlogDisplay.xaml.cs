using HeavenMariaBeyond.Domain;
using HeavenMariaBeyond.Domain.Interfaces;
using HeavenMariaBeyond.Wpf.ViewModels;
using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HeavenMariaBeyond.Wpf.Views
{
    /// <summary>
    /// Interaction logic for BlogDisplay
    /// </summary>
    public partial class BlogDisplay : UserControl
    {
        private static readonly DependencyProperty HtmlProperty = DependencyProperty.Register(
            "Html", typeof(string), typeof(BlogDisplay),
            new PropertyMetadata(string.Empty, (view, _) => ((BlogDisplay)view).RenderHtml()));

        private readonly IPathProvider _pathProvider;
        private bool _rendering;
        private bool _mediaHostMapped;

        public BlogDisplay(IPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
            InitializeComponent();
            SetBinding(HtmlProperty, new Binding(nameof(BlogDisplayViewModel.Html)));
        }

        private void OnLoaded(object sender, RoutedEventArgs e) => RenderHtml();

        private async void RenderHtml()
        {
            if (!IsLoaded || _rendering)
                return;

            _rendering = true;
            try
            {
                await webView.EnsureCoreWebView2Async();
                if (!IsLoaded)
                    return;

                if (!_mediaHostMapped)
                {
                    webView.CoreWebView2.SetVirtualHostNameToFolderMapping(ApplicationConstData.DiaryMediaHost,
                        _pathProvider.DiaryRootDirectory, CoreWebView2HostResourceAccessKind.Allow);
                    _mediaHostMapped = true;
                }

                var html = (string)GetValue(HtmlProperty);
                webView.CoreWebView2.NavigateToString(string.IsNullOrEmpty(html) ? "<html></html>" : html);
                displayError.Visibility = Visibility.Collapsed;
                webView.Visibility = Visibility.Visible;
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
                if (IsLoaded)
                {
                    webView.Visibility = Visibility.Hidden;
                    displayError.Visibility = Visibility.Visible;
                }
            }
            finally
            {
                _rendering = false;
            }
        }
    }
}
