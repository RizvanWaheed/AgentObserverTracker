using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Abp;
using Abp.Extensions;
using Abp.Notifications;
using Abp.Timing;
using Abp.Web.Security.AntiForgery;
using AgentObserver.Controllers;
using AgentObserver.Models.TokenAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using RestSharp;
using Newtonsoft.Json;
using Abp.UI;
using AgentObserver.Configuration;
using AgentObserver.Users.Dto;
using System.Collections.Generic;
using Abp.Domain.Repositories;
using AgentObserver.Observer;
using System;
using System.Linq;
using System.Web.Mvc;

namespace AgentObserver.Web.Host.Controllers
{
    public class HomeController : AgentObserverControllerBase
    {
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IRepository<UserActivity, int> _activityRepository;
        private readonly IRepository<UserAvailablitiy, int> _userAvailablitiyRepository;
        private readonly IRepository<UserBreaks, int> _userBreaksRepository;
        private readonly IRepository<ActivityUser, int> _activityUserRepository;
        public HomeController(INotificationPublisher notificationPublisher, IWebHostEnvironment env, 
                                    IRepository<UserAvailablitiy, int> userAvailablitiyRepository,
                                    IRepository<UserActivity, int> activityRepository,
                                    IRepository<ActivityUser, int> activityUserRepository,
                                    IRepository<UserBreaks, int> userBreaksRepository)
        {
            _notificationPublisher = notificationPublisher;
            _appConfiguration = env.GetAppConfiguration();
            _activityRepository = activityRepository;
            _userAvailablitiyRepository = userAvailablitiyRepository;
            _userBreaksRepository = userBreaksRepository;
            _activityUserRepository = activityUserRepository;
        }

        public IActionResult Index()
        {
            return Redirect("/swagger");
        }

        /// <summary>
        /// This is a demo code to demonstrate sending notification to default tenant admin and host admin uers.
        /// Don't use this code in production !!!
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> TestNotification(string message = "")
        {
            if (message.IsNullOrEmpty())
            {
                message = "This is a test notification, created at " + Clock.Now;
            }

            var defaultTenantAdmin = new UserIdentifier(1, 2);
            var hostAdmin = new UserIdentifier(null, 1);

            await _notificationPublisher.PublishAsync(
                "App.SimpleMessage",
                new MessageNotificationData(message),
                severity: NotificationSeverity.Info,
                userIds: new[] { defaultTenantAdmin, hostAdmin }
            );

            return Content("Sent notification: " + message);
        }
        #region desktop app endpoints

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public System.Web.Mvc.ActionResult PostActivity([FromBody] List<UserActivityDto> items)
        {
            try
            {
                foreach (var item in items)
                {
                    var actDate = DateTime.Now;
                    DateTime.TryParse(item.ActivityTime, out actDate);
                    var temp = new UserActivity()
                    {
                        ActivityTime = actDate,
                        UserName = item.UserName,
                        PressedKey = item.PressedKey,
                        ActivityType = item.ActivityType,
                        NetWorkActivity = item.NetWorkActivity
                    };
                    _activityRepository.Insert(temp);
                }
                //db.SaveChanges();

                //var userName = items.FirstOrDefault()?.UserName;
                //var response = userName != null ? GetUserAvailabilityList(userName) : null;
                //if (response == null || response.Count == 0) return Json("No Data");
                //else 
                return new System.Web.Mvc.JsonResult();
            }
            catch (Exception ex)
            {
                return new System.Web.Mvc.JsonResult();
            }
        }

