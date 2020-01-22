using ScaffoldEF.Commands;
using ScaffoldEF.Data;
using System;
using System.Reflection;

namespace ScaffoldEF
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Exec.Run(args);
            }
            catch (TargetInvocationException tex)
            {
                Console.WriteLine(tex.InnerException?.Message ?? "Execution failed, no further information was provided");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }

    sealed class CommandAttribute : Attribute
    {
        public CommandAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}