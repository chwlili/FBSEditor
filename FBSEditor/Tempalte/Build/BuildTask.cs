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
        private string path = "";
        private StringBuilder output = new StringBuilder();
        private List<Dictionary<string, Atom>> stack = new List<Dictionary<string, Atom>>();

        public BuildTask(string path)
        {
            this.path = path;
        }

        public string Build()
        { 
            var lexer = new TemplateLexer(new AntlrInputStream(File.ReadAllText(path)));
            var parser = new TemplateParser(new CommonTokenStream(lexer));

            var document = parser.document();
            document.Accept<int>(this);

            var txt = output.ToString();
            Console.WriteLine(txt);
            return txt;
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

        public override int VisitVar([NotNull] TemplateParser.VarContext context)
        {
            var varName = context.key.Text;
            if (!string.IsNullOrEmpty(varName))
            {
                if (context.value != null)
                {
                    Value(varName, ExecExpr(context.value));
                }
            }
            return base.VisitVar(context);
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
                if (context.expr0 != null) { VisitVar(context.expr0); }
                else if (context.expr1 != null) { ExecExpr(context.expr1); }

                while (true)
                {
                    for (int i = 0; i < codes.Length; i++)
                    {
                        codes[i].Accept(this);
                    }

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

        public override int VisitWhile([NotNull] TemplateParser.WhileContext context)
        {
            PushStack();
            try
            {
                while (true)
                {
                    var codes = context.code();
                    var exprValue = ExecExpr(context.condition);
                    if (exprValue.type == AtomType.BOOL && (bool)exprValue.value)
                    {
                        for (int i = 0; i < codes.Length; i++)
                        {
                            codes[i].Accept(this);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            finally
            {
                PopStack();
            }
            return 0;
        }

        public override int VisitExpr([NotNull] TemplateParser.ExprContext context)
        {
            ExecExpr(context);
            return 0;
        }

        private Atom ExecExpr([NotNull] TemplateParser.ExprContext context)
        {
            if (context.v != null) { return ParseLiteral(context.v); }
            if (context.prop != null) { return RefValue(context.prop); }
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
                    case TemplateLexer.SHIFTL:return BitShiftL(context.l, context.r);
                    case TemplateLexer.SHIFTR:return BitShiftR(context.l, context.r);
                    case TemplateLexer.BIT_AND:return BitAnd(context.l, context.r);
                    case TemplateLexer.BIT_OR:return BitOr(context.l, context.r);
                    case TemplateLexer.BIT_XOR:return BitXor(context.l, context.r);
                    case TemplateLexer.BIT_INVERT:return BitInvert(context.r);
                    case TemplateLexer.SET:return Equal(context.l, context.r);
                    case TemplateLexer.SET_ADD:return PlusEqual(context.l, context.r);
                    case TemplateLexer.SET_SUB:return MinusEqual(context.l, context.r);
                    case TemplateLexer.SET_MUL:return MulEqual(context.l, context.r);
                    case TemplateLexer.SET_DIV:return DivEqual(context.l, context.r);
                    case TemplateLexer.SET_MOD:return ModEqual(context.l, context.r);
                    case TemplateLexer.SET_SHIFTL:return ShiftLeftEqual(context.l, context.r);
                    case TemplateLexer.SET_SHIFTR: return ShiftRightEqual(context.l, context.r);
                    case TemplateLexer.SET_BIT_AND:return BitAndEqual(context.l, context.r);
                    case TemplateLexer.SET_BIT_OR: return BitOrEqual(context.l, context.r);
                    case TemplateLexer.SET_BIT_XOR: return BitXorEqual(context.l, context.r);
                }
            }
            return Atom.NULL;
        }

        #region 赋值运算
        private Atom Equal(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            if (left.prop != null)
            {
                return RefValue(left.prop, ExecExpr(right));
            }
            return Atom.NULL;
        }
        private Atom PlusEqual(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            if (left.prop != null)
            {
                var l = RefValue(left.prop);
                var r = ExecExpr(right);
                var v = Atom.NULL;

                if (l.type == r.type && l.type == AtomType.INT) { v = new Atom(AtomType.INT, (long)l.value + (long)r.value); }
                else if (l.type == r.type && l.type == AtomType.FLOAT) { v = new Atom(AtomType.FLOAT, (float)l.value + (float)r.value); }
                else if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { v = new Atom(AtomType.FLOAT, (long)l.value + (float)r.value); }
                else if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { v = new Atom(AtomType.FLOAT, (float)l.value + (long)r.value); }
                else if (l.type != r.type && l.type == AtomType.STRING || r.type == AtomType.STRING) { v = new Atom(AtomType.STRING, l.ToString() + r.ToString()); }
                else { }

                RefValue(left.prop, v);
                return v;
            }
            return Atom.NULL;
        }
        private Atom MinusEqual(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            if (left.prop != null)
            {
                var l = RefValue(left.prop);
                var r = ExecExpr(right);
                var v = Atom.NULL;

                if (l.type == r.type && l.type == AtomType.INT) { v = new Atom(AtomType.INT, (long)l.value - (long)r.value); }
                else if (l.type == r.type && l.type == AtomType.FLOAT) { v = new Atom(AtomType.FLOAT, (float)l.value - (float)r.value); }
                else if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { v = new Atom(AtomType.FLOAT, (long)l.value - (float)r.value); }
                else if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { v = new Atom(AtomType.FLOAT, (float)l.value - (long)r.value); }
                else { }

                RefValue(left.prop, v);
                return v;
            }
            return Atom.NULL;
        }
        private Atom MulEqual(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            if (left.prop != null)
            {
                var l = RefValue(left.prop);
                var r = ExecExpr(right);
                var v = Atom.NULL;

                if (l.type == r.type && l.type == AtomType.INT) { v = new Atom(AtomType.INT, (long)l.value * (long)r.value); }
                else if (l.type == r.type && l.type == AtomType.FLOAT) { v = new Atom(AtomType.FLOAT, (float)l.value * (float)r.value); }
                else if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { v = new Atom(AtomType.FLOAT, (long)l.value * (float)r.value); }
                else if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { v = new Atom(AtomType.FLOAT, (float)l.value * (long)r.value); }
                else { }

                RefValue(left.prop, v);
                return v;
            }
            return Atom.NULL;
        }
        private Atom DivEqual(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            if (left.prop != null)
            {
                var l = RefValue(left.prop);
                var r = ExecExpr(right);
                var v = Atom.NULL;

                if (l.type == r.type && l.type == AtomType.INT) { v = new Atom(AtomType.INT, (long)l.value / (long)r.value); }
                else if (l.type == r.type && l.type == AtomType.FLOAT) { v = new Atom(AtomType.FLOAT, (float)l.value / (float)r.value); }
                else if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { v = new Atom(AtomType.FLOAT, (long)l.value / (float)r.value); }
                else if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { v = new Atom(AtomType.FLOAT, (float)l.value / (long)r.value); }
                else { }

                RefValue(left.prop, v);
                return v;
            }
            return Atom.NULL;
        }
        private Atom ModEqual(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            if (left.prop != null)
            {
                var l = RefValue(left.prop);
                var r = ExecExpr(right);
                var v = Atom.NULL;

                if (l.type == r.type && l.type == AtomType.INT) { v = new Atom(AtomType.INT, (long)l.value % (long)r.value); }
                else if (l.type == r.type && l.type == AtomType.FLOAT) { v = new Atom(AtomType.FLOAT, (float)l.value % (float)r.value); }
                else if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { v = new Atom(AtomType.FLOAT, (long)l.value % (float)r.value); }
                else if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { v = new Atom(AtomType.FLOAT, (float)l.value % (long)r.value); }
                else { }

                RefValue(left.prop, v);
                return v;
            }
            return Atom.NULL;
        }
        private Atom ShiftLeftEqual(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            if (left.prop != null)
            {
                var l = RefValue(left.prop);
                var r = ExecExpr(right);
                var v = Atom.NULL;

                if (l.type == r.type && l.type == AtomType.INT) { v = new Atom(AtomType.INT, (long)l.value << (int)r.value); }
                else { }

                RefValue(left.prop, v);
                return v;
            }
            return Atom.NULL;
        }
        private Atom ShiftRightEqual(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            if (left.prop != null)
            {
                var l = RefValue(left.prop);
                var r = ExecExpr(right);
                var v = Atom.NULL;

                if (l.type == r.type && l.type == AtomType.INT) { v = new Atom(AtomType.INT, (long)l.value >> (int)r.value); }
                else { }

                RefValue(left.prop, v);
                return v;
            }
            return Atom.NULL;
        }
        private Atom BitAndEqual(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            if (left.prop != null)
            {
                var l = RefValue(left.prop);
                var r = ExecExpr(right);
                var v = Atom.NULL;

                if (l.type == r.type && l.type == AtomType.INT) { v = new Atom(AtomType.INT, (long)l.value & (long)r.value); }
                else { }

                RefValue(left.prop, v);
                return v;
            }
            return Atom.NULL;
        }
        private Atom BitOrEqual(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            if (left.prop != null)
            {
                var l = RefValue(left.prop);
                var r = ExecExpr(right);
                var v = Atom.NULL;

                if (l.type == r.type && l.type == AtomType.INT) { v = new Atom(AtomType.INT, (long)l.value | (long)r.value); }
                else { }

                RefValue(left.prop, v);
                return v;
            }
            return Atom.NULL;
        }
        private Atom BitXorEqual(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            if (left.prop != null)
            {
                var l = RefValue(left.prop);
                var r = ExecExpr(right);
                var v = Atom.NULL;

                if (l.type == r.type && l.type == AtomType.INT) { v = new Atom(AtomType.INT, (long)l.value ^ (long)r.value); }
                else { }

                RefValue(left.prop, v);
                return v;
            }
            return Atom.NULL;
        }
        #endregion

        #region 自增自减
        private Atom Increment(TemplateParser.ExprContext right)
        {
            var oldValue = RefValue(right.varName);
            if (oldValue.type == AtomType.INT)   { return RefValue(right.varName, new Atom(oldValue.type, (long)oldValue.value + 1)); }
            if (oldValue.type == AtomType.FLOAT) { return RefValue(right.varName, new Atom(oldValue.type, (float)oldValue.value + 1)); }

            return Atom.NULL;
        }
        private Atom Decrement(TemplateParser.ExprContext right)
        {
            var oldValue = RefValue(right.varName);
            if (oldValue.type == AtomType.INT) { return RefValue(right.varName, new Atom(oldValue.type, (long)oldValue.value - 1)); }
            if (oldValue.type == AtomType.FLOAT) { return RefValue(right.varName, new Atom(oldValue.type, (float)oldValue.value - 1)); }

            return Atom.NULL;
        }
        #endregion

        #region 逻辑运算
        private Atom LogicAnd(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? ParseLiteral(left.v) : ExecExpr(left);
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);
            
            if (l.type == AtomType.BOOL && r.type == AtomType.BOOL) { return new Atom(AtomType.BOOL, (bool)l.value && (bool)r.value); }

            return Atom.FALSE;
        }
        private Atom LogicOr(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? ParseLiteral(left.v) : ExecExpr(left);
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);

            if (l.type == AtomType.BOOL && r.type == AtomType.BOOL) { return new Atom(AtomType.BOOL, (bool)l.value || (bool)r.value); }

            return Atom.FALSE;
        }
        private Atom LogicNot(TemplateParser.ExprContext right)
        {
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);

            if (r.type == AtomType.BOOL) { return new Atom(AtomType.BOOL, !(bool)r.value); }

            return Atom.FALSE;
        }
        #endregion

        #region 比较运算
        private Atom CompareLess(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? ParseLiteral(left.v) : ExecExpr(left);
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.BOOL, (long)l.value < (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (float)l.value < (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (long)l.value < (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.BOOL, (float)l.value < (long)r.value); }

            return Atom.FALSE;
        }
        private Atom CompareLessEqual(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? ParseLiteral(left.v) : ExecExpr(left);
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.BOOL, (long)l.value <= (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (float)l.value <= (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (long)l.value <= (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.BOOL, (float)l.value <= (long)r.value); }

            return Atom.FALSE;
        }
        private Atom CompareGreater(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? ParseLiteral(left.v) : ExecExpr(left);
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.BOOL, (long)l.value > (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (float)l.value > (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (long)l.value > (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.BOOL, (float)l.value > (long)r.value); }

            return Atom.FALSE;
        }
        private Atom CompareGreaterEqual(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? ParseLiteral(left.v) : ExecExpr(left);
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.BOOL, (long)l.value >= (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (float)l.value >= (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (long)l.value >= (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.BOOL, (float)l.value >= (long)r.value); }

            return Atom.FALSE;
        }
        private Atom CompareEqual(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? ParseLiteral(left.v) : ExecExpr(left);
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.BOOL, (long)l.value == (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (float)l.value == (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (long)l.value == (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.BOOL, (float)l.value == (long)r.value); }

            return Atom.FALSE;
        }
        private Atom CompareNotEqual(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? ParseLiteral(left.v) : ExecExpr(left);
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.BOOL, (long)l.value != (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (float)l.value != (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.BOOL, (long)l.value != (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.BOOL, (float)l.value != (long)r.value); }

            return Atom.FALSE;
        }
        #endregion

        #region 位运算
        private Atom BitShiftL(TemplateParser.ExprContext left,TemplateParser.ExprContext right)
        {
            var l = left.v != null ? ParseLiteral(left.v) : ExecExpr(left);
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);
            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.INT, (long)l.value << (int)r.value); }
            return Atom.NULL;
        }
        private Atom BitShiftR(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? ParseLiteral(left.v) : ExecExpr(left);
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);
            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.INT, (long)l.value >> (int)r.value); }
            return Atom.NULL;
        }
        private Atom BitAnd(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? ParseLiteral(left.v) : ExecExpr(left);
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);
            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.INT, (long)l.value & (long)r.value); }
            return Atom.NULL;
        }
        private Atom BitOr(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? ParseLiteral(left.v) : ExecExpr(left);
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);
            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.INT, (long)l.value | (long)r.value); }
            return Atom.NULL;
        }
        private Atom BitXor(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? ParseLiteral(left.v) : ExecExpr(left);
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);
            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.INT, (long)l.value ^ (long)r.value); }
            return Atom.NULL;
        }
        private Atom BitInvert(TemplateParser.ExprContext right)
        {
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);
            if (r.type == AtomType.INT) { return new Atom(AtomType.INT, ~(long)r.value); }
            return Atom.NULL;
        }
        #endregion

        #region 四则运算
        private Atom Add(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? ParseLiteral(left.v) : ExecExpr(left);
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.INT, (long)l.value + (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (float)l.value + (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (long)l.value + (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.FLOAT, (float)l.value + (long)r.value); }
            if (l.type != r.type && l.type == AtomType.STRING || r.type == AtomType.STRING) { return new Atom(AtomType.STRING, l.ToString() + r.ToString()); }

            return Atom.NULL;
        }
        private Atom Sub(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            if (left == null) { return Minus(right); }

            var l = left.v != null ? ParseLiteral(left.v) : ExecExpr(left);
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.INT, (long)l.value - (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (float)l.value - (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (long)l.value - (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.FLOAT, (float)l.value - (long)r.value); }

            return Atom.NULL;
        }
        private Atom Mul(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? ParseLiteral(left.v) : ExecExpr(left);
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.INT, (long)l.value * (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (float)l.value * (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (long)l.value * (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.FLOAT, (float)l.value * (long)r.value); }

            return Atom.NULL;
        }
        private Atom Div(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? ParseLiteral(left.v) : ExecExpr(left);
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.INT, (long)l.value / (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (float)l.value / (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (long)l.value / (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.FLOAT, (float)l.value / (long)r.value); }

            return Atom.NULL;
        }
        private Atom Mod(TemplateParser.ExprContext left, TemplateParser.ExprContext right)
        {
            var l = left.v != null ? ParseLiteral(left.v) : ExecExpr(left);
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);

            if (l.type == r.type && l.type == AtomType.INT) { return new Atom(AtomType.INT, (long)l.value % (long)r.value); }
            if (l.type == r.type && l.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (float)l.value % (float)r.value); }
            if (l.type == AtomType.INT && r.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (long)l.value % (float)r.value); }
            if (l.type == AtomType.FLOAT && r.type == AtomType.INT) { return new Atom(AtomType.FLOAT, (float)l.value % (long)r.value); }

            return Atom.NULL;
        }
        private Atom Minus(TemplateParser.ExprContext right)
        {
            var r = right.v != null ? ParseLiteral(right.v) : ExecExpr(right);

            if (r.type == AtomType.INT) { return new Atom(AtomType.INT, -(long)r.value); }
            if (r.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, -(float)r.value); }

            return Atom.NULL;
        }
        #endregion

        #region 字面量
        private Atom ParseLiteral(TemplateParser.ExprValueContext value)
        {
            if (value.integerValue != null)
            {
                long t = 0L;
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
        #endregion

        #region 作用域

        private void PushStack()
        {
            stack.Add(new Dictionary<string, Atom>());
        }

        private void PopStack()
        {
            stack.RemoveAt(stack.Count - 1);
        }

        private Atom Value(string name)
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

        private Atom Value(string name, Atom obj)
        {
            for (int i = stack.Count - 1; i >= 0; i--)
            {
                var dictionary = stack[i];
                if (dictionary.ContainsKey(name))
                {
                    dictionary[name] = obj;
                    return obj;
                }
            }
            stack[stack.Count - 1][name] = obj;
            return obj;
        }

        private Atom RefValue(TemplateParser.ExprPropContext prop)
        {
            var names = prop.IDENT();
            if (names.Length == 1)
            {
                return Value(names[0].GetText());
            }
            return Atom.NULL;
        }
        private Atom RefValue(TemplateParser.ExprPropContext prop, Atom value)
        {
            var names = prop.IDENT();
            if (names.Length == 1)
            {
                Value(names[0].GetText(), value);
            }

            return value;
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
