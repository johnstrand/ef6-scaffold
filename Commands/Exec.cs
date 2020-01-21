using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ScaffoldEF.Commands
{
    static class Exec
    {
        internal static void Run(string[] args)
        {
            args = args.Select(arg => arg.Trim()).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToArray();

            var commands = typeof(Exec).Assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<CommandAttribute>() != null)
                .ToDictionary(
                    t => t.GetCustomAttribute<CommandAttribute>().Name,
                    t => t);

            Validate(args, commands);

            var command = args[0];

            var subcommands = commands[command].GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(m => m.GetCustomAttribute<CommandAttribute>() != null)
                .ToDictionary(
                    m => m.GetCustomAttribute<CommandAttribute>().Name,
                    m => m);

            args = args[1..^0];

            Validate(args, subcommands, command);

            var subcommand = args[0];

            args = args[1..^0];

            var method = subcommands[subcommand];

            var parameters = method.GetParameters();
            var mismatch = parameters.Length != args.Length;

            mismatch.AssertIsFalse(
                $"{command} {subcommand} requires {parameters.Length} argument(s), received {args.Length}",
                $"{command} {subcommand} {string.Join(" ", parameters.Select(p => $"<{p.Name}>"))}");

            // TODO: Handle conversion to other types
            method.Invoke(null, args);
        }

        private static void Validate(string[] args, IDictionary dict, string prefix = "")
        {
            var keys = new HashSet<string>(dict.Keys.Cast<string>());
            var error = new List<string>();
            if (args.Length > 0 && !keys.Contains(args[0]))
            {
                error.Add($"Unknown command {(prefix.Length == 0 ? "" : prefix + " ")}{args[0]}");
            }

            if (args.Length == 0 || error.Any())
            {
                error.Add("Available commands:");
                foreach (var command in dict.Keys.Cast<string>())
                {
                    error.Add($"  * {(prefix.Length == 0 ? "" : prefix + " ")}{command}");
                }
            }

            error.Any().AssertIsFalse(string.Join(Environment.NewLine, error));
        }
    }
}
