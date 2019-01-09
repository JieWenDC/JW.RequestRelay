using JW.RequestRelay.Util.Formatting;
using JW.RequestRelay.Util.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JW.RequestRelay.Util.Text
{
    /// <summary>
    ///这个类用于从格式化字符串中提取动态值。
    /// 的工作原理是相反的。 <see cref="string.Format(string,object)"/>
    /// </summary>
    /// <example>
    /// str字符串格式 "My name is Neo."  format字符串 "My name is {name}.".
    ///提取"Neo"为name 
    /// </example>
    public partial class FormattedStringValueExtracter
    {
        /// <summary>
        /// 提取动态值的格式化字符串。
        /// </summary>
        /// <param name="str">包含动态值的字符串</param>
        /// <param name="format">字符串格式</param>
        /// <param name="ignoreCase">，要搜索大小写不敏感的。</param>
        public ExtractionResult Extract(string str, string format, bool ignoreCase = false)
        {
            var stringComparison = ignoreCase
                ? StringComparison.InvariantCultureIgnoreCase
                : StringComparison.InvariantCulture;

            if (str == format) //TODO: think on that!
            {
                return new ExtractionResult(true);
            }

            var formatTokens = new FormatStringTokenizer().Tokenize(format);
            if (formatTokens.ExistsData())
            {
                return new ExtractionResult(str == "");
            }

            var result = new ExtractionResult(true);

            for (var i = 0; i < formatTokens.Count; i++)
            {
                var currentToken = formatTokens[i];
                var previousToken = i > 0 ? formatTokens[i - 1] : null;

                if (currentToken.Type == FormatStringTokenType.ConstantText)
                {
                    if (i == 0)
                    {
                        if (!str.StartsWith(currentToken.Text, stringComparison))
                        {
                            result.IsMatch = false;
                            return result;
                        }

                        str = str.Substring(currentToken.Text.Length);
                    }
                    else
                    {
                        var matchIndex = str.IndexOf(currentToken.Text, stringComparison);
                        if (matchIndex < 0)
                        {
                            result.IsMatch = false;
                            return result;
                        }

                        Debug.Assert(previousToken != null, "previousToken can not be null since i > 0 here");

                        result.Matches.Add(new NameValue(previousToken.Text, str.Substring(0, matchIndex)));
                        str = str.Substring(matchIndex + currentToken.Text.Length);
                    }
                }
            }

            var lastToken = formatTokens.Last();
            if (lastToken.Type == FormatStringTokenType.DynamicValue)
            {
                result.Matches.Add(new NameValue(lastToken.Text, str));
            }

            return result;
        }

        /// <summary>
        /// Checks if given <see cref="str"/> fits to given <see cref="format"/>.
        /// Also gets extracted values.
        /// </summary>
        /// <param name="str">String including dynamic values</param>
        /// <param name="format">Format of the string</param>
        /// <param name="values">Array of extracted values if matched</param>
        /// <param name="ignoreCase">True, to search case-insensitive</param>
        /// <returns>True, if matched.</returns>
        public static bool IsMatch(string str, string format, out string[] values, bool ignoreCase = false)
        {
            var result = new FormattedStringValueExtracter().Extract(str, format, ignoreCase);
            if (!result.IsMatch)
            {
                values = new string[0];
                return false;
            }

            values = result.Matches.Select(m => m.Value).ToArray();
            return true;
        }

        /// <summary>
        /// Used as return value of <see cref="Extract"/> method.
        /// </summary>
        public partial class ExtractionResult
        {
            /// <summary>
            /// Is fully matched.
            /// </summary>
            public bool IsMatch { get; set; }

            /// <summary>
            /// List of matched dynamic values.
            /// </summary>
            public List<NameValue> Matches { get; private set; }

            internal ExtractionResult(bool isMatch)
            {
                IsMatch = isMatch;
                Matches = new List<NameValue>();
            }
        }
    }
}