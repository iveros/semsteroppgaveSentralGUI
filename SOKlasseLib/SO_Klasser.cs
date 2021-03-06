﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
//test
//test eirik
namespace SOKlasseLib
{
    interface InterfaceSentral
    {
        void RegistrerKunde(string innNavn, string innAdresse, int innKID, string innPassord, string ipAdresse);
        void SlettKunde(int KID);
        void MottaForbruk();
        string GenererRapport();
        double ForbrukForesporsel(int KID);
    }

    interface InterfaceHus
    {
        double Forbruk { get; set; }
        int SendSentralIntervall { set; }
        int KID { get; set; }

        void MottaFraKort();
        void SendTilKort(string kommando);
        void SendSentral();
        void MottaSentral(string fraAdresse);
    }

    public class Sentral : InterfaceSentral
    {
        public List<Kunde> KundeListe;
        public string SentralIP { get; set; }
        private Socket kommSokkel;      //socket for å motta data fra klient

        public Sentral()
        {
            KundeListe = new List<Kunde>();
            SentralIP = "127.0.0.1";
        }

        public void RegistrerKunde(string innNavn, string innAdresse, int innKID, string innPassord, string ipAdresse)
        {
            KundeListe.Add(new Kunde(innNavn, innAdresse, innKID, innPassord, ipAdresse));
        }


         // DETTE ER EN MIDLERTIDIG FUNKSJON FOR Å SKRIVE UT KUNDELISE
        public string SkrivUtKundeliste(int KID)
        {
            /*for(int i = 1; i <= KID; i++)
            {

            }*/
            string utskrift = ("Navn: " + KundeListe[KID].KundeNavn + "Kid: " + KundeListe[KID].KID);
            return utskrift;
            
        }



        public void SlettKunde(int KID)
        {
            if (KundeListe[IndexKID(KID)] != null)
                KundeListe.RemoveAt(KID);
        }

        public int IndexKID(int finnKID)
        {
            int returnIndex = -1;
            for (int i = 0; i < KundeListe.Count; i++)
            {
                if (KundeListe[i].KID == finnKID)
                    returnIndex = i;
            }
            return returnIndex;
        }

        public void MottaForbruk() //regelmessig mottak
        {
            throw new NotImplementedException();
        }

        public string GenererRapport()
        {
            throw new NotImplementedException();
        }

        public void MottaServer()
        {
            //oppretter en tråd for hver tilkoblede kunde
            //sjekker at kunden har riktig kid og passord
            //svarer med å sende forbruket fra hus

            Socket lytteSokkel = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint sentralServer = new IPEndPoint(IPAddress.Parse(SentralIP), 9050);    //oppretter server for å motta klienter

            lytteSokkel.Bind(sentralServer);    //ser etter nye klienter som kobler seg til serverKobling
            lytteSokkel.Listen(10);

            int recv = -1;
            while (recv != 0)   //recv = 0 når det sendes en tom melding
            {
                Console.Write("\nVenter på tilkobling fra klient. ");
                kommSokkel = lytteSokkel.Accept();   //setter kommSokkel til referanse til nye tilkoblingen

                byte[] mottatData = new byte[1024];
                recv = kommSokkel.Receive(mottatData);      //mottar data fra kunde
                string mottattTekst = Encoding.ASCII.GetString(mottatData, 0, recv);

                if (mottattTekst.Contains("LOGGINN="))      //hvis en kunde prøver å logge seg inn
                {
                    Console.WriteLine("Mottat kundelogginn");
                    behandleLoggInn(mottattTekst);
                }
                else if (mottattTekst.Contains("TIDSOPPDATERING:"))     //regelmessig forbruk fra hus
                {
                    Console.WriteLine("Mottat tidsoppdatering fra hus");
                    behandleRegelmessig(mottattTekst);
                }
            }
            lytteSokkel.Close();
        }

