using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyWebSite.Models;
using ParkyWebSite.Repository.IRepository;
using System.IO;
using System.Threading.Tasks;

namespace ParkyWebSite.Controllers
{
    [Authorize]  //доступ мають авторизовані користувачі
    public class NationalParksController : Controller
    {
        private readonly INationalParkRepository _npRepo;
        public NationalParksController(INationalParkRepository npRepo)
        {
            _npRepo = npRepo;
        }

        public IActionResult Index()
        {
            return View(new NationalPark() { }); //пвертаємо пустий об'єкт(бо загружаючи табличку з даними ми користуємось апі)
        }
        [Authorize(Roles ="Admin")] //доступ мають авторизовані користувачі адміни
        public async Task<IActionResult> Upsert(int? id)
        {
            NationalPark obj = new NationalPark();

            if (id == null)
            {
                //this will be true for Insert/Create
                return View(obj);
            }

            //Flow will come here for update
            obj = await _npRepo.GetAsync(Details.NationalParkApiPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWToken"));
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(NationalPark obj)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray(); //byte[]
                        }
                    }
                    obj.Picture = p1;
                }
                else
                {
                    var objFromDb = await _npRepo.GetAsync(Details.NationalParkApiPath, obj.Id, HttpContext.Session.GetString("JWToken"));
                    obj.Picture = objFromDb.Picture;
                }
                if (obj.Id == 0)
                {
                    await _npRepo.CreateAsync(Details.NationalParkApiPath, obj, HttpContext.Session.GetString("JWToken"));
                }
                else
                {
                    await _npRepo.UpdateAsync(Details.NationalParkApiPath + obj.Id, obj, HttpContext.Session.GetString("JWToken"));
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(obj);
            }
        }

        public async Task<IActionResult> GetAllNationalPark()
        {
            return Json(new { data = await _npRepo.GetAllAsync(Details.NationalParkApiPath, HttpContext.Session.GetString("JWToken")) });
        }
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id) //not null
        {
            var status = await _npRepo.DeleteAsync(Details.NationalParkApiPath, id, HttpContext.Session.GetString("JWToken"));
            if(status)
            {
                return Json(new { success = true, message="Видалення успішне" });
            }
            return Json(new { success = false, message = "Видалення не вдалося" });
        }
    }
}
