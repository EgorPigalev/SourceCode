using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FindInitialDistribution
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Нахождение первоначального распределения транспортной задачи по методу северо-заподного угла");
                    char otv; // Переменная для диалога с пользователем
                    Console.Clear();
                    fileWork.VvodNameFile();
                    decision.matrxD = fileWork.GetData();
                    if(fileWork.Proverka(decision.matrxD) == true)
                    {
                        Console.ReadKey();
                        break;
                    }
                    double summa; // Переменная для хранения значения целевой функции
                    summa = decision.GetDistribution(ref decision.matrxD);
                    Console.Clear();
                    fileWork.SaveNameFile();
                    Console.WriteLine("Результат распределения занесён в файл " + fileWork.pathEnd);// вывод матрицы с рапределёнными поставками
                    fileWork.Record(decision.matrxD, summa);
                    while (true)
                    {
                        try
                        {
                            Console.Write("\nПовторить программу?\nДа(Y)/Нет(N)\nОтвет: ");
                            otv = Convert.ToChar(Console.ReadLine());
                            break;
                        }
                        catch
                        {
                            Console.WriteLine("Введены некорректные данные!");
                        }
                    }
                    if(!(otv.Equals('Y') || otv.Equals('y') || otv.Equals('н') || otv.Equals('Н')))
                    {
                        break;
                    }
                    else if(!(otv.Equals('N') || otv.Equals('N') || otv.Equals('Т') || otv.Equals('т')))
                    {
                        Console.Clear();
                    }
                    else
                    {
                        Console.Clear();
                    }
                }
            }
            catch
            {
                Console.WriteLine("В работе программы произошла ошибка");
            }
        }
    }
}

