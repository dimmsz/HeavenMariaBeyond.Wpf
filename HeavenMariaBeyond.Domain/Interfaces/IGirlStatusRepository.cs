using HeavenMariaBeyond.Domain.Entities.Main;

namespace HeavenMariaBeyond.Domain.Interfaces
{
    public interface IGirlStatusRepository
    {
        Status GetStatus(string girlId);

        int Hide(string girlId);
        int Unhide(string girlId);
        void SetHidden(IReadOnlyCollection<string> girlIds, bool hidden);

        int Favorite(string girlId);
        int Unfavorite(string girlId);

        int MarkAsLegend(string girlId);
        int UnMarkAsLegend(string girlId);
    }
}
