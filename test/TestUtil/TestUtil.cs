using System;

public class TestUtil
{
    public static int GenerateNumber(int max = 10000)
    {
        return (int) Math.Round((double) new Random().Next());
    }

    public static string GenerateAssetName(string prefix = "CHROMA")
    {
        return prefix + "_" + GenerateNumber();
    }

    public static byte[] GenerateId()
    {
        return Chromia.Postchain.Client.Util.Sha256(
            BitConverter.GetBytes(GenerateNumber())
        );
        
    }
}