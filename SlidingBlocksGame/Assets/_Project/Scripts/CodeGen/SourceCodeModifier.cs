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
                // Найдем последний using в файле
                var lastUsingIndex = content.LastIndexOf("using ", StringComparison.Ordinal);
                if (lastUsingIndex != -1)
                {
                    var insertIndex = content.IndexOf('\n', lastUsingIndex) + 1;
                    return content.Insert(insertIndex, requiredUsing + Environment.NewLine);
                }

                // Если using'ов нет, добавим в начало файла
                return requiredUsing + Environment.NewLine + Environment.NewLine + content;
            }

            return content;
        }

        private static string ModifyContent(string content, string fileName)
        {
            var classStack = new Stack<string>();
            string pattern =
                @"(?<=\n|^)(\s*)((?:public|internal|private|protected)?\s*(?:sealed\s+)?(?:struct|class)\s+(\w+)(?:\s*:\s*(?:TagComponentTemplate|ComponentTemplate)<[^>]+>)?)";

            return Regex.Replace(content, pattern, match =>
            {
                string indentation = match.Groups[1].Value;
                string classDeclaration = match.Groups[2].Value;
                string className = match.Groups[3].Value;
                string namespaceName = ExtractNamespace(content) ?? "global";
                string assemblyName = "Assembly-CSharp";

                // Определяем, является ли это вложенным классом
                if (indentation.Length > 0 && classStack.Count > 0)
                {
                    // Если отступ меньше, чем у предыдущего класса, удаляем классы из стека
                    while (classStack.Count > 0 && indentation.Length <= GetIndentationLength(classStack.Peek()))
                    {
                        classStack.Pop();
                    }

                    classStack.Push(indentation + className);
                    className = string.Join("/", classStack.Reverse().Select(c => c.Trim()));
                }
                else
                {
                    classStack.Clear();
                    classStack.Push(className);
                }

                if (!classDeclaration.Contains("TagComponentTemplate") && !classDeclaration.Contains("ComponentTemplate"))
                {
                    return match.Value; // Возвращаем исходное объявление без изменений
                }

                Debug.Log(classDeclaration);
                Debug.Log(className);
                Debug.Log(indentation);

                string attribute =
                    $"\n\t\t[MovedFrom(autoUpdateAPI: false, sourceNamespace: \"{namespaceName}\", sourceClassName: \"{className}\", sourceAssembly: \"{assemblyName}\")]\n";

                return attribute + "\t\t" + classDeclaration;
            });
        }

        private static int GetIndentationLength(string classDeclaration)
        {
            return classDeclaration.TakeWhile(char.IsWhiteSpace).Count();
        }

        private static string ExtractNamespace(string content)
        {
            var namespaceMatch = Regex.Match(content, @"namespace\s+([\w.]+)");
            return namespaceMatch.Success ? namespaceMatch.Groups[1].Value : null;
        }

        [Button]
        public static void Main(string path)
        {
            ModifySourceFiles(path);
        }
    }
}