using System.IO.Compression;

namespace DotnetSpider.Enterprise.Agent
{
    public class ZipUtil
    {
        public static void Zip(string srcFile, string dstFile)
        {
            ZipFile.CreateFromDirectory(srcFile, dstFile);
        }

        public static void UnZip(string srcFile, string dstFile)
        {
            ZipFile.ExtractToDirectory(srcFile, dstFile);
        }
    }
}