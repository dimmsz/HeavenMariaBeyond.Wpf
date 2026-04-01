namespace HeavenMaria.Domain.Entities.Main
{
    public class Diary
    {
        public DateTime DiaryDate { get; set; }
        public DateTime DiaryGotDate { get; set; }
        public string DiaryUrl { get; set; }
        public string DiaryCode { get; set; }
        public string GirlID { get; set; }
        public string DiaryTitle { get; set; }
        public byte[] DiaryBody { get; set; }
        public string FrameTypeString { get; set; }
        public string FrameUrl { get; set; }
        public string DiaryRestrictionString { get; set; }
        public string DiaryImageListString { get; set; }
        public bool MovieFrameCompleted { get; set; }
        public string Raw { get; set; }
        public byte[] MaxZstdOld { get; set; }
        public byte[] MaxZstdNew { get; set; }
    }
}
