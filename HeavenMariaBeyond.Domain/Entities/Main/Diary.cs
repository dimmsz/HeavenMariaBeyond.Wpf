namespace HeavenMariaBeyond.Domain.Entities.Main
{
    public class Diary
    {
        public DateTime diary_date { get; set; }
        public DateTime created_at { get; set; }
        public string diary_url { get; set; } = string.Empty;
        public string diary_code { get; set; } = string.Empty;
        public string girl_id { get; set; } = string.Empty;
        public string diary_title { get; set; } = string.Empty;
        public byte[]? diary_body { get; set; }
        public string? frame_type { get; set; }
        public string? frame_url { get; set; }
        public string? diary_restriction { get; set; }
        public string? diary_image_list { get; set; }
        public bool movie_frame_completed { get; set; }
        public byte[]? max_zstd_old { get; set; }
        public byte[]? max_zstd_new { get; set; }
        public string? raw { get; set; }
        public DateTime? moved_at { get; set; }

        public DateTime DiaryDate { get => diary_date; set => diary_date = value; }
        public DateTime CreatedAt { get => created_at; set => created_at = value; }
        public string DiaryUrl { get => diary_url; set => diary_url = value; }
        public string DiaryCode { get => diary_code; set => diary_code = value; }
        public string GirlID { get => girl_id; set => girl_id = value; }
        public string DiaryTitle { get => diary_title; set => diary_title = value; }
        public byte[]? DiaryBody { get => diary_body; set => diary_body = value; }
        public string? FrameTypeString { get => frame_type; set => frame_type = value; }
        public string? FrameUrl { get => frame_url; set => frame_url = value; }
        public string? DiaryRestrictionString { get => diary_restriction; set => diary_restriction = value; }
        public string? DiaryImageListString { get => diary_image_list; set => diary_image_list = value; }
        public bool MovieFrameCompleted { get => movie_frame_completed; set => movie_frame_completed = value; }
        public string? Raw { get => raw; set => raw = value; }
        public byte[]? MaxZstdOld { get => max_zstd_old; set => max_zstd_old = value; }
        public byte[]? MaxZstdNew { get => max_zstd_new; set => max_zstd_new = value; }
        public DateTime? MovedAt { get => moved_at; set => moved_at = value; }
    }
}
