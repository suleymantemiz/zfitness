using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace ZFitness
{
    class mika
    {
        public mika()
        {
        }


        public void kart_gonder_sil(MySqlConnection baglanti, List<kisiler> kisiler, string ip, string port,
            ref List<string> mesaj)
        {
            try
            {
                IPEndPoint ipm = new IPEndPoint(IPAddress.Parse(ip.TrimEnd()), Convert.ToInt32(port.TrimEnd()));
                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                bool baglimi = false;
                try
                {
                    // Socket timeout ayarları - bağlantı koptuktan sonra daha iyi performans için
                    server.ReceiveTimeout = 10000; // 10 saniye
                    server.SendTimeout = 10000; // 10 saniye
                    server.Connect(ipm);
                    baglimi = true;
                }
                catch (SocketException ex)
                {
                    baglimi = false;
                    if (ex.Message.IndexOf("Bağlanılan belli bir") != -1)
                    {
                        mesaj.Add("Bağlantı yoktur_0");
                    }
                    else if (ex.Message.IndexOf("Bir yuva işlemi erişilemeyen") != -1)
                    {
                        mesaj.Add("Bağlantı yoktur_1");
                    }
                    else if (ex.Message.IndexOf("Bağlanılan uygun olarak") != -1)
                    {
                        mesaj.Add(ex.Message+" Bağlantı yoktur_2");
                    }
                    else if (ex.Message.IndexOf("A request to send") != -1)
                    {
                        mesaj.Add("Bağlantı yoktur_3");
                    }
                    else if (ex.Message.IndexOf("A socket operation") != -1)
                    {
                        mesaj.Add("Bağlantı yoktur_4");
                    }
                    else if (ex.Message.IndexOf("A connection attempt") != -1)
                    {
                        mesaj.Add("- hata : Bağlantı yoktur");
                    }
                    else
                    {
                        mesaj.Add(ex.Message);
                    }
                }

                if (baglimi == true)
                {
                    for (int kis = 0; kis < kisiler.Count; kis++)
                    {
                        string grubum = "GENEL";

                        if (kisiler[kis].door_id.TrimEnd() != "")
                        {
                            if (kisiler[kis].door_id.TrimEnd() == "1")
                            {
                                grubum = "GENEL";
                            }
                            else
                            {
                                grubum = kisiler[kis].grup;
                            }
                        }

                        string reelkartno = kisiler[kis].kart_no.TrimEnd();
                        string isim = kisiler[kis].kisi_adi.TrimEnd();
                        string kisi_id = kisiler[kis].kisi_id.TrimEnd();
                        string sifre = "";

                        if (kisiler[kis].gondermi)
                        {
                            #region GONDER

                            if (reelkartno != "" && kisi_id.TrimEnd() != "")
                            {
                                string cvalid = "";
                                byte[] ckart = new byte[5];
                                string cid = "";
                                string[] cadi = new string[16];
                                string[] cgrup = new string[8];
                                string ccredit = "";
                                string csurevarmi = "0";
                                string csuregun = "";
                                string csureay = "";
                                string csureyil = "";
                                string cpass = "";
                                cvalid = "1";
                                csuregun = "00";
                                csureay = "00";
                                csureyil = "00";
                                csurevarmi = "2";
                                ccredit = (kisiler[kis].limit.TrimEnd() != ""
                                    ? kisiler[kis].limit.TrimEnd() != "0" ? "1" : "0"
                                    : "0");
                                cid = kisi_id;
                                cpass = (sifre != null ? sifre.TrimEnd() != "" ? sifre.TrimEnd() : "0" : "0");
                                //REEL KART AYARLA// 5 BYTE
                                Int64 deger = Convert.ToInt64(reelkartno.TrimEnd());
                                string hexim = deger.ToString("X");
                                byte[] dd = hextenbytecevir(hexim);
                                if (dd.Length == 1)
                                {
                                    ckart[0] = 0x00;
                                    ckart[1] = 0x00;
                                    ckart[2] = 0x00;
                                    ckart[3] = 0x00;
                                    ckart[4] = dd[0];
                                }

                                if (dd.Length == 2)
                                {
                                    ckart[0] = 0x00;
                                    ckart[1] = 0x00;
                                    ckart[2] = 0x00;
                                    ckart[3] = dd[0];
                                    ckart[4] = dd[1];
                                }

                                if (dd.Length == 3)
                                {
                                    ckart[0] = 0x00;
                                    ckart[1] = 0x00;
                                    ckart[2] = dd[0];
                                    ckart[3] = dd[1];
                                    ckart[4] = dd[2];
                                }

                                if (dd.Length == 4)
                                {
                                    ckart[0] = 0x00;
                                    ckart[1] = dd[0];
                                    ckart[2] = dd[1];
                                    ckart[3] = dd[2];
                                    ckart[4] = dd[3];
                                }

                                if (dd.Length == 5)
                                {
                                    ckart[0] = dd[0];
                                    ckart[1] = dd[1];
                                    ckart[2] = dd[2];
                                    ckart[3] = dd[3];
                                    ckart[4] = dd[4];
                                }

                                //PANEL ID AYARLA // 2 BYTE
                                byte[] data3 = new byte[2];
                                data3[0] = (byte) (Convert.ToInt32(cid) & 0xFF);
                                data3[1] = (byte) ((Convert.ToInt32(cid) >> 8) & 0xFF);
                                // AD SOYAD AYARLA
                                cadi[0] = " ";
                                cadi[1] = " ";
                                cadi[2] = " ";
                                cadi[3] = " ";
                                cadi[4] = " ";
                                cadi[5] = " ";
                                cadi[6] = " ";
                                cadi[7] = " ";
                                cadi[8] = " ";
                                cadi[9] = " ";
                                cadi[10] = " ";
                                cadi[11] = " ";
                                cadi[12] = " ";
                                cadi[13] = " ";
                                cadi[14] = " ";
                                cadi[15] = " ";
                                if (isim.TrimEnd() != "")
                                {
                                    for (int g = 0; g < isim.Length; g++)
                                    {
                                        if (g < 16)
                                        {
                                            if (isim.TrimEnd().Length >= g + 1)
                                            {
                                                if (isim.Substring(g, 1) != "")
                                                {
                                                    cadi[g] = isim.Substring(g, 1);
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }

                                //ADI SOYADI ICIN ENCODE
                                byte[] ad1 =
                                    System.Text.Encoding.Default.GetBytes((cadi[0].TrimEnd() != "" ? cadi[0] : " "));
                                byte[] ad2 =
                                    System.Text.Encoding.Default.GetBytes((cadi[1].TrimEnd() != "" ? cadi[1] : " "));
                                byte[] ad3 =
                                    System.Text.Encoding.Default.GetBytes((cadi[2].TrimEnd() != "" ? cadi[2] : " "));
                                byte[] ad4 =
                                    System.Text.Encoding.Default.GetBytes((cadi[3].TrimEnd() != "" ? cadi[3] : " "));
                                byte[] ad5 =
                                    System.Text.Encoding.Default.GetBytes((cadi[4].TrimEnd() != "" ? cadi[4] : " "));
                                byte[] ad6 =
                                    System.Text.Encoding.Default.GetBytes((cadi[5].TrimEnd() != "" ? cadi[5] : " "));
                                byte[] ad7 =
                                    System.Text.Encoding.Default.GetBytes((cadi[6].TrimEnd() != "" ? cadi[6] : " "));
                                byte[] ad8 =
                                    System.Text.Encoding.Default.GetBytes((cadi[7].TrimEnd() != "" ? cadi[7] : " "));
                                byte[] ad9 =
                                    System.Text.Encoding.Default.GetBytes((cadi[8].TrimEnd() != "" ? cadi[8] : " "));
                                byte[] ad10 =
                                    System.Text.Encoding.Default.GetBytes((cadi[9].TrimEnd() != "" ? cadi[9] : " "));
                                byte[] ad11 =
                                    System.Text.Encoding.Default.GetBytes((cadi[10].TrimEnd() != "" ? cadi[10] : " "));
                                byte[] ad12 =
                                    System.Text.Encoding.Default.GetBytes((cadi[11].TrimEnd() != "" ? cadi[11] : " "));
                                byte[] ad13 =
                                    System.Text.Encoding.Default.GetBytes((cadi[12].TrimEnd() != "" ? cadi[12] : " "));
                                byte[] ad14 =
                                    System.Text.Encoding.Default.GetBytes((cadi[13].TrimEnd() != "" ? cadi[13] : " "));
                                byte[] ad15 =
                                    System.Text.Encoding.Default.GetBytes((cadi[14].TrimEnd() != "" ? cadi[14] : " "));
                                byte[] ad16 =
                                    System.Text.Encoding.Default.GetBytes((cadi[15].TrimEnd() != "" ? cadi[15] : " "));
                                // GRUP AYARLA
                                cgrup[0] = "";
                                cgrup[1] = "";
                                cgrup[2] = "";
                                cgrup[3] = "";
                                cgrup[4] = "";
                                cgrup[5] = "";
                                cgrup[6] = "";
                                cgrup[7] = "";
                                if (grubum.TrimEnd() != "")
                                {
                                    for (int g = 0; g < grubum.TrimEnd().Length; g++)
                                    {
                                        if (g < 8)
                                        {
                                            if (grubum.TrimEnd().Length >= g + 1)
                                            {
                                                if (grubum.Substring(g, 1) != "")
                                                {
                                                    cgrup[g] = grubum.Substring(g, 1);
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }

                                // GRUP ICIN ENCODE
                                byte bg1 = 0;
                                byte bg2 = 0;
                                byte bg3 = 0;
                                byte bg4 = 0;
                                byte bg5 = 0;
                                byte bg6 = 0;
                                byte bg7 = 0;
                                byte bg8 = 0;
                                byte[] gr1 =
                                    System.Text.Encoding.Default.GetBytes((cgrup[0].TrimEnd() != "" ? cgrup[0] : " "));
                                byte[] gr2 =
                                    System.Text.Encoding.Default.GetBytes((cgrup[1].TrimEnd() != "" ? cgrup[1] : " "));
                                byte[] gr3 =
                                    System.Text.Encoding.Default.GetBytes((cgrup[2].TrimEnd() != "" ? cgrup[2] : " "));
                                byte[] gr4 =
                                    System.Text.Encoding.Default.GetBytes((cgrup[3].TrimEnd() != "" ? cgrup[3] : " "));
                                byte[] gr5 =
                                    System.Text.Encoding.Default.GetBytes((cgrup[4].TrimEnd() != "" ? cgrup[4] : " "));
                                byte[] gr6 =
                                    System.Text.Encoding.Default.GetBytes((cgrup[5].TrimEnd() != "" ? cgrup[5] : " "));
                                byte[] gr7 =
                                    System.Text.Encoding.Default.GetBytes((cgrup[6].TrimEnd() != "" ? cgrup[6] : " "));
                                byte[] gr8 =
                                    System.Text.Encoding.Default.GetBytes((cgrup[7].TrimEnd() != "" ? cgrup[7] : " "));
                                if (cgrup[0].TrimEnd() != "")
                                {
                                    bg1 = gr1[0];
                                }

                                if (cgrup[1].TrimEnd() != "")
                                {
                                    bg2 = gr2[0];
                                }

                                if (cgrup[2].TrimEnd() != "")
                                {
                                    bg3 = gr3[0];
                                }

                                if (cgrup[3].TrimEnd() != "")
                                {
                                    bg4 = gr4[0];
                                }

                                if (cgrup[4].TrimEnd() != "")
                                {
                                    bg5 = gr5[0];
                                }

                                if (cgrup[5].TrimEnd() != "")
                                {
                                    bg6 = gr6[0];
                                }

                                if (cgrup[6].TrimEnd() != "")
                                {
                                    bg7 = gr7[0];
                                }

                                if (cgrup[7].TrimEnd() != "")
                                {
                                    bg8 = gr8[0];
                                }

                                // PASSWORD AYARLA // 2 BYTE
                                byte[] data4 = new byte[2];
                                data4[0] = (byte) (Convert.ToInt32(cpass) & 0xFF);
                                data4[1] = (byte) ((Convert.ToInt32(cpass) >> 8) & 0xFF);
                                byte[] csureaybyte = new byte[1];
                                byte[] csureyilbyte = new byte[1];
                                csureaybyte = hextenbytecevir(csureay.TrimEnd());
                                csureyilbyte = hextenbytecevir(csureyil.TrimEnd());


                                csurevarmi = "3";
                                csuregun = "1";

                                if (kisiler[kis].sontarih.TrimEnd() != "")
                                {
                                    try
                                    {
                                        DateTime tarih = Convert.ToDateTime(kisiler[kis].sontarih.TrimEnd());
                                        if (tarih < DateTime.Now.AddYears(5))
                                        {
                                            csurevarmi = "1";
                                            csuregun = tarih.Day.ToString();
                                            csureay = tarih.Month.ToString();
                                            csureyil = Convert.ToString(tarih.Year - 2000);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }

                                byte[] data1 = new byte[]
                                {
                                    0x55, 0x55, 0xCC, 0xCC, 0x6F, 45,
                                    Convert.ToByte(cvalid),
                                    ckart[0], ckart[1], ckart[2], ckart[3], ckart[4],

                                    data3[0], data3[1],

                                    ad1[0], ad2[0], ad3[0], ad4[0], ad5[0], ad6[0], ad7[0], ad8[0], ad9[0], ad10[0],
                                    ad11[0], ad12[0], ad13[0], ad14[0], ad15[0], ad16[0],

                                    bg1, bg2, bg3, bg4, bg5, bg6, bg7, bg8,

                                    data4[0], data4[1],

                                    Convert.ToByte(ccredit), Convert.ToByte(csurevarmi), Convert.ToByte(csuregun),
                                    (csurevarmi == "1" ? Convert.ToByte(csureay) : csureaybyte[0]),
                                    (csurevarmi == "1" ? Convert.ToByte(csureyil) : csureyilbyte[0])
                                };
                                server.Send(data1);
                                byte[] data = new byte[1024];
                                int receivedDataLength = server.Receive(data);
                                string stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);
                                string hex = BitConverter.ToString(data);
                                string[] tarihal = hex.Split('-');
                                if (tarihal[0].ToString() == "55" && tarihal[1].ToString() == "55" &&
                                    tarihal[2].ToString() == "CC" && tarihal[3].ToString() == "CC" &&
                                    tarihal[4].ToString() == "AF" &&
                                    (Convert.ToInt64(tarihal[5], 16)).ToString() == "9")
                                {
                                    mesaj.Add("KİŞİ İD : " + kisiler[kis].kisi_id + ", KARTNO : " +
                                              kisiler[kis].kart_no + " , GRUP : '" + grubum + "' CİHAZA GONDERILDI");
                                    string msj = "";
                                    kredi_gonder(server, ip.TrimEnd(), port.TrimEnd(), kisiler[kis].limit.TrimEnd(),
                                        kisiler[kis].kart_no.TrimEnd(), ref msj);
                                    if (msj.TrimEnd() != "")
                                    {
                                        mesaj.Add("KİŞİ İD : " + kisiler[kis].kisi_id + ", KARTNO : " +
                                                  kisiler[kis].kart_no + " , GRUP : '" + grubum +
                                                  "' KREDI GONDERILIRKEN HATA  : " + msj);
                                    }
                                    else
                                    {
                                        if (baglanti != null)
                                        {
                                            if (baglanti.State == ConnectionState.Open)
                                            {
                                                string updd = "update clients set is_update = 'yes' where card_id = '" +
                                                              kisiler[kis].kart_no.TrimEnd() + "'";
                                                using (MySqlCommand cmd = new MySqlCommand(updd, baglanti))
                                                {
                                                    cmd.ExecuteNonQuery();
                                                }
                                            }
                                        }
                                    }
                                }
                                else // panelden olumsuz cevap yada baska bı cevap geldı
                                {
                                    mesaj.Add("KİŞİ İD : " + kisiler[kis].kisi_id +
                                              " KİŞİ CİHAZDAN SİLİNEMEDİ : PANELDEN DÜZGÜN CEVAP ALINAMADI");
                                }
                            }
                            else // kısının kart nosu yada ıd sı yok
                            {
                                mesaj.Add("KİŞİ İD : " + kisiler[kis].kisi_id +
                                          " KİŞİ CİHAZDAN SİLİNEMEDİ : KART NO YADA İD BOŞTUR");
                            }

                            #endregion
                        }
                        else if (kisiler[kis].silmi)
                        {
                            #region SIL

                            if (reelkartno.TrimEnd() != "" && kisi_id.TrimEnd() != "")
                            {
                                string cvalid = "";
                                byte[] ckart = new byte[5];
                                string cid = "";
                                string[] cadi = new string[16];
                                string[] cgrup = new string[8];
                                string ccredit = "";
                                string csurevarmi = "0";
                                string csuregun = "";
                                string csureay = "";
                                string csureyil = "";
                                string cpass = "";
                                cvalid = "0";
                                csuregun = "00";
                                csureay = "00";
                                csureyil = "00";
                                csurevarmi = "2";
                                ccredit = "0";
                                cid = kisi_id;
                                cpass = "0";
                                //REEL KART AYARLA// 5 BYTE
                                Int64 deger = Convert.ToInt64(reelkartno.TrimEnd());
                                string hexim = deger.ToString("X");
                                byte[] dd = hextenbytecevir(hexim);
                                if (dd.Length == 1)
                                {
                                    ckart[0] = 0x00;
                                    ckart[1] = 0x00;
                                    ckart[2] = 0x00;
                                    ckart[3] = 0x00;
                                    ckart[4] = dd[0];
                                }

                                if (dd.Length == 2)
                                {
                                    ckart[0] = 0x00;
                                    ckart[1] = 0x00;
                                    ckart[2] = 0x00;
                                    ckart[3] = dd[0];
                                    ckart[4] = dd[1];
                                }

                                if (dd.Length == 3)
                                {
                                    ckart[0] = 0x00;
                                    ckart[1] = 0x00;
                                    ckart[2] = dd[0];
                                    ckart[3] = dd[1];
                                    ckart[4] = dd[2];
                                }

                                if (dd.Length == 4)
                                {
                                    ckart[0] = 0x00;
                                    ckart[1] = dd[0];
                                    ckart[2] = dd[1];
                                    ckart[3] = dd[2];
                                    ckart[4] = dd[3];
                                }

                                if (dd.Length == 5)
                                {
                                    ckart[0] = dd[0];
                                    ckart[1] = dd[1];
                                    ckart[2] = dd[2];
                                    ckart[3] = dd[3];
                                    ckart[4] = dd[4];
                                }

                                //PANEL ID AYARLA // 2 BYTE
                                byte[] data3 = new byte[2];
                                data3[0] = (byte) (Convert.ToInt32(cid) & 0xFF);
                                data3[1] = (byte) ((Convert.ToInt32(cid) >> 8) & 0xFF);
                                // AD SOYAD AYARLA
                                cadi[0] = " ";
                                cadi[1] = " ";
                                cadi[2] = " ";
                                cadi[3] = " ";
                                cadi[4] = " ";
                                cadi[5] = " ";
                                cadi[6] = " ";
                                cadi[7] = " ";
                                cadi[8] = " ";
                                cadi[9] = " ";
                                cadi[10] = " ";
                                cadi[11] = " ";
                                cadi[12] = " ";
                                cadi[13] = " ";
                                cadi[14] = " ";
                                cadi[15] = " ";
                                //ADI SOYADI ICIN ENCODE
                                byte[] ad1 =
                                    System.Text.Encoding.Default.GetBytes((cadi[0].TrimEnd() != "" ? cadi[0] : " "));
                                byte[] ad2 =
                                    System.Text.Encoding.Default.GetBytes((cadi[1].TrimEnd() != "" ? cadi[1] : " "));
                                byte[] ad3 =
                                    System.Text.Encoding.Default.GetBytes((cadi[2].TrimEnd() != "" ? cadi[2] : " "));
                                byte[] ad4 =
                                    System.Text.Encoding.Default.GetBytes((cadi[3].TrimEnd() != "" ? cadi[3] : " "));
                                byte[] ad5 =
                                    System.Text.Encoding.Default.GetBytes((cadi[4].TrimEnd() != "" ? cadi[4] : " "));
                                byte[] ad6 =
                                    System.Text.Encoding.Default.GetBytes((cadi[5].TrimEnd() != "" ? cadi[5] : " "));
                                byte[] ad7 =
                                    System.Text.Encoding.Default.GetBytes((cadi[6].TrimEnd() != "" ? cadi[6] : " "));
                                byte[] ad8 =
                                    System.Text.Encoding.Default.GetBytes((cadi[7].TrimEnd() != "" ? cadi[7] : " "));
                                byte[] ad9 =
                                    System.Text.Encoding.Default.GetBytes((cadi[8].TrimEnd() != "" ? cadi[8] : " "));
                                byte[] ad10 =
                                    System.Text.Encoding.Default.GetBytes((cadi[9].TrimEnd() != "" ? cadi[9] : " "));
                                byte[] ad11 =
                                    System.Text.Encoding.Default.GetBytes((cadi[10].TrimEnd() != "" ? cadi[10] : " "));
                                byte[] ad12 =
                                    System.Text.Encoding.Default.GetBytes((cadi[11].TrimEnd() != "" ? cadi[11] : " "));
                                byte[] ad13 =
                                    System.Text.Encoding.Default.GetBytes((cadi[12].TrimEnd() != "" ? cadi[12] : " "));
                                byte[] ad14 =
                                    System.Text.Encoding.Default.GetBytes((cadi[13].TrimEnd() != "" ? cadi[13] : " "));
                                byte[] ad15 =
                                    System.Text.Encoding.Default.GetBytes((cadi[14].TrimEnd() != "" ? cadi[14] : " "));
                                byte[] ad16 =
                                    System.Text.Encoding.Default.GetBytes((cadi[15].TrimEnd() != "" ? cadi[15] : " "));
                                // GRUP AYARLA
                                // GRUP ICIN ENCODE
                                byte bg1 = 0;
                                byte bg2 = 0;
                                byte bg3 = 0;
                                byte bg4 = 0;
                                byte bg5 = 0;
                                byte bg6 = 0;
                                byte bg7 = 0;
                                byte bg8 = 0;
                                // PASSWORD AYARLA // 2 BYTE
                                byte[] data4 = new byte[2];
                                data4[0] = (byte) (Convert.ToInt32(cpass) & 0xFF);
                                data4[1] = (byte) ((Convert.ToInt32(cpass) >> 8) & 0xFF);
                                byte[] data1 = new byte[]
                                {
                                    0x55, 0x55, 0xCC, 0xCC, 0x6F, 45,
                                    Convert.ToByte(cvalid),
                                    ckart[0], ckart[1], ckart[2], ckart[3], ckart[4],

                                    data3[0], data3[1],

                                    ad1[0], ad2[0], ad3[0], ad4[0], ad5[0], ad6[0], ad7[0], ad8[0], ad9[0], ad10[0],
                                    ad11[0], ad12[0], ad13[0], ad14[0], ad15[0], ad16[0],

                                    bg1, bg2, bg3, bg4, bg5, bg6, bg7, bg8,

                                    data4[0], data4[1],

                                    Convert.ToByte(ccredit), Convert.ToByte(csurevarmi), Convert.ToByte(csuregun), 0, 0
                                };
                                server.Send(data1);
                                byte[] data = new byte[1024];
                                int receivedDataLength = server.Receive(data);
                                string stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);
                                string hex = BitConverter.ToString(data);
                                string[] tarihal = hex.Split('-');
                                if (tarihal[0].ToString() == "55" && tarihal[1].ToString() == "55" &&
                                    tarihal[2].ToString() == "CC" && tarihal[3].ToString() == "CC" &&
                                    tarihal[4].ToString() == "AF" &&
                                    (Convert.ToInt64(tarihal[5], 16)).ToString() == "9")
                                {
                                    mesaj.Add("KİŞİ İD : " + kisiler[kis].kisi_id + " CİHAZDAN SİLİNDİ");
                                    if (baglanti != null)
                                    {
                                        if (baglanti.State == ConnectionState.Open)
                                        {
                                            string updd = "update cards set is_update = 'yes' where card_id = '" +
                                                          kisiler[kis].kart_no.TrimEnd() + "'";
                                            using (MySqlCommand cmd = new MySqlCommand(updd, baglanti))
                                            {
                                                cmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                                else // panelden olumsuz cevap yada baska bı cevap geldı
                                {
                                    mesaj.Add("KİŞİ İD : " + kisiler[kis].kisi_id +
                                              " KİŞİ CİHAZDAN SİLİNEMEDİ : PANELDEN DÜZGÜN CEVAP ALINAMADI");
                                }
                            }
                            else // kısının kart nosu yada ıd sı yok
                            {
                                mesaj.Add("KİŞİ İD : " + kisiler[kis].kisi_id +
                                          " KİŞİ CİHAZDAN SİLİNEMEDİ : KART NO YADA İD BOŞTUR");
                            }

                            #endregion
                        }
                        else
                        {
                            mesaj.Add("KİŞİ İD : " + kisiler[kis].kisi_id +
                                      " GÖNDERME VE SİLME AKTİF DEĞİL. HİÇBİR İŞLEM YAPILMADI");
                        }
                    }

                    server.Shutdown(SocketShutdown.Both);
                    server.Close();
                }
            }
            catch (Exception ex)
            {
                mesaj.Add("KİŞİ GÖNDERME SİLME İÇİNDE GENEL HATA : " + ex.Message);
            }
        }

        public void kredi_gonder(Socket server, string ip, string port, string kredi, string kartno, ref string mesaj)
        {
            try
            {
                string credit_value = kredi.TrimEnd();
                byte[] ckart = new byte[5];
                Int64 deger = Convert.ToInt64(kartno.TrimEnd());
                string hexim = deger.ToString("X");
                byte[] dd = hextenbytecevir(hexim);
                if (dd.Length == 1)
                {
                    ckart[0] = 0x00;
                    ckart[1] = 0x00;
                    ckart[2] = 0x00;
                    ckart[3] = 0x00;
                    ckart[4] = dd[0];
                }

                if (dd.Length == 2)
                {
                    ckart[0] = 0x00;
                    ckart[1] = 0x00;
                    ckart[2] = 0x00;
                    ckart[3] = dd[0];
                    ckart[4] = dd[1];
                }

                if (dd.Length == 3)
                {
                    ckart[0] = 0x00;
                    ckart[1] = 0x00;
                    ckart[2] = dd[0];
                    ckart[3] = dd[1];
                    ckart[4] = dd[2];
                }

                if (dd.Length == 4)
                {
                    ckart[0] = 0x00;
                    ckart[1] = dd[0];
                    ckart[2] = dd[1];
                    ckart[3] = dd[2];
                    ckart[4] = dd[3];
                }

                if (dd.Length == 5)
                {
                    ckart[0] = dd[0];
                    ckart[1] = dd[1];
                    ckart[2] = dd[2];
                    ckart[3] = dd[3];
                    ckart[4] = dd[4];
                }

                byte[] data53 = new byte[2];
                data53[0] = (byte) (Convert.ToInt32(credit_value.TrimEnd() != "" ? credit_value.TrimEnd() : "0") &
                                    0xFF);
                data53[1] =
                    (byte) ((Convert.ToInt32(credit_value.TrimEnd() != "" ? credit_value.TrimEnd() : "0") >> 8) & 0xFF);
                byte[] data11 = new byte[]
                {
                    0x55, 0x55, 0xCC, 0xCC, 0x7E, 13,
                    ckart[0], ckart[1], ckart[2], ckart[3], ckart[4], data53[0], data53[1]
                };
                server.Send(data11);
                byte[] data = new byte[1024];
                int receivedDataLength = server.Receive(data);
                string stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);
                string hex = BitConverter.ToString(data);
                string[] tarihal = hex.Split('-');
                if (tarihal[0].ToString() == "55" && tarihal[1].ToString() == "55" && tarihal[2].ToString() == "CC" &&
                    tarihal[3].ToString() == "CC" && tarihal[4].ToString() == "BE" &&
                    (Convert.ToInt64(tarihal[5], 16)).ToString() == "7")
                {
                    mesaj = "";
                }
                else
                {
                    mesaj = "Cihazdan yanıt alınamadı.";
                }
            }
            catch (Exception ex)
            {
                mesaj = ex.Message;
            }
        }


        public DataTable mika_vericek(string ip, string port, ref int gelen, ref string mesaj)
        {
            DataTable mika_hareket_dt = new DataTable();
            mika_hareket_dt.Columns.Add("kisi_id", typeof(string));
            mika_hareket_dt.Columns.Add("tarih", typeof(DateTime));
            mika_hareket_dt.Columns.Add("cihaz_ip", typeof(string));
            mika_hareket_dt.Columns.Add("cihaz_port", typeof(string));
            mika_hareket_dt.Columns.Add("kapi_no", typeof(string));
            mika_hareket_dt.Columns.Add("kart_no", typeof(string));
            mika_hareket_dt.Columns.Add("event", typeof(string));
            gelen = 0;
            mesaj = "";

            try
            {
                IPEndPoint ipend = new IPEndPoint(IPAddress.Parse(ip.TrimEnd()), Convert.ToInt32(port.TrimEnd()));
                Socket serversocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                bool baglimi = false;
                try
                {
                    // Socket timeout ayarları - bağlantı koptuktan sonra daha iyi performans için
                    serversocket.ReceiveTimeout = 10000; // 10 saniye
                    serversocket.SendTimeout = 10000; // 10 saniye
                    serversocket.Connect(ipend);
                    baglimi = true;
                }
                catch (SocketException ex)
                {
                    baglimi = false;
                    if (ex.Message.IndexOf("Bağlanılan belli bir") != -1)
                    {
                        mesaj = "- hata : Bağlantı yoktur";
                    }
                    else if (ex.Message.IndexOf("Bir yuva işlemi erişilemeyen") != -1)
                    {
                        mesaj = "- hata : Bağlantı yoktur";
                    }
                    else if (ex.Message.IndexOf("Bağlanılan uygun olarak") != -1)
                    {
                        mesaj = "- hata : Bağlantı yoktur";
                    }
                    else if (ex.Message.IndexOf("A request to send") != -1)
                    {
                        mesaj = "- hata : Bağlantı yoktur";
                    }
                    else if (ex.Message.IndexOf("A socket operation") != -1)
                    {
                        mesaj = "- hata : Bağlantı yoktur";
                    }
                    else if (ex.Message.IndexOf("A connection attempt") != -1)
                    {
                        mesaj = "- hata : Bağlantı yoktur";
                    }
                    else
                    {
                        mesaj = "- hata : " + ex.Message;
                    }
                }

                if (baglimi == true)
                {
                    try
                    {
                        // Cihazdan tüm kayıtları almak için döngü - cihaz veri yok diyene kadar devam et
                        int totalRecordsProcessed = 0;
                        int consecutiveEmptyResponses = 0;
                        const int maxEmptyResponses = 3; // 3 kez boş yanıt gelirse dur
                        
                        for (int o = 0; o < 1000; o++) // Maksimum 1000 iterasyon (güvenlik için)
                        {
                            byte son = 0x00;
                            if (o > 0)
                            {
                                son = 0x01;
                            }

                            byte[] data1 = new byte[]
                            {
                                0x55, 0x55, 0xCC, 0xCC, 0x76, 7, son
                            };
                            serversocket.Send(data1);
                            byte[] data = new byte[1024];
                            int receivedDataLength = serversocket.Receive(data);
                            string stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);
                            string hex = "";
                            hex = BitConverter.ToString(data);
                            string[] deger = hex.Split('-');
                            if (deger[0].ToString() == "55" && deger[1].ToString() == "55" &&
                                deger[2].ToString() == "CC" && deger[3].ToString() == "CC" &&
                                deger[4].ToString() == "B6" && deger[6].ToString() != "00")
                            {
                                int toplamlog = Convert.ToInt32(deger[6].ToString());
                                if (toplamlog > 0)
                                {
                                    // Veri geldi, boş yanıt sayacını sıfırla
                                    consecutiveEmptyResponses = 0;
                                    
                                    int j = 0;
                                    // Tüm kayıtları işle, sadece ilk 6'sını değil
                                    for (int i = 1; i <= toplamlog; i++)
                                    {
                                        j = 11 + ((i - 1) * 28);
                                        string valid = "0";
                                        string reader_index = "0";
                                        string kartnom = "";
                                        string kisi_id = "";
                                        string grup = "";
                                        string sonuc = "10";
                                        String zaman = "";
                                        //gecerlımı
                                        valid = (Convert.ToInt64(deger[j + 4], 16)).ToString();
                                        //okuyucu no
                                        reader_index = (Convert.ToInt64(deger[j + 5], 16)).ToString();
                                        //kart no

                                        kartnom = deger[j + 6].ToString() + "" + deger[j + 7].ToString() + "" +
                                                  deger[j + 8].ToString() + "" + deger[j + 9].ToString() + "" +
                                                  deger[j + 10].ToString();
                                        //id
                                        kisi_id = BitConverter
                                            .ToUInt16(new byte[2] {(byte) data[j + 11], (byte) data[j + 12]}, 0)
                                            .ToString();

                                        if (kartnom.TrimEnd() != "")
                                        {
                                            kartnom = Convert.ToString(Int64.Parse(kartnom,
                                                System.Globalization.NumberStyles.HexNumber));
                                        }

                                        //grubu
                                        //for (int a = 0; a < 8; a++)
                                        //{
                                        //    grup += deger[j + 13 + a].ToString();
                                        //}
                                        //if (grup.TrimEnd() != "") { byte[] cevir1 = hextencevir(grup); grup = Encoding.ASCII.GetString(cevir1); }
                                        // sonuc
                                        sonuc = (Convert.ToInt64(deger[j + 21], 16)).ToString();
                                        // kart okutulan zaman
                                        string gun = (Convert.ToInt64(deger[j + 22], 16)).ToString();
                                        string ay = (Convert.ToInt64(deger[j + 23], 16)).ToString();
                                        string yil = (Convert.ToInt64(deger[j + 24], 16)).ToString();
                                        string saat = (Convert.ToInt64(deger[j + 25], 16)).ToString();
                                        string dakika = (Convert.ToInt64(deger[j + 26], 16)).ToString();
                                        string saniye = (Convert.ToInt64(deger[j + 27], 16)).ToString();
                                        zaman = "20" + yil + "-" + ay + "-" + gun + " " + saat + ":" + dakika + ":" +
                                                saniye;


                                        string EventType = "BOŞ VERİ"; // kapı acıl degıl// mıkada bos degerdır
                                        if (sonuc.TrimEnd() == "0")
                                        {
                                            EventType = "LEVEL LİMİT AŞILDI";
                                        } // level lımıt asıldı
                                        else if (sonuc.TrimEnd() == "1")
                                        {
                                            EventType = "KAYITLI DEĞIL";
                                        } // gecersız kart / kısı kayıtlı degıl
                                        else if (sonuc.TrimEnd() == "2")
                                        {
                                            EventType = "GEÇERSIZ ZAMAN";
                                        } // gecersız zaman((mika da son gırıs tarıhı verıldıyse buraya duser))
                                        else if (sonuc.TrimEnd() == "3")
                                        {
                                            EventType = "ANTIPASSBACK İHLALİ";
                                        } // antıpassback ıhlalı
                                        else if (sonuc.TrimEnd() == "4")
                                        {
                                            EventType = "GRUP TANIMSIZ";
                                        } // access grubu tanımsız
                                        else if (sonuc.TrimEnd() == "5")
                                        {
                                            EventType = "YETERSİZ KREDİ";
                                        } // yetersız kredı
                                        else if (sonuc.TrimEnd() == "6")
                                        {
                                            EventType = "ZAMAN HATASI";
                                        } // zaman hatası
                                        else if (sonuc.TrimEnd() == "7")
                                        {
                                            EventType = "LİMİT AŞILDI";
                                        } // lımıt asıldı
                                        else if (sonuc.TrimEnd() == "9")
                                        {
                                            EventType = "GEÇERSIZ GEÇİŞ";
                                        } // gecersız gecıs
                                        else if (sonuc.TrimEnd() == "11")
                                        {
                                            EventType = "KAPI AÇIK KALDI";
                                        } // kapı uzun sure acık kaldı
                                        else if (sonuc.TrimEnd() == "9999")
                                        {
                                            EventType = "KAPI MANUEL AÇILDI";
                                        } // kapı manuel acıldı
                                        else if (sonuc.TrimEnd() == "10")
                                        {
                                            EventType = "BASARILI";
                                        }

                                        if (valid == "1")
                                        {
                                            if (EventType.TrimEnd() != "" && sonuc.TrimEnd() != "11" &&
                                                sonuc.TrimEnd() != "9999")
                                            {
                                                try
                                                {
                                                    if (kartnom.TrimEnd() != "")
                                                    {
                                                        //  mesaj += "valid : " + valid + " - kisi_id = " + kisi_id.TrimEnd() + " - kartnom : " + kartnom.TrimEnd() + " ındx : " + reader_index + ".." + zaman + Environment.NewLine;

                                                        DataRow dw = mika_hareket_dt.NewRow();
                                                        dw["kisi_id"] = kisi_id.TrimEnd();
                                                        dw["tarih"] = Convert.ToDateTime(zaman.TrimEnd());
                                                        dw["cihaz_ip"] = ip.TrimEnd();
                                                        dw["cihaz_port"] = port.TrimEnd();
                                                        dw["kapi_no"] = (reader_index != null
                                                            ? reader_index.TrimEnd() != ""
                                                                ?
                                                                Convert.ToInt32(reader_index.TrimEnd()) + 1 + ""
                                                                : "1"
                                                            : "1");
                                                        dw["kart_no"] = kartnom.TrimEnd();
                                                        dw["event"] = EventType.TrimEnd();
                                                        mika_hareket_dt.Rows.Add(dw);
                                                        gelen++;
                                                        totalRecordsProcessed++;
                                                        
                                                        // Her kaydı konsola yazdır
                                                        Console.WriteLine("        KART NO : " + kartnom.TrimEnd() + ", TARiH : " + zaman.TrimEnd() + " HAREKETi TABLOYA YAZILDI");
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    // Hata durumunda log ekleyelim
                                                    mesaj += "Kayıt işlenirken hata: " + ex.Message + " ";
                                                }
                                            }
                                        }
                                    } // for bıttı
                                    
                                    // Toplam işlenen kayıt sayısını logla ve konsola yazdır
                                    if (totalRecordsProcessed > 0)
                                    {
                                        Console.WriteLine("        Toplam " + totalRecordsProcessed + " kayıt işlendi");
                                    }
                                    
                                    // Debug: Her iterasyonda kaç kayıt geldiğini logla ve konsola yazdır
                                    Console.WriteLine("        İterasyon " + (o + 1) + ": " + toplamlog + " kayıt alındı");
                                    
                                    // Başarılı durumda mesaj değişkenini boş bırak
                                    mesaj = "";
                                }
                                else
                                {
                                    // Cihazdan boş yanıt geldi
                                    consecutiveEmptyResponses++;
                                    Console.WriteLine("        İterasyon " + (o + 1) + ": Boş yanıt (" + consecutiveEmptyResponses + "/" + maxEmptyResponses + ")");
                                    
                                    if (consecutiveEmptyResponses >= maxEmptyResponses)
                                    {
                                        // 3 kez boş yanıt geldi, tüm kayıtlar alındı
                                        Console.WriteLine("        Tüm kayıtlar alındı, döngü sonlandırılıyor");
                                        mesaj = "";
                                        break;
                                    }
                                    // Bir sonraki iterasyona devam et
                                    continue;
                                }
                            }
                            else
                            {
                                break;
                            }
                        } // for bıttı
                        
                        // Tüm kayıtlar alındıktan sonra log silme işlemi yap
                        string mjk = "";
                        mika_log_sil(serversocket, ref mjk);
                        if (mjk.TrimEnd() != "")
                        {
                            mesaj += mjk.TrimEnd();
                        }
                    }
                    catch (Exception ex)
                    {
                        mesaj = "- hata : " + ex.Message;
                    }

                    serversocket.Shutdown(SocketShutdown.Both);
                    serversocket.Close();
                }
            }
            catch (Exception ex)
            {
                mesaj = "- hata : " + ex.Message;
            }

            return mika_hareket_dt;
        }

        public bool mika_log_sil(Socket serversocket, ref string mesaj)
        {
            mesaj = "";
            bool sonuc = false;
            try
            {
                byte[] data1 = new byte[]
                {
                    0x55, 0x55, 0xCC, 0xCC, 0x77, 6
                };
                serversocket.Send(data1);
                byte[] data = new byte[1024];
                int receivedDataLength = serversocket.Receive(data);
                string stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);
                string hex = BitConverter.ToString(data);
                string[] deger = hex.Split('-');
                if (deger[0].ToString() == "55" && deger[1].ToString() == "55" && deger[2].ToString() == "CC" &&
                    deger[3].ToString() == "CC" && deger[4].ToString() == "B7" &&
                    (Convert.ToInt64(deger[5], 16)).ToString() == "10")
                {
                    mesaj = "";
                    sonuc = true;
                }
                else
                {
                    mesaj = "Cihazdan yanıt alınamadı";
                    sonuc = false;
                }
            }
            catch (Exception ex)
            {
                mesaj = "- hata : " + ex.Message;
                sonuc = false;
            }

            return sonuc;
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

        public byte[] hextenbytecevir(string deger)
        {
            if (deger.Length == 1 || deger.Length == 3 || deger.Length == 5 || deger.Length == 7 || deger.Length == 9 ||
                deger.Length == 11 || deger.Length == 13)
            {
                deger = "0" + deger;
            }

            SoapHexBinary shb = SoapHexBinary.Parse(deger);
            return shb.Value;
        }

        byte ConvertToByte(BitArray bits)
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
}