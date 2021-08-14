using System;
using SpaceGame.Business.Application;
using SpaceGame.Business.Utilities;

namespace SpaceGame.Business.DTOs.Event
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