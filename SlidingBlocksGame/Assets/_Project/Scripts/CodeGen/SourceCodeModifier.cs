using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.CodeGen
{
    public class SourceCodeModifier : MonoBehaviour
    {
        private const string REQUIRED_USING = "using UnityEngine.Scripting.APIUpdating;";

        public static void ModifySourceFiles(string directoryPath)
        {
            foreach (var file in Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories))
            {
                ModifyFile(file);
            }
        }

        private static void ModifyFile(string filePath)
        {
            string content = File.ReadAllText(filePath);
            string modifiedContent = AddRequiredUsing(content);
            modifiedContent = ModifyContent(modifiedContent, Path.GetFileNameWithoutExtension(filePath));

            if (content != modifiedContent)
            {
                File.WriteAllText(filePath, modifiedContent);
                Console.WriteLine($"Modified: {filePath}");
            }
        }

        private static string AddRequiredUsing(string content)
        {
            const string requiredUsing = "using UnityEngine.Scripting.APIUpdating;";
            if (!content.Contains(requiredUsing))
            {
                var lastUsingIndex = content.LastIndexOf("using ", StringComparison.Ordinal);
                if (lastUsingIndex != -1)
                {
                    var insertIndex = content.IndexOf('\n', lastUsingIndex) + 1;
                    return content.Insert(insertIndex, requiredUsing + Environment.NewLine);
                }

                return requiredUsing + Environment.NewLine + Environment.NewLine + content;
            }

            return content;
        }

        private static string ModifyContent(string content, string fileName)
        {
            var classStack = new Stack<string>();
            string namespaceName = ExtractNamespace(content) ?? "global";
            string assemblyName = "Assembly-CSharp";

            string pattern =
                @"((?:[\r\n]+\s*)+)(\[MovedFrom\([^\)]*\)\]\s*(?:[\r\n]+\s*)*)?(public|internal|private|protected)?\s*(sealed\s+)?(struct|class)\s+(\w+)(\s*:\s*(?:TagComponentTemplate|ComponentTemplate)<[^>]+>)?";

            return Regex.Replace(content, pattern, match =>
            {
                string leadingWhitespace = match.Groups[1].Value;
                string existingAttribute = match.Groups[2].Value;
                string accessModifier = match.Groups[3].Value;
                string sealedKeyword = match.Groups[4].Value;
                string structOrClass = match.Groups[5].Value;
                string className = match.Groups[6].Value;
                string inheritance = match.Groups[7].Value;

                // Определяем, является ли это вложенным классом
                string fullClassName = className;
                if (classStack.Count > 0)
                {
                    fullClassName = string.Join("/", classStack.Reverse().Concat(new[] { className }));
                }

                classStack.Push(className);

                // Проверяем, нужно ли добавлять атрибут
                if (!string.IsNullOrEmpty(inheritance) &&
                    (inheritance.Contains("TagComponentTemplate") || inheritance.Contains("ComponentTemplate")))
                {
                    string newAttribute =
                        $"[MovedFrom(autoUpdateAPI: false, sourceNamespace: \"{namespaceName}\", sourceClassName: \"{fullClassName}\", sourceAssembly: \"{assemblyName}\")]";
                    string classDeclaration =
                        $"{accessModifier} {sealedKeyword}{structOrClass} {className}{inheritance}";

                    return $"{leadingWhitespace}{newAttribute}\n{leadingWhitespace}{classDeclaration}";
                }

                // Если атрибут не нужен, возвращаем исходное объявление
                return match.Value;
            });
        }

        private static string ExtractNamespace(string content)
        {
            var namespaceMatch = Regex.Match(content, @"namespace\s+([\w.]+)");
            return namespaceMatch.Success ? namespaceMatch.Groups[1].Value : null;
        }


        private static void RemoveUnnecessaryUsingFromFile(string filePath)
        {
            string content = File.ReadAllText(filePath);
            bool hasRequiredClasses = CheckForRequiredClasses(content);

            if (!hasRequiredClasses)
            {
                string modifiedContent = RemoveRequiredUsing(content);
                if (content != modifiedContent)
                {
                    File.WriteAllText(filePath, modifiedContent);
                    Console.WriteLine($"Removed unnecessary using from: {filePath}");
                }
            }
        }

        private static bool CheckForRequiredClasses(string content)
        {
            string pattern = @"(struct|class)\s+\w+\s*:\s*(?:TagComponentTemplate|ComponentTemplate)<[^>]+>";
            return Regex.IsMatch(content, pattern);
        }

        private static string RemoveRequiredUsing(string content)
        {
            var usingIndex = content.IndexOf(REQUIRED_USING, StringComparison.Ordinal);
            if (usingIndex != -1)
            {
                int endIndex = content.IndexOf('\n', usingIndex) + 1;
                return content.Remove(usingIndex, endIndex - usingIndex);
            }

            return content;
        }

        [Button]
        public static void Main(string path)
        {
            ModifySourceFiles(path);
        }

        [Button]
        public static void RemoveUnnecessaryUsing(string directoryPath)
        {
            foreach (var file in Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories))
            {
                RemoveUnnecessaryUsingFromFile(file);
            }
        }
    }
}