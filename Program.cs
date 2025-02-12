// See https://aka.ms/new-console-template for more information
using System;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;

namespace ThermalPrinterExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Use either Serial or TCP based on your printer type

            string printerPort = "COM3"; // Use appropriate port for USB (COMx)
            string printerIp = "192.168.1.100"; // Use printer IP for network printer
            int printerPortNumber = 9100; // Default port for network thermal printers

            string textToPrint = "Hello, Thermal Printer!";

            if (args.Length > 0)
            {
                printerPort = args[0];
            }

            // Example for Serial Port (USB or RS232)
            PrintViaSerialPort(printerPort, textToPrint);

            // Example for TCP (Network printer)
            //PrintViaTcp(printerIp, printerPortNumber, textToPrint);
        }

        // Method to print via Serial Port (for USB or RS232)
        static void PrintViaSerialPort(string port, string text)
        {
            //using (SerialPort serialPort = new SerialPort(port))
            //{
            //    serialPort.BaudRate = 9600; // Typically 9600 for thermal printers
            //    serialPort.Parity = Parity.None;
            //    serialPort.StopBits = StopBits.One;
            //    serialPort.DataBits = 8;
            //    serialPort.Open();

            //    // ESC/POS Command to reset the printer and prepare it for printing
            //    byte[] resetCommand = new byte[] { 0x1B, 0x40 }; // ESC @ (reset)
            //    serialPort.Write(resetCommand, 0, resetCommand.Length);                

            //    // ESC/POS command to print text
            //    byte[] textBytes = Encoding.ASCII.GetBytes(text + "\n");

            //    serialPort.Write(textBytes, 0, textBytes.Length);

            //    // ESC/POS command to cut paper
            //    byte[] cutCommand = new byte[] { 0x1D, 0x56, 0x01 }; // ESC i (cut paper)
            //    serialPort.Write(cutCommand, 0, cutCommand.Length);

            //    serialPort.Close();
            //}

            SerialPort serialPort = new SerialPort(port);

            try
            {
                serialPort.BaudRate = 9600; // Typically 9600 for thermal printers
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                serialPort.DataBits = 8;

                Console.WriteLine("Trying to open port {0}", port);
                
                serialPort.Open();
                
                // ESC/POS Command to reset the printer and prepare it for printing
                byte[] resetCommand = new byte[] { 0x1B, 0x40 }; // ESC @ (reset)

                Console.WriteLine("Reseting print command...");

                serialPort.Write(resetCommand, 0, resetCommand.Length);                

                // ESC/POS command to print text
                byte[] textBytes = Encoding.ASCII.GetBytes(text + "\n");

                Console.WriteLine("Sending print command...");

                serialPort.Write(textBytes, 0, textBytes.Length);

                // ESC/POS command to cut paper
                byte[] cutCommand = new byte[] { 0x1D, 0x56, 0x01 }; // ESC i (cut paper)

                Console.WriteLine("Sending paper-cut command...");

                serialPort.Write(cutCommand, 0, cutCommand.Length);

                serialPort.Close();

                Console.WriteLine("Closed port");
            }
            catch(Exception e)
            {                
                Console.WriteLine("Exception: {0}",e.Message);
            }

            Console.WriteLine("Press any key to exit");

            Console.ReadKey();
            
        }

        // Method to print via TCP (Network Printer)
        static void PrintViaTcp(string ipAddress, int port, string text)
        {
            try
            {
                using (TcpClient client = new TcpClient(ipAddress, port))
                using (NetworkStream stream = client.GetStream())
                {
                    // ESC/POS Command to reset the printer and prepare it for printing
                    byte[] resetCommand = new byte[] { 0x1B, 0x40 }; // ESC @ (reset)
                    stream.Write(resetCommand, 0, resetCommand.Length);

                    // ESC/POS command to print text
                    byte[] textBytes = Encoding.ASCII.GetBytes(text + "\n");
                    stream.Write(textBytes, 0, textBytes.Length);

                    // ESC/POS command to cut paper
                    byte[] cutCommand = new byte[] { 0x1D, 0x56, 0x01 }; // ESC i (cut paper)
                    stream.Write(cutCommand, 0, cutCommand.Length);
                }

                Console.WriteLine("Printed via Network Printer!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error printing: " + ex.Message);
            }
        }
    }
}
