using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Configuration;

namespace ZFitness
{
    class Program
    {
        // Renkli konsol yazdırma fonksiyonları
        private static void WriteColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void WriteHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("+==============================================================+");
            Console.WriteLine($"|                    {title,-40}  |");
            Console.WriteLine("+==============================================================+");
            Console.ResetColor();
        }

        private static void WriteSection(string section)
        {
            int totalWidth = 60; // Toplam satır uzunluğu
            string text = $" {section} ";
            int padding = (totalWidth - text.Length) / 2;
            if (padding < 0) padding = 0;
            string line = new string('=', padding) + text + new string('=', totalWidth - text.Length - padding);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n" + line);
            Console.ResetColor();
        }

        private static void WriteSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK] {message}");
            Console.ResetColor();
        }

        private static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {message}");
            Console.ResetColor();
        }

        private static void WriteInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"[INFO] {message}");
            Console.ResetColor();
        }

        private static void WriteWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"[WARNING] {message}");
            Console.ResetColor();
        }

        public static string dosyayolu = AppDomain.CurrentDomain.BaseDirectory;
        public static string path_log = dosyayolu + "\\log";
        public static string logsFileName = "loglar.txt";
        public static string path_yazilmayan = Path.Combine(path_log, logsFileName);
        public static string path = path_log + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
        public static string loglar = "";
        public static string tab = "        ";
        public static string bosluk = " ";
        public static string yenisatir = Environment.NewLine;

        public static string alttancizgi =
            "-------------------------------------------------------------------------------------";

        public static string sqlbaglanti = "Database=zfitness_fitness_db; Data Source=zfitness.az; Port=3306; User ID=zfitness_fitnes_user; password=r6.a1&WdhFl)a1%4+5S!; Connection Timeout=30; Command Timeout=30;";

        //public static 

        #region TABLOLAR

        // YETKI TABLOSU
        /*
        device_ip
        device_port
        device_door
        clients_id
        clients_name
        card_id
        clients_limit
        expiration_date
        is_active
        is_update
        */

        // HAREKETLER TABLOSU
        // cardno, record_date, door_no, status

        #endregion


        static void Main(string[] args)
        {
            // Konsol encoding'ini UTF-8 yap
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            
            // Config dosyasını yeniden yükle
            ConfigurationManager.RefreshSection("connectionStrings");
            
            if (IsRunning())
            {
                MessageBox.Show("Zaten Çalışıyor");
                return;
            }

            try
            {
                if (!System.IO.Directory.Exists(path_log))
                {
                    System.IO.Directory.CreateDirectory(path_log);
                }
            }
            catch (Exception ex)
            {
                loglar = "YEDEKLER KLASÖRÜ YOK VEYA OLUŞTURULAMADI : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine(tab + loglar);
                File.AppendAllText(path, yenisatir + loglar);
            }


            mika_udp_basla();

            Thread.Sleep(2000);

            Thread myNewThread = new Thread(() => islemyap());
            myNewThread.Start();

            Console.ReadLine();
        }


        private static bool IsRunning()
        {
            Process proc = Process.GetCurrentProcess();
            Process[] procs = Process.GetProcessesByName(proc.ProcessName);
            return procs.Any(p => p.Id != proc.Id && p.MainModule.FileName == proc.MainModule.FileName);
        }

        public static void islemyap()
        {
            // ZFitness Salonu mesajı
            MessageBox.Show("ZFitness Salonuna Hoş Geldiniz!");
            
            // Program başlığı
            WriteHeader("ZFitness Programı v4.70");
            WriteInfo($"Başlatma Zamanı: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            
            // UDP başlatma mesajı
            WriteSuccess($"UDP Dinleme Başlatıldı - Port: {mikaudpport} - Local IP: {mikalocal_ip}");
            
            // Log dosyasına yazma
            loglar = "ZFitness Programı v4.70 " + tab + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            File.AppendAllText(path, yenisatir + yenisatir + loglar);
            
            //loglar = "UDP DİNLEME BAŞLATILDI - Port: " + mikaudpport + " - Local IP: " + mikalocal_ip;
           // File.AppendAllText(path, yenisatir + loglar);
            
            loglar = alttancizgi;
            File.AppendAllText(path, yenisatir + loglar);

            while (true)
            {
                WriteSection("KiŞi GÖNDERiMi BAŞLIYOR");
                loglar = "KiŞi GÖNDERiMi BAŞLIYOR : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                File.AppendAllText(path, yenisatir + loglar);

                mika mika = new mika();


                #region GONDER

                try
                {
                    MySqlConnection baglanti = new MySqlConnection(sqlbaglanti);
                    baglanti.Open();

                    DataTable dt_ipler = new DataTable();
                    string sor1 = "select DISTINCT(device_ip) from device_add_card";
                    using (MySqlDataAdapter cmd = new MySqlDataAdapter(sor1, baglanti))
                    {
                        cmd.Fill(dt_ipler);
                    }

                    if (dt_ipler.Rows.Count > 0)
                    {
                        for (int xz = 0; xz < dt_ipler.Rows.Count; xz++)
                        {
                            if (dt_ipler.Rows[xz]["device_ip"] != null)
                            {
                                if (dt_ipler.Rows[xz]["device_ip"].ToString().TrimEnd() != "")
                                {
                                    string cihazip = dt_ipler.Rows[xz]["device_ip"].ToString().TrimEnd();
                                    string cihazport = "9780";
                                    List<kisiler> dict = new List<kisiler>();

                                    #region GONDER VERILERINI AL

                                    DataTable dt = new DataTable();
                                    string sor = "select * from device_add_card where device_ip = '" +
                                                 cihazip.TrimEnd() + "'";
                                    using (MySqlDataAdapter cmd = new MySqlDataAdapter(sor, baglanti))
                                    {
                                        cmd.Fill(dt);
                                    }

                                    //cihazip = "169.254.1.1"; // dt_ipler.Rows[xz]["device_ip"].ToString().TrimEnd();

                                    if (dt.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dt.Rows.Count; i++)
                                        {
                                            kisiler kisi = new kisiler();

                                            kisi.cihaz_ip = cihazip.TrimEnd();
                                            kisi.cihaz_port = cihazport.TrimEnd();

                                            #region cihaz_port

                                            if (!kisi.devam_Etme)
                                            {
                                                try
                                                {
                                                    if (dt.Rows[i]["device_port"] != null)
                                                    {
                                                        if (dt.Rows[i]["device_port"].ToString().TrimEnd() != "")
                                                        {
                                                            kisi.cihaz_port = dt.Rows[i]["device_port"].ToString()
                                                                .TrimEnd();
                                                            cihazport = dt.Rows[i]["device_port"].ToString().TrimEnd();
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    kisi.devam_Etme = true;
                                                    loglar = "KiŞi ID : " + kisi.kisi_id + " - device_port değeri bozuk: " + ex.Message;
                                                    Console.WriteLine(tab + loglar);
                                                    File.AppendAllText(path, yenisatir + loglar);
                                                }
                                            }

                                            #endregion

                                            #region door_id

                                            if (!kisi.devam_Etme)
                                            {
                                                try
                                                {
                                                    if (dt.Rows[i]["device_door"] != null)
                                                    {
                                                        string deviceDoor = dt.Rows[i]["device_door"].ToString().TrimEnd();
                                                        if (deviceDoor != "")
                                                        {
                                                            kisi.door_id = deviceDoor;
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    kisi.devam_Etme = true;
                                                    loglar = "KiŞi ID : " + kisi.kisi_id + " - device_door değeri bozuk: " + ex.Message;
                                                    Console.WriteLine(tab + loglar);
                                                    File.AppendAllText(path, yenisatir + loglar);
                                                }
                                            }

                                            #endregion

                                            #region kisi_id

                                            if (!kisi.devam_Etme)
                                            {
                                                try
                                                {
                                                    if (dt.Rows[i]["clients_id"] != null)
                                                    {
                                                        string clientId = dt.Rows[i]["clients_id"].ToString().TrimEnd();
                                                        if (clientId != "")
                                                        {
                                                            kisi.kisi_id = clientId;
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    kisi.devam_Etme = true;
                                                    loglar = "KiŞi ID : " + kisi.kisi_id + " - clients_id değeri bozuk: " + ex.Message;
                                                    Console.WriteLine(tab + loglar);
                                                    File.AppendAllText(path, yenisatir + loglar);
                                                }
                                            }

                                            #endregion

                                            #region kisi_adi

                                            if (!kisi.devam_Etme)
                                            {
                                                try
                                                {
                                                    if (dt.Rows[i]["clients_name"] != null)
                                                    {
                                                        string clientName = dt.Rows[i]["clients_name"].ToString().TrimEnd();
                                                        if (clientName != "")
                                                        {
                                                            kisi.kisi_adi = clientName;
                                                        }
                                                        else
                                                        {
                                                            kisi.kisi_adi = "Bilinmeyen";
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    kisi.devam_Etme = true;
                                                    loglar = "KiŞi ID : " + kisi.kisi_id + " - clients_name değeri bozuk: " + ex.Message;
                                                    Console.WriteLine(tab + loglar);
                                                    File.AppendAllText(path, yenisatir + loglar);
                                                }
                                            }

                                            #endregion

                                            #region grup

                                            if (!kisi.devam_Etme)
                                            {
                                                try
                                                {
                                                    if (dt.Rows[i]["device_group"] != null)
                                                    {
                                                        string deviceGroup = dt.Rows[i]["device_group"].ToString().TrimEnd();
                                                        if (deviceGroup != "")
                                                        {
                                                            kisi.grup = deviceGroup;
                                                        }
                                                        else
                                                        {
                                                            kisi.grup = "UYE";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        kisi.grup = "UYE";
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    kisi.devam_Etme = true;
                                                    loglar = "KiŞi ID : " + kisi.kisi_id + " - device_group değeri bozuk: " + ex.Message;
                                                    Console.WriteLine(tab + loglar);
                                                    File.AppendAllText(path, yenisatir + loglar);
                                                }
                                            }

                                            #endregion

                                            #region kart_no

                                            if (!kisi.devam_Etme)
                                            {
                                                try
                                                {
                                                    if (dt.Rows[i]["card_id"] != null)
                                                    {
                                                        if (dt.Rows[i]["card_id"].ToString().TrimEnd() != "")
                                                        {
                                                            kisi.kart_no = dt.Rows[i]["card_id"].ToString().TrimEnd();
                                                        }
                                                    }
                                                }
                                                catch
                                                {
                                                    kisi.devam_Etme = true;
                                                }
                                            }

                                            #endregion

                                            #region limit

                                            if (!kisi.devam_Etme)
                                            {
                                                try
                                                {
                                                    if (dt.Rows[i]["clients_limit"] != null)
                                                    {
                                                        string clientLimit = dt.Rows[i]["clients_limit"].ToString().TrimEnd();
                                                        if (clientLimit != "")
                                                        {
                                                            kisi.limit = clientLimit;
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    kisi.devam_Etme = true;
                                                    loglar = "KiŞi ID : " + kisi.kisi_id + " - clients_limit değeri bozuk: " + ex.Message;
                                                    Console.WriteLine(tab + loglar);
                                                    File.AppendAllText(path, yenisatir + loglar);
                                                }
                                            }

                                            #endregion

                                            #region sontarih

                                            if (!kisi.devam_Etme)
                                            {
                                                try
                                                {
                                                    if (dt.Rows[i]["expiration_date"] != null)
                                                    {
                                                        string expDate = dt.Rows[i]["expiration_date"].ToString().TrimEnd();
                                                        if (expDate != "")
                                                        {
                                                            kisi.sontarih = expDate;
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    kisi.devam_Etme = true;
                                                    loglar = "KiŞi ID : " + kisi.kisi_id + " - expiration_date değeri bozuk: " + ex.Message;
                                                    Console.WriteLine(tab + loglar);
                                                    File.AppendAllText(path, yenisatir + loglar);
                                                }
                                            }

                                            #endregion

                                            #region gondermi

                                            if (!kisi.devam_Etme)
                                            {
                                                try
                                                {
                                                    if (dt.Rows[i]["is_active"] != null)
                                                    {
                                                        string isActive = dt.Rows[i]["is_active"].ToString().TrimEnd();
                                                        if (isActive != "")
                                                        {
                                                            kisi.gondermi = (isActive == "yes" ? true : false);
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    kisi.devam_Etme = true;
                                                    loglar = "KiŞi ID : " + kisi.kisi_id + " - is_active değeri bozuk: " + ex.Message;
                                                    Console.WriteLine(tab + loglar);
                                                    File.AppendAllText(path, yenisatir + loglar);
                                                }
                                            }

                                            #endregion

                                            #region islenmis_verimi

                                            if (!kisi.devam_Etme)
                                            {
                                                try
                                                {
                                                    if (dt.Rows[i]["is_update"] != null)
                                                    {
                                                        if (dt.Rows[i]["is_update"].ToString().TrimEnd() != "")
                                                        {
                                                            kisi.islenmis_verimi =
                                                                (dt.Rows[i]["is_update"].ToString().TrimEnd() == "yes"
                                                                    ? true
                                                                    : false);
                                                        }
                                                    }
                                                }
                                                catch
                                                {
                                                    kisi.devam_Etme = true;
                                                }
                                            }

                                            #endregion

                                            if (cihazip.TrimEnd() != "")
                                            {
                                                if (kisi.kisi_id.TrimEnd() != "")
                                                {
                                                    if (!kisi.devam_Etme)
                                                    {
                                                        if (kisi.cihaz_ip.TrimEnd() != "" &&
                                                            kisi.cihaz_port.TrimEnd() != "" &&
                                                            kisi.door_id.TrimEnd() != "" &&
                                                            kisi.kisi_adi.TrimEnd() != "" &&
                                                            kisi.kart_no.TrimEnd() != "" &&
                                                            kisi.limit.TrimEnd() != "" &&
                                                            kisi.sontarih.TrimEnd() != "" && kisi.grup.TrimEnd() != "")
                                                        {
                                                            if (!kisi.islenmis_verimi)
                                                            {
                                                                dict.Add(kisi);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            loglar = "KiŞi ID : " + kisi.kisi_id.TrimEnd() +
                                                                     " GELEN VERiDE EKSiK DEĞERLER VARDI";
                                                            Console.WriteLine(tab + loglar);
                                                            File.AppendAllText(path, yenisatir + loglar);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        loglar = "KiŞi ID : " + kisi.kisi_id.TrimEnd() +
                                                                 " GELEN VERiDE BOZUK DEĞER VARDI";
                                                        Console.WriteLine(tab + loglar);
                                                        File.AppendAllText(path, yenisatir + loglar);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    if (dict.Count > 0)
                                    {
                                        List<string> mesaj = new List<string>();

                                        mika.kart_gonder_sil(baglanti, dict, cihazip.TrimEnd(), cihazport.TrimEnd(),
                                            ref mesaj);

                                        if (mesaj.Count > 0)
                                        {
                                            for (int msj = 0; msj < mesaj.Count; msj++)
                                            {
                                                loglar = mesaj[msj].TrimEnd();
                                                Console.WriteLine(tab + loglar);
                                                File.AppendAllText(path, yenisatir + loglar);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    loglar = "VERiTABANINDA CiHAZ VERiSi DOLU OLMAYAN VERiYE DENK GELDi";
                                    Console.WriteLine(tab + loglar);
                                    File.AppendAllText(path, yenisatir + loglar);
                                }
                            }
                            else
                            {
                                loglar = "VERiTABANINDA CiHAZ VERiSi DOLU OLMAYAN VERiYE DENK GELDi";
                                Console.WriteLine(tab + loglar);
                                File.AppendAllText(path, yenisatir + loglar);
                            }
                        } // FOR END
                    }
                    else
                    {
                        loglar = "VERİTABANINDA GÖNDERİLECEK KİŞİ VERİSİ BULUNAMADI";
                        Console.WriteLine(tab + loglar);
                        File.AppendAllText(path, yenisatir + loglar);
                    }
                }
                catch (Exception ex)
                {
                    loglar = "Veritabanı bağlantı hatası: " + ex.Message + " - Program cihazlardan veri almaya devam edecek";
                    Console.WriteLine(tab + loglar);
                    File.AppendAllText(path, yenisatir + loglar);
                    // Bağlantı hatası olsa bile program çalışmaya devam etsin
                }

                #endregion


                WriteSection("KiŞi SiLME BAŞLIYOR");
                loglar = "KiŞi SiLME BAŞLIYOR : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                File.AppendAllText(path, yenisatir + loglar);


                #region SIL

                try
                {
                    MySqlConnection baglanti = new MySqlConnection(sqlbaglanti);
                    baglanti.Open();

                    DataTable dt_ipler = new DataTable();
                    string sor1 = "select DISTINCT(device_ip) from device_del_card";
                    using (MySqlDataAdapter cmd = new MySqlDataAdapter(sor1, baglanti))
                    {
                        cmd.Fill(dt_ipler);
                    }

                    if (dt_ipler.Rows.Count > 0)
                    {
                        for (int xz = 0; xz < dt_ipler.Rows.Count; xz++)
                        {
                            if (dt_ipler.Rows[xz]["device_ip"] != null)
                            {
                                if (dt_ipler.Rows[xz]["device_ip"].ToString().TrimEnd() != "")
                                {
                                    string cihazip = dt_ipler.Rows[xz]["device_ip"].ToString().TrimEnd();
                                    string cihazport = "9780";
                                    List<kisiler> dict = new List<kisiler>();

                                    #region SIL VERILERINI AL

                                    DataTable dt = new DataTable();
                                    string sor = "select * from device_del_card where device_ip = '" +
                                                 cihazip.TrimEnd() + "'";
                                    using (MySqlDataAdapter cmd = new MySqlDataAdapter(sor, baglanti))
                                    {
                                        cmd.Fill(dt);
                                    }

                                    //cihazip = "169.254.1.1"; // dt_ipler.Rows[xz]["device_ip"].ToString().TrimEnd();

                                    if (dt.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dt.Rows.Count; i++)
                                        {
                                            kisiler kisi = new kisiler();

                                            kisi.cihaz_ip = cihazip.TrimEnd();
                                            kisi.cihaz_port = cihazport.TrimEnd();

                                            #region kisi_id

                                            if (!kisi.devam_Etme)
                                            {
                                                try
                                                {
                                                    if (dt.Rows[i]["card_id"] != null)
                                                    {
                                                        if (dt.Rows[i]["card_id"].ToString().TrimEnd() != "")
                                                        {
                                                            kisi.kisi_id = dt.Rows[i]["card_id"].ToString().TrimEnd();
                                                        }
                                                    }
                                                }
                                                catch
                                                {
                                                    kisi.devam_Etme = true;
                                                }
                                            }

                                            #endregion

                                            #region kart_no

                                            if (!kisi.devam_Etme)
                                            {
                                                try
                                                {
                                                    if (dt.Rows[i]["card_id"] != null)
                                                    {
                                                        if (dt.Rows[i]["card_id"].ToString().TrimEnd() != "")
                                                        {
                                                            kisi.kart_no = dt.Rows[i]["card_id"].ToString().TrimEnd();
                                                        }
                                                    }
                                                }
                                                catch
                                                {
                                                    kisi.devam_Etme = true;
                                                }
                                            }

                                            #endregion

                                            #region silmi

                                            if (!kisi.devam_Etme)
                                            {
                                                // device_del_card tablosundaki kayıtlar zaten silinecek kayıtlar
                                                kisi.silmi = true;
                                            }

                                            #endregion

                                            #region islenmis_verimi

                                            if (!kisi.devam_Etme)
                                            {
                                                try
                                                {
                                                    if (dt.Rows[i]["is_update"] != null)
                                                    {
                                                        if (dt.Rows[i]["is_update"].ToString().TrimEnd() != "")
                                                        {
                                                            kisi.islenmis_verimi =
                                                                (dt.Rows[i]["is_update"].ToString().TrimEnd() == "yes"
                                                                    ? true
                                                                    : false);
                                                        }
                                                    }
                                                }
                                                catch
                                                {
                                                    kisi.devam_Etme = true;
                                                }
                                            }

                                            #endregion

                                            if (cihazip.TrimEnd() != "")
                                            {
                                                if (kisi.kisi_id.TrimEnd() != "")
                                                {
                                                    if (!kisi.devam_Etme)
                                                    {
                                                        if (kisi.cihaz_ip.TrimEnd() != "" &&
                                                            kisi.cihaz_port.TrimEnd() != "" &&
                                                            kisi.kart_no.TrimEnd() != "" && kisi.kisi_id != "")
                                                        {
                                                            if (!kisi.islenmis_verimi)
                                                            {
                                                                dict.Add(kisi);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            loglar = "KiŞi ID : " + kisi.kisi_id.TrimEnd() +
                                                                     " GELEN VERiDE EKSiK DEĞERLER VARDI";
                                                            Console.WriteLine(tab + loglar);
                                                            File.AppendAllText(path, yenisatir + loglar);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        loglar = "KiŞi ID : " + kisi.kisi_id.TrimEnd() +
                                                                 " GELEN VERiDE BOZUK DEĞER VARDI";
                                                        Console.WriteLine(tab + loglar);
                                                        File.AppendAllText(path, yenisatir + loglar);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    if (dict.Count > 0)
                                    {
                                        List<string> mesaj = new List<string>();

                                        mika.kart_gonder_sil(baglanti, dict, cihazip.TrimEnd(), cihazport.TrimEnd(),
                                            ref mesaj);

                                        if (mesaj.Count > 0)
                                        {
                                            for (int msj = 0; msj < mesaj.Count; msj++)
                                            {
                                                loglar = mesaj[msj].TrimEnd();
                                                Console.WriteLine(tab + loglar);
                                                File.AppendAllText(path, yenisatir + loglar);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    loglar = "VERiTABANINDA CiHAZ VERiSi DOLU OLMAYAN VERiYE DENK GELDi";
                                    Console.WriteLine(tab + loglar);
                                    File.AppendAllText(path, yenisatir + loglar);
                                }
                            }
                            else
                            {
                                loglar = "VERiTABANINDA CiHAZ VERiSi DOLU OLMAYAN VERiYE DENK GELDi";
                                Console.WriteLine(tab + loglar);
                                File.AppendAllText(path, yenisatir + loglar);
                            }
                        } // FOR END
                    }
                    else
                    {
                        loglar = "VERİTABANINDA SİLİNECEK KİŞİ VERİSİ BULUNAMADI";
                        Console.WriteLine(tab + loglar);
                        File.AppendAllText(path, yenisatir + loglar);
                    }
                }
                catch (Exception ex)
                {
                    loglar = "Hata " + ex.Message;
                    WriteError(loglar);
                    File.AppendAllText(path, yenisatir + loglar);
                }

                #endregion


                WriteSection("CiHAZDAN VERi ALMA iŞLEMi BAŞLIYOR");
                loglar = "CiHAZDAN VERi ALMA iŞLEMi BAŞLIYOR : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                File.AppendAllText(path, yenisatir + loglar);


                #region VERILERI CEK

                try
                {
                    DataTable mika_hareket_dt = new DataTable();
                    mika_hareket_dt.Columns.Add("kisi_id", typeof(string));
                    mika_hareket_dt.Columns.Add("tarih", typeof(DateTime));
                    mika_hareket_dt.Columns.Add("cihaz_ip", typeof(string));
                    mika_hareket_dt.Columns.Add("cihaz_port", typeof(string));
                    mika_hareket_dt.Columns.Add("kapi_no", typeof(string));
                    mika_hareket_dt.Columns.Add("kart_no", typeof(string));
                    mika_hareket_dt.Columns.Add("event", typeof(string));

                    MySqlConnection baglanti = new MySqlConnection(sqlbaglanti);
                    baglanti.Open();

                    DataTable dt_ipler = new DataTable();
                    string sor1 = "select DISTINCT(device_ip) from device";
                    using (MySqlDataAdapter cmd = new MySqlDataAdapter(sor1, baglanti))
                    {
                        cmd.Fill(dt_ipler);
                    }

                    if (dt_ipler.Rows.Count > 0)
                    {
                        for (int xz = 0; xz < dt_ipler.Rows.Count; xz++)
                        {
                            if (dt_ipler.Rows[xz]["device_ip"] != null &&
                                dt_ipler.Rows[xz]["device_ip"].ToString().TrimEnd() != "")
                            {
                                string cihazip = dt_ipler.Rows[xz]["device_ip"].ToString().TrimEnd();
                                string cihazport = "9780";

                                int gelenveri = 0;
                                string msj = "";
                                int retryCount = 0;
                                const int maxRetries = 3;
                                
                                // Bağlantı koptuktan sonra tüm kayıtları almak için retry mekanizması
                                while (retryCount < maxRetries)
                                {
                                    mika_hareket_dt = mika.mika_vericek(cihazip.TrimEnd(), cihazport.TrimEnd(),
                                        ref gelenveri, ref msj);
                                    
                                    // Eğer veri geldiyse veya hata mesajı boşsa döngüden çık
                                    if (gelenveri > 0 || string.IsNullOrEmpty(msj) || msj.TrimEnd() == "")
                                    {
                                        break;
                                    }
                                    
                                    retryCount++;
                                    if (retryCount < maxRetries)
                                    {
                                        // 2 saniye bekle ve tekrar dene
                                        Thread.Sleep(2000);
                                        loglar = cihazip + " BAĞLANTI YENİDEN DENENİYOR... DENEME " + retryCount + "/" + maxRetries;
                                        Console.WriteLine(tab + loglar);
                                        File.AppendAllText(path, yenisatir + loglar);
                                    }
                                }

                                if (msj.TrimEnd() != "")
                                {
                                    loglar = cihazip + " VERiLER ALINIRKEN HATA : " + msj.TrimEnd();
                                    WriteError(loglar);
                                    File.AppendAllText(path, yenisatir + loglar);
                                }
                                else
                                {
                                    if (gelenveri > 0)
                                    {
                                        loglar = cihazip + " iPLi CiHAZDAN VERiLER ALINDI, GELEN VERi ADEDi : " +
                                                 gelenveri + " (Retry: " + retryCount + ")";
                                        WriteSuccess(loglar);
                                        File.AppendAllText(path, yenisatir + loglar);
                                    }
                                    else
                                    {
                                        loglar = cihazip + " iPLi CiHAZDAN HiC VERi GELMEDi (Retry: " + retryCount + ")";
                                        WriteWarning(loglar);
                                        File.AppendAllText(path, yenisatir + loglar);
                                    }

                                    try
                                    {
                                        if (mika_hareket_dt.Rows.Count > 0)
                                        {
                                            for (int mk = 0; mk < mika_hareket_dt.Rows.Count; mk++)
                                            {
                                                if (mika_hareket_dt.Rows[mk]["kart_no"] != null &&
                                                    mika_hareket_dt.Rows[mk]["tarih"] != null &&
                                                    mika_hareket_dt.Rows[mk]["kapi_no"] != null &&
                                                    mika_hareket_dt.Rows[mk]["event"] != null)
                                                {
                                                    if (mika_hareket_dt.Rows[mk]["kart_no"].ToString().TrimEnd() !=
                                                        "" &&
                                                        mika_hareket_dt.Rows[mk]["tarih"].ToString().TrimEnd() != "" &&
                                                        mika_hareket_dt.Rows[mk]["kapi_no"].ToString().TrimEnd() !=
                                                        "" &&
                                                        mika_hareket_dt.Rows[mk]["event"].ToString().TrimEnd() != "")
                                                    {
                                                        string insert =
                                                            "insert into device_records (cardno, record_date, door_no, status) VALUES (@cardno, @record_date, @door_no, @status)";
                                                        using (MySqlCommand cmd = new MySqlCommand(insert, baglanti))
                                                        {
                                                            cmd.Parameters.AddWithValue("@cardno",
                                                                mika_hareket_dt.Rows[mk]["kart_no"].ToString()
                                                                    .TrimEnd());
                                                            cmd.Parameters.AddWithValue("@record_date",
                                                                Convert.ToDateTime(mika_hareket_dt.Rows[mk]["tarih"]
                                                                        .ToString().TrimEnd())
                                                                    .ToString("yyyy-MM-dd HH:mm:ss"));
                                                            cmd.Parameters.AddWithValue("@door_no",
                                                                mika_hareket_dt.Rows[mk]["kapi_no"].ToString()
                                                                    .TrimEnd());
                                                            cmd.Parameters.AddWithValue("@status",
                                                                mika_hareket_dt.Rows[mk]["event"].ToString().TrimEnd());

                                                            int kontrol = cmd.ExecuteNonQuery();
                                                            if (kontrol > 0)
                                                            {
                                                                loglar = "KART NO : " +
                                                                         mika_hareket_dt.Rows[mk]["kart_no"].ToString()
                                                                             .TrimEnd() + ", TARiH : " +
                                                                         mika_hareket_dt.Rows[mk]["tarih"].ToString()
                                                                             .TrimEnd() + " HAREKETi TABLOYA YAZILDI";
                                                                Console.WriteLine(tab + loglar);
                                                                File.AppendAllText(path, yenisatir + loglar);
                                                            }
                                                            else
                                                            {
                                                                loglar = "KART NO : " +
                                                                         mika_hareket_dt.Rows[mk]["kart_no"].ToString()
                                                                             .TrimEnd() + ", TARiH : " +
                                                                         mika_hareket_dt.Rows[mk]["tarih"].ToString()
                                                                             .TrimEnd() +
                                                                         " HAREKETi TABLOYA YAZILAMADI (MYSQL HATASI)";
                                                                Console.WriteLine(tab + loglar);
                                                                File.AppendAllText(path, yenisatir + loglar);

                                                                string logs =
                                                                    mika_hareket_dt.Rows[mk]["kart_no"].ToString()
                                                                        .TrimEnd() + "_" +
                                                                    mika_hareket_dt.Rows[mk]["tarih"].ToString()
                                                                        .TrimEnd() + "_" +
                                                                    mika_hareket_dt.Rows[mk]["kapi_no"].ToString()
                                                                        .TrimEnd() + "_" +
                                                                    mika_hareket_dt.Rows[mk]["event"].ToString()
                                                                        .TrimEnd();

                                                                try
                                                                { // Write to log file using 'using' statement for proper resource management
                                                                    AppendLogsToLogFile(logs);
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    loglar = "LOG YAZILIRKEN HATA : " + ex.Message;
                                                                    Console.WriteLine(tab + loglar);
                                                                    File.AppendAllText(path, yenisatir + loglar);
                                                                    AppendLogsToLogFile(logs);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        loglar = "ONLINE VERi YAZILIRKEN HATA : " + ex.Message;
                                        Console.WriteLine(tab + loglar);
                                        File.AppendAllText(path, yenisatir + loglar);
                                    }
                                }
                            }
                            else
                            {
                                loglar = "VERiTABANI DE CiHAZ VERiSi DOLU OLMAYAN VERiYE DENK GELDi";
                                Console.WriteLine(tab + loglar);
                                File.AppendAllText(path, yenisatir + loglar);
                            }
                        }
                    }
                    else
                    {
                        loglar = "VERiTABANI DE CiHAZ VERiSi DOLU VERi YOK";
                        Console.WriteLine(tab + loglar);
                        File.AppendAllText(path, yenisatir + loglar);
                    }
                }
                catch (Exception ex)
                {
                    loglar = "Hata " + ex.Message;
                    Console.WriteLine(tab + loglar);
                    File.AppendAllText(path, yenisatir + loglar);
                }

                #endregion


                WriteSection("TXT DE BEKLEYEN VERiLER KONTROL EDiLiYOR");
                loglar = "TXT DE BEKLEYEN VERiLER KONTROL EDiLiYOR : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                File.AppendAllText(path, yenisatir + loglar);


                #region TXT KONTROL

                if (File.Exists(logsFileName))
                {
                    DataTable dt_1 = new DataTable();
                    dt_1.Columns.Add("kartno", typeof(string));
                    dt_1.Columns.Add("tarih", typeof(string));
                    dt_1.Columns.Add("kapino", typeof(string));
                    dt_1.Columns.Add("sonuc", typeof(string));

                    dt_1 = vericek(logsFileName);

                    if (dt_1.Rows.Count > 0)
                    {
                        try
                        {
                            int gonderilen = 0;

                            MySqlConnection baglanti = new MySqlConnection(sqlbaglanti);
                            baglanti.Open();

                            for (int i = 0; i < dt_1.Rows.Count; i++)
                            {
                                try
                                {
                                    if (dt_1.Rows[i]["kartno"] != null && dt_1.Rows[i]["tarih"] != null &&
                                        dt_1.Rows[i]["kapino"] != null && dt_1.Rows[i]["sonuc"] != null)
                                    {
                                        if (dt_1.Rows[i]["kartno"].ToString().TrimEnd() != "" &&
                                            dt_1.Rows[i]["tarih"].ToString().TrimEnd() != "" &&
                                            dt_1.Rows[i]["kapino"].ToString().TrimEnd() != "" &&
                                            dt_1.Rows[i]["sonuc"].ToString().TrimEnd() != "")
                                        {
                                            using (DataTable dt34 = new DataTable())
                                            {
                                                string oncesor =
                                                    "select * from device_records where record_date = '" +
                                                    Convert.ToDateTime(dt_1.Rows[i]["tarih"].ToString().TrimEnd())
                                                        .ToString("yyyy-MM-dd HH:mm:ss") + "'";
                                                using (MySqlDataAdapter cmdf = new MySqlDataAdapter(oncesor, baglanti))
                                                {
                                                    cmdf.Fill(dt34);
                                                }

                                                if (dt34.Rows.Count <= 0)
                                                {
                                                    string insert =
                                                        "insert into device_records (cardno,record_date,door_no,status) VALUES ('" +
                                                        dt_1.Rows[i]["kartno"].ToString().TrimEnd() + "','" +
                                                        Convert.ToDateTime(dt_1.Rows[i]["tarih"].ToString().TrimEnd())
                                                            .ToString("yyyy-MM-dd HH:mm:ss") + "','" +
                                                        dt_1.Rows[i]["kapino"].ToString().TrimEnd() + "','" +
                                                        dt_1.Rows[i]["sonuc"].ToString().TrimEnd() + "')";
                                                    using (MySqlCommand cmd = new MySqlCommand(insert, baglanti))
                                                    {
                                                        int kontrol = cmd.ExecuteNonQuery();
                                                        if (kontrol > 0)
                                                        {
                                                            gonderilen++;
                                                            loglar = "KART NO : " +
                                                                     dt_1.Rows[i]["kartno"].ToString().TrimEnd() +
                                                                     ", TARiH : " +
                                                                     dt_1.Rows[i]["tarih"].ToString().TrimEnd() +
                                                                     " HAREKETi TABLOYA YAZILDI";
                                                            Console.WriteLine(tab + loglar);
                                                            File.AppendAllText(path, yenisatir + loglar);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    gonderilen++;
                                                    loglar = "KART NO : " +
                                                             dt_1.Rows[i]["kartno"].ToString().TrimEnd() +
                                                             ", TARiH : " + dt_1.Rows[i]["tarih"].ToString().TrimEnd() +
                                                             " HAREKETi ZATEN TABLOYA YAZILMIS";
                                                    Console.WriteLine(tab + loglar);
                                                    File.AppendAllText(path, yenisatir + loglar);
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    loglar = "TXT OKUNURKEN FOR ICINDE HATA : " + ex.Message;
                                    Console.WriteLine(tab + loglar);
                                    File.AppendAllText(path, yenisatir + loglar);
                                }
                            }

                            if (gonderilen == dt_1.Rows.Count)
                            {
                                if (File.Exists(logsFileName))
                                {
                                    File.WriteAllText(logsFileName, string.Empty);
                                }

                                loglar = "TXT OKUNDU VE BÜTÜN VEiRLER VERiTABANiNA GÖNDERiLDi, TOPLAM KAYIT : " +
                                         gonderilen;
                                Console.WriteLine(tab + loglar);
                                File.AppendAllText(path, yenisatir + loglar);
                            }
                        }
                        catch (Exception ex)
                        {
                            loglar = "TXT OKUNURKEN HATA : " + ex.Message;
                            Console.WriteLine(tab + loglar);
                            File.AppendAllText(path, yenisatir + loglar);
                        }
                    }
                }

                #endregion

                loglar = "TXT DE BEKLEYEN VERiLER KONTROL EDiLDi : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine(tab + loglar);
                File.AppendAllText(path, yenisatir + loglar);


                WriteWarning("iŞLEMLER 1 DK SONRA TEKRAR BAŞLAYACAK");
                loglar = "iŞLEMLER 1 DK SONRA TEKRAR BAŞLAYACAK : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                File.AppendAllText(path, yenisatir + loglar);


                Thread.Sleep(60000); // 2 dk bekle
            } // while end
        }


        #region MIKA UDP

        static string bironceki_mika_udpveri = "";
        static string mikalocal_ip = "";
        static int mikaudpport = 9781;
        static UdpClient mikaudp = new UdpClient();

        public static bool mika_udp_basla()
        {
            // ------ UDP YI BURDA BASLAT
            mikalocal_ip = mikaGetLocalIPAddress();
            mikaudp.EnableBroadcast = true;
            try
            {
                mikaudp.Client.Bind(new IPEndPoint(IPAddress.Any, mikaudpport));
                mikaudp.BeginReceive(new AsyncCallback(mikaverigeldi), mikaudp);
                
                // UDP başlatma mesajı
              //  loglar = "UDP DİNLEME BAŞLATILDI - Port: " + mikaudpport + " - Local IP: " + mikalocal_ip;
              //  Console.WriteLine(tab + loglar);
              //   File.AppendAllText(path, yenisatir + loglar);
                
                return true;
            }
            catch (Exception ex)
            {
                loglar = "MIKA UDP BAŞLARKEN HATA - " + ex.Message;
                Console.WriteLine(tab + loglar);
                File.AppendAllText(path, yenisatir + loglar);
                return false;
            }
        }

        public static string mikaGetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }

                throw new Exception("");
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private static void mikaverigeldi(IAsyncResult res)
        {
            string mjh = "";
            try
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, mikaudpport);
                byte[] received = mikaudp.EndReceive(res, ref RemoteIpEndPoint);
                
                string stringData = Encoding.ASCII.GetString(received, 0, received.Length);
                string termina_ip = "";
                termina_ip = RemoteIpEndPoint.Address.ToString();

                string hex = BitConverter.ToString(received);
                if (bironceki_mika_udpveri.TrimEnd() != hex)
                {
                    // UDP veri geldi debug mesajı - sadece yeni veri geldiğinde
                    loglar = "UDP VERİ GELDİ - Gönderen IP: " + RemoteIpEndPoint.Address.ToString() + " - Veri Uzunluğu: " + received.Length;
                    Console.WriteLine(tab + loglar);
                    File.AppendAllText(path, yenisatir + loglar);
                    
                    bironceki_mika_udpveri = hex;
                    string[] tarihal = hex.Split('-');
                    if (tarihal[0].ToString() == "55" && tarihal[1].ToString() == "55" &&
                        tarihal[2].ToString() == "CC" && tarihal[3].ToString() == "CC" &&
                        tarihal[4].ToString() == "15" && (Convert.ToInt64(tarihal[5], 16)).ToString() == "50")
                    {
                        string valid = "0";
                        string reader_index = "";
                        string kartno = "";
                        string kisi_id = "";
                        string grup = "";
                        string sonuc = "10";
                        String zaman = "";
                        //gecerlımı
                        valid = (Convert.ToInt64(tarihal[6], 16)).ToString();
                        //okuyucu no
                        reader_index = (Convert.ToInt64(tarihal[7], 16)).ToString();
                        //kart no
                        kartno = tarihal[8].ToString() + "" + tarihal[9].ToString() + "" + tarihal[10].ToString() + "" +
                                 tarihal[11].ToString() + "" + tarihal[12].ToString();
                        if (kartno.TrimEnd() != "")
                        {
                            kartno = Convert.ToString(Int64.Parse(kartno, System.Globalization.NumberStyles.HexNumber));
                        }

                        //id
                        kisi_id = BitConverter.ToUInt16(new byte[2] {(byte) received[13], (byte) received[14]}, 0)
                            .ToString();

                        //grubu
                        for (int a = 0; a < 8; a++)
                        {
                            grup += tarihal[15 + a].ToString();
                        }

                        if (grup.TrimEnd() != "")
                        {
                            byte[] cevir1 = mikahextencevir(grup);
                            grup = Encoding.ASCII.GetString(cevir1);
                        }

                        // sonuc
                        sonuc = (Convert.ToInt64(tarihal[23], 16)).ToString();
                        // kart okutulan zaman
                        string gun = (Convert.ToInt64(tarihal[24], 16)).ToString();
                        string ay = (Convert.ToInt64(tarihal[25], 16)).ToString();
                        string yil = (Convert.ToInt64(tarihal[26], 16)).ToString();
                        string saat = (Convert.ToInt64(tarihal[27], 16)).ToString();
                        string dakika = (Convert.ToInt64(tarihal[28], 16)).ToString();
                        string saniye = (Convert.ToInt64(tarihal[29], 16)).ToString();
                        zaman = "20" + yil + "-" + ay + "-" + gun + " " + saat + ":" + dakika + ":" + saniye;
                        string dt = Convert.ToDateTime(zaman.TrimEnd()).ToString("yyyy-MM-dd HH:mm:ss");
                        //  string kapi__yonu = Sql.sql1.cihaz_yonu_dondur(terminal_id.TrimEnd(), reader_index.TrimEnd());


                        string EventType = "BOS VERi"; // kapı acıl degıl// mıkada bos degerdır
                        if (sonuc.TrimEnd() == "0")
                        {
                            EventType = "LEVEL LIMIT ASILDI";
                        } // level lımıt asıldı
                        else if (sonuc.TrimEnd() == "1")
                        {
                            EventType = "KAYITLI DEGIL";
                        } // gecersız kart / kısı kayıtlı degıl
                        else if (sonuc.TrimEnd() == "2")
                        {
                            EventType = "GECERSIZ ZAMAN";
                        } // gecersız zaman((mika da son gırıs tarıhı verıldıyse buraya duser))
                        else if (sonuc.TrimEnd() == "3")
                        {
                            EventType = "ANTIPASSBACK IHLALI";
                        } // antıpassback ıhlalı
                        else if (sonuc.TrimEnd() == "4")
                        {
                            EventType = "GRUP TANIMSIZ";
                        } // access grubu tanımsız
                        else if (sonuc.TrimEnd() == "5")
                        {
                            EventType = "YETERSIZ KREDI";
                        } // yetersız kredı
                        else if (sonuc.TrimEnd() == "6")
                        {
                            EventType = "ZAMAN HATASI";
                        } // zaman hatası
                        else if (sonuc.TrimEnd() == "7")
                        {
                            EventType = "LIMIT ASILDI";
                        } // lımıt asıldı
                        else if (sonuc.TrimEnd() == "9")
                        {
                            EventType = "GECERSIZ GECIS";
                        } // gecersız gecıs
                        else if (sonuc.TrimEnd() == "11")
                        {
                            EventType = "KAPI ACIK KALDI";
                        } // kapı uzun sure acık kaldı
                        else if (sonuc.TrimEnd() == "9999")
                        {
                            EventType = "KAPI MANUEL ACILDI";
                        } // kapı manuel acıldı
                        else if (sonuc.TrimEnd() == "10")
                        {
                            EventType = "BASARILI";
                        }

                        if (kartno.TrimEnd() != "" && kartno.TrimEnd() != "0")
                        {
                            mjh += " ONLINE VERi GELDi -> KARTNO = " + kartno.TrimEnd() + " TARiH = " + dt.TrimEnd();

                            try
                            {
                                Thread myNewThread = new Thread(() => hareket_yaz(kisi_id, kartno, dt.TrimEnd(),
                                    reader_index.ToString(), EventType.ToString().TrimEnd(), termina_ip.TrimEnd()));
                                myNewThread.Start();
                                
                                // UDP verisi işlendikten sonra cihazdan log silme işlemi
                                try
                                {
                                    // Cihaza TCP bağlantısı kur ve log sil
                                    IPEndPoint ipend = new IPEndPoint(IPAddress.Parse(termina_ip.TrimEnd()), 9780);
                                    Socket serversocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                    serversocket.Connect(ipend);
                                    
                                    string mjk = "";
                                    mika mikaInstance = new mika();
                                    mikaInstance.mika_log_sil(serversocket, ref mjk);
                                    
                                    serversocket.Shutdown(SocketShutdown.Both);
                                    serversocket.Close();
                                }
                                catch (Exception ex)
                                {
                                    // Log silme hatası olsa bile devam et
                                    Console.WriteLine("[ERROR] UDP LOG SİLME HATASI: " + ex.Message);
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mjh = "hh : " + ex.Message;
            }

            if (mjh.TrimEnd() != "")
            {
                loglar = "ONLINE VERi MESAJI : " + mjh.TrimEnd();
                WriteInfo(loglar);
                File.AppendAllText(path, yenisatir + loglar);
            }

            mikaudp.BeginReceive(new AsyncCallback(mikaverigeldi), null);
        }

        public static void hareket_yaz(string kisi_id, string kartno, string tarih, string kapi_no, string sonuc,
            string ip)
        {
            string logs;
            try
            {
                MySqlConnection baglanti = new MySqlConnection(sqlbaglanti);
                baglanti.Open();

                string insert = "insert into device_records (cardno,record_date,door_no,status) VALUES ('" +
                                kartno.TrimEnd() + "','" +
                                Convert.ToDateTime(tarih.TrimEnd()).ToString("yyyy-MM-dd HH:mm:ss") + "','" +
                                kapi_no.TrimEnd() + "','" + sonuc.TrimEnd() + "')";
                using (MySqlCommand cmd = new MySqlCommand(insert, baglanti))
                {
                    int kontrol = cmd.ExecuteNonQuery();
                    if (kontrol > 0)
                    {
                        loglar = "KART NO : " + kartno.TrimEnd() + ", TARiH : " + tarih.TrimEnd() +
                                 " HAREKETi TABLOYA YAZILDI";
                        Console.WriteLine("        " + loglar);
                        File.AppendAllText(path, yenisatir + loglar);
                    }
                    else
                    {
                        logs = kartno.TrimEnd() + "_" + tarih.TrimEnd() + "_" + kapi_no.TrimEnd() + "_" +
                               sonuc.TrimEnd();
                        AppendLogsToLogFile(logs);
                        loglar = "KART NO : " + kartno.TrimEnd() + ", TARiH : " + tarih.TrimEnd() +
                                 " HAREKETi TABLOYA YAZILAMADI (MYSQL HATASI)";
                        Console.WriteLine(tab + loglar);
                        File.AppendAllText(path, yenisatir + loglar);
                    }
                }
            }
            catch (Exception ex)
            {
                loglar = "ONLINE VERi YAZILIRKEN HATA : " + ex.Message;
                Console.WriteLine(tab + loglar);
                File.AppendAllText(path, yenisatir + loglar);
                // AppendLogsToLogFile(logs);
            }
        }

        #endregion


        static DataTable vericek(string okunacakyol)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("kartno", typeof(string));
            dt.Columns.Add("tarih", typeof(string));
            dt.Columns.Add("kapino", typeof(string));
            dt.Columns.Add("sonuc", typeof(string));

            StreamReader streamReader = new StreamReader(okunacakyol, Encoding.Default);
            string sirayla_gelen;
            while ((sirayla_gelen = streamReader.ReadLine()) != null)
            {
                string sirayla_gelen_trimstart = sirayla_gelen.TrimStart();
                if (sirayla_gelen_trimstart != null)
                {
                    if (sirayla_gelen_trimstart.TrimEnd() != "")
                    {
                        try
                        {
                            // string logs = kartno.TrimEnd() + "_" + tarih.TrimEnd() + "_" + kapi_no.TrimEnd() + "_" + sonuc.TrimEnd();
                            // 23423424_2020-01-01 01:01:01_1_BASARILI
                            // 10658990_2021-07-05 11:57:05_0_BASARILI
                            string[] gelen_ayrilmis = sirayla_gelen_trimstart.Split('_');
                            if (gelen_ayrilmis != null)
                            {
                                if (gelen_ayrilmis.Length > 0)
                                {
                                    string kartno = "";
                                    string tarih = "";
                                    string kapino = "";
                                    string sonuc = "";
                                    for (int i = 0; i < gelen_ayrilmis.Length; ++i)
                                    {
                                        if (gelen_ayrilmis[i] != null && gelen_ayrilmis[i].TrimEnd() != "")
                                        {
                                            if (i == 0)
                                                kartno = gelen_ayrilmis[i].ToString();
                                            else if (i == 1)
                                                tarih = gelen_ayrilmis[i].ToString();
                                            else if (i == 2)
                                                kapino = gelen_ayrilmis[i].ToString();
                                            else if (i == 3)
                                                sonuc = gelen_ayrilmis[i].ToString();
                                        }
                                    }

                                    if (tarih.TrimEnd() != "")
                                    {
                                        try
                                        {
                                            DateTime ff = DateTime.Parse(tarih);
                                            tarih = ff.ToString("yyyy-MM-dd HH:mm:ss");
                                        }
                                        catch
                                        {
                                            tarih = "";
                                        }
                                    }

                                    if (kartno.TrimEnd() != "" && tarih.TrimEnd() != "" && kapino.TrimEnd() != "" &&
                                        sonuc.TrimEnd() != "")
                                    {
                                        DataRow row = dt.NewRow();
                                        row["kartno"] = kartno.TrimEnd();
                                        row["tarih"] = tarih.TrimEnd();
                                        row["kapino"] = kapino.TrimEnd();
                                        row["sonuc"] = sonuc.TrimEnd();
                                        dt.Rows.Add(row);
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }

            streamReader.Close();

            return dt;
        }

        // Method to append logs to logs.txt
        private static void AppendLogsToLogFile(string logs)
        {
            try
            {
                // Ensure the log directory exists
                if (!Directory.Exists(path_log))
                {
                    Directory.CreateDirectory(path_log);
                }

                // Check if logs file exists
                if (!File.Exists(path_yazilmayan))
                {
                    // Create logs file if it doesn't exist
                    using (StreamWriter createLogFile = File.CreateText(path_yazilmayan))
                    {
                        createLogFile.WriteLine("Logs:");
                    }
                }
                else
                {
                    // Write logs to logs file
                    using (StreamWriter logWriter = new StreamWriter(path_yazilmayan, true))
                    {
                        logWriter.WriteLine(logs);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the full exception including the stack trace
                Console.WriteLine($"LOG DOSYASINA YAZMA HATASI {path_yazilmayan}: {ex.ToString()}");
                Console.WriteLine($"LOG KLASÖRÜ: {path_log}");
                Console.WriteLine($"LOG DOSYASI: {logsFileName}");

                // Optionally, you can re-throw the exception to propagate it further
                // throw;
            }
        }


        public static byte[] mikahextencevir(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return raw;
        }

        public static Boolean mikaGetBit(byte bytedeger, int indexi)
        {
            return (1 == ((bytedeger >> indexi) & 1));
        }

        public static byte[] hextencevir(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return raw;
        }

        public static byte[] hextenbytecevir(string deger)
        {
            if (deger.Length == 1 || deger.Length == 3 || deger.Length == 5 || deger.Length == 7 || deger.Length == 9 ||
                deger.Length == 11 || deger.Length == 13)
            {
                deger = "0" + deger;
            }

            SoapHexBinary shb = SoapHexBinary.Parse(deger);
            return shb.Value;
        }

        static byte ConvertToByte(BitArray bits)
        {
            if (bits.Count != 8)
            {
                throw new ArgumentException("bits");
            }

            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }
    }

    public class kisiler : IDisposable
    {
        public bool devam_Etme = false;

        public string cihaz_ip = "169.254.1.1";
        public string cihaz_port = "9780";
        public string door_id = "";
        public string kisi_id = "";
        public string kisi_adi = "";
        public string kart_no = "";
        public string limit = "";
        public string sontarih = "";
        public string grup = "";
        public bool gondermi = false;
        public bool silmi = false;
        public bool islenmis_verimi = false;

        public kisiler()
        {
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object) this);
        }

        private void Dispose(bool v)
        {
        }

        ~kisiler()
        {
            this.Dispose(false);
        }
    }
}