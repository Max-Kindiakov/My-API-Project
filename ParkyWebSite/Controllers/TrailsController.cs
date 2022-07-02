using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyWebSite.Models;
using ParkyWebSite.Models.ViewModel;
using ParkyWebSite.Repository.IRepository;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWebSite.Controllers
{
    public class TrailsController : Controller
    {
        private readonly INationalParkRepository _npRepo;
        private readonly ITrailRepository _trailRepo;
        public TrailsController(INationalParkRepository npRepo, ITrailRepository trailRepo) //ін'єкйія залежності
        {
            _npRepo = npRepo;
            _trailRepo = trailRepo;
        }

        public IActionResult Index()
        {
            return View(new Trail() { }); //пвертаємо пустий об'єкт(бо загружаючи табличку з даними ми користуємось апі)
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<NationalPark> npList = await _npRepo.GetAllAsync(Details.NationalParkApiPath, HttpContext.Session.GetString("JWToken"));
            TrailsVM objVM = new TrailsVM()
            {
                NationalParkList = npList.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                Trail = new Trail()  //для створення!
            };
           

            if (id == null)
            {
                //this will be true for Insert/Create
                return View(objVM);
            }

            //Flow will come here for update
            objVM.Trail = await _trailRepo.GetAsync(Details.TrailApiPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWToken"));
            if (objVM.Trail == null)
            {
                return NotFound();
            }
            return View(objVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailsVM obj)
        {
            if (ModelState.IsValid) //перевіряємо чи вписані дані в наш трейл
            {
                
                if (obj.Trail.Id == 0)
                {
                    await _trailRepo.CreateAsync(Details.TrailApiPath, obj.Trail, HttpContext.Session.GetString("JWToken"));
                }
                else
                {
                    await _trailRepo.UpdateAsync(Details.TrailApiPath + obj.Trail.Id, obj.Trail, HttpContext.Session.GetString("JWToken"));
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                IEnumerable<NationalPark> npList = await _npRepo.GetAllAsync(Details.NationalParkApiPath, HttpContext.Session.GetString("JWToken"));
                TrailsVM objVM = new TrailsVM()
                {
                    NationalParkList = npList.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    }),
                    Trail = obj.Trail  //для створення!
                };
                return View(objVM);
            }
        }

        public async Task<IActionResult> GetAllTrail()
        {
            return Json(new { data = await _trailRepo.GetAllAsync(Details.TrailApiPath, HttpContext.Session.GetString("JWToken")) });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id) //not null
        {
            var status = await _trailRepo.DeleteAsync(Details.TrailApiPath, id, HttpContext.Session.GetString("JWToken"));
            if(status)
            {
                return Json(new { success = true, message="Видалення успішне" });
            }
            return Json(new { success = false, message = "Видалення не вдалося" });
        }
    }
}
