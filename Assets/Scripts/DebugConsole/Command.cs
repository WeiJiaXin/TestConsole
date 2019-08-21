using System;
using System.Collections.Generic;

namespace Lowy.DebugConsole
{
    public class Command
    {
        public string title;
        public Action onClick;
        public List<CommandArg> args;
    }

}
