using System;
using System.IO;

//C#, .NET 6.0

//Программа работает по принципу конечного автомата с памятью
//Программа реализует составление таблиц лексем и создание свёртки.
//На основании свёртки она восстанавливает код. Реализовал отдельным классом
//Вывод в отдельный файл output.txt
//(Если вам не требуется, то удалите этот класс).

//Код НЕ ПО МОДЕЛЯМ ООП!!!!!

//Надеюсь, данные материалы помогут кому-либо. 
#region DLC_REVERSE
public class ReverseCode
{
    public static string[] ItemsOfConvolution = new string[0];
    public static string code = "";
    private static void Parse(string convolution)
    {
        ItemsOfConvolution = convolution.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }
    private static void doReverse(List<string> I, List<string> C, List<string> R, List<string> K)
    {
        for(int i = 0; i < ItemsOfConvolution.Length; i++)
        {
            string[] item = ItemsOfConvolution[i].Split('_');
            switch(item[0])
            {
                case "I":
                    code += I[int.Parse(item[1]) - 1] + " ";
                    break;
                case "C":
                    code += C[int.Parse(item[1]) - 1] + " ";
                    break;
                case "R":
                    code += R[int.Parse(item[1]) - 1] + " ";
                    break;
                case "K":
                    code += K[int.Parse(item[1]) - 1] + " ";
                    break;
            }
        }
        return;
    }
    public static void CreateFileWithReversedCode(string convolution, 
        List<string> I, List<string> C, List<string> R, List<string> K)
    {
        Parse(convolution);
        doReverse(I,C,R,K);
        File.WriteAllText("output.txt", code);
        GC.Collect();
        return;
    }
}
#endregion
internal class Lexer
{
    //Реализуем поля, которые нам понадобятся

    //Тип лексемы
    static int type = 0;

    //Временные поля, свёртка
    static string code = "";
    static string tempItem = "";
    static string convolution = "";

    //Разделители, ключевые слова, операторы
    static char[] limiters = { ',', '.', '(', ')', '[', ']', ':', ';', '+', '-', '*', '/', '<', '>', '@' };
    static string[] reservedWords = { "program", "var", "real", "integer", "begin", "for", "downto", "do", "begin", "end", "writeln" };

    static List<string> identificators = new List<string>(); //Тип 1. I
    static List<string> constantos = new List<string>(); //Тип 2. C
    static List<string> splitChars = new List<string>();  //Тип 3. R
    static List<string> keywords = new List<string>(); //Тип 4. K

