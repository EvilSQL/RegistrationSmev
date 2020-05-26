using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Text;

namespace RegistrationSMEV
{
    class Program
    {
        public static string APIKey = Properties.Settings.Default.APIKey;
        public static string HTTPHost = Properties.Settings.Default.HTTPHost;

        public static String x509Dir = Properties.Settings.Default.CertDirectory;
        public static String dataFile = Properties.Settings.Default.RegFileName;
        public static String dbFile = Properties.Settings.Default.dbfile;

        public static String certExt = "*.cer";
        public static string registrationLogFile = @DateTime.Today.ToShortDateString() + "_reg.log";

        public const string issuer = "ООО \"Линк-сервис\"";

        public static String dataTimeToShort = "["+ DateTime.Now.ToShortDateString() + ":" + DateTime.Now.ToShortTimeString() + "]";

        public static Dictionary<string, string> failedList = new Dictionary<string, string>();
        public static HashSet<string> failedX509 = new HashSet<string>();
        public static HashSet<string> SuccessRegX509 = new HashSet<string>();
        public static List<string> SerialDb = new List<string>();

        public static string[] mArray = { "ич", "лы" };
        public static string[] fArray = { "зы", "ва", "на" };

        //public static Microsoft.Office.Interop.Excel.Application myExcel;
        //public static Microsoft.Office.Interop.Excel.Workbook myWorkbook;
        //public static Microsoft.Office.Interop.Excel.Worksheet worksheet;

        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);

        static Process GetExcelProcess(Microsoft.Office.Interop.Excel.Application excelApp)
        {
            int id;
            GetWindowThreadProcessId(excelApp.Hwnd, out id);
            return Process.GetProcessById(id);
        }

        static void TerminateExcelProcess(Microsoft.Office.Interop.Excel.Application excelApp)
        {
            var process = GetExcelProcess(excelApp);
            if (process != null)
            {
                process.Kill();
            }
        }

