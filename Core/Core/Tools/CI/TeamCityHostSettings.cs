using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Text;
// ReSharper disable MemberCanBeMadeStatic.Global

namespace NoeticTools.Git2SemVer.Core.Tools.CI
{
    public sealed class TeamCityHostSettings
    {
        private const string BuildNumberEnvVarName = "BUILD_NUMBER";
        private const string TeamCityVersionEnvVarName = "TEAMCITY_VERSION";

        /// <summary>
        ///     Returns true currently hosted on a teamcity agent.
        /// </summary>
        public bool IsHost()
        {
            return !string.IsNullOrWhiteSpace(Version) &&
                   !string.IsNullOrWhiteSpace(BuildNumber);
        }

        public string Version => Environment.GetEnvironmentVariable(TeamCityVersionEnvVarName) ?? "";

        public string BuildNumber
        {
            get
            {
                var buildNumberVariable = Environment.GetEnvironmentVariable(BuildNumberEnvVarName);
                if (buildNumberVariable == null)
                {
                    Console.WriteLine("== BuildNumberEnvVarName not found");
                    return "";
                }
                return int.TryParse(buildNumberVariable!, out var buildNumber) ? buildNumber.ToString(CultureInfo.InvariantCulture) : "";
            }
        }
    }
}
