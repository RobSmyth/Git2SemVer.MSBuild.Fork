using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Text;

namespace NoeticTools.Git2SemVer.Core.Tools.CI
{
    public sealed class TeamCityHostSettings
    {
        public const string BuildNumberEnvVarName = "BUILD_NUMBER";
        private const string TeamCityVersionEnvVarName = "TEAMCITY_VERSION";

        /// <summary>
        ///     Returns true currently hosted on a teamcity agent.
        /// </summary>
        public bool IsHost()
        {
            return !string.IsNullOrWhiteSpace(TeamCityVersionEnvVarName) &&
                   !string.IsNullOrWhiteSpace(BuildNumberEnvVarName);
        }

        public string Version => Environment.GetEnvironmentVariable(TeamCityVersionEnvVarName) ?? "";

        public string BuildNumber
        {
            get
            {
                var buildNumberVariable = Environment.GetEnvironmentVariable(TeamCityHostSettings.BuildNumberEnvVarName);
                return int.TryParse(buildNumberVariable!, out var buildNumber) ? buildNumber.ToString(CultureInfo.InvariantCulture) : "";
            }
        }
    }
}
