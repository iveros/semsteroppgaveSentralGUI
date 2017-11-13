using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//nyeste klasseLib tester opplastning til git

namespace SOKlasseLib
{
    interface InterfaceSentral
    {
        void RegistrerKunde(string innNavn, string innAdresse, int innKID, string innPassord, string ipAdresse);
        void SlettKunde(int KID);
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
        void MottaSentral();
    }

    public class Sentral : InterfaceSentral
    {
        static public List<Kunde> KundeListe;
        private Socket kommSokkel;      //socket for å motta data fra klient
        static public string SentralIP = "127.0.0.1";       //IP-adresse for å koble seg til server

        public Sentral()
        {
            KundeListe = new List<Kunde>();
        }

        public void RegistrerKunde(string innNavn, string innAdresse, int innKID, string innPassord, string ipAdresse)
        {
            KundeListe.Add(new Kunde(innNavn, innAdresse, innKID, innPassord, ipAdresse));
        }

        public void SlettKunde(int KID)
        {
            if (KundeListe[GetKIDIndex(KID)] != null)
                KundeListe.RemoveAt(KID);
        }

        static public int GetKIDIndex(int finnKID)
        {
            int returnIndex = -1;
            for (int i = 0; i < KundeListe.Count; i++)
            {
                if (KundeListe[i].KID == finnKID)
                    returnIndex = i;
            }
            return returnIndex;
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
            IPEndPoint sentralServer = new IPEndPoint(IPAddress.Parse(SentralIP), 9050);    //oppretter server for å motta forespørsel fra HUS og KUNDE

            lytteSokkel.Bind(sentralServer);    //ser etter nye klienter som kobler seg til serverKobling
            lytteSokkel.Listen(10);

            int recv = -1;
            while (recv != 0)   //recv = 0 når det sendes en tom melding
            {
                //Console.Write("\nVenter på tilkobling fra klient. ");
                kommSokkel = lytteSokkel.Accept();   //setter kommSokkel til referanse til nye tilkoblingen

                byte[] mottatData = new byte[1024];
                recv = kommSokkel.Receive(mottatData);      //mottar data fra HUS og KUNDE
                string mottattTekst = Encoding.ASCII.GetString(mottatData, 0, recv);

                if (mottattTekst.Contains("LOGGINN="))      //hvis en KUNDE prøver å logge seg inn
                {
                    //Console.WriteLine("Mottat kundelogginn");
                    behandleLoggInn(mottattTekst);
                }
                else if (mottattTekst.Contains("TIDSOPPDATERING:"))     //regelmessig forbruk fra HUS
                {
                    //Console.WriteLine("Mottat tidsoppdatering fra hus");
                    behandleRegelmessig(mottattTekst);
                }
            }
            lytteSokkel.Close();
        }

