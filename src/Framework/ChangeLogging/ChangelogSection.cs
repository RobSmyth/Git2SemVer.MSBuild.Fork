using System.Text.RegularExpressions;
using NoeticTools.Git2SemVer.Core.Exceptions;


namespace NoeticTools.Git2SemVer.Framework.ChangeLogging;

internal sealed class ChangelogSection
{
    private readonly string _changelogSectionPattern = @"^(?<=.*?\<\!-- Start {0} section -->.*?).*(?=^\<\!-- End {0} section -->.*?)";
    private readonly ChangelogDocument _document;
    private readonly string _name;
    private readonly Regex _regex;

    public ChangelogSection(string name, ChangelogDocument document)
    {
        _name = name;
        _document = document;
        _regex = new Regex(string.Format(_changelogSectionPattern, name),
                           RegexOptions.Multiline | RegexOptions.Singleline);
    }

    public string Content
    {
        get
        {
            var sourceMatch = _regex.Match(_document.Content);
            if (!sourceMatch.Success)
            {
                throw new
                    Git2SemVerInvalidFormatException($"The {_document.Name} changelog is missing missing a start or end {_name} section marker marker like '<!-- Start {_name} section -->'.");
            }

            return sourceMatch.Value;
        }
        set
        {
            var destMatch = _regex.Match(_document.Content);
            if (!destMatch.Success)
            {
                throw new
                    Git2SemVerInvalidFormatException($"The {_document.Name} changelog is missing missing a start or end {_name} section marker marker like '<!-- Start {_name} section -->'.");
            }

            var newContent = _regex.Replace(_document.Content, value, 1);
            _document.Content = newContent;
        }
    }
}