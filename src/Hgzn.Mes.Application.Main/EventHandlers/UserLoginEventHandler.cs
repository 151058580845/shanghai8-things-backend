using Hgzn.Mes.Domain.Events;
using MediatR;

namespace Hgzn.Mes.Application.Main.EventHandlers
{
    public class UserLoginEventHandler : INotificationHandler<UserLoginEvent>
    {
        public Task Handle(UserLoginEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}