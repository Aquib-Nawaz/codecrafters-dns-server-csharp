namespace codecrafters_dns_server;


public static class Utility
{
    public static void print2Hex(byte [] array){
        string hexRepresentation = BitConverter.ToString(array).Replace("-", " ");
        int length = hexRepresentation.Length;
        for(int i =0; i<length; i+=48){
            Console.Write(hexRepresentation.Substring(i, Math.Min(24, length-i)));
            if(i+24<length){
                Console.Write(" ");
                Console.WriteLine(hexRepresentation.Substring(i+24, Math.Min(24, length-i-24)));
            }
            else{
                Console.WriteLine();
            }
        }
    }

    public static void addBigEndianToList(List<byte> list, int value, int length){
        for(int i=length-1; i>=0; i--){
            list.Add((byte)((value >> (i*8)) & 0xFF));
        }
    
    }
}
