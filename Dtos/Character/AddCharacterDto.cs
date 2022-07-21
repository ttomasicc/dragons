using dragons.Models;

namespace dragons.Dtos.Character
{
    public class AddCharacterDto
    {
        public string Name { get; set; } = "Unknown";
        public int Strength { get; set; } = 10;
        public int Defense { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public RpgClass Class { get; set; } = RpgClass.Magician;
    }
}