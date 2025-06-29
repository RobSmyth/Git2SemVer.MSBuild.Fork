using System.Text.RegularExpressions;


namespace NoeticTools.Git2SemVer.Core.ConventionCommits;

public sealed class ConventionalCommitsParser : IConventionalCommitsParser
{
    private readonly Regex _bodyRegex = new("""
                                            \A
                                            (
                                              (?<body>\S ((\n|\r\n)|.)*? )
                                              (\Z | (?: (\n|\r\n) ) )
                                            )?
                                            (

                                              (?<footer>
                                                (
                                                  (?: (\n|\r\n)+ )
                                                  (?<token> (BREAKING\sCHANGE) | ( \w[\w-]+ ) )
                                                  ( \( (?<scope>\w[\w-]+) \) )?
                                                  ( \:\s|\s\# )
                                                  (?<value>
                                                    .*
                                                    (?:
                                                      (\n|\r\n).*
                                                    )*?
                                                  )
                                                )+
                                              )?
                                              \Z
                                            )
                                            """,
                                            RegexOptions.IgnorePatternWhitespace |
                                            RegexOptions.Multiline);

    private readonly Regex _summaryRegex = new("""
                                               \A
                                                 (?<ChangeType>\w[\w\-]*)
                                                   (\((?<scope>[\w\-\.]+)\))?(?<breakFlag>!)?: \s+(?<desc>\S.*?)
                                               \Z
                                               """,
                                               RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

    public CommitMessageMetadata Parse(string commitSummary, string commitMessageBody)
    {
        var summaryMatch = _summaryRegex.Match(commitSummary);
        if (!summaryMatch.Success)
        {
            return new CommitMessageMetadata();
        }

        var changeType = summaryMatch.GetGroupValue("ChangeType");
        var breakingChangeFlagged = summaryMatch.GetGroupValue("breakFlag").Length > 0;
        var changeDescription = summaryMatch.GetGroupValue("desc");

        var bodyMatch = _bodyRegex.Match(commitMessageBody);
        var bodyMatches = _bodyRegex.Matches(commitMessageBody);
        foreach (Match match in bodyMatches)
        {
            if (match.Success)
            {
                var group = match.Groups["footer"];
                if (!group.Success)
                {
                    continue;
                }

                //Console.WriteLine($"[{match.Groups["token"].Value} | {match.Groups["description"].Value}]");
            }
        }

        var body = bodyMatch.GetGroupValue("body");

        var keyValuePairs = GetFooterKeyValuePairs(bodyMatch);

        return new CommitMessageMetadata(changeType, breakingChangeFlagged, changeDescription, body, keyValuePairs);
    }

    private static List<(string key, string value)> GetFooterKeyValuePairs(Match match)
    {
        var keyValuePairs = new List<(string key, string value)>();

        var keywords = match.Groups["token"].Captures;
        var values = match.Groups["value"].Captures;

        for (var captureIndex = 0; captureIndex < keywords.Count; captureIndex++)
        {
            var keyword = keywords[captureIndex].Value;
            var value = values[captureIndex].Value.TrimEnd();

            // todo - scope AND !

            //Console.WriteLine($"{keyword} | {value}");
            keyValuePairs.Add((keyword, value));
        }
        //foreach (Match keywordMatch in keywords)
        //{
        //    if (!keywordMatch.Success)
        //    {
        //        continue;
        //    }

        //    var keyword = match.Groups["keyword"].Value;
        //    var description = match.Groups["description"].Value;
        //    Console.WriteLine($"{keyword} | {description}");
        //    keyValuePairs.Add((keyword, description));
        //}

        return keyValuePairs;
    }
}