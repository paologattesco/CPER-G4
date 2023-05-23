using ITS.CPER.WebPage.Data.Models;
using ITS.CPER.WebPage.Data.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITS.CPER.WebPage.Data.Controllers;

public class HomeController : Controller
{
    // GET: HomeController
    public async Task<IActionResult> Index([FromServices] InfluxDBService service)
    {
        var results = await service.QueryAsync(async query =>
        {
            var flux = "from(bucket:\"SmartWatchers\") |> range(start: 0)";
            var tables = await query.QueryAsync(flux, "ProjectWork");
            return tables.SelectMany(table =>
                table.Records.Select(record =>
                    new SmartWatch_Data
                    {
                        //Activity_Id = record.ToString,
                        Initial_Latitude = int.Parse(record.ToString()),
                        Initial_Longitude = int.Parse(record.ToString()),
                        //Heartbeat = int.Parse(record.ToString()),
                        NumberOfPoolLaps = int.Parse(record.ToString()),
                        Distance = int.Parse(record.ToString())
                    }));
        });

        return View(results);
    }

    // GET: HomeController/Details/5
    public ActionResult Details(int id)
    {
        return View();
    }

    // GET: HomeController/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: HomeController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    // GET: HomeController/Edit/5
    public ActionResult Edit(int id)
    {
        return View();
    }

    // POST: HomeController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    // GET: HomeController/Delete/5
    public ActionResult Delete(int id)
    {
        return View();
    }

    // POST: HomeController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }
}
