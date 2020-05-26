using System.Collections.Generic;

namespace RegistrationSMEV
{
    // Структура Юзера (Иностранцем вход воспрещен!)
    public struct User
    {
        // Excel
        //public int inn, serial, number, issueId;
        public string issueDate, birthDate, birthPlace, lastName, firstName, middleName, inn, serial, number, issueId;

        // x509Cert
        //public int snils, ogrn, ogrnip, serialNumber;
        public string issuerOrgName, startDate, expiryDate, gender, snils, ogrn, ogrnip, serialNumber, ownerType;

        public User(string inn_, string serial_, string number_, string issueId_, string issueDate_, string lastName_, 
            string firstName_, string middleName_, string birthDate_, string birthPlace_, string snils_, string ogrn_,
            string ogrnip_, string serialNumber_, string issuerOrgName_, string startDate_, string expiryDate_, string gender_, string type_, string citizenship_, string ownerType_)
        {
            // Берем с Таблицы Excel
            inn = inn_;
            serial = serial_;
            number = number_;
            issueId = issueId_;
            issueDate = issueDate_;
            lastName = lastName_;
            firstName = firstName_;
            middleName = middleName_;
            birthDate = birthDate_;
            birthPlace = birthPlace_;
            
            // Берем с открытого ключа x509cert
            snils = snils_;
            ogrn = ogrn_;
            ogrnip = ogrnip_;
            serialNumber = serialNumber_;
            issuerOrgName = issuerOrgName_;
            startDate = startDate_;
            expiryDate = expiryDate_;

            ownerType = ownerType_;
            gender = gender_;
        }
    }

    public struct x509Cert
    {
        public string certInn;
        public string certCn;
        public string certF;
        public string certIo;
        public string certOgrn;
        public string certOgrnIp;
        public string certSnils;
        public string certThumb;
        public string certSerial;
        public string certNotBefore;
        public string certNotAfter;

    }

    public class db
    {
        public static List<User> list = new List<User>();
        public static List<x509Cert> x509Certlist = new List<x509Cert>();

        public static string type = "RF_PASSPORT"; // type: "RF_PASSPORT", 
        public static string citizenship = "RUS"; //citizenship: "RUS", 


        public static void Write(string inn_, string serial_, string number_, string issueId_, string issueDate_, string lastName_,
            string firstName_, string middleName_, string birthDate_, string birthPlace_, string snils_, string ogrn_,
            string ogrnip_, string serialNumber_, string issuerOrgName_, string startDate_, string expiryDate_, string gender_, string ownerType_)
        {
            list.Add(new User(inn_, serial_, number_, issueId_, issueDate_, lastName_, 
            firstName_, middleName_, birthDate_, birthPlace_, snils_, ogrn_,
            ogrnip_, serialNumber_, issuerOrgName_, startDate_, expiryDate_, gender_, type, citizenship, ownerType_));
        }
    }
}
