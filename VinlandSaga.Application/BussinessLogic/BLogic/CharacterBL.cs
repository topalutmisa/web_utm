using System;
using System.Collections.Generic;
using System.Linq;
using VinlandSaga.Application.BussinessLogic.Core;
using VinlandSaga.Application.BussinessLogic.Interfaces;
using VinlandSaga.Domain.DTOs;
using VinlandSaga.Domain.Models;

namespace VinlandSaga.Application.BussinessLogic.BLogic
{
    public class CharacterBL : BaseApi, ICharacterBL
    {
        public CharacterDto GetCharacter(Guid characterId)
        {
            try
            {
                var character = _context.Characters.FirstOrDefault(c => c.Id == characterId);
                if (character == null) return null;

                return MapToDto(character);
            }
            catch
            {
                return null;
            }
        }

        public List<CharacterDto> GetAllCharacters()
        {
            try
            {
                return _context.Characters
                    .OrderBy(c => c.Name)
                    .Select(c => MapToDto(c))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<CharacterDto>();
            }
        }

        public bool CreateCharacter(CharacterDto characterDto)
        {
            try
            {
                var character = new Character
                {
                    Id = Guid.NewGuid(),
                    Name = characterDto.Name,
                    Description = characterDto.Description,
                    ImageUrl = characterDto.ImageUrl,
                    Clan = characterDto.Clan,
                    Status = characterDto.Status,
                    Occupation = characterDto.Occupation,
                    BirthDate = characterDto.BirthDate,
                    DeathDate = characterDto.DeathDate
                };

                _context.Characters.Add(character);
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateCharacter(CharacterDto characterDto)
        {
            try
            {
                var character = _context.Characters.FirstOrDefault(c => c.Id == characterDto.Id);
                if (character == null) return false;

                character.Name = characterDto.Name;
                character.Description = characterDto.Description;
                character.ImageUrl = characterDto.ImageUrl;
                character.Clan = characterDto.Clan;
                character.Status = characterDto.Status;
                character.Occupation = characterDto.Occupation;
                character.BirthDate = characterDto.BirthDate;
                character.DeathDate = characterDto.DeathDate;

                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteCharacter(Guid characterId)
        {
            try
            {
                var character = _context.Characters.FirstOrDefault(c => c.Id == characterId);
                if (character == null) return false;

                _context.Characters.Remove(character);
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<CharacterDto> SearchCharacters(string searchTerm)
        {
            try
            {
                return _context.Characters
                    .Where(c => c.Name.Contains(searchTerm) || 
                               c.Description.Contains(searchTerm) ||
                               c.Clan.Contains(searchTerm))
                    .OrderBy(c => c.Name)
                    .Select(c => MapToDto(c))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<CharacterDto>();
            }
        }

        public List<CharacterDto> GetCharactersByClan(string clan)
        {
            try
            {
                return _context.Characters
                    .Where(c => c.Clan == clan)
                    .OrderBy(c => c.Name)
                    .Select(c => MapToDto(c))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<CharacterDto>();
            }
        }

        public List<CharacterDto> GetCharactersByStatus(string status)
        {
            try
            {
                return _context.Characters
                    .Where(c => c.Status == status)
                    .OrderBy(c => c.Name)
                    .Select(c => MapToDto(c))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<CharacterDto>();
            }
        }

        public List<CharacterDto> GetRelatedCharacters(Guid characterId)
        {
            try
            {
                var relatedIds = _context.CharacterRelationships
                    .Where(cr => cr.CharacterId1 == characterId || cr.CharacterId2Id == characterId)
                    .Select(cr => cr.CharacterId1 == characterId ? cr.CharacterId2Id : cr.CharacterId1)
                    .Distinct()
                    .ToList();

                return _context.Characters
                    .Where(c => relatedIds.Contains(c.Id))
                    .OrderBy(c => c.Name)
                    .Select(c => MapToDto(c))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<CharacterDto>();
            }
        }

        public bool AddCharacterRelationship(Guid character1Id, Guid character2Id, string relationshipType)
        {
            try
            {
                // Проверяем, что связь не существует
                if (_context.CharacterRelationships.Any(cr => 
                    (cr.CharacterId1 == character1Id && cr.CharacterId2Id == character2Id) ||
                    (cr.CharacterId1 == character2Id && cr.CharacterId2Id == character1Id)))
                {
                    return true; // Уже существует
                }

                var relationship = new CharacterRelationship
                {
                    Id = Guid.NewGuid(),
                    SourceCharacterId = character1Id,
                    TargetCharacterId = character2Id,
                    CharacterId1 = character1Id,
                    CharacterId2Id = character2Id,
                    RelationType = relationshipType
                };

                _context.CharacterRelationships.Add(relationship);
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveCharacterRelationship(Guid character1Id, Guid character2Id)
        {
            try
            {
                var relationship = _context.CharacterRelationships.FirstOrDefault(cr =>
                    (cr.CharacterId1 == character1Id && cr.CharacterId2Id == character2Id) ||
                    (cr.CharacterId1 == character2Id && cr.CharacterId2Id == character1Id));

                if (relationship == null) return true; // Уже нет

                _context.CharacterRelationships.Remove(relationship);
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int GetCharactersCount()
        {
            try
            {
                return _context.Characters.Count();
            }
            catch
            {
                return 0;
            }
        }

        public List<CharacterDto> GetFeaturedCharacters(int count = 5)
        {
            try
            {
                // Возвращаем персонажей с наибольшим количеством связей
                var featuredIds = _context.CharacterRelationships
                    .GroupBy(cr => cr.CharacterId1)
                    .OrderByDescending(g => g.Count())
                    .Take(count)
                    .Select(g => g.Key)
                    .ToList();

                return _context.Characters
                    .Where(c => featuredIds.Contains(c.Id))
                    .OrderBy(c => c.Name)
                    .Select(c => MapToDto(c))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<CharacterDto>();
            }
        }

        public List<CharacterDto> GetRecentlyAddedCharacters(int count = 10)
        {
            try
            {
                return _context.Characters
                    .OrderByDescending(c => c.Id) // Предполагаем, что новые ID больше
                    .Take(count)
                    .Select(c => MapToDto(c))
                    .Where(dto => dto != null)
                    .ToList();
            }
            catch
            {
                return new List<CharacterDto>();
            }
        }

        private CharacterDto MapToDto(Character character)
        {
            if (character == null) return null;

            try
            {
                // Получаем связи персонажа
                var relationships = _context.CharacterRelationships
                    .Where(cr => cr.CharacterId1 == character.Id || cr.CharacterId2Id == character.Id)
                    .Select(cr => cr.RelationType)
                    .Distinct()
                    .ToList();

                return new CharacterDto
                {
                    Id = character.Id,
                    Name = character.Name,
                    Description = character.Description,
                    ImageUrl = character.ImageUrl,
                    Clan = character.Clan,
                    Status = character.Status,
                    Occupation = character.Occupation,
                    BirthDate = character.BirthDate,
                    DeathDate = character.DeathDate,
                    Relationships = relationships,
                    Appearances = new List<string>() // Можно расширить позже
                };
            }
            catch
            {
                return null;
            }
        }
    }
} 