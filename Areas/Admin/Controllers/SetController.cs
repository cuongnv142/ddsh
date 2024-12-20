﻿using DongThucVatQuangTri.Applications.AnimalsAndPlant.BranchManage;
using DongThucVatQuangTri.Applications.AnimalsAndPlant.ClassManage.Dtos;
using DongThucVatQuangTri.Applications.AnimalsAndPlant.ClassManage;
using DongThucVatQuangTri.Applications.Banners.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DongThucVatQuangTri.Applications.AnimalsAndPlant.SetManage;
using Microsoft.AspNetCore.Authorization;
using DongThucVatQuangTri.Applications.AnimalsAndPlant.SetManage.Dtos;
using DongThucVatQuangTri.Applications.Common;

namespace DongThucVatQuangTri.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Policy = "AdministatorOrNationParkPolicy")]
    public class SetController : BaseController
    {
        private readonly IManageSet _manageSet;
        private readonly IManageClass _manageClass;
        public SetController(IManageSet manageSet, IManageClass manageClass)
        {
            _manageClass = manageClass;
            _manageSet = manageSet;
        }
        public async Task<IActionResult> Index(int loai, string keyword = "", int PageIndex = 1, int PageSize = 10)
        {
            var request = new GetSetRequest()
            {
                PageIndex = PageIndex,
                PageSize = PageSize,
                keyword = keyword,
                loai = loai
            };
            if (loai == 1)
            {
                ViewBag.Loai = "Động Vật";
            }
            if (loai == 0)
            {
                ViewBag.Loai = "Thực vật";
            }
            var ListItem = await _manageClass.getAllItem();
            ViewBag.Categories = ListItem.Where(x => x.Loai == loai).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
            });
            var data = await _manageSet.GetAlllPaging(request,true);
            ViewBag.Keyword = keyword;
            if (TempData["result"] != null)
            {
                ViewBag.SuscessMsg = TempData["result"];
            }
            if (TempData["error"] != null)
            {
                ViewBag.ErrorMsg = TempData["error"];
            }
            return View(data.ResultObj);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(int loai, CreateSetRequest request)
        {
            int LoaiDtv = loai;
            
            if (loai == 1)
            {
                ViewBag.Loai = "động vật";
            }
            if (loai == 0)
            {
                ViewBag.Loai = "thực vật";
            }
            if (!ModelState.IsValid)
                return View();
            var result = await _manageSet.createItem(request);
            if (result == -2)
            {
                TempData["error"] = "Bộ đã tồn tại";
                return RedirectToAction("Index", new { loai = LoaiDtv });
            }
            if (result > 0)
            {
                TempData["result"] = "Thêm thành công";
                return RedirectToAction("Index", new { loai = LoaiDtv });
            }
            else
            {
                TempData["error"] = "Thêm không thành công";
                return RedirectToAction("Index", new { loai = LoaiDtv });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int loai, UpdateSetRequest request)
        {
            var checkRole = await _manageSet.getItemById((int)request.Id);
            if (!HelperMethod.CheckUser(checkRole.CreatedBy, User))
            {
                TempData["error"] = "Bạn không được quyền chỉnh sửa";
                return RedirectToAction("Index", new { loai = checkRole.Loai });
            }
            int LoaiDtv = loai;
            var ListItem = await _manageClass.getAllItem();
            ViewBag.Categories = ListItem.Where(x => x.Loai == loai).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
            });
            if (loai == 1)
            {
                ViewBag.Loai = "động vật";
            }
            if (loai == 0)
            {
                ViewBag.Loai = "thực vật";
            }
            if (!ModelState.IsValid)
                return View();

            var result = await _manageSet.updateItem(request);
            if (result == -2)
            {
                TempData["error"] = "Bộ đã tồn tại";
                return RedirectToAction("Index", new { loai = LoaiDtv });

            }
            if (result > 0)
            {
                TempData["result"] = "Cập nhật thông tin thành công";
                return RedirectToAction("Index", new { loai = LoaiDtv });

            }
            TempData["error"] = "Cập nhật không thành công";
            return RedirectToAction("Index", new { loai = LoaiDtv });
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int loai, int Id)
        {
            int LoaiDtv = loai;
            if (loai == 1)
            {
                ViewBag.Loai = "Động Vật";
            }
            if (loai == 0)
            {
                ViewBag.Loai = "Thực vật";
            }
            if (!ModelState.IsValid)
                return RedirectToAction("Index", new { loai = LoaiDtv });
            var item = await _manageSet.getItemById(Id);
            if (!HelperMethod.CheckUser(item.CreatedBy, User))
            {
                TempData["error"] = "Bạn không được quyền xóa";
                return RedirectToAction("Index", new { loai = item.Loai });
            }
            var result = await _manageSet.deleteItem(Id);
            if (result > 0)
            {
                TempData["result"] = "Xóa  thành công";
                return RedirectToAction("Index", new { loai = LoaiDtv });

            }
            TempData["error"] = "Xóa không thành công";
            return RedirectToAction("Index", new { loai = LoaiDtv });
        }
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(ChangeStatusRequest request)
        {
            var item = await _manageSet.getItemById(request.Id);
            if (!HelperMethod.CheckUser(item.CreatedBy, User))
            {
                return Json(new { success = false, message = "Bạn không có quyền thay đổi" });
            }
            var result = await _manageSet.ChangeStatus(request);
           
            if (result > 0)
            {
                return Json(new { success = true, message = "Thuộc tính đã được thay đổi." });
            }
            return Json(new { success = true, message = "Thuộc tính không được thay đổi." });
        }
        [HttpGet]
        public async Task<IActionResult> Details(int Id)
        {

            var result = await _manageSet.getItemById(Id);
            var loai = result.Loai;
            if (loai == 1)
            {
                ViewBag.Loai = "Động Vật";
            }
            if (loai == 0)
            {
                ViewBag.Loai = "Thực vật";
            }
            return View(result);
        }
    }
}
