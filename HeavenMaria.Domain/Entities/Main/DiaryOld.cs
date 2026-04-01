namespace HeavenMaria.Domain.Entities.Main
{
    public class DiaryOld
    {
        public DateTime DiaryDate { get; set; }
        public DateTime DiaryGotDate { get; set; }
        public string DiaryUrl { get; set; } = string.Empty;
        public string DiaryCode { get; set; } = string.Empty;
        public string GirlID { get; set; } = string.Empty;
        public byte[] DiaryTitle { get; set; } = Array.Empty<byte>();
        public byte[] DiaryBody { get; set; } = Array.Empty<byte>();
        public string FrameTypeString { get; set; } = string.Empty;
        //photo,movie,none
        public string FrameUrl { get; set; } = string.Empty;
        //string RawURL
        public string DiaryRestrictionString { get; set; } = string.Empty;
        //normal,secret,limited
        public byte[] DiaryImageListString { get; set; } = Array.Empty<byte>();
        //RawURL
        public bool MovieFrameCompleted { get; set; }
    }
}
