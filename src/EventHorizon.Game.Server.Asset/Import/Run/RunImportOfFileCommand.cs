namespace EventHorizon.Game.Server.Asset.Import.Run
{

    using EventHorizon.Game.Server.Asset.Core.Command;

    using MediatR;

    using Microsoft.AspNetCore.Http;

    public struct RunImportOfFileCommand
        : IRequest<StandardCommandResult>
    {
        public IFormFile File { get; }

        public RunImportOfFileCommand(
            IFormFile file
        )
        {
            File = file;
        }
    }
}
