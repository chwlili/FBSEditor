using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FBSEditor.Editor;
using FBSEditor.Build;
using System.IO;

namespace FBSEditor.Generator
{
    public class CSGenerator
    {
        public static void Generate(BuildCommandPackage pack, FBSFile[] files)
        {
            foreach (var file in files)
            {
                Generate(pack, file);
            }
        }

        private static void Generate(BuildCommandPackage pack, FBSFile file)
        {
            var sb = new StringBuilder();

            var hasNamespace = !string.IsNullOrEmpty(file.NameSpace);
            var classIndent = hasNamespace ? "    " : "";
            var fieldIndent = "    ";

            if(hasNamespace)
            {
                sb.AppendFormat("namespace {0}\n", file.NameSpace);
                sb.AppendFormat("{{\n");
            }

            foreach(var type in file.Tables)
            {
                sb.Append(FormatComment(type.Comment, classIndent));
                sb.AppendFormat("{0}public class {1}\n", classIndent, type.Name);
                sb.AppendFormat("{0}{{\n", classIndent);
                foreach (var field in type.Fields)
                {
                    sb.Append(FormatComment(field.Comment, classIndent + fieldIndent));
                    sb.AppendFormat("{0}public {1} {2} {{ get; set; }}\n\n", classIndent + fieldIndent, field.Type, field.Name);
                }
                sb.AppendFormat("{0}}}\n\n", classIndent);
            }
            foreach(var type in file.Structs)
            {
                sb.Append(FormatComment(type.Comment, classIndent));
                sb.AppendFormat("{0}public class {1}\n", classIndent, type.Name);
                sb.AppendFormat("{0}{{\n", classIndent);
                foreach (var field in type.Fields)
                {
                    sb.Append(FormatComment(field.Comment, classIndent + fieldIndent));
                    sb.AppendFormat("{0}public {1} {2} {{ get; set; }}\n\n", classIndent + fieldIndent, field.Type, field.Name);
                }
                sb.AppendFormat("{0}}}\n\n", classIndent);
            }
            foreach(var type in file.Enums)
            {
                sb.Append(FormatComment(type.Comment, classIndent));
                sb.AppendFormat("{0}public enum {1}\n", classIndent, type.Name);
                sb.AppendFormat("{0}{{\n", classIndent);
                foreach (var field in type.Fields)
                {
                    sb.Append(FormatComment(field.Comment, classIndent + fieldIndent));
                    sb.AppendFormat("{0}{1}{2},\n\n", classIndent + fieldIndent, field.Name, string.IsNullOrEmpty(field.Value) ? "" : " = " + field.Value);
                }
                sb.AppendFormat("{0}}}\n\n", classIndent);
            }

            if (hasNamespace)
            {
                sb.AppendFormat("}}");
            }

            var csPath = file.Path.Substring(0, file.Path.Length - Constants.FBSExtName.Length) + ".cs";
            File.WriteAllText(csPath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="classIndent"></param>
        /// <param name="fieldIndent"></param>
        private static string FormatComment(string text,string indent)
        {
            var sb = new StringBuilder();
            if (text != null && text.Length>0)
            {
                sb.AppendFormat("{0}/// <summary>\n", indent);
                foreach (string line in text.Split('\n'))
                {
                    sb.AppendFormat("{0}/// {1}\n", indent, line);
                }
                sb.AppendFormat("{0}/// <summary>\n", indent);
            }

            return sb.ToString();
        }
    }
}
