using HeavenMaria.Domain.Entities.Main;
using HeavenMaria.Domain.Entities.Sub;
using HeavenMaria.Domain.Interfaces;
using Org.BouncyCastle.Asn1.Ocsp;
using System.IO;

namespace HeavenNetCollector.Infrastructure.Providers
{
    public class PathProvider : IPathProvider
    {
        private string CurrentDiaryRootDirPath { get; } =
            @"Q:\HeavenNet\Diaries\";
        private string OldDiaryRootDirPath { get; } =
            @"V:\MegaSync\VisualStudio\Project\HeavenNetCollector\HeavenNetCollector\bin\Debug\Diaries";

        public string GetGirlProfileDirectoryPath(Girl girl)
        {
            string dirName = "profile";
            string dirPath = Path.Combine(CurrentDiaryRootDirPath, girl.GirlId, dirName);
            return dirPath;
        }

        public GirlPath GetDirPath(string girlId, DateOnly date)
        {
            string dtString = date.ToString("yyMMdd");
            string girlRootDirPath = Path.Combine(CurrentDiaryRootDirPath, girlId);

            var girlPathType = new GirlPath
            {
                GirlRootDirPath = girlRootDirPath,
                GirlProfileDirPathWithDate = Path.Combine(girlRootDirPath, "Profile", dtString),
                GirlProfileImageDirPathWithDate = Path.Combine(girlRootDirPath, "Profile", dtString, "Images")
            };

            return girlPathType;
        }

        public string GetDiaryDirectoryPath(DiaryOld diary)
        {
            string girlId = diary.GirlID;
            string dirPath = Path.Combine(CurrentDiaryRootDirPath, girlId, diary.DiaryCode);
            return dirPath;
        }

        public string GetDiaryDirectoryPath(Diary diary)
        {
            string girlId = diary.GirlID;
            string dirPath = Path.Combine(CurrentDiaryRootDirPath, girlId, diary.DiaryCode);
            return dirPath;
        }

        public string GetOldGirlDirectoryPath(Girl girl)
        {
            string dirPath = Path.Combine(OldDiaryRootDirPath, girl.GirlId);
            return dirPath;
        }
    }
}
