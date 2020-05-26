using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationSMEV
{
    class Function
    {
        // Метод очистки консоли
        public static void StrClean(int positionY, int positionX)
        {
            ConsoleWrite("", positionY, positionX);
        }

        public static void ConsoleWrite(string s, int positionX, int positionY)
        {
            try
            {
                Console.SetCursorPosition(positionY, positionX);
                Console.WriteLine(new String(' ', Console.WindowWidth));
                Console.SetCursorPosition(positionY, positionX);
                Console.WriteLine(s);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }

        public static void SerialDbAdd(string folder, string dbname)
        {
            string CertSerial = String.Empty;
            StreamWriter w = File.AppendText(dbname);
            foreach (string str in Directory.GetFiles(folder, "*.cer", SearchOption.AllDirectories))
            {
                X509CertificateParser parser = new X509CertificateParser();
                X509Certificate cert = parser.ReadCertificate(File.ReadAllBytes(str));
                CertSerial = "00" + cert.SerialNumber.ToString(16).ToUpper();
                w.WriteLine(CertSerial);
            }
            w.Close();

            Console.WriteLine("Информация успешно выгружена в файл: " + dbname);
            Console.ReadLine();
        }

        public static void X509DateView(string folder)
        {
            string DateView = String.Empty;
            StreamWriter w = File.AppendText("X509DateView.txt");
            foreach (string str in Directory.GetFiles(folder, "*.cer", SearchOption.AllDirectories))
            {
                X509CertificateParser parser = new X509CertificateParser();
                X509Certificate cert = parser.ReadCertificate(File.ReadAllBytes(str));
                DateView = "00" + cert.SerialNumber.ToString(16).ToUpper() + "Действует с: " + cert.NotAfter.ToString("dd.MM.yyyy") + " до: " +cert.NotBefore.ToString("dd.MM.yyyy");
                w.WriteLine(DateView);
            }
            w.Close();

            Console.WriteLine("Информация успешно выгружена в файл: X509DateView.txt");
            Console.ReadLine();
        }

        public static int CountLinesLINQ(FileInfo file) => File.ReadLines(file.FullName).Count();

        public static void regWrite(string message) // Регистрация
        {
            System.IO.File.AppendAllText(Program.registrationLogFile, message + Environment.NewLine, Encoding.Default);
        }

        public static void dbWrite(string message)
        {
            System.IO.File.AppendAllText(Program.dbFile, message + "\r", Encoding.Default);
        }

        public static void dbWriteSuccess(string message, string filename)
        {
            System.IO.File.AppendAllText(filename, message + Environment.NewLine, Encoding.Default);
        }

        public static void dbWriteNoReg(string message)
        {
            System.IO.File.AppendAllText("SerialDb_NOreg.db", message + Environment.NewLine, Encoding.Default);
        }

        public static string GetResultsWithOutHyphen(string input)
        {
            return input.Replace("-", "");
        }

        public static string GetResultsWithNull(string input)
        {
            return input.Replace(" ", "");
        }


        // Парсинг Inn
        public static string CurrentInn(string inn)
        {
            String[] currentStr = inn.Split(new char[] { '\t' }, StringSplitOptions.None);
            String currentInn = String.Empty;

            if (currentStr[0].Contains("-")) currentStr[0] = GetResultsWithOutHyphen(currentStr[0]); // Убираем дефис в строке INN
            if (currentStr[0].Contains(" ")) currentStr[0] = GetResultsWithNull(currentStr[0]); // Убираем пробелы в строке INN
            if (currentStr[0].Length < 12)
                currentInn += "00" + currentStr[0]; // Добавляем 00 если нету
            else
                currentInn = currentStr[0];

            return currentInn;
        }
    }
}
