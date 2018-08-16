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
        private List<Dictionary<string, Atom>> stack = new List<Dictionary<string, Atom>>();

        public BuildTask(string path)
        {
            var lexer = new TemplateLexer(new AntlrInputStream(File.ReadAllText(path)));
            var parser = new TemplateParser(new CommonTokenStream(lexer));

            var document = parser.document();
            document.Accept<int>(this);

            var txt = output.ToString();
            Console.WriteLine(txt);
        }


        public override int VisitDocument([NotNull] TemplateParser.DocumentContext context)
        {
            return base.VisitDocument(context);
        }

        public override int VisitText([NotNull] TemplateParser.TextContext context)
        {
            PushStack();
            output.Append(context.GetText());
            return base.VisitText(context);
        }

        public override int VisitIf([NotNull] TemplateParser.IfContext context)
        {
            PushStack();
            try
            {
                var codes = context.code();
                if (codes.Length > 0)
                {
                    var v = ExecExpr(context.condition);
                    if (v.type == AtomType.BOOL && (bool)v.value)
                    {
                        for (int i = 0; i < codes.Length; i++) { codes[i].Accept(this); }
                    }
                }
            }
            finally
            {
                PopStack();
            }
            return 0;
        }

        public override int VisitFor([NotNull] TemplateParser.ForContext context)
        {
            PushStack();

            try
            {
                var codes = context.code();
                if (context.expr0 != null) { ExecVar(context.expr0); }
                else if (context.expr1 != null) { ExecExpr(context.expr1); }

                while (true)
                {
                    for (int i = 0; i < codes.Length; i++) { codes[i].Accept(this); }

                    if (context.expr3 != null) { ExecExpr(context.expr3); }
                    if (context.expr2 != null)
                    {
                        var v = ExecExpr(context.expr2);
                        if (v.type == AtomType.BOOL && (bool)v.value == false) { break; }
                    }
                }
            }
            finally
            {
                PopStack();
            }

            return 0;
        }

        private void ExecVar([NotNull] TemplateParser.VarContext context)
        {
            var varName = context.key.Text;
            if (!string.IsNullOrEmpty(varName))
            {
                if (context.value != null)
                {
                    Set(varName, ExecExpr(context.value));
                }
            }
        }

        private Atom ExecExpr([NotNull] TemplateParser.ExprContext context)
        {
            if(context.v!=null)
            {
                return GetValue(context.v);
            }
            if (context.op != null)
            {
                switch (context.op.Type)
                {
                    case TemplateLexer.PARENTHESES_L: return ExecExpr(context.r);
                    case TemplateLexer.INCREMENT: return Increment(context);
                    case TemplateLexer.DECREMENT: return Decrement(context);
                    case TemplateLexer.ADD: return Add(context.l, context.r);
                    case TemplateLexer.SUB: return Sub(context.l, context.r);
                    case TemplateLexer.MUL: return Mul(context.l, context.r);
                    case TemplateLexer.DIV: return Div(context.l, context.r);
                    case TemplateLexer.MOD: return Mod(context.l, context.r);
                    case TemplateLexer.LESS: return CompareLess(context.l, context.r);
                    case TemplateLexer.LESSEQUAL: return CompareLessEqual(context.l, context.r);
                    case TemplateLexer.GREATER: return CompareGreater(context.l, context.r);
                    case TemplateLexer.GREATEREQUAL: return CompareGreaterEqual(context.l, context.r);
                    case TemplateLexer.EQUAL: return CompareEqual(context.l, context.r);
                    case TemplateLexer.NOTEQUAL: return CompareNotEqual(context.l, context.r);
                    case TemplateLexer.LOGIC_AND: return LogicAnd(context.l, context.r);
                    case TemplateLexer.LOGIC_OR: return LogicOr(context.l, context.r);
                    case TemplateLexer.LOGIC_NOT: return LogicNot(context.r);
                }
            }
            if (context.prop != null)
            {
                return GetRefValue(context.prop);
            }
            return Atom.NULL;
        }

        private Atom GetRefValue(TemplateParser.ExprPropContext prop)
        {
            var names = prop.IDENT();
            if(names.Length==1)
            {
                return Get(names[0].GetText());
            }
            return Atom.NULL;
        }
        private Atom SetRefValue(TemplateParser.ExprPropContext prop, Atom value)
        {
            var names = prop.IDENT();
            if(names.Length==1)
            {
                Set(names[0].GetText(), value);
            }

            return value;
        }

        #region 自增自减
        private Atom Increment(TemplateParser.ExprContext right)
        {
            var oldValue = GetRefValue(right.varName);
            if (oldValue.type == AtomType.INT)   { return SetRefValue(right.varName, new Atom(oldValue.type, (long)oldValue.value + 1)); }
            if (oldValue.type == AtomType.FLOAT) { return SetRefValue(right.varName, new Atom(oldValue.type, (float)oldValue.value + 1)); }

            return Atom.NULL;
        }
        private Atom Decrement(TemplateParser.ExprContext right)
        {
            var oldValue = GetRefValue(right.varName);
            if (oldValue.type == AtomType.INT) { return SetRefValue(right.varName, new Atom(oldValue.type, (long)oldValue.value - 1)); }
            if (oldValue.type == AtomType.FLOAT) { return SetRefValue(right.varName, new Atom(oldValue.type, (float)oldValue.value - 1)); }

            return Atom.NULL;
        }
        #endregion

        #region 逻辑运算
        private Atom LogicAnd(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? GetValue(left.v) : ExecExpr(left);
            var r = right.v != null ? GetValue(right.v) : ExecExpr(right);
            
            if (l.type == AtomType.BOOL && r.type == AtomType.BOOL) { return new Atom(AtomType.BOOL, (bool)l.value && (bool)r.value); }

            return Atom.FALSE;
        }
        private Atom LogicOr(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? GetValue(left.v) : ExecExpr(left);
            var r = right.v != null ? GetValue(right.v) : ExecExpr(right);

            if (l.type == AtomType.BOOL && r.type == AtomType.BOOL) { return new Atom(AtomType.BOOL, (bool)l.value || (bool)r.value); }

            return Atom.FALSE;
        }
        private Atom LogicNot(TemplateParser.ExprContext right)
        {
            var r = right.v != null ? GetValue(right.v) : ExecExpr(right);

            if (r.type == AtomType.BOOL) { return new Atom(AtomType.BOOL, !(bool)r.value); }

            return Atom.FALSE;
        }
        #endregion

        #region 比较运算
        private Atom CompareLess(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? GetValue(left.v) : ExecExpr(left);
            var r = right.v != null ? GetValue(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.BOOL, (long)l.value < (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (float)l.value < (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (long)l.value < (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.BOOL, (float)l.value < (long)r.value); }

            return Atom.FALSE;
        }
        private Atom CompareLessEqual(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? GetValue(left.v) : ExecExpr(left);
            var r = right.v != null ? GetValue(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.BOOL, (long)l.value <= (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (float)l.value <= (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (long)l.value <= (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.BOOL, (float)l.value <= (long)r.value); }

            return Atom.FALSE;
        }
        private Atom CompareGreater(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? GetValue(left.v) : ExecExpr(left);
            var r = right.v != null ? GetValue(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.BOOL, (long)l.value > (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (float)l.value > (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (long)l.value > (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.BOOL, (float)l.value > (long)r.value); }

            return Atom.FALSE;
        }
        private Atom CompareGreaterEqual(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? GetValue(left.v) : ExecExpr(left);
            var r = right.v != null ? GetValue(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.BOOL, (long)l.value >= (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (float)l.value >= (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (long)l.value >= (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.BOOL, (float)l.value >= (long)r.value); }

            return Atom.FALSE;
        }
        private Atom CompareEqual(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? GetValue(left.v) : ExecExpr(left);
            var r = right.v != null ? GetValue(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.BOOL, (long)l.value == (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (float)l.value == (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (long)l.value == (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.BOOL, (float)l.value == (long)r.value); }

            return Atom.FALSE;
        }
        private Atom CompareNotEqual(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? GetValue(left.v) : ExecExpr(left);
            var r = right.v != null ? GetValue(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.BOOL, (long)l.value != (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (float)l.value != (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (long)l.value != (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.BOOL, (float)l.value != (long)r.value); }

            return Atom.FALSE;
        }
        #endregion

        #region 四则运算
        private Atom Add(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? GetValue(left.v) : ExecExpr(left);
            var r = right.v != null ? GetValue(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.INT, (long)l.value + (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (float)l.value + (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (long)l.value + (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.FLOAT, (float)l.value + (long)r.value); }
            if (l.type != r.type && l.type == AtomType.STRING || r.type == AtomType.STRING) { return new Atom(AtomType.STRING, l.ToString() + r.ToString()); }

            return Atom.NULL;
        }
        private Atom Sub(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? GetValue(left.v) : ExecExpr(left);
            var r = right.v != null ? GetValue(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.INT, (long)l.value - (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (float)l.value - (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (long)l.value - (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.FLOAT, (float)l.value - (long)r.value); }

            return Atom.NULL;
        }
        private Atom Mul(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? GetValue(left.v) : ExecExpr(left);
            var r = right.v != null ? GetValue(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.INT, (long)l.value * (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (float)l.value * (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (long)l.value * (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.FLOAT, (float)l.value * (long)r.value); }

            return Atom.NULL;
        }
        private Atom Div(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? GetValue(left.v) : ExecExpr(left);
            var r = right.v != null ? GetValue(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.INT, (long)l.value / (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (float)l.value / (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (long)l.value / (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.FLOAT, (float)l.value / (long)r.value); }

            return Atom.NULL;
        }
        private Atom Mod(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? GetValue(left.v) : ExecExpr(left);
            var r = right.v != null ? GetValue(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.INT, (long)l.value % (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (float)l.value % (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (long)l.value % (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.FLOAT, (float)l.value % (long)r.value); }

            return Atom.NULL;
        }
        #endregion

        private Atom GetValue(TemplateParser.ExprValueContext value)
        {
            if (value.integerValue != null)
            {
                long t = 0l;
                if (long.TryParse(value.integerValue.Text, out t))
                {
                    return new Atom(AtomType.INT, t);
                }
                else
                {
                    //error
                }
            }
            else if (value.floatValue != null)
            {
                double t = 0;
                if(double.TryParse(value.floatValue.Text,out t))
                {
                    return new Atom(AtomType.FLOAT, t);
                }
                else
                {
                    //error
                }
            }
            else if (value.boolValue != null)
            {
                bool t = false;
                if(bool.TryParse(value.boolValue.Text,out t))
                {
                    return new Atom(AtomType.BOOL, t);
                }
                else
                {
                    //error
                }
            }
            else if (value.stringValue != null)
            {
                return new Atom(AtomType.STRING, value.stringValue);
            }
            return new Atom(AtomType.NULL, 0);
        }

        #region 名称域

        private void PushStack()
        {
            stack.Add(new Dictionary<string, Atom>());
        }

        private void PopStack()
        {
            stack.RemoveAt(stack.Count - 1);
        }

        private Atom Get(string name)
        {
            for (int i = stack.Count - 1; i >= 0; i--)
            {
                var dictionary = stack[i];
                if(dictionary.ContainsKey(name))
                {
                    return dictionary[name];
                }
            }
            return Atom.NULL;
        }

        private Atom Set(string name, Atom obj)
        {
            stack[stack.Count - 1][name] = obj;
            return obj;
        }

        #endregion

        enum AtomType { VOID, NULL, BOOL, INT, FLOAT, STRING, OBJECT, REF }

        struct Atom
        {
            public static Atom NULL = new Atom(AtomType.NULL, 0);
            public static Atom VOID = new Atom(AtomType.VOID, 0);
            public static Atom TRUE = new Atom(AtomType.BOOL, true);
            public static Atom FALSE = new Atom(AtomType.BOOL, false);

            public object value;
            public AtomType type;

            public Atom(AtomType type,object value)
            {
                this.type = type;
                this.value = value;
            }

            public override string ToString()
            {
                if(type== AtomType.NULL)
                {
                    return "null";
                }
                else if(type== AtomType.VOID)
                {
                    return "void";
                }
                return value.ToString();
            }
        }

    }
}
