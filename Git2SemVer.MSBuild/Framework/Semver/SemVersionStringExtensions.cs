using System.Text.RegularExpressions;
using Semver;


namespace NoeticTools.Git2SemVer.MSBuild.Framework.Semver;

/// <summary>
///     Helper string extensions for working with Semantic Versioning strings.
/// </summary>
public static class SemVersionStringExtensions
{
    /// <summary>
    ///     Returns true if the identifier complies to Semantic Versioning 2.0 allowed characters.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Semantic Versioning 2.0 states: "Identifiers MUST comprise only ASCII alphanumerics and hyphens [0-9A-Za-z-].
    ///         Identifiers MUST NOT be empty. "
    ///     </para>
    /// </remarks>
    public static bool IsAlphanumericOrHyphens(this string identifier)
    {
        var regex = new Regex(@"^[a-zA-Z0-9-]*$");
        return regex.IsMatch(identifier);
    }

    /// <summary>
    ///     Is this string composed entirely of ASCII digits '0' to '9'?
    /// </summary>
    public static bool IsDigits(this string value)
    {
        foreach (var c in value)
            if (!c.IsDigit())
            {
                return false;
            }

        return true;
    }

    /// <summary>
    ///     Split a string, map the parts, and return a read only list of the values.
    /// </summary>
    /// <remarks>
    ///     Splitting a string, mapping the result and storing into a <see cref="IReadOnlyList{T}" />
    ///     is a common operation in this package. This method optimizes that. It avoids the
    ///     performance overhead of:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>Constructing the params array for <see cref="string.Split(char[])" /></description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Constructing the intermediate <see cref="T:string[]" /> returned by
    ///                 <see cref="string.Split(char[])" />
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see cref="System.Linq.Enumerable.Select{TSource,TResult}(IEnumerable{TSource},Func{TSource,TResult})" />
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>Not allocating list capacity based on the size</description>
    ///         </item>
    ///     </list>
    ///     Benchmarking shows this to be 30%+ faster and that may not reflect the whole benefit
    ///     since it doesn't fully account for reduced allocations.
    /// </remarks>
    public static IReadOnlyList<T> SplitAndMapToReadOnlyList<T>(
        this string value,
        char splitOn,
        Func<string, T> func)
    {
        if (value.Length == 0)
        {
            return ReadOnlyList<T>.Empty;
        }

        // Figure out how many items the resulting list will have
        var count = 1; // Always one more item than there are separators
        for (var i = 0; i < value.Length; i++)
            if (value[i] == splitOn)
            {
                count++;
            }

        // Allocate enough capacity for the items
        var items = new List<T>(count);
        var start = 0;
        for (var i = 0; i < value.Length; i++)
            if (value[i] == splitOn)
            {
                items.Add(func(value.Substring(start, i - start)));
                start = i + 1;
            }

        // Add the final items from the last separator to the end of the string
        items.Add(func(value.Substring(start, value.Length - start)));

        return items.AsReadOnly();
    }

    /// <summary>
    ///     Convert dot delimited metadata identifiers string to a list of metadata identifiers.
    /// </summary>
    public static IReadOnlyList<MetadataIdentifier> ToMetadataIdentifiers(this string metadata)
    {
        return metadata.ToNormalisedSemVerString().Split('.').Select(x => new MetadataIdentifier(x)).ToReadOnlyList();
    }

    /// <summary>
    ///     Create dot delimited metadata identifiers string from a list of metadata identifiers.
    /// </summary>
    public static string ToMetadataString(this IReadOnlyList<MetadataIdentifier> metadata)
    {
        return string.Join(".", metadata);
    }

    /// <summary>
    ///     Replace all Semantic Version non-compliant identifier characters with dash ("-") characters.
    /// </summary>
    public static string ToNormalisedSemVerIdentifier(this string identifier)
    {
        var regex = new Regex(@"[^a-zA-Z0-9-]");
        return regex.Replace(identifier, "-");
    }

    /// <summary>
    ///     Replace all Semantic Version non-compliant version characters with dash ("-") characters.
    /// </summary>
    public static string ToNormalisedSemVerString(this string versionString)
    {
        var regex = new Regex(@"[^a-zA-Z0-9-\.]");
        return regex.Replace(versionString, "-");
    }

    /// <summary>
    ///     Convert dot delimited prerelease identifiers string to a list of prerelease identifiers.
    /// </summary>
    public static IReadOnlyList<PrereleaseIdentifier> ToPrereleaseIdentifiers(this string prerelease)
    {
        return prerelease.ToNormalisedSemVerString().Split('.').Select(x => new PrereleaseIdentifier(x)).ToReadOnlyList();
    }

    /// <summary>
    ///     Create dot delimited prerelease identifiers string from a list of prerelease identifiers.
    /// </summary>
    public static string ToPrereleaseString(this IReadOnlyList<PrereleaseIdentifier> prerelease)
    {
        return string.Join(".", prerelease);
    }
}