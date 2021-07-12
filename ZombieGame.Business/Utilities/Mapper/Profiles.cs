using ZombieGame.Business.DTOs.Response;
using ZombieGame.Business.Hubs.HubModels;

namespace ZombieGame.Business.Utilities.Mapper
{
    public class Profiles : AutoMapper.Profile
    {
        public Profiles()
        {
            CreateMap<GameRoom, RoomOnDashboardDto>();
        }
    }
}