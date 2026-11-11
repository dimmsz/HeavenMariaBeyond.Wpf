using HeavenMariaBeyond.Domain.Entities.Dtos;
using HeavenMariaBeyond.Domain.Entities.Main;
using HeavenMariaBeyond.Domain.Enums;

namespace HeavenMariaBeyond.Service
{
    /// <summary>
    /// 女の子の属性（お気に入り、非表示、伝説）を切り替えるユーティリティサービスです。
    /// このクラスは不変オブジェクト（新しい DTO を返す）として動作します。
    /// </summary>
    public class ToggleGirlAttributeService
    {
        /// <summary>
        /// 指定した属性を切り替えた新しい <see cref="GirlWithStatusDto"/> を返します。
        /// </summary>
        /// <param name="currentDto">現在の状態を表す DTO。null は許容しません。</param>
        /// <param name="attribute">切り替える対象の属性。</param>
        /// <returns>属性を切り替えた新しい <see cref="GirlWithStatusDto"/>。属性が <see cref="GirlAttribute.None"/> の場合は元の DTO を返します。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="currentDto"/> が null の場合にスローされます。</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="attribute"/> が既知の列挙値でない場合にスローされます。</exception>
        public static GirlWithStatusDto Toggle(
            GirlWithStatusDto currentDto,
            GirlAttribute attribute)
        {
            if (currentDto == null) throw new ArgumentNullException(nameof(currentDto));

            return attribute switch
            {
                GirlAttribute.Favorite => Create(currentDto, favorite: !currentDto.Favorite),
                GirlAttribute.Hidden => Create(currentDto, hidden: !currentDto.Hidden),
                GirlAttribute.Legend => Create(currentDto, legend: !currentDto.Legend),
                GirlAttribute.None => currentDto,
                _ => throw new ArgumentOutOfRangeException(nameof(attribute))
            };
        }

        private static GirlWithStatusDto Create(
            GirlWithStatusDto dto,
            bool? favorite = null,
            bool? hidden = null,
            bool? legend = null)
        {
            var girl = new Girl
            {
                GirlId = dto.GirlId,
                DisplayName = dto.DisplayName,
                ProfileUrl = dto.ProfileUrl,
                CreatedAt = dto.CreatedAt
            };

            var status = new Status
            {
                GirlId = dto.GirlId,
                UpdatedAt = DateTimeOffset.UtcNow,
                Favorite = favorite ?? dto.Favorite,
                Hidden = hidden ?? dto.Hidden,
                Legend = legend ?? dto.Legend,
                ActivityStatus = dto.ActivityStatus
            };

            return new GirlWithStatusDto(girl, status);
        }
    }
}
