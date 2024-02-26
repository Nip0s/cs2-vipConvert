
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;
namespace Iks_VIPConvert;

public class PluginConfig : BasePluginConfig
{
    [JsonPropertyName("host")] public string host { get; set; } = "host";
    [JsonPropertyName("database")] public string database { get; set; } = "database";
    [JsonPropertyName("user")] public string user { get; set; } = "user";
    [JsonPropertyName("pass")] public string pass { get; set; } = "pass";
    [JsonPropertyName("port")] public string port { get; set; } = "3306";
    [JsonPropertyName("sid")] public int sid { get; set; } = 2;

    [JsonPropertyName("ConvertVips")]
    public Dictionary<string, List<string>>  ConvertVips { get; set; } = new Dictionary<string, List<string>>()
    {
        {"VipGod", new List<string>() {
            "@css/vipgod"
        }},
        {"VipLite", new List<string>() {
            "@css/viplite"
        }},
        {"VipPremium", new List<string>() {
            "@css/vipprem"
        }}
    };
}
