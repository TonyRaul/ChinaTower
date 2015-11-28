﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;

namespace ChinaTower.StationPlanning.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        // GET: /
        public IActionResult Index()
        {
            var oneYear = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddYears(-1);
            var StatusStatistics = DB.Towers
                .GroupBy(x => x.Status)
                .Select(x => new { Type = x.Key, Count = x.Count() })
                .ToList();
            ViewBag.CityStatistics = DB.Towers
                .GroupBy(x => x.City)
                .Select(x => new { Type = x.Key, Count = x.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();
            ViewBag.ProviderStatistics = DB.Towers
                .GroupBy(x => x.Provider)
                .Select(x => new { Type = x.Key, Count = x.Count() })
                .ToList();
            ViewBag.DateStatistics = DB.Towers
                .Where(x => x.Status == Models.TowerStatus.正常 || x.Status == Models.TowerStatus.预选)
                .Where(x => x.Time > oneYear)
                .GroupBy(x => new DateTime(x.Time.Year, x.Time.Month, 1))
                .Select(x => new { Date = x.Key, Normal = x.Where(y => y.Status == Models.TowerStatus.正常).Count(), Pre = x.Where(y => y.Status == Models.TowerStatus.预选).Count() })
                .ToList();
            ViewBag.Normal = StatusStatistics.Where(x => x.Type == Models.TowerStatus.正常).SingleOrDefault() == null ? 0 : StatusStatistics.Where(x => x.Type == Models.TowerStatus.正常).SingleOrDefault().Count;
            ViewBag.Store = StatusStatistics.Where(x => x.Type == Models.TowerStatus.储备).SingleOrDefault() == null ? 0 : StatusStatistics.Where(x => x.Type == Models.TowerStatus.储备).SingleOrDefault().Count;
            ViewBag.Hard = StatusStatistics.Where(x => x.Type == Models.TowerStatus.难点).SingleOrDefault() == null ? 0 : StatusStatistics.Where(x => x.Type == Models.TowerStatus.难点).SingleOrDefault().Count;
            ViewBag.Pre = StatusStatistics.Where(x => x.Type == Models.TowerStatus.预选).SingleOrDefault() == null ? 0 : StatusStatistics.Where(x => x.Type == Models.TowerStatus.预选).SingleOrDefault().Count;
            ViewBag.NormalRatio = StatusStatistics.Where(x => x.Type == Models.TowerStatus.正常).SingleOrDefault() == null ? 0 : StatusStatistics.Where(x => x.Type == Models.TowerStatus.正常).SingleOrDefault().Count / StatusStatistics.Sum(x => x.Count);
            ViewBag.StoreRatio = StatusStatistics.Where(x => x.Type == Models.TowerStatus.储备).SingleOrDefault() == null ? 0 : StatusStatistics.Where(x => x.Type == Models.TowerStatus.储备).SingleOrDefault().Count / StatusStatistics.Sum(x => x.Count);
            ViewBag.HardRatio = StatusStatistics.Where(x => x.Type == Models.TowerStatus.难点).SingleOrDefault() == null ? 0 : StatusStatistics.Where(x => x.Type == Models.TowerStatus.难点).SingleOrDefault().Count / StatusStatistics.Sum(x => x.Count);
            ViewBag.PreRatio = 100 - ViewBag.Normal - ViewBag.Store - ViewBag.Hard;
            return View();
        }
    }
}
