using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.Behavioral.VisitorPattern
{
    using ClassicExpressionPrinterType = Dictionary<Type, Action<Expression, StringBuilder>>;

    class Visitor
    {
        public static void Invoke()
        {
            var additionExpression = new AdditionExpression(new DoubleExpression(10), new AdditionExpression(new DoubleExpression(11), new DoubleExpression(12)));
            var printerExpression = new StringBuilder();
            additionExpression.Print_Internal(printerExpression);
            Console.WriteLine("Internal Print : " + printerExpression);
            printerExpression.Clear();
            ClassicExpressionPrinter.Print_ConditionalMethod(additionExpression, printerExpression);
            Console.WriteLine("ConditionalMethod : " + printerExpression);

            var expressionPrinter = new ExpressionPrinter();
            expressionPrinter.Visit(additionExpression);
            expressionPrinter.Clear();
            expressionPrinter.Print(additionExpression);
            Console.WriteLine("Classic Visitor : " + expressionPrinter);


            var expressionCalculator = new ExpressionCalculator();
            expressionCalculator.Visit(additionExpression);
            Console.WriteLine("Classic Visitor : " + expressionPrinter + "=" + expressionCalculator);

            var expressionPrinter1 = new GenericExpressionPrinter();
            expressionPrinter1.Visit(additionExpression);
            Console.WriteLine("Generic Visitor : " + expressionPrinter1);


            var expressionCalculator1 = new GenericExpressionCalculator();
            expressionCalculator1.Visit(additionExpression);
            Console.WriteLine("Generic Visitor : " + expressionPrinter1 + "=" + expressionCalculator1);
        }
    }


    /// <summary>
    /// IExpressionVisitor has one drawback. if any expression is added/removed then respective method must be added/removed from IExpressionVisitor & its concrete impl.
    /// so we can use IVisitor<IVisitable> where Ivisitable will be Expression/Concrete expression
    /// </summary>
    interface IVisitor
    {

    }

    interface IVisitor<IVisitable>
    {
        void Visit(IVisitable visitable);
    }

    interface IExpressionVisitor
    {
        void Visit(Expression expression);
        void Visit(AdditionExpression additionExpression);
        void Visit(DoubleExpression doubleExpression);
    }

    abstract class Expression
    {
        public abstract void Print_Internal(StringBuilder stringBuilder);

        public abstract void Accept(IExpressionVisitor visitor);

        public virtual void Accept(IVisitor visitor)
        {
            if (visitor is IVisitor<Expression> expression)
                expression.Visit(this);
        }
    }

    class DoubleExpression : Expression
    {
        public double Value { get; set; }

        public DoubleExpression(double value)
        {
            Value = value;
        }

        public override void Print_Internal(StringBuilder stringBuilder)
        {
            stringBuilder.Append(Value);
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void Accept(IVisitor visitor)
        {
            if (visitor is IVisitor<DoubleExpression> expression)
                expression.Visit(this);
        }
    }

    class AdditionExpression : Expression
    {
        public Expression Left { get; set; }
        public Expression Right { get; set; }

        public AdditionExpression(Expression left, Expression right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public override void Print_Internal(StringBuilder sb)
        {
            sb.Append("(");
            Left.Print_Internal(sb);
            sb.Append(" + ");
            Right.Print_Internal(sb);
            sb.Append(")");
        }

        public override void Accept(IExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void Accept(IVisitor visitor)
        {
            if (visitor is IVisitor<AdditionExpression> expression)
                expression.Visit(this);
        }
    }

    static class ClassicExpressionPrinter
    {
        static ClassicExpressionPrinterType printerLookup;

        static ClassicExpressionPrinter()
        {
            printerLookup = new ClassicExpressionPrinterType
            {
                [typeof(DoubleExpression)] = (e, sb) =>
                {
                    var doubleExpression = (DoubleExpression)e;
                    sb.Append(doubleExpression.Value);
                },
                [typeof(AdditionExpression)] = (e, sb) =>
                {
                    var additionExpression = (AdditionExpression)e;
                    sb.Append("(");
                    Print_ConditionalMethod(additionExpression.Left, sb);
                    sb.Append(" + ");
                    Print_ConditionalMethod(additionExpression.Right, sb);
                    sb.Append(")");
                },
            };
        }

        public static void Print_LookupMethod(Expression expression, StringBuilder sb)
        {
            printerLookup[expression.GetType()](expression, sb);
        }

        public static void Print_ConditionalMethod(Expression expression, StringBuilder sb)
        {
            switch (expression)
            {
                case DoubleExpression doubleExpression:
                    {
                        sb.Append(doubleExpression.Value);
                        break;
                    }
                case AdditionExpression additionExpression:
                    {
                        sb.Append("(");
                        Print_ConditionalMethod(additionExpression.Left, sb);
                        sb.Append(" + ");
                        Print_ConditionalMethod(additionExpression.Right, sb);
                        sb.Append(")");
                        break;
                    }
                default:
                    break;
            }
        }
    }

    class ExpressionPrinter : IExpressionVisitor
    {
        StringBuilder sb = new StringBuilder();

        #region IExpressionVisitor

        public void Visit(Expression expression)
        {

        }

        public void Visit(AdditionExpression additionExpression)
        {
            sb.Append("(");
            additionExpression.Left.Accept((IExpressionVisitor)this);
            sb.Append(" + ");
            additionExpression.Right.Accept((IExpressionVisitor)this);
            sb.Append(")");
        }

        public void Visit(DoubleExpression doubleExpression)
        {
            sb.Append(doubleExpression.Value);
        }


        //public void Print(Expression expression)
        //{
        //    Print_Impl(expression);
        //}

        //public void Print_Impl(AdditionExpression additionExpression)
        //{
        //}

        public void Print(Expression expression)
        {

        }

        /// <summary>
        /// since dynamic is used if any of Expression is passed which is not part of Print(T) then runtime crash will occur 
        /// and it has performance impact
        /// </summary>
        /// <param name="additionExpression"></param>
        public void Print(AdditionExpression additionExpression)
        {
            sb.Append("(");
            Print((dynamic)additionExpression.Left);
            sb.Append(" + ");
            Print((dynamic)additionExpression.Right);
            sb.Append(")");
        }

        //public void Print(DoubleExpression doubleExpression)
        //{
        //    sb.Append(doubleExpression.Value);
        //}

        #endregion

        public override string ToString()
        {
            return sb.ToString();
        }

        internal void Clear()
        {
            sb = new StringBuilder();
        }
    }

    class ExpressionCalculator : IExpressionVisitor
    {
        double Result;

        #region IExpressionVisitor

        public void Visit(Expression expression)
        {

        }

        public void Visit(AdditionExpression additionExpression)
        {
            additionExpression.Left.Accept((IExpressionVisitor)this);
            var leftResult = Result;
            additionExpression.Right.Accept((IExpressionVisitor)this);
            var rightResult = Result;
            Result = leftResult + rightResult;
        }

        public void Visit(DoubleExpression doubleExpression)
        {
            Result = doubleExpression.Value;
        }

        #endregion

        public override string ToString()
        {
            return Result.ToString();
        }

    }

    class GenericExpressionPrinter : IVisitor
    , IVisitor<Expression>
    , IVisitor<AdditionExpression>
    , IVisitor<DoubleExpression>
    {
        StringBuilder sb = new StringBuilder();

        #region IVisitor

        public void Visit(Expression visitable)
        {

        }

        public void Visit(AdditionExpression visitable)
        {
            sb.Append("(");
            visitable.Left.Accept((IVisitor)this);
            sb.Append(" + ");
            visitable.Right.Accept((IVisitor)this);
            sb.Append(")");
        }

        public void Visit(DoubleExpression visitable)
        {
            sb.Append(visitable.Value);
        }

        #endregion

        public override string ToString()
        {
            return sb.ToString();
        }
    }

    class GenericExpressionCalculator : IVisitor, IVisitor<Expression>, IVisitor<AdditionExpression>, IVisitor<DoubleExpression>
    {
        double Result;

        #region IVisitor

        public void Visit(Expression visitable)
        {

        }

        public void Visit(AdditionExpression visitable)
        {
            visitable.Left.Accept((IVisitor)this);
            var leftResult = Result;
            visitable.Right.Accept((IVisitor)this);
            var rightResult = Result;
            Result = leftResult + rightResult;
        }

        public void Visit(DoubleExpression visitable)
        {
            Result = visitable.Value;
        }

        #endregion

        public override string ToString()
        {
            return Result.ToString();
        }

    }
}
