namespace HeavenMaria.Domain.DisplayHtml
{
    public class DiaryEnums
    {
        /// <summary>
        /// 日記の属性(限定等)列挙型
        /// </summary>
        public enum DiaryDivision
        {
            secret, limited, normal
        }

        /// <summary>
        /// フレームの種類
        /// </summary>
        public enum FrameType
        {
            photoframe, movieframe, none
        }

        public class FrameClass
        {
            public string RawURL = string.Empty;
            public string DirectURL = string.Empty;
            public string LocalFileName = string.Empty;
        }
        public class DiaryImageClass
        {
            public string RawURL = string.Empty;
            public string DirectURL = string.Empty;
            public string LocalFileName = string.Empty;
        }
    }
}
