using SpaceGame.Business.DTOs.Response;
using SpaceGame.Business.Game;

namespace SpaceGame.Business.Utilities.Mapper
{
    public class Profiles : AutoMapper.Profile
    {
        public Profiles()
        {
            CreateMap<GameRoom, RoomOnDashboardDto>();
        }
    }
}