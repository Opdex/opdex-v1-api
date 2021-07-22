using System.Text.RegularExpressions;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Extensions
{
    /// <summary>
    /// Extension methods on <see cref="string"/> related to SQL statements.
    /// </summary>
    public static class SqlStringExtensions
    {
        private static readonly Regex MultipleWhitespaceRegex = new Regex(@"\s+", RegexOptions.Compiled);

        /// <summary>
        /// <para>
        /// Remove excess whitespace from beginning, end, and middle of <paramref name="sql"/> string.
        /// It will leave a single leading and/or trailing whitespace character if <paramref name="sql"/> contains
        /// one or more whitespace characters at the beginning or end originally.
        /// </para>
        /// <para>
        /// CALLERS NOTE: This method is somewhat simplistic in assuming that SQL bind parameters are used for all values
        /// and that <paramref name="sql"/> does NOT contain any quoted literals,
        /// e.g. 'My string with   intended  inner whitespace', that should remain as-is (without removing inner whitespace).
        /// </para>
        /// </summary>
        public static string RemoveExcessWhitespace(this string sql)
        {
            return string.IsNullOrEmpty(sql)
                ? sql
                : MultipleWhitespaceRegex.Replace(sql, " ");
        }
    }
}
