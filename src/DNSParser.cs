using System.Data.Common;
using System.Linq;

namespace codecrafters_dns_server;

public class DNSParser
{
    private byte [] request;
    private int id, nq;
    byte [] header = new byte[12];

    public DNSParser(byte [] req){
        request = req;
        id = request[0]<<8 + request[1];
        nq = (request[4]<<8)|request[5];
    }

    public byte [] getHeader(){
        
        header[0] = request[0];
        header[1] = request[1];
        
        int aa=0;
        header[2] = (byte)((1<<7)|(0<<3)|(aa<<2)|(0<<1)|0);
        
        int ra = 0, rz=0, rc=0;
        header[3] = (byte)((ra<<7)|(rz<<4)|rc); 
        
        int na=0;
        header[4] = (byte)(nq >> 8);
        header[5] = (byte)(nq & 0xFF);
        header[6] = (byte)(na >> 8);
        header[7] = (byte)(na & 0xFF);
        
        int nra=0, ara=0; 
        header[8] = (byte)(nra >> 8);
        header[9] = (byte)(nra & 0xFF);
        header[10] = (byte)(ara >> 8);
        header[11] = (byte)(ara & 0xFF);
        
        return header;
    }

    public byte [] getQuestion(){
        List<byte> questions = new List<byte>();
        int reqIdx = 12;
        try{
            for(int i=0; i<nq; i++){
                int labelLength = request[reqIdx];
                while(labelLength!=0){
                    for(int j=0; j<=labelLength; j++){
                        questions.Add(request[reqIdx++]);
                    }
                    labelLength = request[reqIdx];
                }
                questions.Add(0);

                questions.Add(0);
                questions.Add(1);
                
                questions.Add(0);
                questions.Add(1);
                reqIdx+=5;
            }
        }
        catch(IndexOutOfRangeException){
            Console.WriteLine("Index Out of Bound idx: "+reqIdx);
        }
        return questions.ToArray();
    }

    public byte[] getResponse(){
        getHeader();
        byte [] question = getQuestion();
        byte [] response = new byte[header.Length + question.Length];
        Array.Copy(header, 0, response, 0,header.Length);
        Array.Copy(question, 0, response, header.Length, question.Length);
        Console.WriteLine($"Sending Response: {response.Length}");
        Utility.print2Hex(response);
        return response;
    }
}
