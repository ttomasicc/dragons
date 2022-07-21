using dragons.Dtos.Character;
using dragons.Dtos.Weapon;
using dragons.Models;

namespace dragons.Services.WeaponService
{
    public interface IWeaponService
    {
        Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto weapon);
    }
}