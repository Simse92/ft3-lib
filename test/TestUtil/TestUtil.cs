using System;
using System.Collections.Generic;
using Chromia.Postchain.Client.GTV;

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

    public static byte[] BlockchainAccountId(byte[] chainId)
    {
        var gtv = new List<dynamic>(){
            "B",
            chainId
            }.ToArray();

        return Gtv.Hash(gtv);
    }
}