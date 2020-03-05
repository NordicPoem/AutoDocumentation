using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AutoDocumentation
{
    class Program : MethodInformation
    {
        static void Main(string[] args)
        {
            // Create objects for file operations and reflection operations.
            FileManager FileManager = new FileManager("AutoDocumentationMethods", @"..\");
            MethodInformation methodInfo = new MethodInformation();
            bool isDone = false;
            bool validUserInput = false;
            int printAllOption = 0;

            // Delete file if it already exists.
            FileManager.DeleteFileIfExists(FileManager.FilePath);

            // Print method info if our assembly type is not compiler generated.
            Assembly ChosenAssembly = typeof(Console).Assembly;

            // Get list of assemblies.
            Dictionary<int, Type> assemblyDictionary = methodInfo.GetAssemblyList(ChosenAssembly);

            while (!isDone)
            {
                // Print list of assemblies in assemblyDictionary.
                methodInfo.PrintAssemblyList(assemblyDictionary);
                printAllOption = assemblyDictionary.Count + 1;

                while (!validUserInput)
                {
                    // Get the user's menu selection.
                    int userInput = GetMenuSelection();

                    // Get input from user (which class to get methods for)
                    if (userInput >= 1 && userInput <= assemblyDictionary.Count)
                    {
                        if (assemblyDictionary.TryGetValue(userInput, out Type type))
                        { PrintHeader(type.Name); }

                        // Print list of methods in a type.
                        methodInfo.PrintMethodList(type);

                        // Write to file.
                        List<string> outputStrings = methodInfo.DisplayBaseMethodDefinitions(type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
                        FileManager.WriteToFile(outputStrings);

                        validUserInput = true;
                    }
                    else if (userInput == printAllOption)
                    {
                        foreach (Type type in ChosenAssembly.GetTypes())
                        {
                            if (!methodInfo.IsCompilerGenerated(type))
                            {
                                PrintHeader(type.Name);
                                methodInfo.PrintMethodList(type);

                                // Write to file.
                                List<string> outputStrings = methodInfo.DisplayBaseMethodDefinitions(type.GetMethods(BindingFlags.Public |
                                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static));
                                FileManager.WriteToFile(outputStrings);

                                validUserInput = true;
                            }
                        }
                    }
                    else if (userInput > printAllOption)
                    { Console.WriteLine($"Please select a value between 1 and {printAllOption} "); }
                    else
                    { Console.WriteLine("Possible malformed input, please enter a valid value and try again."); }
                }

                // Check if the user is finished.
                isDone = CheckIfFinished();
                validUserInput = false;
            }

            #region Debugging Specific

            // Keeping the console open if we're debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            { Console.ReadKey(); }

            #endregion
        }

        /// <summary>
        /// Gets the user's menu selection.
        /// </summary>
        /// <returns>user's input if true, otherwise -1 (invalid input).</returns>
        public static int GetMenuSelection()
        {
            Console.Write("Select an assembly from the list above: ");
            if (int.TryParse(Console.ReadLine(), out int userInput))
            {
                Console.WriteLine();
                return userInput;
            }
            else
            {
                Console.WriteLine();
                return -1;
            }
        }

        /// <summary>
        /// CHeck if the user is finished or wants to continue selecting items.
        /// </summary>
        /// <returns>true if finished, otherwise false.</returns>
        public static bool CheckIfFinished()
        {
            Console.Write("Continue? (Y/N): ");
            string input = Console.ReadLine();

            while (!input.ToUpper().Equals("Y") && !input.ToUpper().Equals("N"))
            {
                Console.Write("Please enter a valid input: ");
                input = Console.ReadLine();
            }

            if (input.ToUpper().Equals("Y"))
            {
                PrintDivider();
                return false;
            }
            else
            { return true; }
        }

        /// <summary>
        /// Prints a menu header to console with passed in title.
        /// </summary>
        /// <param name="title">The title of the header.</param>
        public static void PrintHeader(string title)
        {
            Console.WriteLine($"---{title}---");
        }

        /// <summary>
        /// Prints a divider to the console.
        /// </summary>
        public static void PrintDivider()
        {
            Console.WriteLine("-------------------------------");
        }
    }
}
