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
            var character = GetById<Character>(characterId);
            return character != null ? MapToDto(character) : null;
        }

        public List<CharacterDto> GetAllCharacters()
        {
            var characters = GetAll<Character>();
            return characters.OrderBy(c => c.Name)
                           .Select(c => MapToDto(c))
                           .Where(dto => dto != null)
                           .ToList();
        }

        public bool CreateCharacter(CharacterDto characterDto)
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

            return Create(character);
        }

        public bool UpdateCharacter(CharacterDto characterDto)
        {
            var character = GetById<Character>(characterDto.Id);
            if (character == null) return false;

            character.Name = characterDto.Name;
            character.Description = characterDto.Description;
            character.ImageUrl = characterDto.ImageUrl;
            character.Clan = characterDto.Clan;
            character.Status = characterDto.Status;
            character.Occupation = characterDto.Occupation;
            character.BirthDate = characterDto.BirthDate;
            character.DeathDate = characterDto.DeathDate;

            return Update(character);
        }

        public bool DeleteCharacter(Guid characterId)
        {
            return Delete<Character>(characterId);
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
                var relationship = new CharacterRelationship
                {
                    Id = Guid.NewGuid(),
                    SourceCharacterId = character1Id,
                    TargetCharacterId = character2Id,
                    CharacterId1 = character1Id,
                    CharacterId2Id = character2Id,
                    RelationType = relationshipType
                };

                return Create(relationship);
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
                var relationship = _context.CharacterRelationships
                    .FirstOrDefault(cr => (cr.CharacterId1 == character1Id && cr.CharacterId2Id == character2Id) ||
                                         (cr.CharacterId1 == character2Id && cr.CharacterId2Id == character1Id));
                
                if (relationship == null) return false;

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
            return Count<Character>();
        }

        public List<CharacterDto> GetFeaturedCharacters(int count = 5)
        {
            try
            {
                // Простая логика - берем первых персонажей
                var featuredIds = _context.Characters
                    .OrderBy(c => c.Name)
                    .Take(count)
                    .Select(c => c.Id)
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
                    Age = character.Age,
                    Occupation = character.Occupation,
                    Status = character.Status,
                    Clan = character.Clan,
                    BirthDate = character.BirthDate,
                    DeathDate = character.DeathDate,
                    Relationships = relationships
                };
            }
            catch
            {
                return null;
            }
        }
    }
} 