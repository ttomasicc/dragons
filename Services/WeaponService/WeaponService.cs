using AutoMapper;
using dragons.Data;
using dragons.Dtos.Character;
using dragons.Dtos.Weapon;
using dragons.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace dragons.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public WeaponService(DataContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto weapon)
        {
            ServiceResponse<GetCharacterDto> response = new();
            try
            {
                Character? character = await _context.Characters
                    .FirstOrDefaultAsync(character => character.Id == weapon.CharacterId
                        && character.User.Id == GetUserId());

                if (character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }

                _context.Weapons.Add(new Weapon {
                    Name = weapon.Name,
                    Damage = weapon.Damage,
                    Character = character
                });
                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetCharacterDto>(character);
            } catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private int GetUserId() => int.Parse(
            _httpContextAccessor.HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier)
        );
    }
}