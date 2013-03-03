using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MarkdownDeep;
using jamescms.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace jamescms.Helpers
{
	/// <summary>
	/// Helper class for transforming Markdown.
	/// </summary>
	public static partial class MarkdownHelper
	{
		/// <summary>
		/// Transforms a string of Markdown into HTML.
		/// </summary>
		/// <param name="text">The Markdown that should be transformed.</param>
		/// <returns>The HTML representation of the supplied Markdown.</returns>
		public static IHtmlString Markdown(string text, RequestContext context)
		{
			// Transform the supplied text (Markdown) into HTML.
			var markdownTransformer = new jamesMarkdown();
            markdownTransformer.RequestContext = context;
            markdownTransformer.FormatCodeBlock = new System.Func<MarkdownDeep.Markdown, string, string>(FormatCodePrettyPrint);            
			string html = markdownTransformer.Transform(text);
            
			// Wrap the html in an MvcHtmlString otherwise it'll be HtmlEncoded and displayed to the user as HTML :(
			return new MvcHtmlString(html);
		}

		/// <summary>
		/// Transforms a string of Markdown into HTML.
		/// </summary>
		/// <param name="helper">HtmlHelper - Not used, but required to make this an extension method.</param>
		/// <param name="text">The Markdown that should be transformed.</param>
		/// <returns>The HTML representation of the supplied Markdown.</returns>
		public static IHtmlString Markdown(this HtmlHelper helper, string text)
		{
			return Markdown(text, helper.ViewContext.RequestContext);
		}

        public static Regex rxExtractLanguage = new Regex("^({{(.+)}}[\r\n])", RegexOptions.Compiled);
        public static string FormatCodePrettyPrint(MarkdownDeep.Markdown m, string code)
        {
            // Try to extract the language from the first line
            var match = rxExtractLanguage.Match(code);
            string language = null;

            if (match.Success)
            {
                // Save the language
                var g = (Group)match.Groups[2];
                language = g.ToString();

                // Remove the first line
                code = code.Substring(match.Groups[1].Length);
            }

            // If not specified, look for a link definition called "default_syntax" and
            // grab the language from its title
            if (language == null)
            {
                var d = m.GetLinkDefinition("default_syntax");
                if (d != null)
                    language = d.title;
            }

            // Common replacements
            if (language == "C#")
                language = "csharp";
            if (language == "C++")
                language = "cpp";

            // Wrap code in pre/code tags and add PrettyPrint attributes if necessary
            if (string.IsNullOrEmpty(language))
                return string.Format("<pre><code>{0}</code></pre>\n", code);
            else
                return string.Format("<pre class=\"prettyprint lang-{0}\"><code>{1}</code></pre>\n",
                                    language.ToLowerInvariant(), code);
        }
	}

}