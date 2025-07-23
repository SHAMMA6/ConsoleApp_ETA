using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ConsoleApp_ETA_eReceipts.Hasher
{
    public static class ETAHashing
    {
        public static string HashedSerializedData(string json)
        {
            using var doc = JsonDocument.Parse(json);
            string serialized = Serialize(doc.RootElement);

            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(serialized));

            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private static string Serialize(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Object => SerializeObject(element),
                JsonValueKind.Array => SerializeArrayWithoutParentKey(element),
                JsonValueKind.String => Quote(element.GetString()),
                JsonValueKind.Number => Quote(element.GetRawText()),
                JsonValueKind.True => Quote("true"),
                JsonValueKind.False => Quote("false"),
                JsonValueKind.Null => Quote("null"),
                _ => Quote(element.ToString())
            };
        }

        private static string SerializeObject(JsonElement obj)
        {
            var sb = new StringBuilder();
            foreach (var prop in obj.EnumerateObject())
            {
                string upperKey = Quote(prop.Name.ToUpperInvariant());

                if (prop.Value.ValueKind == JsonValueKind.Array)
                {
                    sb.Append(upperKey);
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        sb.Append(upperKey);
                        sb.Append(Serialize(item));
                    }
                }
                else
                {
                    sb.Append(upperKey);
                    sb.Append(Serialize(prop.Value));
                }
            }
            return sb.ToString();
        }

        private static string SerializeArrayWithoutParentKey(JsonElement arr)
        {
            var sb = new StringBuilder();
            foreach (var item in arr.EnumerateArray())
                sb.Append(Serialize(item));

            return sb.ToString();
        }

        private static string Quote(string? value) => $"\"{value ?? string.Empty}\"";
    }
}