internal static class fileWork
{
    public static string pathStart; // Путь к исходному файлу
    public static string pathEnd; // Путь к конечному файлу
    public static void VvodNameFile()
    {
        char otv; // Переменная для диалога
        while (true)
        {
            Console.WriteLine("После нажатия Enter, Вам необходимо указать csv файл где хранятся входные данные");
            Console.ReadKey();
            OpenFileDialog OD = new OpenFileDialog();
            OD.DefaultExt = ".csv";
            OD.Filter = "Text documents (.csv)|*.csv";
            if(OD.ShowDialog() == DialogResult.OK)
            {
                pathStart = OD.FileName;
                break;
            }
            else
            {
                while (true)
                {
                    try
                    {
                        Console.Write("\nЖелаете повторить выбор входных данных?\nДа(Y)/Нет(N)\nОтвет: ");
                        otv = Convert.ToChar(Console.ReadLine());
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("Введены некорректные данные!");
                    }
                }
                if((otv.Equals('N') || otv.Equals('N') || otv.Equals('Т') || otv.Equals('т')))
                {
                    return;
                }
            }
        }
    }
    public static void SaveNameFile()
    {
        char otv; // Переменная для диалога с пользователем
        while (true)
        {
            Console.WriteLine("После нажатия Enter, Вам необходимо указать csv файл куда будет сохранён результат");
            Console.ReadKey();
            SaveFileDialog OD = new SaveFileDialog();
            OD.DefaultExt = ".csv";
            OD.Filter = "Text documents (.csv)|*.csv";
            if(OD.ShowDialog() == DialogResult.OK)
            {
                pathEnd = OD.FileName;
                break;
            }
            else
            {
                while (true)
                {
                    try
                    {
                        Console.Write("\nЖелаете повторить выбор файла?\nДа(Y)/Нет(N)\nОтвет: ");
                        otv = Convert.ToChar(Console.ReadLine());
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("Введены некорректные данные!");
                    }
                }
                if((otv.Equals('N') || otv.Equals('N') || otv.Equals('Т') || otv.Equals('т')))
                {
                    return;
                }
            }
        }
    }
    public static string[,] GetData() // Считывание данных из файла
    {
        var lines = File.ReadAllLines(pathStart);
        string[][] text = new string[lines.Length][];
        for (int i = 0; i < text.Length; i++)
        {
            text[i] = lines[i].Split(';');
        }
        string[,] d = new string[text.Length, text[text.Length - 1].Length];
        int j = 0;
        int k;
        foreach (string[] line in text)
        {
            k = 0;
            foreach (string s in line)
            {
                d[j, k] = s;
                k++;
            }
            j++;
        }
        return d;
    }
    public static bool Proverka(string[,] a)
    {
        for (int i = 0; i < a.GetLength(0); i++)
        {
            for (int j = 0; j < a.GetLength(1); j++)
            {
                if(i == 0 && j == 0) j++;
                try
                {
                    if(Convert.ToDouble(a[i, j]) <= 0)
                    {
                        Console.WriteLine("В таблицы распределений, не могут быть отрицательные значения или 0");
                        return true;
                    }
                }
                catch
                {
                    Console.WriteLine("Во входных данных присутствуют некоректные данные, исправьте их и попробуйте снова");
                    return true;
                }
            }
        }
        return false;
    }
    public static void Record(string[,] a, double summa) // Запись данных в csv файл
    {
        for (int i = 0; i < a.GetLength(0); i++)
        {
            for (int j = 0; j < a.GetLength(1); j++)
            {
                File.AppendAllText(pathEnd, a[i, j] + ";", Encoding.UTF8);
            }
            File.AppendAllText(pathEnd, Environment.NewLine);
        }
        File.AppendAllText(pathEnd, Environment.NewLine);
        File.AppendAllText(pathEnd, "\nF =;" + summa + " у. д. е.;", Encoding.UTF8);
        File.AppendAllText(pathEnd, Environment.NewLine);
        int p = 1; // Переменная хранящая номер записи
        for (int i = 0; i < a.GetLength(0); i++) // Циклы для прохождения по массиву распределения
        {
            for (int j = 0; j < a.GetLength(1); j++)
            {
                if(i == 0 && j == 0) j++;
                string str = a[i, j]; // Переменная для хранения содержимого ячейки
                int k = 0; // Количесво символов / в ячейке
                int index = 0; // Переменная, которая хранит индекс символа |
                for (int e = 0; e < str.Length; e++)
                {
                    if(str[e] == '|')
                    {
                        k++;
                        index = e;
                    }
                }
                if(k != 0) // Если в ячейке есть символ |
                {
                    // Вывод записи кому и с кем нужно заключить договор
                    File.AppendAllText(pathEnd, p + ") " + i + " поставщику требуется заключить договор с " + j + " магазином на поставку " + str.Substring(index + 1) + " единиц продукции\n", Encoding.UTF8);
                    File.AppendAllText(pathEnd, Environment.NewLine);
                    p++;
                }
            }
        }
    }
}
public static class decision
{
    public static string[,] matrxD; //Матрица которая хранит таблицу, где будет хранится распределение
    public static int r = 0;
    public static int c = 0;
    public static int GetDistribution(ref string[,] a) //Выполнения распредления и подсчёта суммы
    {
        int summa = 0; // Переменная для хранения суммы
        for (int i = 1; i < a.GetLength(0); i++) // Цикл по строкам массива
        {
            for (int j = 1; j < a.GetLength(1); j++) // Цикл по столбцам массива
            {
                if(Convert.ToInt32(a[0, j]) != 0) // Проверка, если в ячейке спрос равен 0, то идём дальше
                    if(Convert.ToInt32(a[i, 0]) != 0) // Проверка, если в ячейке предложение равно 0, то переходим на новую строчку
                        if(Convert.ToInt32(a[i, 0]) > Convert.ToInt32(a[0, j])) // Проверка, если предложение больше спроса
                        {
                            summa = summa + Convert.ToInt32(a[i, j]) * Convert.ToInt32(a[0, j]); // К сумме прибавляем произведение цены с поставкой
                            a[i, j] = a[i, j] + " | " + a[0, j]; // В ячеке добавляем символ / и максимальную поставку
                            a[i, 0] = Convert.ToString(Convert.ToInt32(a[i, 0]) - Convert.ToInt32(a[0, j])); // Вычитаем из предложения сделанную поставку
                            a[0, j] = "0"; // Зануляем спрос, так как весь спрос удовлетворён
                        }
                        else if(Convert.ToInt32(a[i, 0]) < Convert.ToInt32(a[0, j]))// Проверка, если предложение меньше спроса
                        {
                            summa = summa + (Convert.ToInt32(a[i, j]) * Convert.ToInt32(a[i, 0])); // К сумме прибавляем произведение цены с поставкой
                            a[i, j] = a[i, j] + " | " + a[i, 0]; // В ячекй добавляем символ / и максимальную поставку
                            a[0, j] = Convert.ToString(Convert.ToInt32(a[0, j]) - Convert.ToInt32(a[i, 0])); // Вычитаем из спроса сделанную поставку
                            a[i, 0] = "0"; // Зануляем предложение, так как у данного поставщика закончилась продукция
                                           // Переходим на новую строку на тот же магазин
                            if(i != i - 1)
                            {
                                i++;
                                j--;
                            }
                        }
                        else
                        {
                            summa = summa + (Convert.ToInt32(a[i, j]) * Convert.ToInt32(a[i, 0])); // К сумме прибавляем произведение цены с поставкой
                            a[i, j] = a[i, j] + " | " + a[i, 0]; // В ячекй добавляем символ / и максимальную поставку
                            a[i, 0] = "0"; // Зануляем предложение, так как у данного поставщика закончилась продукция
                            a[0, j] = "0"; // Зануляем спрос, так как весь спрос удовлетворён
                                           // Переходим на новую строку на тот же магазин
                            if(i != i - 1)
                            {
                                i++;
                                j--;
                            }
                        }
                    else i++;
            }
        }
        return summa;
    }
}

//[TestClass]
//public class UnitTest1
//{
//    [TestMethod]
//    public void a_GetSumma_return_summa() // успешное прохождение теста
//    {
//        string[,] a = new string[4, 4];
//        a[0, 0] = "0";
//        a[0, 1] = "15";
//        a[0, 2] = "5";
//        a[0, 3] = "20";
//        a[1, 0] = "7";
//        a[1, 1] = "46";
//        a[1, 2] = "21";
//        a[1, 3] = "3";
//        a[2, 0] = "18";
//        a[2, 1] = "51";
//        a[2, 2] = "25";
//        a[2, 3] = "34";
//        a[3, 0] = "15";
//        a[3, 1] = "4";
//        a[3, 2] = "25";
//        a[3, 3] = "3";
//        double expected = 1070;
//        double act = Decision.GetDistribution(ref a);
//        Assert.AreEqual(expected, act);
//    }
//}