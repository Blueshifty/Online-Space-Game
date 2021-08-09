using ZombieGame.Business.Application;
using ZombieGame.Business.DTOs.Request;
using ZombieGame.Business.Utilities;

namespace ZombieGame.Business.DTOs.Event
{
    public class DisconnectUserEvent: ApplicationEvent
    {
        public DisconnectUser DisconnectUser { get; }

        public DisconnectUserEvent(DisconnectUser disconnectUser)
        {
            DisconnectUser = disconnectUser;
        }
    }
}