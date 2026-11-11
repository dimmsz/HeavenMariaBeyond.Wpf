namespace HeavenMariaBeyond.Domain.Entities.Main
{
    public class Status
    {
        public string GirlId { get; set; } = string.Empty;//主キー
        public DateTimeOffset UpdatedAt { get; set; }

        public bool Favorite { get; set; }
        public bool Hidden { get; set; }
        public bool Legend { get; set; }

        public ActivityStatus ActivityStatus { get; set; }
    }

    public enum ActivityStatus
    {
        //卒業済み
        Graduated,

        //取得の一時停止
        Ceessation,

        //通常の取り扱い-活動中の嬢
        Normal
    }
}
