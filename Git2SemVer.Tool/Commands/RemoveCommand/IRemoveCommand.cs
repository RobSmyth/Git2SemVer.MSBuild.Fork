using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoeticTools.Git2SemVer.Tool.Commands.RemoveCommand
{
    internal interface IRemoveCommand
    {
        bool HasError { get; }

        void Execute(string inputSolutionFile, bool unattended);
    }
}
