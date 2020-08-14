/*
    Chat Translation Tool
    Copyright (C) 2020 Johanna Sierak

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TranslateTool
{
    static class StringArrayExtensions
    { 
        public static string[] ClearEmpty(this string[] array)
        {
            var result = new List<string>(array.Length);

            for(var i = 0; i < array.Length; i++)
            {
                var str = array[i];

                if(str != null)
                {
                    if(str != string.Empty)
                    {
                        result.Add(str);
                    }
                }    
            }

            return result.ToArray();
        }
    }
    class Command
    {
        private static Dictionary<string, Command> commands = new Dictionary<string, Command>();

        public static string Process(string input)
        {
            input = input.TrimStart('/');

            var match = Regex.Match(input, "\\s");
            var splitIndex = match.Index;

            if (splitIndex > 0)
            {
                var name = input.Substring(0, splitIndex).ToLower();
                string arg = "";

                if (commands.TryGetValue(name, out Command command))
                {
                    if (input.Length > splitIndex + 1)
                    {
                        arg = input.Substring(splitIndex + 1);
                    }

                    command.Run(arg);
                }
                else
                {
                    return name;
                }
            }
            else if(commands.TryGetValue(input, out Command command))
            {
                command.Run(null);
            }
            else
            {
                return input;
            }

            return null;
        }

        public static void Add(bool singleArg, Action<string[]> action, params string[] aliases)
        {
            if (aliases.Length > 0)
            {
                var newCommand = new Command(singleArg, action);

                foreach (var alias in aliases)
                {
                    commands[alias] = newCommand;
                }
            }
        }

        private bool singleArg;
        private Action<string[]> action;

        private Command(bool singleArg, Action<string[]> action)
        {
            this.singleArg = singleArg;
            this.action = action;
        }

        public void Run(string arg)
        {
            if(arg == null)
            {
                action(new string[0]);
            }
            else if(singleArg)
            {
                action(new string[] { arg }.ClearEmpty());
            }
            else
            {
                action(Regex.Split(arg, "\\s").ClearEmpty());
            }
        }
    }
}
