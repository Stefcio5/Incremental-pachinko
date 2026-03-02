using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

public class GZipCompressor : ISaveCompressor
{
    public string Compress(string data)
    {
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            using var output = new MemoryStream();
            using (var gzip = new GZipStream(output, CompressionMode.Compress))
            {
                gzip.Write(bytes, 0, bytes.Length);
            }
            return Convert.ToBase64String(output.ToArray());
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Compression failed: {e.Message}");
            return data;
        }
    }

    public string Decompress(string data)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(data);
            using var input = new MemoryStream(bytes);
            using var gzip = new GZipStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();
            gzip.CopyTo(output);
            return Encoding.UTF8.GetString(output.ToArray());
        }
        catch
        {
            return data; // Fallback - probably uncompressed
        }
    }
}
