using AutoMapper;
using dragons.Data;
using dragons.Dtos.Character;
using dragons.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace dragons.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            ServiceResponse<List<GetCharacterDto>> response = new();
            Character character = _mapper.Map<Character>(newCharacter);

            character.User = await _context.Users.FirstOrDefaultAsync(user => user.Id == GetUserId());

            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            response.Data = await _context.Characters
                .Where(character => character.User.Id == GetUserId())
                .Select(character => _mapper.Map<GetCharacterDto>(character))
                .ToListAsync();

            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            List<Character>? dbCharacters = await _context.Characters
                .Include(character => character.Weapon)
                .Include(character => character.Skills)
                .Where(character => character.User.Id == GetUserId())
                .ToListAsync();

            return new ServiceResponse<List<GetCharacterDto>>
            {
                Data = dbCharacters
                    .Select(character => _mapper.Map<GetCharacterDto>(character))
                    .ToList()
            };
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            Character? dbCharacter = await _context.Characters
                .Include(character => character.Weapon)
                .Include(character => character.Skills)
                .FirstOrDefaultAsync(character => character.Id == id && character.User.Id == GetUserId());

            return new ServiceResponse<GetCharacterDto>
            {
                Data = _mapper.Map<GetCharacterDto>(dbCharacter)
            };
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter)
        {
            ServiceResponse<GetCharacterDto> response = new();

            try
            {
                Character? character = await _context.Characters
                    .Include(character => character.User)
                    .FirstOrDefaultAsync(character => character.Id == updateCharacter.Id);

                if (character.User.Id == GetUserId())
                {
                    character.Name = updateCharacter.Name;
                    character.Strength = updateCharacter.Strength;
                    character.Defense = updateCharacter.Defense;
                    character.Intelligence = updateCharacter.Intelligence;
                    character.Class = updateCharacter.Class;

                    await _context.SaveChangesAsync();

                    response.Data = _mapper.Map<GetCharacterDto>(character);
                } else
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                }
            }
            catch (NullReferenceException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> response = new();

            try
            {
                Character? character = await _context.Characters
                    .FirstOrDefaultAsync(character => character.Id == id && character.User.Id == GetUserId());

                if (character != null)
                {
                    _context.Characters.Remove(character);
                    await _context.SaveChangesAsync();
                    response.Data = _context.Characters
                        .Where(character => character.User.Id == GetUserId())
                        .Include(character => character.Weapon)
                        .Include(character => character.Skills)
                        .Select(character => _mapper.Map<GetCharacterDto>(character))
                        .ToList();
                } else
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto characterSkill)
        {
            ServiceResponse<GetCharacterDto> response = new();
            
            try
            {
                Character? character = await _context.Characters
                    .Include(character => character.Weapon)
                    .Include(character => character.Skills)
                    .FirstOrDefaultAsync(character => character.Id == characterSkill.CharacterId &&
                        character.User.Id == GetUserId());

                if (character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }

                Skill? skill = await _context.Skills
                    .FirstOrDefaultAsync(skill => skill.Id == characterSkill.SkillId);

                if (skill == null)
                {
                    response.Success = false;
                    response.Message = "Skill not found.";
                    return response;
                }

                character.Skills.Add(skill);
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