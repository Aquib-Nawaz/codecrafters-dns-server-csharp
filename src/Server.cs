
using System.Net;
using System.Net.Sockets;

using codecrafters_dns_server;

class Server{

    public static void Main(string[] args){
        // You can use print statements as follows for debugging, they'll be visible when running tests.
        Console.WriteLine("Logs from your program will appear here!");

        // Uncomment this block to pass the first stage
        // // Resolve UDP address
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        int port = 2053;
        IPEndPoint udpEndPoint = new IPEndPoint(ipAddress, port);
        IPEndPoint? resolverIp=null;

        // Create UDP socket
        UdpClient udpClient = new UdpClient(udpEndPoint);

        if(args.Length == 2){
            if(args[0] != "--resolver"){
                Console.WriteLine("Invalid arguments");
                return;
            }
            resolverIp = Utility.getIpFromString(args[1]);
            if(resolverIp == null){
                Console.WriteLine("Invalid resolver address");
                return;
            }
        }

        while (true)
        {
            // Receive data
            IPEndPoint sourceEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receivedData = udpClient.Receive(ref sourceEndPoint);

            Console.WriteLine($"Received {receivedData.Length} bytes from {sourceEndPoint} :");
            Utility.print2Hex(receivedData);
            // Create an empty response
            byte[] response;
            if(resolverIp==null){
                DNSParser dNSParser = new DNSParser(receivedData);
                response = dNSParser.getResponse();
            }
            else{
                DNSResolver dNSResolver = new DNSResolver(receivedData, resolverIp);
                response = dNSResolver.getResponse();
            }
            // Send response
            udpClient.Send(response, response.Length, sourceEndPoint);
        }

    }   
}