        private void behandleLoggInn(string kundeTekst)
        {
            Console.WriteLine(kundeTekst);

            int kidLengde = kundeTekst.IndexOf(',') - kundeTekst.IndexOf('=');
            int innKID = Convert.ToInt32(kundeTekst.Substring(
                kundeTekst.IndexOf('=') + 1, kidLengde - 1));     //skiller ut KID fra sendt data

            int passordLengde = kundeTekst.IndexOf('#') - kundeTekst.IndexOf(',');
            string innPassord = kundeTekst.Substring(
                kundeTekst.IndexOf(',') + 1, passordLengde - 1);  //skiller ut passord fra sendt data

            if (KundeListe[IndexKID(innKID)].KID == innKID &&
                KundeListe[IndexKID(innKID)].Passord.Equals(innPassord)) //hvis både KID og passord er riktig
            {
                double lagretForbruk = KundeListe[IndexKID(innKID)].Forbruk;  //DENNE MÅ ENDRES UT FRA TOLKING AV OPPGAVE

                Console.WriteLine("Riktig logginn");
                IPEndPoint klientServer =
                    new IPEndPoint(IPAddress.Parse(KundeListe[IndexKID(innKID)].IPAdresse), 9050); //oppretter kobling til kunde sin IP
                byte[] forbrukSvar = Encoding.ASCII.GetBytes(lagretForbruk.ToString());    //finner forbruk fra HUS-klassen

                //kommSokkel.Connect(klientServer);     //trenger ikke connect fordi er allerede koblet til server
                kommSokkel.Send(forbrukSvar, forbrukSvar.Length, SocketFlags.None);     //svarer kunde med å sende forbruk
            }
            else
                throw new ArgumentException("Feil KID eller passord");
        }

        private void behandleRegelmessig(string husTekst)
        {
            //finn KID og forbruksverdi
            //finn indeks til KID
            //oppdater forbuk til kunden i kundeliste

            int kidLengde = husTekst.IndexOf('.') - husTekst.IndexOf('=');
            int innKID = Convert.ToInt32(husTekst.Substring(husTekst.IndexOf('=') + 1, kidLengde - 1));     //skiller ut KID fra sendt data

            int forbrukLengde = husTekst.IndexOf('#') - husTekst.LastIndexOf('=');
            string innForbruk = husTekst.Substring(husTekst.LastIndexOf('=') + 1, forbrukLengde - 1);  //skiller ut forbruk fra sendt data

            //Console.WriteLine(husTekst);
            KundeListe[IndexKID(innKID)].Forbruk = Convert.ToDouble(innForbruk);
        }

        public double ForbrukForesporsel(int KID)    //sender forespørsel til HUS og returnerer fobruk
        {
            double returVerdi = -1;
            byte[] sendData = Encoding.ASCII.GetBytes("forbruk");   //hus-klassen reagerer på forespørsel som inneholder "forbruk"

            Socket klientSokkel =
                new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                    ProtocolType.Tcp); //oppretter kommunikasjonssokkel
            IPEndPoint klientServer =
                new IPEndPoint(IPAddress.Parse(KundeListe[IndexKID(KID)].IPAdresse),
                    9050); //oppretter server med adresse og portnummer
            klientSokkel.Connect(klientServer); //kobler til server
            klientSokkel.Send(sendData, sendData.Length, SocketFlags.None); //sender data til klient (hus)

