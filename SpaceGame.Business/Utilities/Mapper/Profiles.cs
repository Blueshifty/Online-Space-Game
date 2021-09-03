using SpaceGame.Business.DTOs.Response;
using SpaceGame.Business.Game;
using SpaceGame.Business.Game.Consumables;
using SpaceGame.Business.Game.Planets;
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
            CreateMap<Planet, PlanetDto>();
            CreateMap<Consumable, ConsumableDto>();
        }
    }
}