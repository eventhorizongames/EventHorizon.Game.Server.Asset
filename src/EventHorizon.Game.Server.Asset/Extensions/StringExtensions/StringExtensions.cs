#pragma warning disable CA1050 // Declare types in namespaces
using System.Diagnostics.CodeAnalysis;

public static class StringExtensions
#pragma warning restore CA1050 // Declare types in namespaces
{
    public static bool IsBlank(
        [NotNullWhen(false)] this string? stringObj
    ) => string.IsNullOrWhiteSpace(
        stringObj
    );

    public static bool IsNotBlank(
        [NotNullWhen(true)] this string? stringObj
    ) => !IsBlank(stringObj);
}
