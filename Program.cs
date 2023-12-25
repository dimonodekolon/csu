using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Threading.Tasks.Dataflow;
using System.Collections.Generic;
//using System.Math;


namespace Lab5
{
    internal class Program
    {
        static void Main(string[] args)//ввод
        {
            Console.WriteLine("Введите математическое выражение: ");
            string inputExpression = Console.ReadLine();

            List<Token> ParsedInputExpression = Parse(inputExpression);
            List<Token> RPN = ConvertToRPN(ParsedInputExpression);

            double result = Calculate(RPN);
            Console.Write("Результат: " + result);
        }

        static List<Token> Parse(string inputExpression) //парсим выражение   
        {
            List<Token> result = new List<Token>();
            string number = "";

            foreach (char i in inputExpression)
            {
                if (i == ' ') continue;

                if (char.IsDigit(i))
                {
                    number += i;
                }

                else if (IsOperator(i))
                {
                    if (!string.IsNullOrEmpty(number))
                    {
                        Number num = new Number() { number = Convert.ToDouble(number) };
                        result.Add(num);
                        number = "";
                    }

                    Operator op = new Operator() { operation = i };
                    result.Add(op);
                }

                else if (i == '(' || i == ')')
                {
                    if (!string.IsNullOrEmpty(number))
                    {
                        Number num = new Number() { number = Convert.ToDouble(number) };
                        result.Add(num);
                        number = "";
                    }

                    Bracket par = new Bracket() { isOpen = (i == '(') };
                    result.Add(par);
                }
            }

            if (!string.IsNullOrEmpty(number))
            {
                Number num = new Number() { number = Convert.ToDouble(number) };
                result.Add(num);
            }

            return result;
        }

        static bool IsOperator(char c)
        {
            return c == '-' || c == '+' || c == '*' || c == '/';
        }

        static void Print(List<Token> ListToPrint) //вывод 
        {
            foreach (Token token in ListToPrint)
            {
                switch (token)
                {
                    case Number num:
                        Console.Write(num.number + " ");
                        break;

                    case Operator op:
                        Console.Write(op.operation + " ");
                        break;

                    case Bracket br:
                        Console.Write(br.isOpen ? "( " : ") ");
                        break;
                }
            }

            Console.Write("\n");
        }

        static Number CalculateOneExpression(Number first, Number second, Operator op) //вычисление выражения
        {
            Number result = new();
            if (op.operation == '+')
            {
                result.number = first.number + second.number;
            }

            if (op.operation == '-')
            {
                result.number = first.number - second.number;
            }

            if (op.operation == '*')
            {
                result.number = first.number * second.number;
            }

            if (op.operation == '/')
            {
                result.number = first.number / second.number;
            }

            return result;
        }

        static int Preority(Token operation) //приоритезируем
        {
            if (operation is Operator)
            {
                switch (((Operator)operation).operation)
                {
                    case '+' or '-':
                        return 1;

                    case '/' or '*':
                        return 2;

                    default:
                        return 0;
                }
            }

            else
            {
                return 0;
            }
        }

        static List<Token> ConvertToRPN(List<Token> userInput)
        {
            Stack<Token> operators = new Stack<Token>();
            List<Token> result = new List<Token>();

            foreach (Token i in userInput)
            {
                if (i is Number)
                {
                    result.Add((Number)i);
                }

                else if (i is Operator)
                {
                    while (operators.Count > 0 && Preority(operators.Peek()) >= Preority(i))
                    {
                        result.Add(operators.Pop());
                    }

                    operators.Push((Operator)i);
                }

                else if (i is Bracket)
                {
                    if (((Bracket)i).isOpen)
                    {
                        operators.Push((Bracket)i);
                    }

                    else
                    {
                        while (operators.Count > 0 && !(operators.Peek() is Bracket))
                        {
                            result.Add(operators.Pop());
                        }

                        operators.Pop();
                    }
                }
            }

            while (operators.Count > 0)
            {
                result.Add(operators.Pop());
            }

            return result;
        }

        static double Calculate(List<Token> outputList) //вычисление всех выражений
        {
            Stack<double> num = new();

            foreach (Token i in outputList)
            {
                if (i is Number number)
                {
                    num.Push(number.number);
                }

                else if (i is Operator)
                {
                    double first = num.Pop();
                    double second = num.Pop();

                    Number firstNum = new();
                    firstNum.number = first;

                    Number secondNum = new();
                    secondNum.number = second;

                    double result = (CalculateOneExpression(firstNum, secondNum, (Operator)i)).number;

                    num.Push(result);
                }
            }

            double resultNum = num.Pop();
            return resultNum;
        }
    }

    class Token
    {

    }

    class Number : Token
    {
        public double number;
    }

    class Operator : Token
    {
        public char operation;
    }

    class Bracket : Token
    {
        public bool isOpen;
    }
}