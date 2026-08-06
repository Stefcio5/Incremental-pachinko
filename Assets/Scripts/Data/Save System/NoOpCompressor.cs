public class NoOpCompressor : ISaveCompressor
{
    public string Compress(string data) => data;
    public string Decompress(string data) => data;
}
