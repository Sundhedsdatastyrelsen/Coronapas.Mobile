using PeterO.Cbor;

namespace SSICPAS.Core.Services.Utils
{
    public static class CborUtils
    {
        public static string ToJson(byte[] cborDataFormatBytes)
        {
            // Convert from bytes to CBORObject then to jsonString
            CBORObject cborObjectFromBytes = CBORObject.DecodeFromBytes(cborDataFormatBytes);
            string jsonString = cborObjectFromBytes.ToJSONString();

            return jsonString;
        }
    }
}
