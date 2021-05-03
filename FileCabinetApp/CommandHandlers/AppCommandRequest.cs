using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public class AppCommandRequest
    {
        public string Command { get; }

        public string Parameters { get; }

        public AppCommandRequest(string command, string parameters)
        {
            this.Command = command;
            this.Parameters = parameters;
        }
    }
}
