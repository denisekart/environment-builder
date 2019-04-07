namespace EnvironmentBuilder.Implementation.Json
{
    internal static class JConstants
    {
        public const char NodeOpeningToken = '{';
        public const char NodeClosingToken = '}';

        public const char EnumerationOpeningToken = '[';
        public const char EnumerationClosingToken = ']';

        public const char KeyValueOpeningClosingToken = '"';
        public const char KeyValueSeparationToken = ':';

        public const char SeparatorToken = ',';

        public const char EscapeToken = '\\';

        public const char SegmentRootToken = '$';
        public const char SegmentSeparatorToken = '.';
        public const char WildcardSegmentToken = '*';
        public const char SegmentIndexerStart = '[';
        public const char SegmentIndexerEnd = ']';
    }
}