using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace M7
{
    internal class Program
    {

        abstract class VirtualMas //Базовый класс: хранения данных с защитой от выхода за размеры массива
        {
            protected string[,] Mas;//Двумерный массив Таблица данных
            private int XMax = 0, YMax = 0;//Размер массива
            private string[,] CopyMas;//Временный массив

            public void wmas(int x, int y, string MyDate)
            {
                if (Mas == null)
                {
                    if (XMax <= x) { XMax = x + 1; Mas = new string[XMax, YMax]; }
                    if (YMax <= y) { YMax = y + 1; Mas = new string[XMax, YMax]; }
                }
                else
                {
                    if (XMax <= x || YMax <= y)
                    {
                        CopyMas = new string[XMax, YMax];
                        CopyMas = Mas;
                        int xx = XMax; int yy = YMax;
                        if (XMax <= x) { xx = x + 1; Mas = new string[xx, yy]; }
                        if (YMax <= y) { yy = y + 1; Mas = new string[xx, yy]; }
                        for (int iy = 0; iy < YMax; iy++)
                            for (int ix = 0; ix < XMax; ix++)
                                Mas[ix, iy] = CopyMas[ix, iy];
                        if (XMax <= x) { XMax = x + 1; }
                        if (YMax <= y) { YMax = y + 1; }
                    }
                }
                Mas[x, y] = MyDate;
            }

            public string rmas(int x, int y)
            {
                if (x >= 0 && y >= 0 && x < XMax && y < YMax)
                { return Mas[x, y]; }
                else
                { return ""; }
            }

            public bool optmas(out int x, out int y)
            {
                if (XMax == 0 && YMax == 0)
                { x = 0; y = 0; return false; }
                else
                { x = XMax; y = YMax; return true; }
            }

            //Реализация доп.логики классов

            public abstract void ViewTable();

            public virtual int FindtMmas()
            {
                return 0;
            }

        }

        class FunctionalMas : VirtualMas //Функциональный класс: задает функции над массивами
        {

            private void decode_line(ref int iline, string line)
            {
                string str = "";
                int j = 0;
                if (line != null)
                {
                    for (int i = 0; i < line.Length; i++)
                        if (line.Substring(i, 1) != ",")
                        { str = str + line.Substring(i, 1); }
                        else
                        { wmas(j, iline, str); j++; str = ""; }
                    wmas(j, iline, str);
                }
                iline++;
            }

            public void ReadMasFromFile(string fname) //Чтение файла в массив
            {
                StreamReader sr = new StreamReader(fname);
                int iline = 0;
                string line = sr.ReadLine();
                decode_line(ref iline, line);
                while (line != null)
                {
                    line = sr.ReadLine();
                    decode_line(ref iline, line);
                }
                sr.Close();
            }

            public void WriteMasFromFile(string fname) //Запись массива в файл
            {
                int XM, YM;
                if (optmas(out XM, out YM) == true) //Если таблица не пуста, параллельно читаем ее размерность
                {
                    string sline = "";
                    StreamWriter sw = new StreamWriter(fname);
                    for (int iline = 0; iline < YM; iline++)
                    {
                        sline = "";
                        for (int ipole = 0; ipole < XM; ipole++)
                        {
                            if (ipole == 0)
                            { sline = rmas(ipole, iline); }
                            else
                            { sline = sline + "," + rmas(ipole, iline); }
                        }
                        sw.WriteLine(sline);
                    }
                    sw.WriteLine();
                    sw.Close();
                }
            }

            public override int FindtMmas()
            {
                //Переопределенная функция
                return 1;
            }

            public void SortMas()
            {
                //Функция еще не реалзована: на стадии разработки
            }

            public void SelectMas()
            {
                //Функция еще не реалзована: на стадии разработки
            }

            public override void ViewTable()
            {
                int XM, YM;
                if (optmas(out XM, out YM) == true) //Если таблица не пуста, параллельно читаем ее размерность
                {
                    for (int i1 = 0; i1 < YM; i1++)
                    {
                        for (int i2 = 0; i2 < XM; i2++)
                            Console.Write(rmas(i2, i1) + " , ");
                        Console.WriteLine("");
                    }
                }
            }

        }

        class InterpretatorMas : FunctionalMas //Класс интерпретатор: расширяет функции до уровня текста
        {
            private string lengop = "rus";

            public InterpretatorMas(string leng)
            {
                if (leng == "rus") { lengop = "rus"; } else { lengop = "eng"; }
            }

            public void interpr(string comm)
            {
                if (comm == "Вывксти данные на экран;")
                {
                    ViewTable();
                }
                if (comm == "Модифицировать данные;")
                {
                    wmas(1, 1, "Test Data");
                }
                if (comm.Length > 25)
                    if (comm.Substring(0, 24) == "Читать данные из файла: ")
                    {
                        ReadMasFromFile(comm.Substring(24, comm.Length - 25));
                    }
                if (comm.Length > 25)
                    if (comm.Substring(0, 24) == "Записать данные в файл: ")
                    {
                        WriteMasFromFile(comm.Substring(24, comm.Length - 25));
                    }
            }
        }

        static class Fync //Статический метод - для исполнения списка команд
        {

            public static void qMas(string[] command)
            {
                InterpretatorMas vMas = new InterpretatorMas("rus");
                for (int i = 0; i < command.Length; i++)
                    vMas.interpr(command[i]);
            }
        }

        static void Main(string[] args)
        {
            string[] comm = new string[3];
            comm[0] = "Читать данные: text.txt;";
            comm[1] = "Модифицировать данные;";
            comm[2] = "Вывксти данные на экран;";
            Fync.qMas(comm);
        }
    }
}
