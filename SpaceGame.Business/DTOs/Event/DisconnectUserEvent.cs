using SpaceGame.Business.Application;
using SpaceGame.Business.DTOs.Request;
using SpaceGame.Business.Utilities;

namespace SpaceGame.Business.DTOs.Event
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