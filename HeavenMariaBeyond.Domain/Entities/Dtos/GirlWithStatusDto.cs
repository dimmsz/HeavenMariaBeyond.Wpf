using HeavenMariaBeyond.Domain.Entities.Main;

namespace HeavenMariaBeyond.Domain.Entities.Dtos
{
    public class GirlWithStatusDto
    {
        public Girl Girl { get; }
        public string GirlId => Girl.GirlId;
        public string DisplayName => Girl.DisplayName;
        public string ProfileUrl => Girl.ProfileUrl;
        public DateTimeOffset CreatedAt => Girl.CreatedAt;

        public Status Status { get; }
        public DateTimeOffset UpdatedAt => Status.UpdatedAt;
        public bool Favorite => Status.Favorite;
        public bool Hidden => Status.Hidden;
        public bool Legend => Status.Legend;
        public ActivityStatus ActivityStatus => Status.ActivityStatus;

        // ★ 変換コンストラクタ
        public GirlWithStatusDto(Girl girl, Status status)
        {
            Girl = girl ?? throw new ArgumentNullException(nameof(girl));
            Status = status ?? throw new ArgumentNullException(nameof(status));
        }

    }
}
