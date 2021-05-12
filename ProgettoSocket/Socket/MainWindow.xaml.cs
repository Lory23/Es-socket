using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//Aggiunta delle seguenti librerie
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace socket
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool var = true;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCreaSocket_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IPEndPoint sourceSocket = new IPEndPoint(IPAddress.Parse(txtIPR.Text), int.Parse(txtPortR.Text));
                Thread ricezione = new Thread(new ParameterizedThreadStart(SocketReceive));
                ricezione.Start(sourceSocket);
                txtIPR.IsEnabled = false;
                txtPortR.IsEnabled = false;
                ((Button)sender).Visibility = Visibility.Collapsed;
            }
            catch
            {
                MessageBox.Show("Impossibile creare socket", "ErrorShipsh");
            }
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SocketSend(IPAddress.Parse(txtIP.Text), int.Parse(txtPort.Text), txtMsg.Text);
            }
            catch
            {
                MessageBox.Show("Impossibile creare socket", "ErrorShipsh");
            }


        }

        public async void SocketReceive(object socksource)
        {
            IPEndPoint ipendp = (IPEndPoint)socksource;

            Socket t = new Socket(ipendp.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            t.Bind(ipendp);

            Byte[] bytesRicevuti = new Byte[256];

            string message;

            int contaCaratteri = 0;

            await Task.Run(() =>
            {
                while (true)
                {
                    if(t.Available >0)
                    {
                        message = "";

                        contaCaratteri = t.Receive(bytesRicevuti, bytesRicevuti.Length,0);
                        message = message + Encoding.ASCII.GetString(bytesRicevuti, 0, contaCaratteri);

                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {

                            lblRicevi.Content = message;
                        }));

                    }

                }

            });

        }

        public void SocketSend(IPAddress dest, int destport, string message)
        {
            Byte[] byteInviati = Encoding.ASCII.GetBytes(message);

            Socket s = new Socket(dest.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint remote_endpoint = new IPEndPoint(dest, destport);

            s.SendTo(byteInviati, remote_endpoint);
        }
    
    }
}
