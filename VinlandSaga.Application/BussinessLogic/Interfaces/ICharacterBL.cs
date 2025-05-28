using System;
using System.Collections.Generic;
using VinlandSaga.Domain.DTOs;
using VinlandSaga.Domain.Models;

namespace VinlandSaga.Application.BussinessLogic.Interfaces
{
    public interface ICharacterBL
    {
        // CRUD операции
        CharacterDto GetCharacter(Guid characterId);
        List<CharacterDto> GetAllCharacters();
        bool CreateCharacter(CharacterDto characterDto);
        bool UpdateCharacter(CharacterDto characterDto);
        bool DeleteCharacter(Guid characterId);
        
        // Поиск и фильтрация
        List<CharacterDto> SearchCharacters(string searchTerm);
        List<CharacterDto> GetCharactersByClan(string clan);
        List<CharacterDto> GetCharactersByStatus(string status);
        
        // Связи между персонажами
        List<CharacterDto> GetRelatedCharacters(Guid characterId);
        bool AddCharacterRelationship(Guid character1Id, Guid character2Id, string relationshipType);
        bool RemoveCharacterRelationship(Guid character1Id, Guid character2Id);
        
        // Статистика
        int GetCharactersCount();
        List<CharacterDto> GetFeaturedCharacters(int count = 5);
        List<CharacterDto> GetRecentlyAddedCharacters(int count = 10);
    }
} 