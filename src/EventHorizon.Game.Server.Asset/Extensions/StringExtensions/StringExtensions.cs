#pragma warning disable CA1050 // Declare types in namespaces
public static class StringExtensions
#pragma warning restore CA1050 // Declare types in namespaces
{
    public static bool IsBlank(
        this string stringObj
    ) => string.IsNullOrWhiteSpace(
        stringObj
    );

    public static bool IsNotBlank(
        this string stringObj
    ) => !IsBlank(stringObj);
}