        public Microsoft.AspNetCore.Mvc.ActionResult GetUserAvailability(string username)
        {
            var record = _userAvailablitiyRepository
                            .GetAll()
                            .Where(p => p.UserNTLogin == username 
                                && p.AvailabilityTime.Day == DateTime.Now.Day 
                                && p.AvailabilityTime.Month == DateTime.Now.Month 
                                && p.AvailabilityTime.Year == DateTime.Now.Year)
                            .ToList();
            if (record.FirstOrDefault() != null) 
                record.FirstOrDefault().IsBreak = _userBreaksRepository
                                                    .GetAll()
                                                    .OrderByDescending(p => p.CreatedDateTime)
                                                    .FirstOrDefault(p => p.UserNTLogin == username) != null 
                                                                                                            ? _userBreaksRepository
                                                                                                                .GetAll()
                                                                                                                .OrderByDescending(p => p.CreatedDateTime)
                                                                                                                .FirstOrDefault(p => p.UserNTLogin == username)?.IsBreak 
                                                                                                            : false;
            return Json(record, JsonRequestBehavior.AllowGet);
        }
        public List<UserAvailablitiy> GetUserAvailabilityList(string username, DateTime? date)
        {
            var dateToCompare = date ?? DateTime.Now;
            return _userAvailablitiyRepository
                        .GetAll()
                        .Where(p => p.UserNTLogin == username 
                            && p.AvailabilityTime.Day == dateToCompare.Day 
                            && p.AvailabilityTime.Month == dateToCompare.Month 
                            && p.AvailabilityTime.Year == dateToCompare.Year)
                        .ToList();
        }
        //public Microsoft.AspNetCore.Mvc.ActionResult MarkBreak(string ntLogin)
        //{
        //    var userCheckinStatus = db.UserAvailablitiy.FirstOrDefault(p => p.UserNTLogin == ntLogin && p.AvailabilityTime.Day == DateTime.Now.Day && p.AvailabilityTime.Month == DateTime.Now.Month && p.AvailabilityTime.Year == DateTime.Now.Year);
        //    if (userCheckinStatus != null)
        //    {
        //        var currentTime = DateTime.Now;
        //        if (AddBreak(ntLogin, currentTime))
        //        {
        //            userCheckinStatus.BreakEndTime = null;
        //            userCheckinStatus.IsBreak = true;
        //            userCheckinStatus.BreakStartTime = currentTime;
        //            db.UserAvailablitiy.AddOrUpdate(userCheckinStatus);
        //            db.SaveChanges();
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}
        public Microsoft.AspNetCore.Mvc.ActionResult EndBreak(string ntLogin)
        {
            var userCheckinStatus = _userAvailablitiyRepository
                                        .FirstOrDefault(p => p.UserNTLogin == ntLogin 
                                        && p.AvailabilityTime.Day == DateTime.Now.Day 
                                        && p.AvailabilityTime.Month == DateTime.Now.Month 
                                        && p.AvailabilityTime.Year == DateTime.Now.Year);
            if (userCheckinStatus != null)
            {
                var userBreak = _userBreaksRepository
                                    .GetAll()
                                    .OrderByDescending(p => p.CreatedDateTime)
                                    .FirstOrDefault(p => p.BreakEndTime == null && p.UserNTLogin == ntLogin);
                if (userBreak != null)
                {
                    userBreak.BreakEndTime = DateTime.Now;
                    userBreak.IsBreak = false;
                    _userBreaksRepository.Insert(userBreak);
                }
            }
            return RedirectToAction("Index");
        }
        public bool AddBreak(string ntLogin, DateTime date)
        {
            try
            {
                var UnEndedBreak = _userBreaksRepository
                                        .GetAll()
                                        .OrderByDescending(p => p.CreatedDateTime)
                                        .FirstOrDefault(p => p.UserNTLogin == ntLogin && p.BreakStartTime != null && p.BreakEndTime == null);
                if (UnEndedBreak != null)
                {
                    UnEndedBreak.BreakEndTime = date;
                    UnEndedBreak.IsBreak = false;
                    _userBreaksRepository.InsertOrUpdate(UnEndedBreak);
                }
                _userBreaksRepository.Insert(new UserBreaks()
                {
                    UserNTLogin = ntLogin,
                    BreakEndTime = null,
                    BreakStartTime = date,
                    CreatedDateTime = date,
                    IsBreak = true
                });

            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        //public ActionResult MarkCheckin(string ntLogin)
        //{
        //    var record = db.UserAvailablitiy.FirstOrDefault(p => p.UserNTLogin == ntLogin && p.AvailabilityTime.Day == DateTime.Now.Day && p.AvailabilityTime.Month == DateTime.Now.Month && p.AvailabilityTime.Year == DateTime.Now.Year);
        //    if (record != null)
        //    {
        //        record.IsCheckedIn = true;
        //        record.BreakEndTime = DateTime.Now;
        //        record.IsBreak = false;
        //        db.UserAvailablitiy.AddOrUpdate(record);
        //    }
        //    else
        //    {
        //        db.UserAvailablitiy.Add(new UserAvailablitiy()
        //        {
        //            AvailabilityTime = DateTime.Now,
        //            CheckinTime = DateTime.Now,
        //            IsCheckedIn = true,
        //            IsCheckedOut = false,
        //            UserNTLogin = ntLogin,
        //            BreakStartTime = null,
        //            BreakEndTime = null,
        //            IsBreak = null
        //        });
        //    }
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        //public ActionResult MarkCheckout(string ntLogin)
        //{
        //    var record = db.UserAvailablitiy.FirstOrDefault(p => p.UserNTLogin == ntLogin && p.AvailabilityTime.Day == DateTime.Now.Day && p.AvailabilityTime.Month == DateTime.Now.Month && p.AvailabilityTime.Year == DateTime.Now.Year);
        //    record.IsCheckedOut = true;
        //    record.CheckoutTime = DateTime.Now;
        //    db.UserAvailablitiy.AddOrUpdate(record);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        #endregion
    }
}
