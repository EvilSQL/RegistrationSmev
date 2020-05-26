using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationSMEV
{
    class SendRequest
    {

        public static int reqcount = 0;

        // Запрос о соответствии фамильно-именной группы, даты рождения, пола и СНИЛС
        public static async Task<string> VS00113v001_PFR001(string familyName, string firstName, string patronymic, string snils, string birthDate)
        {
            using (var httpclient = new HttpClient())
            {
                // Маска снилс 000-000-000 00
                httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Program.APIKey);
                httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var httpresp = await httpclient.PostAsync(Program.HTTPHost + "api/smev/3/requests/", new StringContent("{'requestName':'VS00113v001-PFR001','requestData':{'familyName':'" + familyName + "','firstName':'" + firstName + "','patronymic':'" + patronymic + "','snils':'" + snils + "', 'birthDate':'" + birthDate + "'},'env':'2'}", Encoding.UTF8, "application/json"));
                var requestcontent = await httpresp.Content.ReadAsStringAsync();

                return requestcontent;
            }
        }

        // (ФИЗ) Запрос выписки из ЕГРИП по запросам органов государственной власти
        public static async Task<string> VS00050v003_FNS001(string value)
        {
            using (var httpclient = new HttpClient())
            {
                // 00
                httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Program.APIKey);
                httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var httpresp = await httpclient.PostAsync(Program.HTTPHost + "api/smev/3/requests/", new StringContent("{'requestName':'VS00050v003-FNS001','requestData':{'value':'" + value + "'},'env':'2'}", Encoding.UTF8, "application/json"));
                var requestcontent = await httpresp.Content.ReadAsStringAsync();

                return requestcontent;
            }
        }

        // РЕГИСТРАЦИЯ
        public static async Task<string> certReg(string serialNumber_, string issuerOrgName_, string startDate_, string expiryDate_, string ownerType_, string snils_, string lastName_, string firstName_, string middleName_, string gender_, string birthDate_, string birthPlace_, string type_, string series_, string number_, string issueId_, string issueDate_, string citizenship_, string ogrn_, string orgINN_)
        {
            string innSTR = string.Empty;

            Function.ConsoleWrite("-= USER :"+ firstName_ + " " + lastName_ + " " + middleName_ + " ПОЛ:" + gender_ + " (" + ownerType_ + ")", 3, 1);
            Function.ConsoleWrite("-= СНИЛС: " + snils_, 4, 1);

            if (ownerType_ == "ORG")
            {
                Function.ConsoleWrite("-= ОГРН: " + ogrn_, 5, 1);
                innSTR = "orgINN";
            }
            else if (ownerType_ == "IB")
            {
                Function.ConsoleWrite("-= ОГРНИП: " + ogrn_, 5, 1);
                innSTR = "personINN";
            }
            else
            {
                Function.ConsoleWrite("-= ОГРН ФИЗ. : " + ogrn_, 5, 1);
                innSTR = "personINN";
            }

            Function.ConsoleWrite("-= Действует с : " + startDate_ + " по " + expiryDate_, 6, 1);

            Function.ConsoleWrite("-= П.СЕРИЯ: " + series_, 8, 1);
            Function.ConsoleWrite("-= П.НОМЕР: " + number_, 9, 1);
            Function.ConsoleWrite("-= П.КОД. ПОД. : " + issueId_, 10, 1);
            Function.ConsoleWrite("-= П.День рожд. : " + birthDate_, 11, 1);
            Function.ConsoleWrite("-= П.Место рожд. : " + birthPlace_, 12, 1);
            Function.ConsoleWrite("-= П.Дата выдачи: " + issueDate_, 13, 1);

            Function.ConsoleWrite("-= Регистрация: " + reqcount + @"\" + db.list.Count, 17, 1);

            //Function.ConsoleWrite("-= type: " + db.type, 17, 1);
            //Function.ConsoleWrite("-= citizenship: " + db.citizenship, 18, 1);

            //Console.WriteLine();

            if (!Properties.Settings.Default.AUTO_REG)
            {
                Function.ConsoleWrite("-= Если информация верна: нажмите ENTER: ", 19, 3);
                Console.ReadLine();
            }
            
            using (var httpclient = new HttpClient())
            {
                reqcount++;

                string StrReg = "{'serialNumber':'" + serialNumber_ + "', 'issuerOrgName':'" + issuerOrgName_ + "', 'startDate':'" + startDate_ + "', 'expiryDate':'" + expiryDate_ + "', 'ownerType':'" + ownerType_ + "', 'snils':'" + snils_ + "',  'lastName':'" + lastName_ + "', 'firstName':'" + firstName_ + "', 'middleName':'" + middleName_ + "', 'gender':'" + gender_ + "', 'birthDate':'" + birthDate_ + "', 'birthPlace':'" + birthPlace_ + "', 'doc':{'type':'" + type_ + "', 'series':'" + series_ + "', 'number':'" + number_ + "', 'issueId':'" + issueId_ + "', 'issueDate':'" + issueDate_ + "'}, 'citizenship':'" + citizenship_ + "', 'ogrn':'" + ogrn_ + "', '"+ innSTR +"':'" + orgINN_ + "'}";
                httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Program.APIKey);
                httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var httpresp = await httpclient.PostAsync(Program.HTTPHost + "api/minSvyaz/esia/RegisterCertificate/send  ", new StringContent(StrReg, Encoding.UTF8, "application/json"));
                var requestcontent = await httpresp.Content.ReadAsStringAsync();
                return requestcontent;
            }
        }


        // 745107874291

        // (ЮЛ) Запрос выписки из ЕГРЮЛ по запросам органов государственной власти
        public static async Task<string> VS00051v003_FNS001(string value)
        {
            using (var httpclient = new HttpClient())
            {
                // 00
                httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Program.APIKey);
                httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var httpresp = await httpclient.PostAsync(Program.HTTPHost + "api/smev/3/requests/", new StringContent("{'requestName':'VS00051v003-FNS001','requestData':{'value':'" + value + "'},'env':'2'}", Encoding.UTF8, "application/json"));
                var requestcontent = await httpresp.Content.ReadAsStringAsync();

                return requestcontent;
            }
        }
    }

    class GetRequest
    {
        // Получение информации о статусе и технической информации о запросе.
        public static async Task<string> Request(string requestID)
        {
            using (var httpclient = new HttpClient())
            {
                httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Program.APIKey);
                var httpresp = await httpclient.GetAsync(Program.HTTPHost + "api/smev/3/requests/" + requestID);
                var requestcontent = await httpresp.Content.ReadAsStringAsync();

                if (!httpresp.IsSuccessStatusCode)
                    return null;

                return requestcontent;
            }
        }
    }
}
