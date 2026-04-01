using HeavenMaria.Domain.Entities.Main;

namespace HeavenMaria.Domain.Interfaces
{
    public interface IGirlStatusRepository
    {
        Status GetStatus(string girlId);

        int Hide(string girlId);
        int Unhide(string girlId);

        int Favorite(string girlId);
        int Unfavorite(string girlId);

        int MarkAsLegend(string girlId);
        int UnMarkAsLegend(string girlId);
    }
}
