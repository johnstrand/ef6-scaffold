using System;
using System.Linq;

namespace ScaffoldEF.Commands
{
    [Command("configure")]
    static class Configure
    {
        [Command("list")]
        internal static void List()
        {
            Settings.ConnectionManager.List().Any().AssertIsTrue("No connections configured, use 'configure add <name> <value>'");
            foreach (var item in Settings.ConnectionManager.List())
            {
                Console.WriteLine($"{item.Key}{Environment.NewLine}{item.Value}");
            }
        }

        [Command("add")]
        internal static void Add(string name, string value)
        {
            Console.WriteLine($"Adding connection string '{name}'");
            Settings.ConnectionManager.Exists(name).AssertIsFalse($"Connection string '{name}' already exists, use update to replace");
            Settings.ConnectionManager.AddOrUpdate(name, value);
            Settings.ConnectionManager.Save();
            Console.WriteLine("Connection string added");
        }

        [Command("remove")]
        internal static void Remove(string name)
        {
            Console.WriteLine($"Removing connection string '{name}'");
            Settings.ConnectionManager.TryRemove(name).AssertIsTrue($"Connection string '{name}' does not exist");
            Settings.ConnectionManager.Save();
            Console.WriteLine("Connection string removed");
        }

        [Command("update")]
        internal static void Update(string name, string value)
        {
            Console.WriteLine($"Updating connection string '{name}");
            Settings.ConnectionManager.Exists(name).AssertIsTrue($"Connection string '{name}' does not exist, use add to create");
            Settings.ConnectionManager.AddOrUpdate(name, value);
            Settings.ConnectionManager.Save();
            Console.WriteLine("Connection string updated");
        }

        [Command("test")]
        internal static void Test(string value)
        {
            try
            {
                using (new Data.DbClient(value)) { }
                Console.WriteLine("Connection succeeded");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection failed: " + ex.Message);
            }
        }
    }
}
