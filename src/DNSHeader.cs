namespace codecrafters_dns_server;

public class DNSHeader
{
    private byte [] request;
    public DNSHeader(byte [] req){
        request = req;
    }

    public byte [] getResponse(){
        byte [] response = new byte[12];
        
        response[0] = request[0];
        response[1] = request[1];
        
        int aa=0;
        response[2] = (byte)((1<<7)|(0<<3)|(aa<<2)|(0<<1)|0);
        
        int ra = 0, rz=0, rc=0;
        response[3] = (byte)((ra<<7)|(rz<<4)|rc); 
        
        int nq = 0, na=0;
        response[4] = (byte)(nq >> 8);
        response[5] = (byte)(nq & 0xFF);
        response[6] = (byte)(na >> 8);
        response[7] = (byte)(na & 0xFF);
        
        int nra=0, ara=0; 
        response[8] = (byte)(nra >> 8);
        response[9] = (byte)(nra & 0xFF);
        response[4] = (byte)(ara >> 8);
        response[5] = (byte)(ara & 0xFF);
        
        return response;
    }

}
