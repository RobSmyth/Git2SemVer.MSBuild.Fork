namespace NoeticTools.Git2SemVer.Core.Diagnostics;

public abstract class DiagnosticCodeBase
{

    protected DiagnosticCodeBase(int id,
                                 string subcategory,
                                 string description,
                                 string resolution,
                                 string message, params object[] messageArgs)
    {
        Code = $"GSV{id:D3}";
        Message = string.Format(message, messageArgs);
        Description = string.Format(description, messageArgs);;
        Resolution = string.Format(resolution, messageArgs);
        Subcategory = subcategory;
        DocFxPageContents = $"""
                     ---
                     uid: {Code}
                     ---

                     <div style="background-color:#944248;padding:0px;margin-bottom:0.5em">
                       <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/Git2SemVer_banner_840x70.png"/>
                     </div>
                     <br/>

                     # {Code} diagnostic code
                     
                     | Property      | Value      |
                     | :---          | :---       |
                     | ID            | {Code}     |
                     | Subcategory   | {Subcategory} |
                     
                     ## Message text

                     ``{Message}``

                     ## Description

                     {Description}

                     ## Resolution

                     {Resolution}
                     """;
    }

    public string Subcategory { get; }

    public string DocFxPageContents { get; }

    /// <summary>
    /// Message that will appear in the compiler output as well as on the documentation page.
    /// </summary>
    public string Message { get; }

    public string MessageWithCode => $"{Code}: {Message}";

    public string SubCategory { get; } = "Versioning";

    public string Code { get; }

    /// <summary>
    /// Description that will appear on the documentation page only.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Resolution that will appear on the documentation page only.
    /// </summary>
    public string Resolution { get; }

    public string HelpLink => $"https://noetictools.github.io/Git2SemVer.MSBuild/Reference/ErrorCodes/{Code}.html";
}