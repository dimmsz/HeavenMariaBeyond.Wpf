using System;

namespace HeavenMariaBeyondWPF.UserObject
{
    public class DiaryRecord
    {
        public string DiaryCode { get; set; } = string.Empty;
        public string DiaryTitle { get; set; } = string.Empty;
        public DateTime DiaryDate { get; set; }
    }
}