        public static void Main(string[] args)
        {
            String Title = "RegistrationSmev v1.2";

            /* DEBUG  */
            //Function.X509DateView(x509Dir);

            /* DEBUG  */
            if(File.Exists("Serial.txt"))
                Function.SerialDbAdd(x509Dir, "Serial.txt");

            if (!File.Exists(dataFile))
                MessageBox.Show(null, dataFile + " не найден.", " ... ", MessageBoxButtons.OK, MessageBoxIcon.Error);

            Console.Title = Title + " loading .";

            if (File.Exists(dbFile))
            {
                long dbLength = new System.IO.FileInfo(dbFile).Length;

                Function.ConsoleWrite(dataTimeToShort + " Создание резервной копии - ... ", 2 , 2);
                Thread.Sleep(500);
                Console.Title = Title + " loading ..";

                if (!Directory.Exists("backup"))
                    Directory.CreateDirectory("backup");

                if (Directory.GetFiles(Application.StartupPath + @"\backup\", "*.backup").Count() > 0)
                {
                    foreach (string strBackup in Directory.GetFiles(Application.StartupPath + @"\backup\", "*.backup"))
                    {
                        var backupfileInfo = new FileInfo(strBackup);
                        long backupfileLength = backupfileInfo.Length;

                        if (backupfileLength != dbLength && dbLength > backupfileLength)
                        {
                            File.Copy(dbFile, @"backup\" + Guid.NewGuid().ToString().Trim('-', '0') + ".backup");
                            Function.ConsoleWrite(dataTimeToShort + " Создание резервной копии - завершено. ", 2, 2);
                            break;
                        }
                        else if (backupfileLength == dbLength)
                        {
                            Function.ConsoleWrite(dataTimeToShort + " Создание резервной копии - не требуется. ", 2, 2);
                            break;
                        }

                        if (backupfileLength < dbLength && backupfileInfo.LastAccessTime < DateTime.Now.AddDays(-2))
                            File.Delete(strBackup);
                    }
                }
                else
                {
                    File.Copy(dbFile, @"backup\" + Guid.NewGuid().ToString().Trim('-', '0') + ".backup");
                }
            }

            if (Properties.Settings.Default.logFileClean)
            {
                foreach (string strLog in Directory.GetFiles(Application.StartupPath, "*.log"))
                {
                    var LogfileInfo = new FileInfo(strLog);
                    long LogfileLength = LogfileInfo.Length;
                    if (LogfileInfo.LastAccessTime < DateTime.Now.AddDays(-2))
                        File.Delete(strLog);
                }
            }

            Console.Title = Title + " loading ... ";
            Thread.Sleep(1500);
            Function.regWrite("------------------------- START: " + dataTimeToShort + " ------------------------- ");


            /*
            if (!File.Exists(dataFile))
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = @"C:\";
                openFileDialog1.CheckFileExists = true;
                openFileDialog1.CheckPathExists = true;
                openFileDialog1.DefaultExt = "xlsx";
                openFileDialog1.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.ReadOnlyChecked = true;
                openFileDialog1.ShowReadOnly = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string inputFile = openFileDialog1.FileName;

                    try
                    {
                        // Открываем
                        myExcel = new Microsoft.Office.Interop.Excel.Application();
                        myExcel.Workbooks.Open(inputFile, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                        myWorkbook = myExcel.ActiveWorkbook;
                        worksheet = (Microsoft.Office.Interop.Excel.Worksheet)myWorkbook.Worksheets[1];
                    }
                    catch (IOException)
                    {

                    }
                }
                else
                {
                    MessageBox.Show(null, "[-] Ты не выбрал файл! ><", "Ну ты ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                try
                {
                    //Сохраняем
                    myWorkbook.SaveAs(@Application.StartupPath + @"\" + dataFile, Microsoft.Office.Interop.Excel.XlFileFormat.xlTextWindows, Missing.Value, Missing.Value, Missing.Value, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                    myWorkbook.Close(false, Type.Missing, Type.Missing); //закрыть не сохраняя
                    myExcel.Quit(); // вышел из Excel
                }
                catch (IOException)
                {

                }
                finally
                {
                    if (myExcel != null)
                        TerminateExcelProcess(myExcel);
                }
            }
            */

            Console.Title = Title + " loading ....";

            var certList = Directory.GetFiles(@x509Dir, certExt);

            X509certificate.AddCertInDir(x509Dir);
            SerialDb.AddRange(File.ReadAllLines(dbFile, Encoding.Default));

            Console.Title = Title + " total cert loaded: " + db.x509Certlist.Count;
            Console.Clear();

            fileTxt.Read(dataFile);

            Thread.Sleep(500);

            Console.Title = Title;

            Function.ConsoleWrite(dataTimeToShort + " Начинаем регистрацию. ", 2, 2);

            fileTxt.StringList.ToList().ForEach(delegate (String Str)
            {
                String[] currentStr = Str.Split(new char[] { '\t' }, StringSplitOptions.None);
                String currentInn = Function.CurrentInn(Str);

                bool certContainsInn = db.x509Certlist.Any(clientInn => clientInn.certInn == currentInn);
                bool certContainsCn = db.x509Certlist.Any(clientInn => String.Compare(clientInn.certF + " " + clientInn.certIo, currentStr[5].TrimEnd().TrimStart(), true) == 0);

                if (!certContainsInn && !certContainsCn)
                {
                    failedX509.Add(dataTimeToShort + " Нет открытого ключа: " + currentInn + " (" + currentStr[5].TrimEnd().TrimStart() + ")");
                    fileTxt.StringList.Remove(Str);
                }
                else if (certContainsInn && !certContainsCn)
                {
                    failedX509.Add(dataTimeToShort + " CN не совпадает (возможно нет открытого ключа): " + currentInn + " fileCN: (" + currentStr[5].TrimEnd().TrimStart() + ")");
                    fileTxt.StringList.Remove(Str);
                }
                else if (!certContainsInn && certContainsCn)
                {
                    failedX509.Add(dataTimeToShort + " Ошибка в ИНН или такого ИНН нет в списке открытых ключей: " + currentInn + " (" + currentStr[5].TrimEnd().TrimStart() + ")");
                    fileTxt.StringList.Remove(Str);
                }

            });

            int StrCount = fileTxt.StringList.Count();

            foreach (String Str in fileTxt.StringList)
            {
                try
                {
                    String[] currentStr = Str.Split(new char[] { '\t' }, StringSplitOptions.None);
                    String currentInn = Function.CurrentInn(Str);

                    foreach (x509Cert cert in db.x509Certlist)
                    {
                        string KeySerialNumber = cert.certSerial;

                        if (currentInn == cert.certInn)
                        {
                            String gender = String.Empty;
                            String firstname = String.Empty;
                            String lastname = String.Empty;
                            String middlname = String.Empty;
                            String X509fio = String.Empty;

                            if (!String.IsNullOrEmpty(cert.certF))
                                lastname = cert.certF;

                            X509fio = lastname + " " + cert.certIo;
                            String[] strIo = cert.certIo.Split(new Char[] { ' ' });

                            if (strIo.Count() >= 2)
                            {
                                firstname = strIo[0]; // ИМЯ 
                                middlname = strIo[1].ToLower(); // ОТЧЕСТВО

                                if (strIo.Count() > 2)
                                    middlname = strIo[1] + " " + strIo[2];

                                String gr = middlname.Substring(middlname.Length - 2).ToLower();

                                bool resultMale = mArray.Any(n => n == gr);
                                bool resultFamle = fArray.Any(n => n == gr);

                                if (resultMale == true)
                                    gender = "M";
                                if (resultFamle == true)
                                    gender = "F";
                            }
                            else
                                firstname = strIo[0];

                            if (String.IsNullOrWhiteSpace(gender))
                            {
                                if (!failedList.ContainsKey(KeySerialNumber))
                                {
                                    failedList.Add(KeySerialNumber, " (" + currentInn + ") отчество: (" + middlname + ") (!не могу определить пол)");
                                    break;
                                }
                            }

                            long snils = Convert.ToInt64(cert.certSnils);
                            String snilsformat = String.Format("{0:000-###-### ##}", snils);

                            String Serial = currentStr[1];
                            String Number = currentStr[2];
                            String CodP = currentStr[3];

                            if (Serial.Contains(" "))
                                Serial = Function.GetResultsWithNull(Serial);
                            if (Number.Contains(" "))
                                Number = Function.GetResultsWithNull(Number);
                            if (CodP.Contains(" "))
                                CodP = Function.GetResultsWithNull(CodP);
                            if (CodP.Contains("-"))
                                CodP = Function.GetResultsWithOutHyphen(CodP);

                            if (Serial.Length != 4)
                            {
                                if (!failedList.ContainsKey(KeySerialNumber))
                                {
                                    failedList.Add(KeySerialNumber, " (" + currentInn + ") неверное значение (Пас. Серия): " + Serial + ")");
                                    break;
                                }
                            }

                            if (Number.Length != 6)
                            {
                                if (!failedList.ContainsKey(KeySerialNumber))
                                {
                                    failedList.Add(KeySerialNumber, " (" + currentInn + ") неверное значение (Пас. Номер): " + Number + ")");
                                    break;
                                }
                            }

                            if (CodP.Length != 6)
                            {
                                if (!failedList.ContainsKey(KeySerialNumber))
                                {
                                    failedList.Add(KeySerialNumber, " (" + currentInn + ") неверное значение (Пас. Код. Под.): " + CodP + ")");
                                    break;
                                }
                            }

                            if (!String.IsNullOrEmpty(middlname)) // Обрабатываем первую букву в фамилии.
                                middlname = middlname.Substring(0, 1).ToUpper() + middlname.Remove(0, 1);

                            string ownerType = String.Empty;
                            string ogrn = String.Empty;
                            string ogrnIP = String.Empty;

                            if (!string.IsNullOrEmpty(cert.certOgrn))
                                ogrn = cert.certOgrn;
                            else
                                ogrnIP = cert.certOgrnIp;

                            if (String.IsNullOrEmpty(ogrnIP) && !String.IsNullOrEmpty(ogrn))
                                ownerType = "ORG";
                            if (String.IsNullOrEmpty(ogrn) && !String.IsNullOrEmpty(ogrnIP))
                                ownerType = "IB";
                            if (String.IsNullOrEmpty(ogrn) && String.IsNullOrEmpty(ogrnIP))
                                ownerType = "PSO";


                            // Обработка дат \ обработка пустых currentStr[4] currentStr[6] // сравнение дат


                            if (SerialDb.Contains(KeySerialNumber))
                            {
                                SuccessRegX509.Add(KeySerialNumber + " (" + currentInn + " " + cert.certF + " " + cert.certIo + ") - уже зарегистрирован.");
                                KeySerialNumber = null;
                                break;
                            }
                            else
                            {
                                SerialDb.Add(KeySerialNumber);

                                db.Write(currentInn, Serial, Number, CodP, currentStr[4], lastname,firstname, middlname,currentStr[6], 
                                    currentStr[7],snilsformat, ogrn, ogrnIP,KeySerialNumber,issuer,
                                    cert.certNotBefore, cert.certNotAfter, gender, ownerType);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    failedX509.Add(dataTimeToShort + " Ошибка при обработки строки: (" + Str + ") : " + ex.ToString());
                    continue;
                }
            }

            int failedCount = failedList.Count() + failedX509.Count();

            Function.ConsoleWrite("-= Str/Failed: " + StrCount + @"/" + failedCount, 16, 1);

            foreach (User u in db.list)
            {
                String currentOgrn = String.Empty;
                if (String.IsNullOrEmpty(u.ogrnip) && !String.IsNullOrEmpty(u.ogrn))
                    currentOgrn = u.ogrn;
                if (String.IsNullOrEmpty(u.ogrn) && !String.IsNullOrEmpty(u.ogrnip))
                    currentOgrn = u.ogrnip;
                if (String.IsNullOrEmpty(u.ogrn) && String.IsNullOrEmpty(u.ogrnip))
                    currentOgrn = String.Empty;

                if (Properties.Settings.Default.REG_ON)
                {
                    Console.WriteLine(SendRequest.certReg(u.serialNumber, u.issuerOrgName, u.startDate, u.expiryDate, u.ownerType, u.snils, u.lastName, u.firstName, u.middleName, u.gender, u.birthDate, u.birthPlace,
                    db.type, u.serial, u.number, u.issueId,
                    u.issueDate, db.citizenship, currentOgrn, u.inn).Result);

                    Function.dbWrite(u.serialNumber);

                }
                else
                {
                    Console.WriteLine("\n Add: " + u.serialNumber);
                    Function.dbWriteNoReg(u.serialNumber);
                }
            }

            Thread.Sleep(500);
            Console.Clear();

            if (Properties.Settings.Default.REG_ON)
                Console.WriteLine("\n\r Регистрация сертификатов - завершена.");
            else
                Console.WriteLine("\n\r Регистрация сертификатов - отключена.");

            SendRequest.reqcount = 0;

            if (failedList.Count > 0)
                foreach (KeyValuePair<String, String> Str in failedList)
                    Function.regWrite(Str.Key + " " + Str.Value);

            if (failedX509.Count > 0)
                foreach (string Str in failedX509)
                    Function.regWrite(Str);

            if (SuccessRegX509.Count > 0)
                foreach (string Str in SuccessRegX509)
                    Function.dbWriteSuccess(Str, @DateTime.Today.ToShortDateString() + "_successError.log");

            if (failedX509.Count == 0 && failedList.Count == 0)
                Function.regWrite(dataTimeToShort + " Ошибок при регистрации нет.");

            Console.WriteLine("\n\r" + dataTimeToShort + " Работа завершена.");

            Function.regWrite("------------------------- END: " + dataTimeToShort + " ------------------------- ");

            Console.ReadLine();
        }
    }
}
