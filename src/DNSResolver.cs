using System.Net;
using System.Net.Sockets;

namespace codecrafters_dns_server;

public class DNSResolver
{
    int nq;
    byte[] request;
    DNSParser parser;
    IPEndPoint resolverIp;

    public DNSResolver(byte[] request, IPEndPoint resolverAddress)
    {
        this.request = request;
        parser = new DNSParser(request);
        nq = parser.nq;
        resolverIp = resolverAddress;   
    }

    public byte[] getHeaderForQuery()
    {
        byte [] header = new byte[12];
        Array.Copy(request, header, 12);
        header[4]=0;
        header[5]=1;
        return header;
    }

    private uint getResponseFromResolver(byte[] individualRequest)
    {
        UdpClient udpClient = new UdpClient();
        udpClient.Connect(resolverIp);

        Console.WriteLine("Sending "+individualRequest.Length+" bytes to resolver at "+resolverIp);
        // Utility.print2Hex(individualRequest);

        udpClient.Send(individualRequest, individualRequest.Length);
        
        byte [] receivedData = udpClient.Receive(ref resolverIp);
      
        udpClient.Close();
        
        Console.WriteLine("Received "+receivedData.Length+" bytes from resolver at "+resolverIp);
        Utility.print2Hex(receivedData);
        if((receivedData[3]&0x04)==0x04){
            return 0;
        }
        int reqIdx = 12;
        while(receivedData[reqIdx]>0){
            reqIdx+=receivedData[reqIdx]+1;
        }
        reqIdx+=5;
        
        while(receivedData[reqIdx]>0){
            if((reqIdx&0xC0)==0xC0){
                reqIdx+=2;
            }
            else{
                reqIdx+=receivedData[reqIdx]+1;
            }
        }
        reqIdx+=11;
        uint ret = (uint)((receivedData[reqIdx]<<24)|(receivedData[reqIdx+1]<<16)|(receivedData[reqIdx+2]<<8)|receivedData[reqIdx+3]);
        Console.WriteLine($"Recieved IP {ret}\n");
        return ret;
    }

    public uint[]? getAllIps(byte[] header){
        uint [] ips = new uint [nq];
        int reqIdx=12;
        for(int i=0; i<nq; i++){
            List<byte> ls = new List<byte>();
            reqIdx = parser.populateQuestion(reqIdx, ls);
            byte [] request = new byte[header.Length+ls.Count];
            Array.Copy(header, request, header.Length);
            Array.Copy(ls.ToArray(), 0, request, header.Length, ls.Count);
            ips[i] = getResponseFromResolver(request);
            if(ips[i]==0){
                return null;
            }
        }

        return ips;
    }

    public byte[] getResponse(){
        byte [] header = getHeaderForQuery();
        uint[] queriedIps = getAllIps(header);
        if(queriedIps == null){
            parser.nq =0;
            header = parser.getHeader();
            header[3]=4;
            return header;
        }
        return parser.getResponse(queriedIps);
    }

}
