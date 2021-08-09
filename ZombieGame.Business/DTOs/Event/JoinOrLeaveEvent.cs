using System;
using ZombieGame.Business.Application;
using ZombieGame.Business.Utilities;

namespace ZombieGame.Business.DTOs.Event
{
    public class JoinOrLeaveEvent: ApplicationEvent
    {
        public bool State { get; set; }

        public JoinOrLeaveEvent(bool state)
        {
            State = state;
        }

        public JoinOrLeaveEvent()
        {
            
        }
    }
}