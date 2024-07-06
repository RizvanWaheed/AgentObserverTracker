using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using AgentObserverWeb.Models;
using Newtonsoft.Json;

namespace AgentObserverWeb.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db;
        public HomeController()
        {
            db = new ApplicationDbContext();
        }
        public ActionResult Index(string date, string username, string manager, string department, string team)
        {
            var data = new UsersActivitiesReturnDto();
            data.AppUsers = JsonConvert.SerializeObject(GetUserList());
            data.ManagerOrSuperVisor = JsonConvert.SerializeObject(GetManagerList());
            data.Department = JsonConvert.SerializeObject(GetDepartmentList());
            data.Team = JsonConvert.SerializeObject(GetTeamList());
            data.Datetime = date != null && date != "" && Convert.ToDateTime(date) != null ? Convert.ToDateTime(date) : DateTime.Now;
            data.Activities = new List<UserActivityReturnDto>();
            try
            {
                IQueryable<ActivityUser> userQuery = db.ActivityUser;
                if (!string.IsNullOrEmpty(manager)) userQuery = userQuery.Where(p => p.ManagerOrSupervisorName.Contains(manager));
                if (!string.IsNullOrEmpty(username)) userQuery = userQuery.Where(p => p.UserName.Contains(username));
                if (!string.IsNullOrEmpty(department)) userQuery = userQuery.Where(p => p.DepartmentName.Contains(department));
                if (!string.IsNullOrEmpty(team)) userQuery = userQuery.Where(p => p.TeamName.Contains(team));
                //else userQuery = db.ActivityUser;
                var usersCompleteDetails = userQuery.ToList();
                var users = userQuery.Select(p => p.UserName).Distinct().Take(50).ToList();
                foreach (var record in users)
                {
                    double totalBreakMinutes = 0;
                    var stats = new UserActivityReturnDto();
                    stats.EmpId = usersCompleteDetails.FirstOrDefault(p => p.UserName == record)?.EmployeeId;
                    var appliedDate = data.Datetime;
                    var activities = db
                                    .UserActivities
                                    .Where(p => p.UserName == record && p.ActivityTime.Day == appliedDate.Value.Day && p.ActivityTime.Month == appliedDate.Value.Month && p.ActivityTime.Year == appliedDate.Value.Year)
                                    .ToList();
                    stats.LastUpdateTime = activities.OrderByDescending(p => p.ActivityTime).FirstOrDefault()?.ActivityTime.ToString("dd MMMM yyyy hh:mm tt");
                    var lastUpdatedMinutes = activities.OrderByDescending(p => p.ActivityTime).FirstOrDefault() != null ? activities.OrderByDescending(p => p.ActivityTime).FirstOrDefault()?.ActivityTime.Minute : 0;
                    var lastUpdatedTime = activities.OrderByDescending(p => p.ActivityTime).Where(p=>p.ActivityTime.Day == appliedDate.Value.Day && p.ActivityTime.Month == appliedDate.Value.Month && p.ActivityTime.Year == appliedDate.Value.Year).FirstOrDefault() != null ? activities.OrderByDescending(p => p.ActivityTime).FirstOrDefault()?.ActivityTime : null;

                    var activityHoursWithTime = activities
                                        .Select(p => new KeyValuePair()
                                        {
                                            Hour = int.Parse(p.ActivityTime.ToString("HH")),
                                            Time = p.ActivityTime.ToString("tt"),
                                            DefaultHourFormat = p.ActivityTime.Hour
                                        })
                                        .Distinct()
                                        .ToList();
                    stats.ActivityDateTime = appliedDate.Value.ToString("dd MMMM yyyy");
                    stats.TotalActiveTimeInMinutes = 0;
                    stats.TotalInActiveTimeInMinutes = 0;
                    stats.UserName = record;
                    stats.HourlyActivities = new List<HourBaseActivity>();
                    var availability = GetUserAvailabilityList(record, data.Datetime);
                    if (availability != null && availability.Count != 0)
                    {
                        stats.CheckinTime = availability.FirstOrDefault()?.CheckinTime.ToString("yyyy-MM-dd hh:mm tt");
                        stats.CheckoutTime = availability.FirstOrDefault()?.CheckoutTime?.ToString("yyyy-MM-dd hh:mm tt");
                        var allBreakTime = db.UserBreaks.OrderByDescending(p=>p.CreatedDateTime)
                                                        .Where(p => p.UserNTLogin == record && 
                                                            p.CreatedDateTime.Day == appliedDate.Value.Day && 
                                                            p.CreatedDateTime.Month == appliedDate.Value.Month && 
                                                            p.CreatedDateTime.Year == appliedDate.Value.Year)
                                                        .ToList();
                        var completedBreaks = allBreakTime.Where(p => p.BreakEndTime != null).ToList();
                        if (allBreakTime != null && allBreakTime.Count > 0)
                        {
                            var breakEndTime = allBreakTime.FirstOrDefault()?.BreakEndTime != null ? allBreakTime.FirstOrDefault()?.BreakEndTime?.ToString("hh:mm tt") : "On Break";
                            stats.BreakTime = allBreakTime.FirstOrDefault()?.BreakStartTime.ToString("hh:mm tt") + " - " + breakEndTime;
                            stats.IsBreak = allBreakTime.FirstOrDefault()?.IsBreak;
                            var breakSpans = new List<TimeSpan>();
                            foreach (var currentSpan in completedBreaks) breakSpans.Add((currentSpan.BreakEndTime.Value - currentSpan.BreakStartTime));
                            if (breakSpans.Count>0)
                            {
                                totalBreakMinutes = breakSpans.Select(p => p.TotalMinutes).Sum();
                                if (totalBreakMinutes>=60) stats.BreakDuration = string.Format("{0:%h} hours {0:%m} minutes", TimeSpan.FromMinutes(totalBreakMinutes));
                                if (totalBreakMinutes<60) stats.BreakDuration = Math.Round(totalBreakMinutes) + " mins";
                            } 
                            //var span =  (availability.FirstOrDefault()?.BreakEndTime ?? DateTime.Now) - breakTime.Value;
                            //totalBreakMinutes = span.TotalMinutes;
                        }
                       var dateToCompare = availability.FirstOrDefault()?.CheckoutTime ?? DateTime.Now;
                        int checkinHour = availability.FirstOrDefault().CheckinTime.Hour;
                        int checkinMins = availability.FirstOrDefault().CheckinTime.Minute;
                        var checkinTimeOrignal = availability.FirstOrDefault().CheckinTime;
                        var uniqueHours = new List<int>();
                        int offset = 0;
                        //while (checkinHour <= int.Parse(dateToCompare.ToString("hh")))
                        while (checkinHour <= dateToCompare.Hour)
                        {
                            var hour = checkinTimeOrignal.AddHours(offset);
                            var newTime = hour.ToString("HH");
                            uniqueHours.Add(int.Parse(newTime));
                            checkinHour++;
                            offset++;
                        }
                        stats.ShiftHours = "";
                        var indexer = 0;
                        foreach (var item in uniqueHours)
                        {
                            var time = activityHoursWithTime.FirstOrDefault(p => p.Hour == item);
                            var nextTime = activityHoursWithTime.FirstOrDefault(p => p.Hour == (item + 1));
                            var HourActivities = activities.Where(p => p.ActivityTime.Hour == time?.DefaultHourFormat).OrderByDescending(p => p.ActivityTime).ToList();
                            var activeRecords = HourActivities.Where(p => p.ActivityType != "No Activity").ToList();
                            var inActiveRecords = HourActivities.Where(p => p.ActivityType == "No Activity").ToList();
                            var timeSpans = new List<TimeSpan>();
                            var timeToassign = "";
                            if (time != null)
                            {
                                if (time?.Hour == 12) timeToassign = time?.Hour + " " + time?.Time;
                                else if (item > 12 && item < 24) timeToassign = (time?.Hour - 12) + " " + time?.Time;
                                else if (item == 24) timeToassign = (12 - time?.Hour) + " " + time?.Time;
                                else timeToassign = time?.Hour + " " + time?.Time;

                            }
                            else
                            {
                                if (item == 12) timeToassign += item.ToString() + " PM";
                                else if (item > 12 && item < 24) timeToassign += (item - 12).ToString() + " PM";
                                else if (item == 24) timeToassign += (item - 12).ToString() + " AM";
                                else timeToassign += item.ToString() + " AM";
                            };
                            if (nextTime != null)
                            {
                                if (time?.Hour == 12) timeToassign += "-" + nextTime?.Hour + " " + nextTime?.Time;
                                else if (item > 12 && item < 24) timeToassign += "-" + (nextTime?.Hour - 12) + " " + nextTime?.Time;
                                else if (item == 24) timeToassign += "-" + (12 - nextTime?.Hour) + " " + nextTime?.Time;
                                else timeToassign += "-" + nextTime?.Hour + " " + nextTime?.Time;

                            }
                            else
                            {
                                if ((item + 1) == 12) timeToassign += "-" + (item + 1).ToString() + " PM";
                                else if ((item + 1) > 12) timeToassign += "-" + ((item + 1) - 12).ToString() + " PM";
                                else if (item == 24) timeToassign += "-" + ((item + 1) - 12).ToString() + " AM";
                                else timeToassign += "-" + (item + 1).ToString() + " AM";
                            }
                            int? checkinTimeOffset = 0;
                            if (indexer == 0 && checkinMins > 1)
                            {
                                checkinTimeOffset = int.Parse(availability.FirstOrDefault()?.CheckinTime.ToString("HH")) == item ? availability.FirstOrDefault()?.CheckinTime.Minute : 0;
                            }
                            int? currentTimeOffset = 0;
                            int lastUpdatedMins = 0;
                            if (lastUpdatedTime != null)
                            {
                                if (lastUpdatedTime.HasValue && item == lastUpdatedTime.Value.Hour && lastUpdatedTime.Value.Hour == dateToCompare.Hour)
                                {
                                    lastUpdatedMins = lastUpdatedTime.Value.Minute;
                                    currentTimeOffset = 60 - dateToCompare.Minute;
                                    currentTimeOffset = currentTimeOffset + (dateToCompare.Minute - lastUpdatedMins);
                                }
                                if (item != lastUpdatedTime.Value.Hour && item == dateToCompare.Hour)
                                {
                                    lastUpdatedMins = dateToCompare.Minute;
                                    currentTimeOffset = 60 - dateToCompare.Minute;
                                    currentTimeOffset = currentTimeOffset + (dateToCompare.Minute - lastUpdatedMins);
                                }
                            }
                            var totalTimeSpans = new List<TimeSpan>();
                            if (activeRecords.Count != 0)
                            {
                                for (int i = 1; i <= activeRecords.Count; i++)
                                {
                                    if (i == activeRecords.Count)
                                    {
                                        var lastSpan = activeRecords[i - 1]?.ActivityTime != null ? activeRecords[i - 1].ActivityTime.Minute : 0;
                                        timeSpans.Add(new TimeSpan(0, 0, lastSpan, 0));
                                    }
                                    else
                                    {
                                        if (i == 1)
                                        {
                                            var firstSpan = activeRecords[i - 1]?.ActivityTime != null && activeRecords[i - 1].ActivityTime.Minute < 60 ? 60 - activeRecords[i - 1].ActivityTime.Minute : 0;
                                            timeSpans.Add(new TimeSpan(0, 0, firstSpan, 0));
                                        }
                                        var current = activeRecords[i - 1];
                                        var next = activeRecords[i];
                                        timeSpans.Add(current.ActivityTime - next.ActivityTime);
                                    }
                                }
                            }
                            else
                            {
                                stats.HourlyActivities.Add(
                                            new HourBaseActivity()
                                            {
                                                TimeOfDay = timeToassign,
                                                ActiveDuration = 0,
                                                InActiveDuration = 60 - (checkinTimeOffset ?? 0) - currentTimeOffset ?? 0,
                                                MaxContinousInActiveDuration = 60 - (checkinTimeOffset ?? 0) - currentTimeOffset ?? 0,
                                                MinInActiveDuration = 0
                                            });
                                stats.TotalActiveTimeInMinutes = stats.TotalActiveTimeInMinutes + 0;
                                stats.TotalInActiveTimeInMinutes = stats.TotalInActiveTimeInMinutes + (60 - (currentTimeOffset ?? 0) - (checkinTimeOffset ?? 0));
                            }
                            var validTimeSpans = timeSpans.Where(p => p.Minutes > 5).ToList();
                            if (validTimeSpans.Count > 0)
                            {
                                var activeDuration = validTimeSpans.Count != 1 ? 60 -  validTimeSpans.Select(p => p.Minutes).Sum() : validTimeSpans.Select(p => p.Minutes).Sum();
                                var inActiveDuration = validTimeSpans.Count != 1 ? Math.Abs(validTimeSpans.Select(p => p.Minutes).Sum() - (checkinTimeOffset ?? 0) - (currentTimeOffset ?? 0)) : Math.Abs(60-(activeDuration + (currentTimeOffset ?? 0)));
                                int minInActivity = 0;
                                int maxInActivity = 0;
                                if (validTimeSpans.Count == 1) minInActivity = 0;
                                else minInActivity = validTimeSpans.Count != 1 ? (validTimeSpans.Min(p => p.Minutes) <= inActiveDuration ? validTimeSpans.Min(p => p.Minutes) : inActiveDuration) : 0;
                                if (indexer == 0 && checkinTimeOffset > 0 && validTimeSpans.Count != 1)
                                {
                                    var span = validTimeSpans.OrderByDescending(p => p.Minutes).Skip(1).Take(1);
                                    maxInActivity = validTimeSpans.Count != 1 ? span.Max(p => p.Minutes) : inActiveDuration;
                                }
                                else maxInActivity = validTimeSpans.Count != 1 ? (validTimeSpans.Max(p => p.Minutes) - (checkinTimeOffset ?? 0) - (currentTimeOffset ?? 0)) : inActiveDuration;
                                stats.HourlyActivities.Add(
                                        new HourBaseActivity()
                                        {
                                            TimeOfDay = timeToassign,
                                            ActiveDuration = activeDuration,
                                            InActiveDuration = inActiveDuration,
                                            IsNetworkAvailable = HourActivities.FirstOrDefault() != null ? HourActivities.FirstOrDefault().NetWorkActivity : false,
                                            MaxContinousInActiveDuration = Math.Abs(maxInActivity),
                                            MinInActiveDuration = Math.Abs(minInActivity)
                                        });
                                stats.TotalActiveTimeInMinutes = stats.TotalActiveTimeInMinutes + activeDuration;
                                stats.TotalInActiveTimeInMinutes = stats.TotalInActiveTimeInMinutes + inActiveDuration;
                            }
                            if (validTimeSpans.Count == 0 && timeSpans.Count > 0)
                            {
                                timeSpans = timeSpans.Where(p => p.Minutes > 1).ToList();
                                timeSpans = timeSpans.Count > 12 ? timeSpans.Where(p => p.Minutes > 2).Take(12).ToList() : timeSpans;
                                var activeDuration = (5 * timeSpans.Count) - (checkinTimeOffset ?? 0);
                                var inActiveDuration = Math.Abs(60 - (currentTimeOffset ?? 0) - activeDuration);
                                stats.HourlyActivities.Add(
                                        new HourBaseActivity()
                                        {
                                            TimeOfDay = timeToassign,
                                            ActiveDuration = activeDuration,
                                            InActiveDuration = inActiveDuration,
                                            MaxContinousInActiveDuration = 0,
                                            MinInActiveDuration = 0,
                                            IsNetworkAvailable = HourActivities.FirstOrDefault() != null ? HourActivities.FirstOrDefault().NetWorkActivity : false,
                                        });
                                stats.TotalActiveTimeInMinutes = stats.TotalActiveTimeInMinutes + activeDuration;
                                stats.TotalInActiveTimeInMinutes = stats.TotalInActiveTimeInMinutes + inActiveDuration;
                            }
                            indexer++;
                        }
                        var shiftInMinutes = stats.TotalActiveTimeInMinutes + stats.TotalInActiveTimeInMinutes;
                        var shiftSpan = TimeSpan.FromMinutes(stats.TotalActiveTimeInMinutes + stats.TotalInActiveTimeInMinutes);
                        var totalActiveTime = TimeSpan.FromMinutes(stats.TotalActiveTimeInMinutes);
                        var totalInActiveMins = Math.Abs(stats.TotalInActiveTimeInMinutes - totalBreakMinutes);
                        // in active minutes minus total break time
                        var totalInActiveTime = TimeSpan.FromMinutes(totalInActiveMins);
                        //var totalInActiveTime = TimeSpan.FromMinutes(stats.TotalInActiveTimeInMinutes);
                        if (stats.TotalActiveTimeInMinutes >= 60) stats.TotalActiveTime = string.Format("{0:%h} hours {0:%m} minutes", totalActiveTime);
                        if (totalInActiveMins >= 60) stats.TotalInActiveTime = string.Format("{0:%h} hours {0:%m} minutes", totalInActiveTime);
                        if (stats.TotalActiveTimeInMinutes < 60) stats.TotalActiveTime = string.Format("{0:%m} minutes", totalActiveTime);
                        if (totalInActiveMins < 60) stats.TotalInActiveTime = string.Format("{0:%m} minutes", totalInActiveTime);

                        if (shiftInMinutes > 60) stats.ShiftHours = string.Format("{0:%h} hours {0:%m} minutes", shiftSpan);
                        if (shiftInMinutes <= 60) stats.ShiftHours = string.Format("{0:%m} minutes", shiftSpan);
                    }
                    else
                    {
                        stats.CheckinTime = null;
                        stats.CheckoutTime = null;
                    }

                    data.Activities.Add(stats);
                }
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
            return View(data);
        }
        public List<KeyValueDto> GetDepartmentList()
        {
            var record = new List<KeyValueDto>();
            var uniqueDepartment = db.ActivityUser
                                    .Select(p => p.DepartmentName)
                                    .Distinct()
                                    .ToList();
            uniqueDepartment.ForEach(p => record.Add(new KeyValueDto() { id = p, text = p }));
            return record;
        }
        public List<KeyValueDto> GetTeamList()
        {
            var record = new List<KeyValueDto>();
            var uniqueTeam = db.ActivityUser
                                    .Select(p => p.TeamName)
                                    .Distinct()
                                    .ToList();
            uniqueTeam.ForEach(p => record.Add(new KeyValueDto() { id = p, text = p }));
            return record;
        }
        public List<KeyValueDto> GetManagerList()
        {
            var users = new List<KeyValueDto>();
            var uniqueManagers = db.ActivityUser
                                    .Select(p => p.ManagerOrSupervisorName)
                                    .Distinct()
                                    .ToList();
            uniqueManagers.ForEach(p => users.Add(new KeyValueDto() { id = p, text = p }));
            return users;
        }
        public List<UserDetailDto> GetUserList()
        {
            var users = new List<UserDetailDto>();
            users.Add(new UserDetailDto() { EmpId = "", id = "", text = "Select an option"});
            users.AddRange(db.ActivityUser
                            .Select(p => 
                                        new UserDetailDto
                                        {
                                            text = p.UserName+" - "+p.EmployeeId,
                                            EmpId = p.EmployeeId,
                                            id = p.UserName
                                        })
                            .ToList());
            return users;
        }
        [HttpPost]
        public ActionResult PostActivity(List<UserActivityDto> items)
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
                    db.UserActivities.Add(temp);
                }
                db.SaveChanges();

                //var userName = items.FirstOrDefault()?.UserName;
                //var response = userName != null ? GetUserAvailabilityList(userName) : null;
                //if (response == null || response.Count == 0) return Json("No Data");
                //else 
                return Json("Success");
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
        
        public ActionResult GetUserAvailability(string username)
        {
            var record = db.UserAvailablitiy.Where(p => p.UserNTLogin == username && p.AvailabilityTime.Day == DateTime.Now.Day && p.AvailabilityTime.Month == DateTime.Now.Month && p.AvailabilityTime.Year == DateTime.Now.Year).ToList();
            if (record.FirstOrDefault()!=null) record.FirstOrDefault().IsBreak = db.UserBreaks.OrderByDescending(p => p.CreatedDateTime).FirstOrDefault(p => p.UserNTLogin == username) != null ? db.UserBreaks.OrderByDescending(p => p.CreatedDateTime).FirstOrDefault(p => p.UserNTLogin == username)?.IsBreak : false;
            return Json(record, JsonRequestBehavior.AllowGet);
        }
        public List<UserAvailablitiy> GetUserAvailabilityList(string username, DateTime? date)
        {
            var dateToCompare = date ?? DateTime.Now;
            return db.UserAvailablitiy.Where(p => p.UserNTLogin == username && p.AvailabilityTime.Day == dateToCompare.Day && p.AvailabilityTime.Month == dateToCompare.Month && p.AvailabilityTime.Year == dateToCompare.Year).ToList();
        }
        public ActionResult MarkBreak(string ntLogin)
        {
            var userCheckinStatus = db.UserAvailablitiy.FirstOrDefault(p => p.UserNTLogin == ntLogin && p.AvailabilityTime.Day == DateTime.Now.Day && p.AvailabilityTime.Month == DateTime.Now.Month && p.AvailabilityTime.Year == DateTime.Now.Year);
            if (userCheckinStatus != null)
            {
                var currentTime = DateTime.Now;
                if (AddBreak(ntLogin, currentTime))
                {
                    userCheckinStatus.BreakEndTime = null;
                    userCheckinStatus.IsBreak = true;
                    userCheckinStatus.BreakStartTime = currentTime;
                    db.UserAvailablitiy.AddOrUpdate(userCheckinStatus);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult EndBreak(string ntLogin)
        {
            var userCheckinStatus = db.UserAvailablitiy.FirstOrDefault(p => p.UserNTLogin == ntLogin && p.AvailabilityTime.Day == DateTime.Now.Day && p.AvailabilityTime.Month == DateTime.Now.Month && p.AvailabilityTime.Year == DateTime.Now.Year);
            if (userCheckinStatus != null)
            {
                var userBreak = db.UserBreaks.OrderByDescending(p => p.CreatedDateTime).FirstOrDefault(p => p.BreakEndTime == null && p.UserNTLogin == ntLogin);
                if (userBreak != null)
                {
                    userBreak.BreakEndTime = DateTime.Now;
                    userBreak.IsBreak = false;
                    db.UserBreaks.AddOrUpdate(userBreak);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
        public bool AddBreak(string ntLogin, DateTime date)
        {
            try
            {
                var UnEndedBreak = db.UserBreaks.OrderByDescending(p => p.CreatedDateTime).FirstOrDefault(p => p.UserNTLogin == ntLogin && p.BreakStartTime != null && p.BreakEndTime == null);
                if (UnEndedBreak != null)
                {
                    UnEndedBreak.BreakEndTime = date;
                    UnEndedBreak.IsBreak = false;
                    db.UserBreaks.AddOrUpdate(UnEndedBreak);
                }
                db.UserBreaks.Add(new UserBreaks()
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
        public ActionResult MarkCheckin(string ntLogin)
        {
            var record = db.UserAvailablitiy.FirstOrDefault(p => p.UserNTLogin == ntLogin && p.AvailabilityTime.Day == DateTime.Now.Day && p.AvailabilityTime.Month == DateTime.Now.Month && p.AvailabilityTime.Year == DateTime.Now.Year);
            if (record != null)
            {
                record.IsCheckedIn = true;
                record.BreakEndTime = DateTime.Now;
                record.IsBreak = false;
                db.UserAvailablitiy.AddOrUpdate(record);
            }
            else
            {
                db.UserAvailablitiy.Add(new UserAvailablitiy()
                {
                    AvailabilityTime = DateTime.Now,
                    CheckinTime = DateTime.Now,
                    IsCheckedIn = true,
                    IsCheckedOut = false,
                    UserNTLogin = ntLogin,
                    BreakStartTime = null,
                    BreakEndTime = null,
                    IsBreak = null
                });
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult MarkCheckout(string ntLogin)
        {
            var record = db.UserAvailablitiy.FirstOrDefault(p => p.UserNTLogin == ntLogin && p.AvailabilityTime.Day == DateTime.Now.Day && p.AvailabilityTime.Month == DateTime.Now.Month && p.AvailabilityTime.Year == DateTime.Now.Year);
            record.IsCheckedOut = true;
            record.CheckoutTime = DateTime.Now;
            db.UserAvailablitiy.AddOrUpdate(record);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }

}
