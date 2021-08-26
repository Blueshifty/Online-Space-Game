using SpaceGame.Business.DTOs.Response;
using SpaceGame.Business.Game;
using SpaceGame.Business.Game.Projectiles;

namespace SpaceGame.Business.Utilities.Mapper
{
    public class Profiles : AutoMapper.Profile
    {
        public Profiles()
        {
            CreateMap<GameRoom, RoomOnDashboardDto>();
            CreateMap<Bullet, BulletDto>();
            CreateMap<Player, PlayerDto>();
        }
    }
}