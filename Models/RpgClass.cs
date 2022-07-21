using System.Text.Json.Serialization;

namespace dragons.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RpgClass
    {
        Fighter = 1,
        Rogue = 2,
        Magician = 3,
        Ranger = 4,
        Cleric = 5,
    }
}