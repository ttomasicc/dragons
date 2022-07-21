using AutoMapper;
using dragons.Data;
using dragons.Dtos.Fight;
using dragons.Models;
using Microsoft.EntityFrameworkCore;

namespace dragons.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public FightService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            ServiceResponse<FightResultDto> response = new()
            {
                Data = new FightResultDto()
            };

            try
            {
                List<Character>? characters = await _context.Characters
                    .Include(character => character.Weapon)
                    .Include(character => character.Skills)
                    .Where(character => request.CharacterIds.Contains(character.Id))
                    .ToListAsync();

                bool defeated = false;
                while (!defeated)
                {
                    foreach (Character attacker in characters)
                    {
                        List<Character>? opponents = characters.Where(character => character.Id != attacker.Id).ToList();
                        Character? opponent = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        string attackUsed = string.Empty;

                        bool useWeapon = new Random().Next(2) == 0;
                        if (useWeapon)
                        {
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker, opponent);
                        } else
                        {
                            Skill skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            attackUsed = skill.Name;
                            damage = DoSkillAttack(attacker, opponent, skill);
                        }

                        response.Data.Log
                            .Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage");

                        if (opponent.HitPoints <= 0)
                        {
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;

                            response.Data.Log
                                .Add($"{opponent.Name} has been defeated!");
                            response.Data.Log
                                .Add($"{attacker.Name} won with {attacker.HitPoints}HP left!");

                            break;
                        }
                    }
                }

                characters.ForEach(character =>
                {
                    character.Fights++;
                    character.HitPoints = 100;
                });

                await _context.SaveChangesAsync();

            } catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
        {
            ServiceResponse<AttackResultDto> response = new();

            try
            {
                Character? attacker = await _context.Characters
                    .Include(character => character.Skills)
                    .FirstOrDefaultAsync(character => character.Id == request.AttackerId);

                Character? opponent = await _context.Characters
                    .Include(character => character.Skills)
                    .FirstOrDefaultAsync(character => character.Id == request.OpponentId);

                Skill? skill = attacker.Skills.FirstOrDefault(skill => skill.Id == request.SkillId);
                
                if (skill == null)
                {
                    response.Success = false;
                    response.Message = $"{attacker.Name} doesn't know that skill.";
                    return response;
                }

                int damage = DoSkillAttack(attacker, opponent, skill);

                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated!";

                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            ServiceResponse<AttackResultDto> response = new();

            try
            {
                Character? attacker = await _context.Characters
                    .Include(character => character.Weapon)
                    .FirstOrDefaultAsync(character => character.Id == request.AttackerId);

                Character? opponent = await _context.Characters
                    .Include(character => character.Weapon)
                    .FirstOrDefaultAsync(character => character.Id == request.OpponentId);

                int damage = DoWeaponAttack(attacker, opponent);

                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated!";

                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };

            } catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private static int DoWeaponAttack(Character attacker, Character opponent)
        {
            int damage = attacker.Weapon.Damage + new Random().Next(attacker.Strength);
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
                opponent.HitPoints -= damage;

            return damage;
        }

        private static int DoSkillAttack(Character attacker, Character opponent, Skill skill)
        {
            int damage = skill.Damage + new Random().Next(attacker.Intelligence);
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
                opponent.HitPoints -= damage;

            return damage;
        }

        public async Task<ServiceResponse<List<HighscoreDto>>> GetHighscore()
        {
            List<Character>? characters = await _context.Characters
                .Where(character => character.Fights > 0)
                .OrderByDescending(character => character.Victories)
                .ThenBy(character => character.Defeats)
                .ToListAsync();

            return new ServiceResponse<List<HighscoreDto>>
            {
                Data = characters.Select(character => _mapper.Map<HighscoreDto>(character)).ToList()
            };
        }
    }
}