        private void behandleLoggInn(string kundeTekst)
        {
            //Console.WriteLine(kundeTekst);

            int kidLengde = kundeTekst.IndexOf(',') - kundeTekst.IndexOf('=');  //brukes til å skille ut KID fra sendt data
            int innKID = Convert.ToInt32(kundeTekst.Substring(
                kundeTekst.IndexOf('=') + 1, kidLengde - 1));     //skiller ut KID fra sendt data

            int passordLengde = kundeTekst.IndexOf('#') - kundeTekst.IndexOf(',');  //brukes til å skille ut passord fra sendt data
            string innPassord = kundeTekst.Substring(
                kundeTekst.IndexOf(',') + 1, passordLengde - 1);  //skiller ut passord fra sendt data

            if (KundeListe[GetKIDIndex(innKID)].KID == innKID &&
                KundeListe[GetKIDIndex(innKID)].Passord.Equals(innPassord)) //hvis både KID og passord er riktig
            {
                double lagretForbruk = KundeListe[GetKIDIndex(innKID)].Forbruk;  //DENNE MÅ ENDRES UT FRA TOLKING AV OPPGAVE (momentan eller oppspart forbruk)

                //Console.WriteLine("Riktig logginn");
                byte[] forbrukSvar = Encoding.ASCII.GetBytes(lagretForbruk.ToString());    //finner forbruk fra HUS-klassen

                //kommSokkel.Connect(klientServer);     //trenger ikke connect fordi er allerede koblet til server
                kommSokkel.Send(forbrukSvar, forbrukSvar.Length, SocketFlags.None);     //svarer kunde gjennom samme kommsokkel som mottak kundedata
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

            ////Console.WriteLine(husTekst);
            KundeListe[GetKIDIndex(innKID)].Forbruk = Convert.ToDouble(innForbruk);     //lagrer mottat forbruk i kundelisten
        }

        public double ForbrukForesporsel(int KID)    //sender forespørsel til HUS og returnerer fobruk
        {
            double returVerdi = -1;
            byte[] sendData = Encoding.ASCII.GetBytes("forbruk");   //hus-klassen reagerer på forespørsel som inneholder ordet "forbruk"

            Socket husSokkel =
                new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                    ProtocolType.Tcp);  //må ha ny kommunikasjonssokkel for ny IPadresse til huset
            IPEndPoint husServer =
                new IPEndPoint(IPAddress.Parse(KundeListe[GetKIDIndex(KID)].IPAdresse),
                    9051);   //oppretter server med IPadresse til huset
            husSokkel.Connect(husServer); //kobler til server
            husSokkel.Send(sendData, sendData.Length, SocketFlags.None); //sender data til klient (hus)

            int recv = -1;
            while (recv != 0)
            {
                byte[] mottatData = new byte[1024];
                recv = husSokkel.Receive(mottatData);    //mottar data fra hus
                string mottattTekst = Encoding.ASCII.GetString(mottatData, 0, recv);    //konverterer mottat data til string
                //Console.WriteLine(mottattTekst);

                if (mottattTekst.Contains("kWh"))   //hvis mottat data inneholder "kWh" betyr det at hus har sendt forbruksdata
                {
                    int forbrukLengde = mottattTekst.IndexOf("kWh") - mottattTekst.IndexOf('=');    //brukes for å skille ut forbruk fra mottat data
                    double funnetForbruk =
                        Convert.ToDouble(mottattTekst.Substring(mottattTekst.IndexOf('=') + 2, forbrukLengde - 2)); //skiller ut forbruket fra mottat data
                    //Console.WriteLine(funnetForbruk);
                    returVerdi = funnetForbruk;
                }
            }
            return returVerdi;
        }
    }

    public class Hus : InterfaceHus     //kjent brukerveiledning til dokumentasjon: MÅ opprette KUNDE før HUS kan opprettes
    {
        //kommuniserer med SENTRAL over TCP/IP
        //mottar informasjon fra sensorkort over seriell kommunikasjon

        public double Forbruk { get; set; }
        public int SendSentralIntervall { get; set; }
        public int KID { get; set; }
        public string EgenIP { get; set; }
        private IPEndPoint SentralServer;

        private string DatoKlokkeslett;

        SerialPort dataPort = new SerialPort("COM3", 9600, Parity.None, 8);

        char tegn;
        string data = "";
        string PotF, PotG, Dato, Kl;
        double momentantForbruk;

        public Hus(int kid)
        {
            Forbruk = -1;   //foreløpig ikke mottat data om forbruk
            KID = kid;
            SendSentralIntervall = 1;   //standard er 15
            ThreadStart ts = new ThreadStart(MottaFraKort);
            Thread leseTraad = new Thread(ts);
            EgenIP = Sentral.KundeListe[Sentral.GetKIDIndex(KID)].IPAdresse;
            SentralServer = new IPEndPoint(IPAddress.Parse(Sentral.SentralIP), 9050); //oppretter server med adresse og portnummer
        }

