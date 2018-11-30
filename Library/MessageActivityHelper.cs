using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Bot.Builder.Community.FormFlow.Advanced
{
    internal class MessageActivityHelper
    {
        internal static string GetSanitizedTextInput(IMessageActivity activity)
        {
            var text = (activity != null ? activity.Text : null);

            var result = text == null ? "" : text.Trim();
            if (result.StartsWith("\""))
            {
                result = result.Substring(1);
            }
            if (result.EndsWith("\""))
            {
                result = result.Substring(0, result.Length - 1);
            }

            return result;
        }

        internal static IMessageActivity BuildMessageWithText(string text)
        {
            return new Activity
            {
                Type = ActivityTypes.Message,
                Text = text
            };
        }

        internal static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = text.Normalize(NormalizationForm.FormD);
            var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).Normalize(NormalizationForm.FormC);
        }
    }
}
