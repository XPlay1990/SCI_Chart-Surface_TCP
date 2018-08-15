
// State object for reading client data asynchronously  
using SciChart.Examples.Examples.Charts3D.CreateRealtime3DCharts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SciChart_RealTime3DSurfaceMesh {
    public class SynchronousSocketClient {

        private CreateRealTime3DSurfaceMeshChart plotter;

        public SynchronousSocketClient(CreateRealTime3DSurfaceMeshChart plotter) {
            this.plotter = plotter;
        }

        public void StartClient() {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];

            // Connect to a remote device.  
            try {
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = System.Net.IPAddress.Parse("127.0.0.1");  //127.0.0.1 as an example

                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 1337);

                // Create a TCP/IP  socket.  
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try {



                    new Thread(() => {
                        sender.Connect(remoteEP);

                        Console.WriteLine("Socket connected to {0}",
                            sender.RemoteEndPoint.ToString());

                        Thread.CurrentThread.IsBackground = true;
                        String incompleteFrame = "";
                        while (true) {
                            // Receive the response from the remote device.
                            int bytesRec = sender.Receive(bytes);
                            incompleteFrame += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                            //switch (encoded) {
                            //    case "\n":
                            //        Console.WriteLine(frames[0]);
                            //        frames.Remove(0);
                            //        break;
                            //    default:
                            //        frames.Add(encoded);
                            //        break;
                            //}
                            String[] frames = incompleteFrame.Split('\n');
                            if (frames.Length > 1) {
                                DateTime before = DateTime.Now;

                                for (int i = 0; i < frames.Length - 1; i++) {
                                    String actualFrame = frames[i];

                                    var charsToRemove = new string[] { "[", "]", "\r", " " };
                                    foreach (var c in charsToRemove) {
                                        actualFrame = actualFrame.Replace(c, string.Empty);
                                    }

                                    String[] splitted = actualFrame.Split(',');

                                    String[] t = new string[] { "1", "2", "3" };

                                    int[] data = Array.ConvertAll(splitted, int.Parse);

                                    //Console.WriteLine("New data: " + string.Join(",", data));

                                    plotter.updateGraph(data);
                                    DateTime after = DateTime.Now;
                                    TimeSpan span = after - before;
                                    int ms = (int)span.TotalMilliseconds;
                                    Console.WriteLine("Time: " + ms + "ms");
                                }
                                incompleteFrame = frames[frames.Length - 1];
                            }
                        }


                    }).Start();

                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                } catch (ArgumentNullException ane) {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                } catch (SocketException se) {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                } catch (Exception e) {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
