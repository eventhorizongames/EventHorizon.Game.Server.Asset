namespace EventHorizon.Game.Server.Asset.Hub.Base;

using EventHorizon.Game.Server.Asset.Policies;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

[Authorize(UserIdOrAdminPolicy.PolicyName)]
public partial class AdminHub
    : Hub
{
    private readonly ISender _sender;

    public AdminHub(
        ISender sender
    )
    {
        _sender = sender;
    }
}
