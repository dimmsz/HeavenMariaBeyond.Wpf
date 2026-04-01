using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using HeavenMaria.Domain.Entities.Main;
using HeavenMaria.Domain.Interfaces;
using System.Text.RegularExpressions;
using static HeavenMaria.Domain.DisplayHtml.DiaryEnums;

namespace HeavenMaria.Infrastructure.Generators
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
        public async Task<string> MakeDiaryHtml(DiaryOld diary)
        {
            var doc = default(IHtmlDocument);
            var parser = new HtmlParser();
            var html = _compressionService.DecompressToString(diary.DiaryBody);
            doc = await parser.ParseDocumentAsync(html);

            // ① <article> タグを取得
            var articleTag = doc.QuerySelector("article");
            if (articleTag is null)
            {
                return "<html><head><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'></head>日記の取得に失敗した可能性があります。</html>";
            }

            // ② 不要なタグを削除
            // コメントノード削除
            foreach (var comment in articleTag.Descendants().OfType<IComment>())
            {
                comment.Remove();
            }

            // 空の <p> タグ削除
            foreach (var p in articleTag.QuerySelectorAll("p").Where(p => string.IsNullOrWhiteSpace(p.TextContent)))
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
            if (GetFrameType(diary) != FrameType.none)
            {
                string localImagePath = Path.Combine(dir, GetFrameUrl(diary).LocalFileName.Split('?')[0]);
                article = article.Replace("amp;", "").Replace(GetFrameUrl(diary).RawURL, localImagePath);
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

                // 不要な "file:" を除去
                if (localPath.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
                    localPath = localPath.Substring(5);

                // 仮想ホストのルートフォルダ
                string baseFolder = @"Q:\HeavenNet\Diaries\";

                // 相対パスを取得
                string relativePath = GetRelativePath(baseFolder, localPath);

                // 仮想ホスト形式に変換
                string virtualPath = $"https://local.test/{relativePath.Replace("\\", "/")}";
                return $"src=\"{virtualPath}\"";
            });

            //文字化け対策
            string top = @"<html><head><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'></head>";
            string bottom = @"</html>";
            article = top + diary.DiaryDate.ToString("yyyy年 MM月dd日  HH時mm分ss秒") + "<br><br><font size=\"5\">" + _compressionService.DecompressToString(diary.DiaryTitle) + "</font><br><br>" + article + bottom;

            //
            return article;
        }

        /// <summary>
        /// フレームタイプを得る
        /// </summary>
        /// <param name="diary"></param>
        /// <returns></returns>
        private FrameType GetFrameType(DiaryOld diary)
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
        private FrameClass GetFrameUrl(DiaryOld diary)
        {
            var result = new FrameClass
            {
                RawURL = diary.FrameUrl,
                DirectURL = "https:" + diary.FrameUrl.Split('?')[0],
                LocalFileName = Path.GetFileName(diary.FrameUrl)
            };
            return result;
        }

        /// <summary>
        /// 画像アドレスを三つに分類します。
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="linkType"></param>
        /// <returns></returns>
        private List<DiaryImageClass> GetImageUrlList(DiaryOld diary)
        {
            var diaryImageListString = _compressionService.DecompressToString(diary.DiaryImageListString);
            if (diaryImageListString == string.Empty)
            {
                return new List<DiaryImageClass>();
            }

            var result = new List<DiaryImageClass>();
            foreach (var s in diaryImageListString.Split(','))
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

        private string GetRelativePath(string basePath, string fullPath)
        {
            Uri baseUri = new Uri(AppendDirectorySeparatorChar(basePath));
            Uri fullUri = new Uri(fullPath);
            Uri relativeUri = baseUri.MakeRelativeUri(fullUri);
            return Uri.UnescapeDataString(relativeUri.ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        private string AppendDirectorySeparatorChar(string path)
        {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                return path + Path.DirectorySeparatorChar;
            return path;
        }
    }
}
