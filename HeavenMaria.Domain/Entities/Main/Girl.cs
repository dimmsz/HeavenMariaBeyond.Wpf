namespace HeavenMaria.Domain.Entities.Main
{
    public class Girl
    {
        public string GirlId { get; set; } = string.Empty;//主キー
        public string DisplayName { get; set; } = string.Empty;
        public string ProfileUrl { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
    }
}
