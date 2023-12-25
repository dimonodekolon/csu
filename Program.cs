using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Threading.Tasks.Dataflow;



class Program
{
    public static void Main() //ввод
    {
        Console.WriteLine("Введите математическое выражение: ");
        string inputExpression = Console.ReadLine();

        List<object> parsedInputExpression = Parsing(inputExpression);
        List<object> outputList = Conversion(parsedInputExpression);

        string result = Calculate(outputList).ToString();
        Console.Write("Результат:");
        Console.Write(result);
    }

    public static List<object> Parsing(string inputExpression)
    {
        bool lastIsDigit = false;
        string num = "";
        List<object> parsingList = new List<object>();

        foreach (char variable in inputExpression)
        {
            if (variable != ' ')
            {
                if (char.IsDigit(variable))
                {
                    if (num == "") num += variable;
                    else
                    {
                        if (lastIsDigit) num += variable;
                        else
                        {
                            parsingList.Add(int.Parse(num));
                            num = Convert.ToString(variable);
                        }
                    }

                    lastIsDigit = true;
                }

                else
                {
                    parsingList.Add(int.Parse(num));
                    parsingList.Add(variable);
                    lastIsDigit = false;
                    num = "";
                }
            }
        }

        if (num != "") parsingList.Add(int.Parse(num));
        return parsingList;
    }

    public static int Preority(object operation) //приоритезация
    {
        switch (operation)
        {
            case '+' or '-':
                return 1;

            case '/' or '*':
                return 2;

            default:
                return 0;
        }
    }

    public static List<object> Conversion(List<object> parsedInput)
    {
        Stack<object> stack = new Stack<object>();
        List<object> output = new List<object>();

        foreach (object variable in parsedInput)
        {
            if (variable is string || variable is int)
            {
                output.Add(variable);
            }

            else if (variable.Equals('('))
            {
                stack.Push(variable);
            }

            else if (variable.Equals(')') && stack.Count != 0)
            {
                while (stack.Count != 0 && !stack.Peek().Equals('('))
                {
                    object item = stack.Pop();
                    output.Add(item);
                }

                stack.Pop();
            }

            else if (stack.Count == 0 || Preority(stack.Peek()) <= Preority(variable))
            {
                stack.Push(variable);
            }

            else if (Preority(stack.Peek()) > Preority(variable))
            {
                while (stack.Count != 0 && !stack.Peek().Equals('('))
                {
                    output.Add(stack.Pop());
                }

                stack.Push(variable);
            }
        }

        while (stack.Count > 0)
        {
            output.Add(stack.Pop());
        }

        return output;
    }

    public static double CalculateOneExpression(double firstNum, double secondNum, char operation) //вычисление результата одного выражения
    {
        switch (operation)
        {
            case '+':
                return firstNum + secondNum;

            case '-':
                return firstNum - secondNum;

            case '*':
                return firstNum * secondNum;

            case '/':
                return firstNum / secondNum;

            default:
                return 0;
        }
    }

    public static object Calculate(List<object> outputList) //вычисление всех выражений
    {
        for (int i = 0; i < outputList.Count; i++)
        {
            if (outputList[i] is char)
            {
                double fisrtNumber = Convert.ToSingle(outputList[i - 2]);
                double secondNumber = Convert.ToSingle(outputList[i - 1]);

                double result = CalculateOneExpression(fisrtNumber, secondNumber, Convert.ToChar(outputList[i]));

                outputList.RemoveRange(i - 2, 3);
                outputList.Insert(i - 2, result);
                i -= 2;
            }

            outputList.Remove("");
        }

        return outputList[0];
    }
}