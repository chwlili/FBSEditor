using Antlr4.Runtime;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = "fe<%5-9*(Nome.a.w(1,2,3)+3)+5%4%>tewf";
            var lexer = new TemplateLexer(new AntlrInputStream(text));
            var parser = new TemplateParser(new CommonTokenStream(lexer));
            var value = parser.document();

            //var alst = MeasureExpr(value.expr()[0]);

            for (int i=0;i<value.ChildCount;i++)
            {
                var child = value.children[i];

                Console.WriteLine(i + ":" + child.GetText());
            }

            Console.WriteLine("press any key ");

            Console.WriteLine(1 << 1);
            Console.WriteLine(2 + 1 << 1);
            Console.Read();
        }
        /*
        private static Atom MeasureExpr(TemplateParser.ExprContext context)
        {
            double a = 20;
            sbyte b = 10;
            var cccc = a + b;
            var dddd = a - b;
            var eeee = a * b;
            var ffff = a / b;
            var gggg = a % b;

            if (context.op != null)
            {
                var op = context.op.Text;
                if ("++".Equals(op))
                {
                    var r = MeasureExpr(context.r);
                    if (r.type <= AtomType.DOUBLE) { r++; }
                }
                else if ("--".Equals(op))
                {
                    var r = MeasureExpr(context.r);
                    if (r.type <= AtomType.DOUBLE) { r--; }
                }

                if ("*".Equals(context.op.Text))
                {
                    var l = MeasureExpr(context.l);
                    var r = MeasureExpr(context.r);
                    var v = l * r;
                    return v;
                }
                else if ("/".Equals(context.op.Text))
                {
                    var l = MeasureExpr(context.l);
                    var r = MeasureExpr(context.r);
                    var v = l / r;
                    return v;
                }
                else if ("+".Equals(context.op.Text))
                {
                    var l = MeasureExpr(context.l);
                    var r = MeasureExpr(context.r);
                    var v = l + r;
                    return v;
                }
                else if ("-".Equals(context.op.Text))
                {
                    var l = MeasureExpr(context.l);
                    var r = MeasureExpr(context.r);
                    var v = l - r;
                    return v;
                }
            }
            else if(context.brace!=null)
            {
                return MeasureExpr(context.brace);
            }
            else if (context.v != null)
            {
                return GetAtom(context.v);
            }
            return new Atom(AtomType.NULL, null);
        }

        private static Atom GetAtom(TemplateParser.ValueContext v)
        {
            if (v.integerValue != null)
            {
                int value = 0;
                if (int.TryParse(v.integerValue.Text, out value))
                {
                    return new Atom(AtomType.INT, value);
                }
            }
            else if (v.floatValue != null)
            {
                double value = 0;
                if (double.TryParse(v.floatValue.Text, out value))
                {
                    return new Atom(AtomType.DOUBLE, value);
                }
            }
            else if (v.boolValue != null)
            {
                return new Atom(AtomType.BOOL, "true".Equals(v.boolValue.Text));
            }
            else if (v.stringValue != null)
            {
                return new Atom(AtomType.STRING, v.stringValue.Text);
            }
            return new Atom(AtomType.NULL, null);
        }

        private struct Atom
        {
            public AtomType type;
            public object value;

            public Atom(AtomType type, object value = null)
            {
                this.type = type;
                this.value = value;
            }

            public bool HasSign()
            {
                return type == AtomType.BYTE || type == AtomType.SHORT || type == AtomType.INT || type == AtomType.LONG;
            }
            public bool IsInteger()
            {
                return type < AtomType.FLOAT;
            }
            public bool IsNumber()
            {
                return type >= AtomType.FLOAT;
            }

            public static Atom operator --(Atom value)
            {
                switch (value.type)
                {
                    case AtomType.SBYTE: value.value = (sbyte)value.value - 1; break;
                    case AtomType.BYTE: value.value = (byte)value.value - 1; break;
                    case AtomType.SHORT: value.value = (short)value.value - 1; break;
                    case AtomType.USHORT: value.value = (ushort)value.value - 1; break;
                    case AtomType.INT: value.value = (int)value.value - 1; break;
                    case AtomType.UINT: value.value = (uint)value.value - 1; break;
                    case AtomType.LONG: value.value = (long)value.value - 1; break;
                    case AtomType.ULONG: value.value = (ulong)value.value - 1; break;
                    case AtomType.FLOAT: value.value = (float)value.value - 1; break;
                    case AtomType.DOUBLE: value.value = (double)value.value - 1; break;
                }
                return value;
            }

            public static Atom operator ++(Atom value)
            {
                switch (value.type)
                {
                    case AtomType.SBYTE: value.value = (sbyte)value.value + 1; break;
                    case AtomType.BYTE: value.value = (byte)value.value + 1; break;
                    case AtomType.SHORT: value.value = (short)value.value + 1; break;
                    case AtomType.USHORT: value.value = (ushort)value.value + 1; break;
                    case AtomType.INT: value.value = (int)value.value + 1; break;
                    case AtomType.UINT: value.value = (uint)value.value + 1; break;
                    case AtomType.LONG: value.value = (long)value.value + 1; break;
                    case AtomType.ULONG: value.value = (ulong)value.value + 1; break;
                    case AtomType.FLOAT: value.value = (float)value.value + 1; break;
                    case AtomType.DOUBLE: value.value = (double)value.value + 1; break;
                }
                return value;
            }

            public static Atom operator *(Atom a, Atom b)
            {
                if (a.type <= AtomType.INT && b.type <= AtomType.INT) { return new Atom(AtomType.INT, (int)a.value * (int)b.value); }
                if (a.type <= AtomType.DOUBLE && b.type <= AtomType.DOUBLE)
                {
                    if (a.type == AtomType.DOUBLE || b.type == AtomType.DOUBLE) { return new Atom(AtomType.DOUBLE, Convert.ToDouble(a.value) * Convert.ToDouble(b.value)); }
                    if (a.type == AtomType.FLOAT  || b.type == AtomType.FLOAT ) { return new Atom(AtomType.FLOAT , (float)a.value  * (float)b.value); }

                    if (a.type == b.type)
                    {
                        if (a.type == AtomType.UINT) { return new Atom(AtomType.UINT, (uint)a.value * (uint)b.value); }
                        if (a.type == AtomType.LONG) { return new Atom(AtomType.LONG, (long)a.value * (long)b.value); }
                        if (a.type == AtomType.ULONG) { return new Atom(AtomType.ULONG, (ulong)a.value * (ulong)b.value); }
                    }
                    else
                    {
                        if (a.type <= AtomType.UINT && b.type <= AtomType.UINT) { return new Atom(AtomType.LONG, (long)a.value * (long)b.value); }
                        if (a.HasSign() == b.HasSign())
                        {

                        }
                    }
                }
                return new Atom(AtomType.NULL);
            }
            public static Atom operator /(Atom a, Atom b)
            {
                if (a.type <= AtomType.INT && b.type <= AtomType.INT) { return new Atom(AtomType.INT, (int)a.value / (int)b.value); }
                if (a.type <= AtomType.DOUBLE && b.type <= AtomType.DOUBLE)
                {
                    if (a.type == AtomType.DOUBLE || b.type == AtomType.DOUBLE) { return new Atom(AtomType.DOUBLE, Convert.ToDouble(a.value) + Convert.ToDouble(b.value)); }
                    if (a.type == AtomType.FLOAT || b.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (float)a.value / (float)b.value); }

                    if (a.type == b.type)
                    {
                        if (a.type == AtomType.UINT) { return new Atom(AtomType.UINT, (uint)a.value / (uint)b.value); }
                        if (a.type == AtomType.LONG) { return new Atom(AtomType.LONG, (long)a.value / (long)b.value); }
                        if (a.type == AtomType.ULONG) { return new Atom(AtomType.ULONG, (ulong)a.value / (ulong)b.value); }
                    }
                    else
                    {
                        if (a.type <= AtomType.UINT && b.type <= AtomType.UINT) { return new Atom(AtomType.LONG, (long)a.value / (long)b.value); }
                        if (a.HasSign() == b.HasSign())
                        {

                        }
                    }
                }
                return new Atom(AtomType.NULL);
            }
            public static Atom operator %(Atom a, Atom b)
            {
                if (a.type <= AtomType.INT && b.type <= AtomType.INT) { return new Atom(AtomType.INT, (int)a.value % (int)b.value); }
                if (a.type <= AtomType.DOUBLE && b.type <= AtomType.DOUBLE)
                {
                    if (a.type == AtomType.DOUBLE || b.type == AtomType.DOUBLE) { return new Atom(AtomType.DOUBLE, Convert.ToDouble(a.value) % Convert.ToDouble(b.value)); }
                    if (a.type == AtomType.FLOAT || b.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (float)a.value % (float)b.value); }

                    if (a.type == b.type)
                    {
                        if (a.type == AtomType.UINT) { return new Atom(AtomType.UINT, (uint)a.value % (uint)b.value); }
                        if (a.type == AtomType.LONG) { return new Atom(AtomType.LONG, (long)a.value % (long)b.value); }
                        if (a.type == AtomType.ULONG) { return new Atom(AtomType.ULONG, (ulong)a.value % (ulong)b.value); }
                    }
                    else
                    {
                        if (a.type <= AtomType.UINT && b.type <= AtomType.UINT) { return new Atom(AtomType.LONG, (long)a.value % (long)b.value); }
                        if (a.HasSign() == b.HasSign())
                        {

                        }
                    }
                }
                return new Atom(AtomType.NULL);
            }

            public static Atom operator +(Atom a, Atom b)
            {
                if (a.type <= AtomType.INT && b.type <= AtomType.INT) { return new Atom(AtomType.INT, (int)a.value + (int)b.value); }
                if (a.type <= AtomType.DOUBLE && b.type <= AtomType.DOUBLE)
                {
                    if (a.type == AtomType.DOUBLE || b.type == AtomType.DOUBLE) { return new Atom(AtomType.DOUBLE, Convert.ToDouble(a.value) + Convert.ToDouble(b.value)); }
                    if (a.type == AtomType.FLOAT || b.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (float)a.value + (float)b.value); }

                    if (a.type == b.type)
                    {
                        if (a.type == AtomType.UINT) { return new Atom(AtomType.UINT, (uint)a.value + (uint)b.value); }
                        if (a.type == AtomType.LONG) { return new Atom(AtomType.LONG, (long)a.value + (long)b.value); }
                        if (a.type == AtomType.ULONG) { return new Atom(AtomType.ULONG, (ulong)a.value + (ulong)b.value); }
                    }
                    else
                    {
                        if (a.type <= AtomType.UINT && b.type <= AtomType.UINT) { return new Atom(AtomType.LONG, (long)a.value + (long)b.value); }
                        if (a.HasSign() == b.HasSign())
                        {

                        }
                    }
                }
                return new Atom(AtomType.NULL);
            }
            public static Atom operator -(Atom a, Atom b)
            {
                if (a.type <= AtomType.INT && b.type <= AtomType.INT) { return new Atom(AtomType.INT, (int)a.value - (int)b.value); }
                if (a.type <= AtomType.DOUBLE && b.type <= AtomType.DOUBLE)
                {
                    if (a.type == AtomType.DOUBLE || b.type == AtomType.DOUBLE) { return new Atom(AtomType.DOUBLE, Convert.ToDouble(a.value) + Convert.ToDouble(b.value)); }
                    if (a.type == AtomType.FLOAT || b.type == AtomType.FLOAT) { return new Atom(AtomType.FLOAT, (float)a.value - (float)b.value); }

                    if (a.type == b.type)
                    {
                        if (a.type == AtomType.UINT) { return new Atom(AtomType.UINT, (uint)a.value - (uint)b.value); }
                        if (a.type == AtomType.LONG) { return new Atom(AtomType.LONG, (long)a.value - (long)b.value); }
                        if (a.type == AtomType.ULONG) { return new Atom(AtomType.ULONG, (ulong)a.value - (ulong)b.value); }
                    }
                    else
                    {
                        if (a.type <= AtomType.UINT && b.type <= AtomType.UINT) { return new Atom(AtomType.LONG, (long)a.value - (long)b.value); }
                        if (a.HasSign() == b.HasSign())
                        {

                        }
                    }
                }
                return new Atom(AtomType.NULL);
            }
        }

        private enum AtomType
        {
            SBYTE,
            BYTE,
            SHORT,
            USHORT,
            INT,
            UINT,
            LONG,
            ULONG,
            FLOAT,
            DOUBLE,
            BOOL,
            STRING,
            NULL
        }*/
    }   
}
