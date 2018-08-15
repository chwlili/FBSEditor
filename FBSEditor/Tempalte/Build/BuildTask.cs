using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Tempalte;

namespace Tempalte.Build
{
    class BuildTask : TemplateParserBaseVisitor<int>
    {
        private StringBuilder output = new StringBuilder();
        private List<Dictionary<string, object>> stack = new List<Dictionary<string, object>>();

        public BuildTask(string path)
        {
            var lexer = new TemplateLexer(new AntlrInputStream(File.ReadAllText(path)));
            var parser = new TemplateParser(new CommonTokenStream(lexer));

            var document = parser.document();
            document.Accept<int>(this);

            output.Append("__ end");
        }


        public override int VisitDocument([NotNull] TemplateParser.DocumentContext context)
        {
            return base.VisitDocument(context);
        }

        public override int VisitText([NotNull] TemplateParser.TextContext context)
        {
            output.Append(context.GetText());
            return base.VisitText(context);
        }

        public override int VisitFor([NotNull] TemplateParser.ForContext context)
        {
            
            return base.VisitFor(context);
        }


        #region 名称域

        private void PushStack()
        {
            stack.Add(new Dictionary<string, object>());
        }

        private void PopStack()
        {
            stack.RemoveAt(stack.Count - 1);
        }

        private object Get(string name)
        {
            for (int i = stack.Count - 1; i >= 0; i--)
            {
                var dictionary = stack[i];
                if(dictionary.ContainsKey(name))
                {
                    return dictionary[name];
                }
            }
            return null;
        }

        private void Set(string name,object obj)
        {
            stack[stack.Count - 1][name] = obj;
        }

        #endregion
    }
}