        public void MottaFraKort()    //Jeg ønsker at vi definerer COM-porten i konstruktøren, ellers må det inn som variabel hver gang vi sender info, og den vil jo "aldri" endre seg?
        {

            dataPort.Open();                                                                        //Åpner kanal for seriell-kommunikasjon

            while (true)                                                                            //Tråden lytter konstant
            {
                if (dataPort.IsOpen)                                                                //Vi har forbindelse
                {
                    tegn = Convert.ToChar(dataPort.ReadChar());                                     //leser symbol fra dataport, lagres på 'tegn'
                    if (tegn == '#')                                                                //dette betyr at vi har mottatt hele strengen
                    {
                        Dato = data.Substring(8, 8);                                                //Vi skiller ut Dato
                        Kl = data.Substring(17, 6);                                                 //-Klokkeslett
                        DatoKlokkeslett = Dato + " " + Kl;

                        PotF = data.Substring(38, 4);                                               //-forbruk1
                        PotG = data.Substring(43, 4);                                               //-og forbruk2

                        momentantForbruk = (Convert.ToInt16(PotF) + Convert.ToInt16(PotG)) / 2.0;     //regner ut snitt av momentanforbruket
                        Forbruk += momentantForbruk / (3600);                                        //legger momentanforbruket til det totale forbruket (Gjør om til KWh)

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
                            dataPort.Write("$O20");                                                 //LED-lys 2 AV
                        data = "";                                                                  //nullstiller data strengen - klar for neste runde
                    }
                    else                                                                            //Hvis ikke #, altså ikke mottatt hele strengen.
                    {
                        data = data + tegn;                                                         //Legger neste tegn til i strengen data
                    }
                }
                else//DET E BRUDD I KOBLINGEN!!!!!!! -skal vi foreta oss noe her?Prøv å etablere pånytt?
                {
                    /*
                    dataPort.Close();
                    dataPort.Dispose(); 
                    dataPort.Open();
                    */
                }
                Thread.Sleep(1000);     //sov (ressurssparing)
            }
        }

        public void SendTilKort(string kommando) //Tenker vi kun sender kommando som variabel, så kan vi ha noen kommandoer å velge mellom i GUI. 
        {                                        // Vi må bestemme når vi skal definere hvilken COM-port som brukes. Jeg vil ha det i konstruktøren.
            dataPort.Write(kommando);            // Dette kan vel være i konstruktøren siden en teknikker(eller noe sånt) installerer systemet for kunden?
        }                                        // Da slipper vi å definere det i alle funksjoner som er avhengig av Seriell kommunikasjon. - Kent

        //brukes for å kunne sende forbruk hvert 15 minutt og når HUS får forespørsel fra SENTRAL
        public void SendSentral()
        {
            Socket klientSokkel =
                new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                    ProtocolType.Tcp); //oppretter kommunikasjonssokkel til sentral
            klientSokkel.Connect(SentralServer);    //kobler til sentralserveren
            string forbrukSvar = string.Format("TIDSOPPDATERING: KID = {0}. Forbruk = {1}#", KID, Forbruk);
            byte[] forbrukByte = Encoding.ASCII.GetBytes(forbrukSvar);

            klientSokkel.Send(forbrukByte, forbrukByte.Length, SocketFlags.None);
        }

        //når HUS mottar forespørsel fra SENTRAL svarer den med å sende forbruk tilbake
        public void MottaSentral()
        {
            Socket lytteSokkel = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint husServer = new IPEndPoint(IPAddress.Parse(EgenIP), 9051);   //oppretter server der HUS mottar forbruksforespørsel fra SENTRAL

            lytteSokkel.Bind(husServer);
            lytteSokkel.Listen(10);     //venter på forespørsel
            ////Console.WriteLine("Venter på forespørsel");
            Socket kommSokkel = lytteSokkel.Accept();   //mottar forespørsel fra klient (sentral)

            ////Console.WriteLine("Klient koblet til. Venter på forespørsel");
            string svarString = string.Format("Koblet til HUS med KID: {0}. Venter på forespørsel", KID);
            byte[] sendData = Encoding.ASCII.GetBytes(svarString);
            kommSokkel.Send(sendData, sendData.Length, SocketFlags.None); //HUS svarer når SENTRAL er koblet til

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
                    kommSokkel.Send(sendData, sendData.Length, SocketFlags.None); //HUS svarer forbruksforespørselen
                    ////Console.WriteLine(svarString);
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
            IPEndPoint SentralServer =
                    new IPEndPoint(IPAddress.Parse(Sentral.SentralIP), 9050);    //oppretter informasjon for å koble til server
            klientSokkel.Connect(SentralServer);    //kobler til sentralserveren
            klientSokkel.Send(sendData, sendData.Length, SocketFlags.None);     //sender data til klient (sentral)

            //trenger ikke: while(recv != 0)
            byte[] mottatData = new byte[1024];
            int recv = klientSokkel.Receive(mottatData);
            string mottattTekst = Encoding.ASCII.GetString(mottatData, 0, recv);

            //Console.WriteLine("Mottat tekst: " + mottattTekst);
            Forbruk = Convert.ToDouble(mottattTekst);   //lagrer mottat forbruk

            klientSokkel.Close();
        }
    }
}

