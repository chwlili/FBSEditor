using System;
using System.Collections.Generic;

namespace FBSEditor.FlatBufferData.Editor
{
    public class ErrorTracking
    {
        public static ErrorViewProvider Provider;

        private static Stack<ErrorScope> ErrorScopes = new Stack<ErrorScope>();
        
        public static void ClearScope()
        {
            ErrorScopes.Clear();
        }

        public static void PushScope(string projectFileName, string filePath, string text, int line, int column)
        {
            ErrorScopes.Push(new ErrorScope() { projectFileName = projectFileName, filePath = filePath, text = text, line = line, column = column });
        }

        public static void PopScope()
        {
            ErrorScopes.Pop();
        }


        public static void ClearError()
        {
            if (Provider != null)
            {
                Provider.ClearError();
            }
        }

        public static void LogError(string projectFileName, string filePath, string text, int line, int column)
        {
            if(Provider!=null)
            {
                Provider.PrintError(projectFileName, filePath, text, line, column);
            }
        }

        public static void LogError(string filePath, string text, int line, int column)
        {
            if(Provider!=null)
            {
                Provider.PrintError(String.Empty, filePath, text, line, column);
            }
        }

        public static void LogCsvError(string filePath, int row, string text)
        {
            if (Provider != null)
            {
                Provider.PrintError(String.Empty, filePath, text, row + 1, 0);
            }
        }

        public static void LogCsvError(string filePath, int row, int col, string text)
        {
            if (Provider != null)
            {
                Provider.PrintError(String.Empty, filePath, filePath + " <" + CsvIndex(row, col) + "> " + text, row + 1, col + 1);
            }
        }
        private static string CsvIndex(int row)
        {
            return (row + 1).ToString();
        }
        private static string CsvIndex(int row, int col)
        {
            return (row + 1) + "," + FormatXlsColumnName(col);
        }
        /// <summary>
        /// 格式化Xls的列名
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private static string FormatXlsColumnName(int index)
        {
            if (index < 24)
                return ((char)('A' + index)).ToString();
            else
                return FormatXlsColumnName((int)Math.Floor(index / 24.0f)) + FormatXlsColumnName(index % 24);
        }


        public static void Log(string text)
        {
            if (Provider != null)
            {
                Provider.PrintLog(text);
            }
        }


        public interface ErrorViewProvider
        {
            void PrintLog(string text);

            void ClearError();

            void PrintError(string projectFileName, string filePath, string text, int line, int column);
        }

        private struct ErrorScope
        {
            public string projectFileName;
            public string filePath;
            public string text;
            public int line;
            public int column;
        }
    }
}
