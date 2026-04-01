using HeavenMaria.Domain.Entities.Dtos;

namespace HeavenMaria.Service
{
    public class IndexFinder
    {
        public static int GetGirlIndex(List<GirlWithStatusDto> Girls, GirlWithStatusDto selectedGirl)
        {
            int girlIndex = Girls.IndexOf(selectedGirl);

            if (Girls == null || Girls.Count() == 0)
            {
                return 0;
            }

            int girlsCount = Girls.Count();

            // インデックス計算
            int targetIndex = girlIndex == 0 ? 1
                            : girlIndex == girlsCount - 1 ? girlsCount - 2
                            : girlIndex + 1;

            // 安全なアクセス
            return targetIndex >= 0 && targetIndex < girlsCount
                   ? targetIndex
                   : 0;
        }
    }
}
