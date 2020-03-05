using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace AutoDocumentation
{
    class MethodInformation
    {
        #region Properties

        public string MethodName { get; set; }
        public MethodBase MethodBase { get; set; }
        public Type Type { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Displays a list of all method definitions - followed by where their base definition resides.
        /// </summary>
        /// <param name="methodInfoArray">An array of method information.</param>
        internal List<string> DisplayBaseMethodDefinitions(MethodInfo[] methodInfoArray)
        {
            int counter = 0;
            List<string> outputStrings = new List<string>();

            // Display base definitions for class methods passed in.
            for (int i = 0; i < methodInfoArray.Length; i++)
            {
                MethodInfo myMethodInfo = methodInfoArray[i];
                MethodBase myMethodBase = myMethodInfo.GetBaseDefinition();

                if (myMethodInfo.DeclaringType.Name != null && myMethodBase.DeclaringType.Name != null)
                {
                    string outputString = FormatOutputString(myMethodInfo.ReflectedType.Name, myMethodInfo.Name,
                        myMethodBase.DeclaringType.Name, IsOverride(myMethodInfo), myMethodInfo.IsPublic);
                    outputStrings.Add(outputString);
                }
                counter++;
            }

            // Display how many methods and write to file.
            //Console.WriteLine($"There were {counter} overridden methods.");
            return outputStrings;
        }

        /// <summary>
        /// Gets of a list of assemblies and writes them to a dictionary.
        /// </summary>
        /// <param name="methodBase">The assembly type to examine.</param>
        internal Dictionary<int, Type> GetAssemblyList(Assembly assembly)
        {
            int counter = 1;
            Dictionary<int, Type> testDictionary = new Dictionary<int, Type>();

            foreach (Type assemblyType in assembly.GetTypes())
            {
                if (!IsCompilerGenerated(assemblyType))
                {
                    testDictionary.Add(counter, assemblyType);
                    counter++;
                }
            }
            return testDictionary;
        }

        /// <summary>
        /// Formats className, methodName, baseClassName, IsOverride, and IsPublic into a single string for export to .csv.
        /// </summary>
        /// <param name="className">The class name of the method.</param>
        /// <param name="methodName">The method name.</param>
        /// <param name="baseClassName">The base class name for an overridden method.</param>
        /// <param name="IsOverride">If the method is overrideen or not.</param>
        /// <param name="IsPublic">If the method is public or nonPublic.</param>
        /// <returns>A formatted output string in common separated format.</returns>
        internal string FormatOutputString(string className, string methodName, string baseClassName, bool IsOverride, bool IsPublic)
        {
            var outputString = $"{className},{methodName},{baseClassName},{IsOverride},{IsPublic}";
            return outputString;
        }

        /// <summary>
        /// Prints a list of all assemblies and their positions in the dictionary.
        /// </summary>
        /// <param name="assemblyDictionary">A dictionary of all assemblies.</param>
        internal void PrintAssemblyList(Dictionary<int, Type> assemblyDictionary)
        {
            int i = 0;
            Program.PrintHeader("ASSEMBLY TYPES");
            for (i = 0; i <= assemblyDictionary.Count; i++)
            {
                if (assemblyDictionary.TryGetValue(i, out Type assemblyType))
                {
                    if (assemblyType != null)
                    { Console.WriteLine($"{i}.) {assemblyType.Name}"); }
                }
            }
            Console.WriteLine($"{i}.) Print all methods");
            Program.PrintDivider();
        }

        /// <summary>
        /// Prints a list of public, nonpublic, instance, and static methods in a type.
        /// </summary>
        /// <param name="type">The type. Contains methods to print.</param>
        internal void PrintMethodList(Type type)
        {
            MethodInfo[] typeInfo = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            string isPublic = "";
            for (int i = 0; i < typeInfo.Length; i++)
            {
                if (typeInfo[i].IsPublic)
                { isPublic = "Public"; }
                else
                { isPublic = "Non-Public"; }
                Console.WriteLine($"{typeInfo[i].Name} - {isPublic}");
            }

            Program.PrintDivider();
        }

        /// <summary>
        /// Print the properties of the passed in method.
        /// </summary>
        /// <param name="methodBase">The method information passed in.</param>
        internal void PrintMethodProperties(MethodBase methodBase)
        {
            // Get method parameters.
            Console.WriteLine("IsPrivate = " + methodBase.IsPrivate);
            Console.WriteLine("IsPublic = " + methodBase.IsPublic);
            Console.WriteLine("IsVirtual = " + methodBase.IsVirtual);
            Console.WriteLine("IsStatic = " + methodBase.IsStatic);
            Console.WriteLine("IsAbstract = " + methodBase.IsAbstract);
            Console.WriteLine("IsAssembly = " + methodBase.IsAssembly);
            Console.WriteLine("IsConstructor = " + methodBase.IsConstructor);
            Program.PrintDivider();
        }

        /// <summary>
        /// Displays a list of method information from an array.
        /// </summary>
        /// <param name="methodInfoArray">An array of method information.</param>
        internal void DisplayMethodInformation(MethodInfo[] methodInfoArray)
        {
            // Display information for all methods.
            for (int i = 0; i < methodInfoArray.Length; i++)
            {
                MethodInfo myMethodInfo = methodInfoArray[i];
                Console.WriteLine("The name of the method is {0}.", GetMethodNameAndParams(myMethodInfo));
            }

            // Divider.
            Program.PrintDivider();
        }

        /// <summary>
        /// Get the full name of the method including class name, method name, and parameters.
        /// </summary>
        /// <param name="methodBase">The method information passed in.</param>
        /// <returns>The class name, method name, and any parameters for that method.</returns>
        internal string GetFullMethodName(MethodBase methodBase)
        {
            // Get full method name.
            string fullName = string.Format("{0}.{1}({2})", methodBase.ReflectedType.Name, methodBase.Name,
                string.Join(", ", methodBase.GetParameters().Select(x => string.Format("{0} {1}", x.ParameterType, x.Name)).ToArray()));

            return fullName;
        }

        /// <summary>
        /// Get just method name and parameters without the class name.
        /// </summary>
        /// <param name="methodBase">The method information passed in.</param>
        /// <returns>The name and parameters of the passed in method.</returns>
        internal string GetMethodNameAndParams(MethodBase methodBase)
        {
            string name = string.Format("{0}({1})", methodBase.Name,
                string.Join(", ", methodBase.GetParameters().Select(x => string.Format("{0} {1}", x.ParameterType, x.Name)).ToArray()));

            return name;
        }

        /// <summary>
        /// Determines if a method is an override or not.
        /// </summary>
        /// <param name="method">The method passed in.</param>
        /// <returns>True if override, otherwise false.</returns>
        internal bool IsOverride(MethodInfo method)
        {
            if (method.GetBaseDefinition().DeclaringType != method.DeclaringType)
            { return true; }
            else
            { return false; }
        }

        /// <summary>
        /// Determines if a type is compiler generated or not.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if compiler generated, otherwise false</returns>
        internal bool IsCompilerGenerated(Type type)
        {
            var attribute = Attribute.GetCustomAttribute(type, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute));
            return attribute != null;
        }

        #endregion
    }
}
