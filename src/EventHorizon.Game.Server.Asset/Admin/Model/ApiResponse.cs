namespace EventHorizon.Game.Server.Asset.Admin.Model
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? ErrorCode { get; set; }
        public T? Result { get; set; }
    }
}
