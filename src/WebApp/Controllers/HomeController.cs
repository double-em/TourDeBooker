using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApp.Models;
using WebApp.Service;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMessageSender _messageSender;

        public HomeController(ILogger<HomeController> logger, IMessageSender messageSender)
        {
            _logger = logger;
            _messageSender = messageSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(BookingModel booking)
        {
            switch (booking.ActionType)
            {
                case BookingType.Book:
                    Publisher.PublishMessage(Constants.Channels.Tour.Booked, booking.ToString());
                    break;
                
                case BookingType.Cancel:
                    Publisher.PublishMessage(Constants.Channels.Tour.Cancelled, booking.ToString());
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}