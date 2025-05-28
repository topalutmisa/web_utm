using System;
using System.Linq;
using System.Web.Mvc;
using VinlandSaga.Application.BussinessLogic;
using VinlandSaga.Application.BussinessLogic.Interfaces;
using VinlandSaga.Domain.DTOs;
using VinlandSaga.Web.Models;

namespace VinlandSaga.Web.Controllers
{
    public class CharactersController : Controller
    {
        private readonly ICharacterBL _characterBL;

        public CharactersController()
        {
            var factory = BusinessLogicFactory.Instance;
            _characterBL = factory.GetCharacterBL();
        }

        public ActionResult Index(string search = "")
        {
            try
            {
                var characters = string.IsNullOrEmpty(search) 
                    ? _characterBL.GetAllCharacters()
                    : _characterBL.SearchCharacters(search);
                
                var model = new CharactersListViewModel
                {
                    Characters = characters.Select(c => new CharacterViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        ImageUrl = c.ImageUrl,
                        Occupation = c.Occupation,
                        Status = c.Status,
                        Clan = c.Clan
                    }).ToList(),
                    SearchTerm = search
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ошибка при загрузке персонажей: " + ex.Message;
                return View(new CharactersListViewModel());
            }
        }

        public ActionResult Details(Guid id)
        {
            try
            {
                var character = _characterBL.GetCharacter(id);
                if (character == null)
                {
                    return HttpNotFound();
                }
                
                var model = new CharacterViewModel
                {
                    Id = character.Id,
                    Name = character.Name,
                    Description = character.Description,
                    ImageUrl = character.ImageUrl,
                    Occupation = character.Occupation,
                    Status = character.Status,
                    Clan = character.Clan,
                    BirthDate = character.BirthDate,
                    DeathDate = character.DeathDate
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ошибка при загрузке персонажа: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Administrator,Moderator")]
        public ActionResult Create()
        {
            return View(new CreateCharacterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Moderator")]
        public ActionResult Create(CreateCharacterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var characterDto = new CharacterDto
                {
                    Name = model.Name,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    Occupation = model.Occupation,
                    Status = model.Status,
                    Clan = model.Clan,
                    BirthDate = model.BirthDate,
                    DeathDate = model.DeathDate
                };
                
                var success = _characterBL.CreateCharacter(characterDto);
                
                if (success)
                {
                    TempData["SuccessMessage"] = "Персонаж успешно создан!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Ошибка при создании персонажа");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка при создании персонажа: " + ex.Message);
                return View(model);
            }
        }

        [Authorize(Roles = "Administrator,Moderator")]
        public ActionResult Edit(Guid id)
        {
            try
            {
                var character = _characterBL.GetCharacter(id);
                if (character == null)
                {
                    return HttpNotFound();
                }
                
                var model = new EditCharacterViewModel
                {
                    Id = character.Id,
                    Name = character.Name,
                    Description = character.Description,
                    ImageUrl = character.ImageUrl,
                    Occupation = character.Occupation,
                    Status = character.Status,
                    Clan = character.Clan,
                    BirthDate = character.BirthDate,
                    DeathDate = character.DeathDate
                };
                
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ошибка при загрузке персонажа: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Moderator")]
        public ActionResult Edit(EditCharacterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var characterDto = new CharacterDto
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    Occupation = model.Occupation,
                    Status = model.Status,
                    Clan = model.Clan,
                    BirthDate = model.BirthDate,
                    DeathDate = model.DeathDate
                };
                
                var success = _characterBL.UpdateCharacter(characterDto);
                
                if (success)
                {
                    TempData["SuccessMessage"] = "Персонаж успешно обновлен!";
                    return RedirectToAction("Details", new { id = model.Id });
                }
                else
                {
                    ModelState.AddModelError("", "Ошибка при обновлении персонажа");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка при обновлении персонажа: " + ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                var success = _characterBL.DeleteCharacter(id);
                
                if (success)
                {
                    TempData["SuccessMessage"] = "Персонаж успешно удален!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ошибка при удалении персонажа";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при удалении персонажа: " + ex.Message;
            }
            
            return RedirectToAction("Index");
        }
    }
} 