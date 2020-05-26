using System.IO;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1;

namespace RegistrationSMEV
{
    class X509certificate
    {
        public static void AddCertInDir(string dir)
        {
            X509CertificateParser parser;
            x509Cert certificate;
            X509Certificate cert;
            System.Security.Cryptography.X509Certificates.X509Certificate2 x509_2;

            foreach (string str in Directory.GetFiles(@dir, "*.cer", SearchOption.AllDirectories))
            {
                parser = new X509CertificateParser();
                cert = parser.ReadCertificate(File.ReadAllBytes(str));
                certificate = new x509Cert();
                x509_2 = new System.Security.Cryptography.X509Certificates.X509Certificate2();
                
                byte[] rawData = File.ReadAllBytes(str);
                x509_2.Import(rawData);

                // Разбор поля subject = SEQ of SET of SEQ of {OID/value}
                DerSequence subject = cert.SubjectDN.ToAsn1Object() as DerSequence;
                foreach (Asn1Encodable setItem in subject)
                {
                    DerSet subSet = setItem as DerSet;
                    if (subSet == null)
                        continue;

                    // Первый элемент множества SET - искомая последовательность SEQ of {OID/value}
                    DerSequence subSeq = subSet[0] as DerSequence;
                    foreach (Asn1Encodable subSeqItem in subSeq)
                    {
                        DerObjectIdentifier oid = subSeqItem as DerObjectIdentifier;

                        if (oid == null)
                            continue;

                        string value = subSeq[1].ToString();

                        if (oid.Id.Equals("2.5.4.3"))
                            certificate.certCn = value;
                        if (oid.Id.Equals("1.2.643.3.131.1.1"))
                            certificate.certInn = value;
                        if (oid.Id.Equals("2.5.4.4"))
                            certificate.certF = value;
                        if (oid.Id.Equals("2.5.4.42"))
                            certificate.certIo = value;
                        if (oid.Id.Equals("1.2.643.100.5"))
                            certificate.certOgrnIp = value;
                        if (oid.Id.Equals("1.2.643.100.1"))
                            certificate.certOgrn = value;
                        if (oid.Id.Equals("1.2.643.100.3"))
                            certificate.certSnils = value;

                        certificate.certThumb = x509_2.Thumbprint;
                        certificate.certSerial = x509_2.SerialNumber.ToUpper();
                        certificate.certNotBefore = cert.NotBefore.ToString("dd.MM.yyyy");
                        certificate.certNotAfter = cert.NotAfter.ToString("dd.MM.yyyy");

                        //value.Distinct();
                        value = string.Empty;
                    }
                }

                AddToList(certificate);
            }
        }

        public static void AddToList(x509Cert columnData)
        {
            db.x509Certlist.Add(columnData);
        }
    }
}
