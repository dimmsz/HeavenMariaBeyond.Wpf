using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using HeavenMariaBeyond.Domain;
using HeavenMariaBeyond.Domain.Entities.Main;
using HeavenMariaBeyond.Domain.Interfaces;
using System.Net;
using System.Text.RegularExpressions;
using static HeavenMariaBeyond.Domain.DisplayHtml.DiaryEnums;

namespace HeavenMariaBeyond.Infrastructure.Generators
{
    public class DiaryHtmlGenerator : IDiaryHtmlGenerator
    {
        private readonly ICompressionService _compressionService;
        private readonly IPathProvider _pathProvider;

        public DiaryHtmlGenerator(ICompressionService compressionService,
            IPathProvider pathProvider)
        {
            _compressionService = compressionService;
            _pathProvider = pathProvider;
        }

        /// <summary>
        /// 表示用のhtmlを作る
        /// </summary>
        /// <param name="diary"></param>
        /// <returns></returns>
        public async Task<string> MakeDiaryHtml(Diary diary, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (diary.DiaryBody is not { Length: > 0 } body)
            {
                throw new InvalidDataException($"Diary body is missing. DiaryCode={diary.DiaryCode}");
            }

            var parser = new HtmlParser();
            var html = _compressionService.DecompressToString(body);
            var doc = await parser.ParseDocumentAsync(html, cancellationToken).ConfigureAwait(false);

            // ① <article> タグを取得
            var articleTag = doc.QuerySelector("article");
            if (articleTag is null)
            {
                throw new InvalidDataException($"Diary body has no article. DiaryCode={diary.DiaryCode}");
            }

            // ② 不要なタグを削除
            // コメントノード削除
            foreach (var comment in articleTag.Descendants().OfType<IComment>())
            {
                comment.Remove();
            }

            // 空白と改行だけの段落を削除
            foreach (var p in articleTag.QuerySelectorAll("p").Where(p =>
                string.IsNullOrWhiteSpace(p.TextContent) && p.Children.All(child => child.LocalName == "br")))
            {
                p.Remove();
            }

            // <script> タグ削除
            foreach (var script in articleTag.QuerySelectorAll("script"))
            {
                script.Remove();
            }

            // diary_informationタグ削除
            foreach (var script in articleTag.QuerySelectorAll(".diary_information"))
            {
                script.Remove();
            }

            // diary_titleタグ削除
            foreach (var script in articleTag.QuerySelectorAll(".diary_title"))
            {
                script.Remove();
            }

            // すべての <img> タグを対象にする
            foreach (var img in articleTag.QuerySelectorAll("img"))
            {
                img.RemoveAttribute("oncontextmenu");
                img.RemoveAttribute("draggable");
            }

            // 条件に合う <span> を探す
            var targetSpan = articleTag.QuerySelectorAll("span")
                .FirstOrDefault(span => span.TextContent.Contains("コンパニオン募集中"));
            targetSpan?.Remove();

            foreach (var contents in articleTag.QuerySelectorAll(".diary_info_contents, .diary_detail"))
            {
                RemoveLeadingLineBreaks(contents);
            }

            foreach (var img in articleTag.QuerySelectorAll("img"))
            {
                var src = img.GetAttribute("src");
                if (string.IsNullOrWhiteSpace(src)) continue;

                // Windowsパス形式かどうかを判定
                if (src.StartsWith(@"V:\") || src.StartsWith(@"C:\") || src.Contains(@":\"))
                {
                    // パスを file:/// 形式に変換
                    var normalized = "file:///" + src.Replace("\\", "/").Trim();
                    img.SetAttribute("src", normalized);
                }
            }

            //stringへ変換
            string article = articleTag.OuterHtml;

            article = Regex.Replace(article, @"src\s*=\s*""\s*(.*?)\s*""", match =>
            {
                string path = match.Groups[1].Value.Trim();
                return $"src=\"{path}\"";
            });

            /*
            //文字サイズ
            doc = SetMinFontsize(doc);

            //日記本体
            string selector = "#diary_list > article";
            if (doc.QuerySelector(selector) == null)
            {
                return "<html><head><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'></head>日記の取得に失敗した可能性があります。</html>";
            }
            var article = doc.QuerySelector(selector).OuterHtml;

            //タグ無効化
            //title
            selector = ".diary_title";
            var titleTag = doc.QuerySelector(selector).OuterHtml;
            article = article.Replace(titleTag, "<!--" + titleTag + "-->");
            article = article.Replace(@"oncontextmenu=""return false""", "");
            //date
            selector = ".diary_headding > div > span";
            var dateTag = doc.QuerySelector(selector).OuterHtml;
            article = article.Replace(dateTag, "<!--" + dateTag + "-->");
            //ad
            selector = ".diary_detail > div:nth-child(5)";
            var adTag = doc.QuerySelector(selector)?.OuterHtml;
            //article = article.Replace(dateTag, "<!--" + adTag + "-->");

            */

            ///日記フォルダの取得
            string dir = _pathProvider.GetDiaryDirectoryPath(diary);

            //フレーム
            if (GetFrameType(diary) != FrameType.none && !string.IsNullOrWhiteSpace(diary.FrameUrl))
            {
                var frame = GetFrameUrl(diary.FrameUrl);
                string localImagePath = Path.Combine(dir, frame.LocalFileName.Split('?')[0]);
                article = article.Replace("amp;", "").Replace(frame.RawURL, localImagePath);
            }

            //一般画像
            foreach (var s in GetImageUrlList(diary))
            {
                string localImagePath = Path.Combine(dir, s.LocalFileName.Split('?')[0]);
                article = article.Replace("amp;", "").Replace(s.RawURL, localImagePath);
            }

            article = Regex.Replace(article, @"src\s*=\s*""\s*(.*?)\s*""", match =>
            {
                string localPath = match.Groups[1].Value.Trim();

                if (Uri.TryCreate(localPath, UriKind.Absolute, out var uri) && uri.IsFile)
                    localPath = uri.LocalPath;
                if (!Path.IsPathFullyQualified(localPath))
                    return match.Value;

                string relativePath = Path.GetRelativePath(_pathProvider.DiaryRootDirectory, localPath);
                if (Path.IsPathRooted(relativePath) || relativePath == ".." ||
                    relativePath.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                    return match.Value;

                // 仮想ホスト形式に変換
                string virtualPath = $"https://{ApplicationConstData.DiaryMediaHost}/{relativePath.Replace("\\", "/")}";
                return $"src=\"{virtualPath}\"";
            });

            // 日記表示用の HTML とスタイル
            string top = """
                <!doctype html>
                <html lang="ja"><head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1">
                <style>
                body { margin: 12px; }
                .diary-header { margin-bottom: 20px; overflow-wrap: anywhere; }
                .diary-header h1 { margin: 8px 0 0; font-size: 1.5rem; }
                .diary_item:has(> .diary_img_contents):has(> .diary_info_contents) {
                    display: grid;
                    grid-template: "photo text" / minmax(0, 2fr) minmax(0, 3fr);
                    align-items: start;
                    gap: 20px;
                }
                .diary_img_contents { grid-area: photo; min-width: 0; overflow: hidden; }
                .diary_img_contents p { margin: 0; }
                .diary_img_contents img { display: block; width: 100%; }
                .diary_info_contents { grid-area: text; min-width: 0; overflow-wrap: anywhere; line-height: 1.5; }
                .diary_info_contents p { margin: 0 0 0.75em; }
                article img { max-width: 100%; height: auto; }
                .diary-zoomable-photo { cursor: zoom-in; }
                .diary-zoomable-photo:focus-visible { outline-offset: -2px; }
                #diary-photo-dialog {
                    box-sizing: border-box;
                    width: calc(100% - 32px);
                    height: calc(100% - 32px);
                    max-width: none;
                    max-height: none;
                    padding: 12px;
                    border: 0;
                    border-radius: 8px;
                    background: #181818;
                }
                #diary-photo-dialog[open] { display: grid; grid-template-rows: auto minmax(0, 1fr); gap: 8px; }
                #diary-photo-dialog::backdrop { background: rgb(0 0 0 / 65%); }
                #diary-photo-dialog form { justify-self: end; }
                #diary-photo-dialog button { padding: 4px 12px; cursor: pointer; }
                #diary-photo-dialog img { display: block; width: 100%; height: 100%; object-fit: contain; cursor: zoom-out; }
                @media (max-width: 600px) {
                    .diary_item:has(> .diary_img_contents):has(> .diary_info_contents) {
                        grid-template: "photo" "text" / minmax(0, 1fr);
                    }
                    .diary_img_contents { max-width: 360px; }
                }
                </style>
                </head><body>
                """;
            string diaryImageList = WebUtility.HtmlEncode(diary.DiaryImageListString ?? string.Empty)
                .Replace("--", "-&#45;");
            string diaryImageListSource = $"<!-- diary_image_list: {diaryImageList} -->";
            string header = $"""
                <header class="diary-header">
                    <time>{diary.DiaryDate:yyyy年 MM月dd日  HH時mm分ss秒}</time>
                    <h1>{WebUtility.HtmlEncode(diary.DiaryTitle)}</h1>
                </header>
                """;
            string photoViewer = """
                <dialog id="diary-photo-dialog" aria-label="写真の拡大表示">
                    <form method="dialog"><button type="submit" autofocus>閉じる</button></form>
                    <img alt="">
                </dialog>
                <script>
                (() => {
                    const dialog = document.querySelector('#diary-photo-dialog');
                    const enlargedPhoto = dialog.querySelector('img');

                    for (const photo of document.querySelectorAll('article img:not([usemap])')) {
                        if (photo.closest('a[href], button, .diary_movieframe')) continue;

                        photo.classList.add('diary-zoomable-photo');
                        photo.tabIndex = 0;
                        photo.setAttribute('role', 'button');
                        photo.setAttribute('aria-haspopup', 'dialog');
                        photo.setAttribute('aria-label', photo.alt ? photo.alt + 'を拡大表示' : '写真を拡大表示');

                        photo.addEventListener('click', () => {
                            if (!photo.naturalWidth || dialog.open) return;

                            enlargedPhoto.alt = photo.alt;
                            enlargedPhoto.src = photo.currentSrc;
                            dialog.showModal();
                        });
                        photo.addEventListener('keydown', event => {
                            if (event.key === 'Enter' || event.key === ' ') {
                                event.preventDefault();
                                photo.click();
                            }
                        });
                    }

                    dialog.addEventListener('click', event => {
                        if (event.target === dialog || event.target === enlargedPhoto) dialog.close();
                    });
                    dialog.addEventListener('close', () => {
                        if (!dialog.open) enlargedPhoto.removeAttribute('src');
                    });
                })();
                </script>
                """;
            return top + diaryImageListSource + header + article + photoViewer + "</body></html>";
        }

        private static void RemoveLeadingLineBreaks(IElement contents)
        {
            var leadingBreaks = new List<IElement>();
            foreach (var node in contents.Descendants())
            {
                if ((node is IText text && !string.IsNullOrWhiteSpace(text.Data)) ||
                    node is IElement { LocalName: "img" or "video" or "audio" or "iframe" or "svg" or "hr" })
                {
                    break;
                }

                if (node is IElement { LocalName: "br" } lineBreak)
                {
                    leadingBreaks.Add(lineBreak);
                }
            }

            foreach (var lineBreak in leadingBreaks)
            {
                lineBreak.Remove();
            }
        }

        /// <summary>
        /// フレームタイプを得る
        /// </summary>
        /// <param name="diary"></param>
        /// <returns></returns>
        private FrameType GetFrameType(Diary diary)
        {
            switch (diary.FrameTypeString)
            {
                case "none":
                    return FrameType.none;
                case "photoframe":
                    return FrameType.photoframe;
                case "movieframe":
                    return FrameType.movieframe;
                default:
                    return FrameType.none;
            }
        }

        /// <summary>
        /// フレームURL(class)を得る
        /// </summary>
        /// <param name="diary"></param>
        /// <returns></returns>
        private FrameClass GetFrameUrl(string frameUrl)
        {
            var result = new FrameClass
            {
                RawURL = frameUrl,
                DirectURL = "https:" + frameUrl.Split('?')[0],
                LocalFileName = Path.GetFileName(frameUrl)
            };
            return result;
        }

        /// <summary>
        /// 画像アドレスを三つに分類します。
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="linkType"></param>
        /// <returns></returns>
        private List<DiaryImageClass> GetImageUrlList(Diary diary)
        {
            var diaryImageListString = diary.DiaryImageListString;
            if (string.IsNullOrWhiteSpace(diaryImageListString))
            {
                return new List<DiaryImageClass>();
            }

            var result = new List<DiaryImageClass>();
            foreach (var s in diaryImageListString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var temp = new DiaryImageClass
                {
                    RawURL = s,
                    DirectURL = "https:" + s.Split('?')[0],
                    LocalFileName = Path.GetFileName(s)
                };
                result.Add(temp);
            }
            return result;
        }

    }
}
