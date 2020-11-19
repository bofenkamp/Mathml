using System;
using System.Collections.Generic;
using System.Xml;

namespace MathXML
{
    class MainClass
    {
        enum OperationType { Sum, Subtract, Multiply, Divide, MAX };

        static void Main(string[] args)
        {
            string username = "";
            OperationType operation = OperationType.MAX;
            int val1 = 0;
            int val2;
            string mostRecentElement = "";

            XmlTextReader reader = new XmlTextReader("math.xml");

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        mostRecentElement = reader.Name.ToLower();
                        break;
                    case XmlNodeType.Text:
                        if (mostRecentElement == "description")
                        {
                            (string, OperationType) info = GetUsernameAndOperation(reader.Value);
                            username = info.Item1;
                            operation = info.Item2;
                        }
                        else if (mostRecentElement == "value1")
                        {
                            val1 = GetValue(reader.Value);
                        }
                        else if (mostRecentElement == "value2")
                        {
                            val2 = GetValue(reader.Value);
                            DisplayEquation(username, operation, val1, val2);
                        }
                        break;
                }
            }
        }

        static (string, OperationType) GetUsernameAndOperation(string description)
        {
            string[] items = description.Split(';');
            return (items[0], GetOperationType(items[1]));
        }

        static OperationType GetOperationType(string operationName)
        {
            if (operationName.ToLower().StartsWith("sum"))
                return OperationType.Sum;
            else if (operationName.ToLower().StartsWith("sub"))
                return OperationType.Subtract;
            else if (operationName.ToLower().StartsWith("mul"))
                return OperationType.Multiply;
            else if (operationName.ToLower().StartsWith("div"))
                return OperationType.Divide;
            else
                return OperationType.MAX;
        }

        static int GetValue(string stringval)
        {
            int val;

            try
            {
                val = int.Parse(stringval);
            }
            catch (FormatException e) //value presented as words, not numerals
            {
                val = GetValueFromWords(stringval.ToLower());
            }

            return val;
        }

        static int GetValueFromWords(string stringVal)
        {
            Dictionary<string, int> wordsToValue = BuildWordsToValueDict();
            string[] words = stringVal.ToLower().Split(new char[] { ' ', '-' });
            int result = 0;
            for (int i = words.Length - 1; i >= 0; i--)
            {
                int val = wordsToValue[words[i]];
                if (val == 100) //hundreds are affected by the value preceding it
                {
                    i--;
                    val *= wordsToValue[words[i]];
                }
                else if (val > 100) //thousands and up are affected by many preceeding values
                {
                    i--;
                    int multiplier = 0; //number of thousands, millions, billions, etc.
                    while (i >= 0 && wordsToValue[words[i]] < 1000)
                    {
                        if (wordsToValue[words[i]] == 100)
                        {
                            i--;
                            multiplier += 100 * wordsToValue[words[i]];
                            break;
                        }
                        multiplier += wordsToValue[words[i]];
                        i--;
                    }
                    val *= multiplier;
                }
                result += val;
            }
            return result;
        }

        static Dictionary<string, int> BuildWordsToValueDict()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            d["and"] = 0;
            d["one"] = 1;
            d["two"] = 2;
            d["three"] = 3;
            d["four"] = 4;
            d["five"] = 5;
            d["six"] = 6;
            d["seven"] = 7;
            d["eight"] = 8;
            d["nine"] = 9;
            d["ten"] = 10;
            d["eleven"] = 11;
            d["twelve"] = 12;
            d["thirteen"] = 13;
            d["fourteen"] = 14;
            d["fifteen"] = 15;
            d["sixteen"] = 16;
            d["seventeen"] = 17;
            d["eighteen"] = 18;
            d["nineteen"] = 19;
            d["twenty"] = 20;
            d["thirty"] = 30;
            d["forty"] = 40;
            d["fifty"] = 50;
            d["sixty"] = 60;
            d["seventy"] = 70;
            d["eighty"] = 80;
            d["ninety"] = 90;
            d["hundred"] = 100;
            d["thousand"] = 1000;
            d["million"] = 1000000;
            d["billion"] = 1000000000;
            return d;
        }

        static void DisplayEquation(string username, OperationType operation, int val1, int val2)
        {
            Dictionary<OperationType, string> operationSymbols = BuildOperationSymbolsDict();
            float solution = GetSolution(operation, val1, val2);
            Console.WriteLine($"{username} - {operation.ToString().ToUpper()} - " +
                $"{val1} {operationSymbols[operation]} {val2} = {solution}");
        }

        static Dictionary<OperationType, string> BuildOperationSymbolsDict()
        {
            Dictionary<OperationType, string> operationSymbols = new Dictionary<OperationType, string>();
            operationSymbols[OperationType.Sum] = "+";
            operationSymbols[OperationType.Subtract] = "-";
            operationSymbols[OperationType.Multiply] = "*";
            operationSymbols[OperationType.Divide] = "/";
            return operationSymbols;
        }

        static float GetSolution(OperationType operation, int val1, int val2)
        {
            switch (operation)
            {
                case OperationType.Sum:
                    return val1 + val2;
                case OperationType.Subtract:
                    return val1 - val2;
                case OperationType.Multiply:
                    return val1 * val2;
                case OperationType.Divide:
                    return (float)val1 / (float)val2;
                default:
                    return 0;
            }
        }
    }
}
