using AutoMapper;
using dragons.Dtos.Character;
using dragons.Dtos.Fight;
using dragons.Dtos.Skill;
using dragons.Dtos.Weapon;
using dragons.Models;
using System;

namespace dragons
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>();
            CreateMap<AddCharacterDto, Character>();
            CreateMap<Weapon, GetWeaponDto>();
            CreateMap<Skill, GetSkillDto>();
            CreateMap<Character, HighscoreDto>();
        }
    }
}