            int recv = -1;
            while (recv != 0)
            {
                byte[] mottatData = new byte[1024];
                recv = klientSokkel.Receive(mottatData);    //mottar data fra hus
                string mottattTekst = Encoding.ASCII.GetString(mottatData, 0, recv);
                Console.WriteLine(mottattTekst);

                if (mottattTekst.Contains("kWh"))
                {
                    int forbrukLengde = mottattTekst.IndexOf("kWh") - mottattTekst.IndexOf('=');
                    double funnetForbruk =
                        Convert.ToDouble(mottattTekst.Substring(mottattTekst.IndexOf('=') + 2, forbrukLengde - 2));
                    Console.WriteLine(funnetForbruk);
                    returVerdi = funnetForbruk;
                }
            }
            return returVerdi;
        }
    }

    public class Hus : InterfaceHus
    {
        //kommuniserer med SENTRAL over TCP/IP
        //mottar informasjon fra sensorkort over seriell kommunikasjon

        public double Forbruk { get; set; }
        public int SendSentralIntervall { get; set; }
        public int KID { get; set; }

        private string DatoKlokkeslett;

        [NonSerialized]SerialPort dataPort;  //Må være ikke-serialiserbar for å unngå exception ved serialisering av objektet hus.
        

        char tegn;
        string data = "";
        string PotF, PotG, Dato, Kl;
        double momentantForbruk;

        public Hus(int kid)
        {
            Forbruk = -1;   //foreløpig ikke mottat data om forbruk
            KID = kid;
            SendSentralIntervall = 1;   //standard er 15            
        }
       public void SettCOMPortOgStartTraad(string nyComPort)
        {
            ThreadStart ts = new ThreadStart(MottaFraKort);
            Thread seriellTraad = new Thread(ts);
            dataPort = new SerialPort(nyComPort, 9600, Parity.None, 8);
            try
            {
                seriellTraad.Start();
            }
            catch (Exception ex) //viser feilmelding -da må vi prøve med en annen comPort.
            {
                MessageBox.Show("Feil, ved etablering med av seriell-Tråd. ", ex.Message.ToString()); //error 
            }
        }

        public void MottaFraKort()
        {
            //Åpner kanal for seriell-kommunikasjon

            try
            {
                dataPort.Open();

                if (dataPort.IsOpen)
                {
                    /* 
                     * Bruk følgende (eller tilsvarende) når du jobber med sensorkortet 
                     * Andre mulige kommandoer finner du beskrevet i kort-dokumentasjonen
                     */
                    // dataPort.Write("$S002");

                    while (true)
                    {
                        tegn = Convert.ToChar(dataPort.ReadChar());
                        if (tegn == '#')
                        {
                            data = data + '#';

                            if (data.Length == 65 && data[0] == '\n' && data[data.Length-1] == '#')//Stringen må ha riktig lengde NB sjekk at lengden er 65 hvis det ikke fungerer(65 i simsim), starte og avslutte med riktig tegn
                            {
                                Dato = data.Substring(data.IndexOf("B") + 1, 4);
                                Kl = data.Substring(data.IndexOf("C") + 1, 6);
                                DatoKlokkeslett = Dato + " " + Kl;

                                PotF = data.Substring(data.IndexOf("F") + 1, 4);
                                PotG = data.Substring(data.IndexOf("G") + 1, 4);
                                momentantForbruk = Convert.ToInt16(PotG) + Convert.ToInt16(PotF);

                                Forbruk += momentantForbruk / 3600.00;                                      //legger momentanforbruket til det totale forbruket (Gjør om til KWh)
                                
                                if (data[24] == 1)                                                          //Sjekker om DI 1 er høy
                                    dataPort.Write("$O01");                                                 //LED-lys 0 PÅ - Alarm1: Hus ikke kontakt med sentral
                                else
                                    dataPort.Write("$O00");                                                 //LED-lys 0 AV

                                if (momentantForbruk > 800)
                                    dataPort.Write("$O11");                                                 //LED-lys 1 PÅ -Alarm2:Forbruk over 8KWh 
                                else
                                    dataPort.Write("$O10");                                                 //LED-lys 1 AV

                                if (Math.Abs(Convert.ToInt16(PotF)) - Math.Abs(Convert.ToInt16(PotG)) > 20) //Sjekker etter feil på mer enn 0.2 KWh mellom målerene
                                    dataPort.Write("$O21");                                                 //LED-lys 2 PÅ -Alarm3:Feil på måleutstyr ->Stor differanse (0.2Kw)
                                else
                                    dataPort.Write("$O20");
                            }
                            data = "";
                            Thread.Sleep(800);
                        }
                        else
                        {
                            data = data + tegn;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Feil ved lesing av serielldata. ", ex.Message.ToString());
            }
            finally
            {
                if (dataPort.IsOpen) dataPort.Close();
            }
        }

        public void SendTilKort(string kommando) //finn kommandoene i dokumentasjonen til kortet
        {                                        
            dataPort.Write(kommando);            
        }                                        

        //brukes for å kunne sende forbruk hvert 15 minutt og når HUS får forespørsel fra SENTRAL
        public void SendSentral()
        {
            string serverIP = "127.0.0.1";

            Socket klientSokkel =
                new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                    ProtocolType.Tcp); //oppretter kommunikasjonssokkel
            IPEndPoint
                klientServer =
                    new IPEndPoint(IPAddress.Parse(serverIP), 9050); //oppretter server med adresse og portnummer
            klientSokkel.Connect(klientServer);
            string forbrukSvar = string.Format("TIDSOPPDATERING: KID = {0}. Forbruk = {1}#", KID, 2063);
            byte[] forbrukByte = Encoding.ASCII.GetBytes(forbrukSvar);

            klientSokkel.Send(forbrukByte, forbrukByte.Length, SocketFlags.None);
        }

        //når HUS mottar forespørsel fra SENTRAL svarer den med å sende forbruk tilbake
        public void MottaSentral(string fraAdresse)
        {
            Socket lytteSokkel = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse(fraAdresse), 9050);

            lytteSokkel.Bind(serverEP);
            lytteSokkel.Listen(10);     //venter på forespørsel
            //Console.WriteLine("Venter på forespørsel");
            Socket kommSokkel = lytteSokkel.Accept(); //mottar forespørsel fra klient (sentral)

            //Console.WriteLine("Klient koblet til. Venter på forespørsel");
            string svarString = string.Format("Koblet til HUS med KID: {0}. Venter på forespørsel", KID);
            byte[] sendData = Encoding.ASCII.GetBytes(svarString);
            kommSokkel.Send(sendData, sendData.Length, SocketFlags.None); //hus svarer når sentral er koblet til

            int recv = -1;
            while (recv != 0)
            {
                byte[] mottatData = new byte[1024];
                recv = kommSokkel.Receive(mottatData);
                string mottattTekst = Encoding.ASCII.GetString(mottatData, 0, recv);
                mottattTekst = mottattTekst.ToLower();

                if (mottattTekst.Contains("forbruk"))   //betyr at sentral sendte forespørsel om forbruk
                {
                    svarString = string.Format("HUS: {0}. Forbruk = {1}kWh", KID, Forbruk);
                    sendData = Encoding.ASCII.GetBytes(svarString);
                    kommSokkel.Send(sendData, sendData.Length, SocketFlags.None); //hus svarer sentral med å sende forbruk
                }
                else
                {
                    svarString =
                        string.Format("Koblet til HUS med KID: {0}. skriv \"Forbruk\" for å motta husets forbruk", KID);
                    sendData = Encoding.ASCII.GetBytes(svarString);
                    kommSokkel.Send(sendData, sendData.Length, SocketFlags.None);   //hus svarer sentral med å sende instruksjon for forbruk
                }
            }
            lytteSokkel.Close();
        }
    }

    public class Kunde      //ferdig med klassen
    {
        public int KID { get; set; }
        public double Forbruk { get; set; }
        public string KundeNavn { get; set; }
        public string Adresse { get; set; }
        public string IPAdresse { get; set; }

        private string passord;
        public string Passord
        {
            get { return passord; }
            set
            {
                if (value.Contains(',') || value.Contains('#'))     //sjekker om passordet inneholder ',' som er et ugyldig tegn
                    throw new ArgumentOutOfRangeException("Passord kan ikke inneholde ',' eller '#'.");
                else
                    passord = value;
            }
        }

        public Kunde(string innNavn, string innAdresse, int innKID, string innPassord, string innIPadresse)
        {
            KundeNavn = innNavn;
            Adresse = innAdresse;
            KID = innKID;
            Passord = innPassord;
            IPAdresse = innIPadresse;
        }

        




        public Kunde(int innKID, string innPassord)
        {
            KID = innKID;
            Passord = innPassord;
        }

        public void SendLoggInn()    //kommuniserer med SENTRAL over TCP/IP
        {
            string sendText = "LOGGINN=" + KID + "," + passord + "#";   //setter KID og passord sammen til en string
            byte[]
                sendData = Encoding.ASCII.GetBytes(sendText);   //konverterer stringen til bytes som kans endes over TCP

            Socket klientSokkel =
                new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                    ProtocolType.Tcp);      //oppretter kommunikasjonssokkel
            IPEndPoint klientServer =
                    new IPEndPoint(IPAddress.Parse(IPAdresse), 9050);    //oppretter server med adresse og portnummer
            klientSokkel.Connect(klientServer); //kobler til server
            klientSokkel.Send(sendData, sendData.Length, SocketFlags.None);     //sender data til klient (sentral)

            //trenger ikke: while(recv != 0)
            byte[] mottatData = new byte[1024];
            int recv = klientSokkel.Receive(mottatData);
            string mottattTekst = Encoding.ASCII.GetString(mottatData, 0, recv);

            Console.WriteLine("Mottat tekst: " + mottattTekst);
            Forbruk = Convert.ToDouble(mottattTekst);

            klientSokkel.Close();
        }
    }
}

