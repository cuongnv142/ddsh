﻿using DongThucVatQuangTri.Applications.AnimalsAndPlant.SpeciesManage;
using DongThucVatQuangTri.Applications.AnimalsAndPlant.SpeciesManage.Dtos;
using DongThucVatQuangTri.Applications.AnimalsAndPlant.SpeciesNationParkManage;
using DongThucVatQuangTri.Applications.AnimalsAndPlant.SpeciesNationParkManage.Dtos;
using DongThucVatQuangTri.Applications.Banners.ManageBanner;
using DongThucVatQuangTri.Applications.Common;
using DongThucVatQuangTri.Applications.Enums;
using DongThucVatQuangTri.Applications.NewsItem.Dtos.NewsDtos;
using DongThucVatQuangTri.Applications.NewsItem.NewsManage;
using DongThucVatQuangTri.Applications.Tours;
using DongThucVatQuangTri.Applications.Tours.Dtos;
using DongThucVatQuangTri.Models;
using DongThucVatQuangTri.Models.EF;
using DongThucVatQuangTri.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace DongThucVatQuangTri.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IManageBanner _manageBanner;
        private readonly IManageNews _manageNews;
        private readonly IPublicManageSpecies _manageSpecies;
        private readonly IManageTour _manageTour;
        private readonly IManageSpeciesNationPark _manageSpeciesNationPark;
        private readonly DongThucVatContext _context;
        public HomeController(ILogger<HomeController> logger,IManageBanner manageBanner, IManageNews manageNews,
            IPublicManageSpecies manageSpecies, IManageTour manageTour, IManageSpeciesNationPark manageSpeciesNationPark,
            DongThucVatContext context)
        {
            _logger = logger;
            _manageBanner = manageBanner;
            _manageNews = manageNews;
            _manageSpecies = manageSpecies;
            _manageTour = manageTour;
            _manageSpeciesNationPark = manageSpeciesNationPark;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var allBanner =await _manageBanner.GetAll();
            var listBanner  = allBanner.Where(x=>x.Status == 1).ToList();
            var listSpecies = await _manageSpecies.getAllItem();
            var DongVat = listSpecies.Where(x => x.Loai == 1).Take(10).ToList();
            var ThucVat = listSpecies.Where(x => x.Loai == 0).Take(10).ToList();
            var item = new HomeViewModel()
            {
                bannerViews = listBanner,
                donngvat = DongVat,
                thucvat = ThucVat,
            };
            return View(item);
        }
        [HttpGet]
        public async Task<IActionResult> Address(int vqg = 0)
        {
            var data=await _manageSpeciesNationPark.getAllItem();
            data = data.Where(x=>!String.IsNullOrEmpty(x.ViDo)&& !String.IsNullOrEmpty(x.KinhDo)).ToList();
            var listModel = new List<SpeciesNationParkViewModel>();
            if (vqg==0)
            {
                foreach (var item in data)
                {
                    var checkUser = _context.appUsers.Where(x=>x.Id.ToString() == item.CreatedBy).Select(x=>x.Roles).FirstOrDefault();
                    if(checkUser!=null && checkUser.Contains("NationParkMuongTe")){
                        listModel.Add(item);
                    }
                }
            }
            if(vqg==1)
            {
                foreach (var item in data)
                {
                    var checkUser = _context.appUsers.Where(x => x.Id.ToString() == item.CreatedBy).Select(x => x.Roles).FirstOrDefault();
                    if (checkUser != null && checkUser.Contains("NationParkNamGiang"))
                    {
                        listModel.Add(item);
                    }
                }
            }
            var model = new addressModel()
            {
                SpeciesAnimal = listModel.Where(x => x.Loai == 1).ToList(),
                SpeciesPlant = listModel.Where(x => x.Loai == 0).ToList(),
            };
            return View(model);
        }
        public async Task<IActionResult> News( int PageIndex = 1, int PageSize = 5)
        {
            var ListNews =(await _manageNews.getAll()).Where(x=>x.Status==1).OrderByDescending(x => x.PostAt).ToList();
            var HotNews = ListNews.Where(x => x.IsHot == 1).Take(4).ToList();
            var ViewNews = ListNews.OrderByDescending(x=>x.TotalView).Take(4).ToList();
            var request = new GetNewsPagingRequest()
            {
                PageIndex = PageIndex,
                PageSize = PageSize,
            };
            var pagi =await _manageNews.GetAlllPaging(request);
            var NewsFirst = ListNews.OrderByDescending(x => x.PostAt).FirstOrDefault();
            var item = new NewsModels()
            {
                isHotNew = HotNews,
                BannerNews = NewsFirst,
                ListNewsPagi = pagi.ResultObj,
                ViewestNews = ViewNews,
            };
            return View(item);
        }
        [HttpGet]
        public async Task<IActionResult> Tour(int PageIndex = 1, int PageSize = 5)
        {
            var request = new GetTourPagingRequest()
            {
                PageIndex = PageIndex,
                PageSize = PageSize,
            };
            var pagi = await _manageTour.GetAlllPaging(request);
            pagi.ResultObj.Items = pagi.ResultObj.Items.OrderBy(x => x.CreatedAt).ToList();
            return View(pagi.ResultObj);
        }
        [HttpGet]
        public async Task<IActionResult> DetailsTour(int Id)
        {
            var item = await _manageTour.getTourById(Id);
            _manageTour.IncreaseView(Id);
            return View(item);
        }
        [HttpGet]
        public async Task<IActionResult> DetailsNews(int Id)
        {
            var item =await _manageNews.getNewsById(Id);
            _manageNews.IncreaseView(Id);
            return View(item);
        }
        [HttpGet]
        public async Task<IActionResult> introduce(int Id)
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}