    //Поиск лексемы
    static void FindLexem(char thisChar, int i)
    {
        if (((thisChar >= 'A') && (thisChar <= 'Z')) || ((thisChar >= 'a') && (thisChar <= 'z')) || (thisChar == '_'))
        {
            if (tempItem == "")
                type = 1;
            tempItem += thisChar;
            return;
        }
        if (((thisChar >= '0') && (thisChar <= '9')) || (thisChar == '.'))
        {
            if (thisChar == '.')
            {
                int out_r;
                if (!int.TryParse(tempItem, out out_r))
                {
                    if ((thisChar == ' ' || thisChar == '\n') && tempItem != "")
                    {
                        Result(tempItem);
                        tempItem = "";
                        return;
                    }
                    foreach (char c in limiters)
                    {
                        if (thisChar == c)
                        {
                            if (tempItem != "")
                                Result(tempItem);
                            type = 3;
                            if (thisChar == ':' && code[i + 1] == '=')
                            {
                                tempItem = thisChar.ToString() + code[i + 1];
                                Result(tempItem);
                                tempItem = "";
                                return;
                            }
                            if (thisChar == '<' && (code[i + 1] == '>' || code[i + 1] == '='))
                            {
                                tempItem = thisChar.ToString() + code[i + 1];
                                Result(tempItem);
                                tempItem = "";
                                return;
                            }
                            if (thisChar == '>' && code[i + 1] == '=')
                            {
                                tempItem = thisChar.ToString() + code[i + 1];
                                Result(tempItem);
                                tempItem = "";
                                return;
                            }
                            tempItem = thisChar.ToString();
                            Result(tempItem);
                            tempItem = "";
                            return;
                        }
                    }
                }
            }
            if (tempItem == "")
                type = 2;
            tempItem += thisChar;
            return;
        }
        if ((thisChar == ' ' || thisChar == '\n') && tempItem != "")
        {
            Result(tempItem);
            tempItem = "";
            return;
        }
        foreach (char c in limiters)
        {
            if (thisChar == c)
            {
                if (tempItem != "")
                    Result(tempItem);
                type = 3;
                if (thisChar == ':' && code[i + 1] == '=')
                {
                    tempItem = thisChar.ToString() + code[i + 1];
                    Result(tempItem);
                    tempItem = "";
                    return;
                }
                if (thisChar == '<' && (code[i + 1] == '>' || code[i + 1] == '='))
                {
                    tempItem = thisChar.ToString() + code[i + 1];
                    Result(tempItem);
                    tempItem = "";
                    return;
                }
                if (thisChar == '>' && code[i + 1] == '=')
                {
                    tempItem = thisChar.ToString() + code[i + 1];
                    Result(tempItem);
                    tempItem = "";
                    return;
                }
                tempItem = thisChar.ToString();
                Result(tempItem);
                tempItem = "";
                return;
            }
        }
    }
    //Заносим результат в нужную таблицу
    static void Result(string str)
    {
        for (int j = 0; j < reservedWords.Length; j++)
        {
            if (str == reservedWords[j])
            {
                for (int i = 0; i < keywords.Count; i++)
                {
                    if (str == keywords[i])
                    {
                        convolution += "K_" + Convert.ToString(j + 1) + " ";
                        return;
                    }
                }
                convolution += "K_" + Convert.ToString(keywords.Count + 1) + " ";
                keywords.Add(str);
                return;
            }
        }
        switch (type)
        {
            case 1:
                for (int j = 0; j < identificators.Count; j++)
                {
                    if (str == identificators[j])
                    {
                        convolution += "I_" + Convert.ToString(j + 1) + " ";
                        return;
                    }
                }
                convolution += "I_" + Convert.ToString(identificators.Count + 1) + " ";
                identificators.Add(str);
                break;
            case 2:
                for (int j = 0; j < constantos.Count; j++)
                {
                    if (str == constantos[j])
                    {
                        convolution += "C_" + Convert.ToString(j + 1) + " ";
                        return;
                    }
                }
                convolution += "C_" + Convert.ToString(constantos.Count + 1) + " ";
                constantos.Add(str);
                break;
            case 3:
                for (int j = 0; j < splitChars.Count; j++)
                {
                    if (str == splitChars[j])
                    {
                        convolution += "R_" + Convert.ToString(j + 1) + " ";
                        return;
                    }
                }
                convolution += "R_" + Convert.ToString(splitChars.Count + 1) + " ";
                splitChars.Add(str);
                break;
        }
        return;
    }
    //Выводим таблицу
    static void PrintTables()
    {
        Console.Clear();
        Red(); Console.WriteLine("Identificators:");
        Green(); for (int i = 0; i < identificators.Count; i++)
        {
            Console.WriteLine($"{i + 1}: {identificators[i]}");
        }
        Red(); Console.WriteLine("Consts:");
        Green(); for (int i = 0; i < constantos.Count; i++)
        {
            Console.WriteLine($"{i + 1}: {constantos[i]}");
        }
        Red(); Console.WriteLine("Keywords: ");
        Green(); for (int i = 0; i < keywords.Count; i++)
        {
            Console.WriteLine($"{i + 1}: {keywords[i]}");
        }
        Red(); Console.WriteLine("SplitChars: ");
        Green(); for (int i = 0; i < splitChars.Count; i++)
        {
            Console.WriteLine($"{i + 1}: {splitChars[i]}");
        }
        Cyan(); Console.WriteLine(convolution);
        return;
    }
    static void Main(string[] arg)
    {
        //Читаем в строку весь код. Копирование всего текста невыгодно по памяти,
        //однако позволяет избавиться от различных ошибок при анализе

        Console.WriteLine("Enter the path to file");
        string path = Console.ReadLine();
        try
        {
            code = File.ReadAllText(path);
        }
        catch
        {
            Console.WriteLine("Error Input... Try again");
            Console.WriteLine("Press any key");
            Console.ReadKey();
            return;
        }


        //Собираем весь мусор принудительно, т.к. сразу после этих операций он явно не уйдёт
        GC.Collect();
        //Создаём флаги, что помогут нам отличать комментарии от самой составляющей кода
        bool stringFlag = false;
        //Флаги, если подняты, помогают нам избежать поиска лексем в комментариях
        bool commentMultiString = false; //Комментарий типа {}
        bool commentMonoString = false; //Комментарий типа //
        //С помощью этого счётчика мы избавимся от случая "комментарий в комментарии"
        int countOfComments = 0;
        //Временная переменная, что нужна для вычленения лексемы
        string possibleLexem = "";
        for (int i = 0; i < code.Length; i++)
        {
            if (stringFlag)
            {
                if (code[i] == '\'')
                {
                    possibleLexem += code[i];
                    constantos.Add(possibleLexem);
                    possibleLexem = "";
                    stringFlag = false;
                    continue;
                }
                else
                {
                    possibleLexem += code[i];
                }
            }
            else if (commentMultiString)
            {
                if (countOfComments == 0)
                {
                    commentMultiString = false;
                }
                else if (code[i] == '{')
                {
                    countOfComments++;
                }
                else if (code[i] == '}')
                {
                    countOfComments--;
                }
            }
            else if (commentMonoString)
            {
                if (code[i] == '\n')
                {
                    countOfComments--;
                    commentMonoString = false;
                }
                else continue;
            }
            else
            {
                if (code[i] == '{')
                {
                    countOfComments++;
                    commentMultiString = true;
                }
                else if (code[i] == '/')
                {
                    if (i != code.Length - 1)
                    {
                        if (code[i + 1] == '/')
                        {
                            countOfComments++;
                            commentMonoString = true;
                        }
                    }
                }
                else if (code[i] == '\'')
                {
                    possibleLexem += code[i];
                    stringFlag = true;
                }
                else
                {
                    FindLexem(code[i], i);
                }
            }
        }
        //Если флаги вдруг остались подняты или комментарии не завершены, то выдаём ошибку
        if (code.Length == 0 || countOfComments != 0 || commentMultiString || commentMonoString || stringFlag)
        {
            Console.WriteLine("Code has errors.");
            return;
        }
        //Выводим таблицу
        PrintTables();
        //Выход с задержкой
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Press any key...");
        Console.ReadKey();


        code = "";
        tempItem = "";

        ReverseCode.CreateFileWithReversedCode(convolution,
        identificators, constantos, splitChars, keywords);

        return;
    }
    #region fonts
    static void Red()
    {
        Console.ForegroundColor = ConsoleColor.Red;
    }
    static void Green()
    {
        Console.ForegroundColor = ConsoleColor.Green;
    }
    static void Cyan()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
    }
    #endregion
}