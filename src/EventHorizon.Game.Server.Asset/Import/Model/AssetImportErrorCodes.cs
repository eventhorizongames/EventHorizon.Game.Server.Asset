namespace EventHorizon.Game.Server.Asset.Import.Model
{
    public static class AssetImportErrorCodes
    {
        public const string GENERAL_IMPORT_ASSETS_ERROR = nameof(GENERAL_IMPORT_ASSETS_ERROR);
        public const string FAILED_TO_CREATE_BACKUP = nameof(FAILED_TO_CREATE_BACKUP);
        public const string FAILED_TO_CLEAN_EXISTING_ASSETS = nameof(FAILED_TO_CLEAN_EXISTING_ASSETS);
        public const string MISSING_API_FILE_ARGUMENT = nameof(MISSING_API_FILE_ARGUMENT);
    }
}
