using HeavenMaria.Domain.Entities.Main;

namespace HeavenMaria.Domain.Entities.Dtos
{
    public class GirlWithStatusDto
    {
        // Girl
        public Girl Girl { get; set; }
        public string GirlId { get; } = string.Empty;
        public string DisplayName { get; } = string.Empty;
        public string ProfileUrl { get; } = string.Empty;
        public DateTimeOffset CreatedAt { get; }

        // Status
        public Status Status { get; set; }
        public DateTimeOffset UpdatedAt { get; }
        public bool Favorite { get; }
        public bool Hidden { get; }
        public bool Legend { get; }
        public ActivityStatus ActivityStatus { get; }

        // ★ 変換コンストラクタ
        public GirlWithStatusDto(Girl girl, Status status)
        {
            if (girl is null) throw new ArgumentNullException(nameof(girl));
            if (status is null) throw new ArgumentNullException(nameof(status));

            this.Girl = girl;
            GirlId = girl.GirlId;
            DisplayName = girl.DisplayName;
            ProfileUrl = girl.ProfileUrl;
            CreatedAt = girl.CreatedAt;

            this.Status = status;
            UpdatedAt = status.UpdatedAt;
            Favorite = status.Favorite;
            Hidden = status.Hidden;
            Legend = status.Legend;
            ActivityStatus = status.ActivityStatus;
        }

        public GirlWithStatusDto() { }
    }
}
