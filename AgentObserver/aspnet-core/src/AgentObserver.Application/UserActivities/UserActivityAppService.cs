using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using AgentObserver.Authorization;
using AgentObserver.Authorization.Roles;
using AgentObserver.Authorization.Users;
using AgentObserver.Observer;
using AgentObserver.Users.Dto;
using AgentObserver.Configuration;
using Newtonsoft.Json;
using RestSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using AgentObserver.Web;
using Microsoft.Extensions.Logging;

namespace AgentObserver.UserActivities
{
    [AbpAuthorize(PermissionNames.Pages_Users)]
    public class UserActivityAppService : AgentObserverAppServiceBase, IUserActivityAppService
    {
        private readonly IAbpSession _abpSession;
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IConfiguration iConfig;
        private readonly UserManager _userManager;
        private readonly IRepository<UserActivity,int> _activityRepository;
        private readonly IRepository<UserAvailablitiy, int> _userAvailablitiyRepository;
        private readonly IRepository<UserBreaks, int> _userBreaksRepository;
        private readonly IRepository<ActivityUser, int> _activityUserRepository;
        
        public UserActivityAppService(
            Microsoft.AspNetCore.Hosting.IWebHostEnvironment env,
            IConfiguration _iConfig,
            IRepository<UserActivity, int> activityRepository,
            UserManager userManager,
            IAbpSession abpSession,
            IRepository<UserAvailablitiy, int> userAvailablitiyRepository,
            IRepository<ActivityUser, int> activityUserRepository,
            IRepository<UserBreaks, int> userBreaksRepository)
            : base()
        {
            _userManager = userManager;
            _abpSession = abpSession;
            iConfig = _iConfig;
            _activityRepository = activityRepository;
            //_appConfiguration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder(), addUserSecrets: true);
            _userAvailablitiyRepository = userAvailablitiyRepository;
            _userBreaksRepository = userBreaksRepository;
            _activityUserRepository = activityUserRepository;
        }
        public async Task<UsersActivitiesReturnDto> GetUserStats(DateTime? date, string manager, string teamMemberName, bool showIndividual)
        {
            var currentUser = await _userManager.GetUserByIdAsync(_abpSession.UserId.Value);
            var user = (await _activityUserRepository.FirstOrDefaultAsync(p => p.EmployeeId == currentUser.UserName));
            if (!string.IsNullOrEmpty(teamMemberName))
            {
                var teamUserDetails = await _activityUserRepository.FirstOrDefaultAsync(p => p.UserName == teamMemberName);
                if (teamUserDetails != null)
                {
                    if (teamUserDetails.ManagerOrSupervisorName.Trim() == currentUser.FullName.Trim())
                    {
                        user = teamUserDetails;
                    }
                    else throw new UserFriendlyException("You are not authorized to get user Stats");
                }
                else throw new UserFriendlyException("User not found");
            }
            try
            {
                var managerListToCheck = await GetManagerList();
                var managerList = new List<KeyValueDto>();

                if (showIndividual)
                {
                    managerList = await GetManagerList();
                    manager =
                            string.IsNullOrEmpty(manager)
                                ? managerList.FirstOrDefault(p => p.text == currentUser.Name) != null
                                    ? currentUser.Name
                                    : null
                                : manager;
                }
                var data = new UsersActivitiesReturnDto
                {
                    IsRoleManager = managerListToCheck.FirstOrDefault(p => p.text == currentUser.Name) != null ? true : false,
                    IsManagerOrSuperVisor = managerList.FirstOrDefault(p => p.text == currentUser.Name) != null ? true : false,
                    AppUsers = JsonConvert.SerializeObject(await GetUserList()),
                    ManagerOrSuperVisor = JsonConvert.SerializeObject(managerList),
                    Department = JsonConvert.SerializeObject(await GetDepartmentList()),
                    Team = JsonConvert.SerializeObject(await GetTeamList()),
                    Datetime = date != null && Convert.ToDateTime(date) != null ? Convert.ToDateTime(date) : DateTime.Now,
                    Activities = new List<UserActivityReturnDto>()
                };
                var users = new List<string>();
                IQueryable<ActivityUser> userQuery = _activityUserRepository.GetAll();
                if (!string.IsNullOrEmpty(manager))
                {
                    userQuery = userQuery.Where(p => p.ManagerOrSupervisorName.Contains(manager));
                    users.Add(user.UserName);
                }
                if (string.IsNullOrEmpty(manager)) userQuery = userQuery.Where(p => p.UserName.Contains(user.UserName));
                //if (!string.IsNullOrEmpty(department)) userQuery = userQuery.Where(p => p.DepartmentName.Contains(department));
                //if (!string.IsNullOrEmpty(team)) userQuery = userQuery.Where(p => p.TeamName.Contains(team));
                //else userQuery = db.ActivityUser;
                var usersCompleteDetails = await userQuery.ToListAsync();

                users = userQuery.Select(p => p.UserName).Distinct().Take(50).ToList();
                foreach (var record in users)
                {
                    double totalBreakMinutes = 0;
                    var stats = new UserActivityReturnDto();
                    stats.EmpId = usersCompleteDetails.FirstOrDefault(p => p.UserName == record)?.EmployeeId;
                    var appliedDate = data.Datetime;
                    var activities = await _activityRepository.
                                    GetAll()
                                    .Where(p => p.UserName == record && p.ActivityTime.Day == appliedDate.Value.Day && p.ActivityTime.Month == appliedDate.Value.Month && p.ActivityTime.Year == appliedDate.Value.Year)
                                    .ToListAsync();
                    stats.LastUpdateTime = activities.OrderByDescending(p => p.ActivityTime).FirstOrDefault()?.ActivityTime.ToString("dd MMMM yyyy hh:mm tt");
                    var lastUpdatedMinutes = activities.OrderByDescending(p => p.ActivityTime).FirstOrDefault() != null ? activities.OrderByDescending(p => p.ActivityTime).FirstOrDefault()?.ActivityTime.Minute : 0;
                    var lastUpdatedTime = activities.OrderByDescending(p => p.ActivityTime).Where(p => p.ActivityTime.Day == appliedDate.Value.Day && p.ActivityTime.Month == appliedDate.Value.Month && p.ActivityTime.Year == appliedDate.Value.Year).FirstOrDefault() != null ? activities.OrderByDescending(p => p.ActivityTime).FirstOrDefault()?.ActivityTime : null;

                    var activityHoursWithTime = activities
                                        .Select(p => new HourKeyValuePair()
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
                    var splitUserName = record.Split("\\");
                    stats.UserName = record;
                    stats.DisplayName = splitUserName != null && splitUserName.Length > 1 ? splitUserName[1] : record;
                    stats.HourlyActivities = new List<HourBaseActivity>();
                    var availability = await GetUserAvailabilityList(record, data.Datetime);
                    if (availability != null && availability.Count != 0)
                    {
                        stats.CheckinTime = availability.FirstOrDefault()?.CheckinTime.ToString("yyyy-MM-dd hh:mm tt");
                        stats.CheckoutTime = availability.FirstOrDefault()?.CheckoutTime?.ToString("yyyy-MM-dd hh:mm tt");
                        var allBreakTime = await _userBreaksRepository.GetAll().OrderByDescending(p => p.CreatedDateTime)
                                                        .Where(p => p.UserNTLogin == record &&
                                                            p.CreatedDateTime.Day == appliedDate.Value.Day &&
                                                            p.CreatedDateTime.Month == appliedDate.Value.Month &&
                                                            p.CreatedDateTime.Year == appliedDate.Value.Year)
                                                        .ToListAsync();
                        var completedBreaks = allBreakTime.Where(p => p.BreakEndTime != null).ToList();
                        if (allBreakTime != null && allBreakTime.Count > 0)
                        {
                            var breakEndTime = allBreakTime.FirstOrDefault()?.BreakEndTime != null ? allBreakTime.FirstOrDefault()?.BreakEndTime?.ToString("hh:mm tt") : "On Break";
                            stats.BreakTime = allBreakTime.FirstOrDefault()?.BreakStartTime.ToString("hh:mm tt") + " - " + breakEndTime;
                            stats.IsBreak = allBreakTime.FirstOrDefault()?.IsBreak;
                            var breakSpans = new List<TimeSpan>();
                            foreach (var currentSpan in completedBreaks) breakSpans.Add((currentSpan.BreakEndTime.Value - currentSpan.BreakStartTime));
                            if (breakSpans.Count > 0)
                            {
                                totalBreakMinutes = breakSpans.Select(p => p.TotalMinutes).Sum();
                                if (totalBreakMinutes >= 60) stats.BreakDuration = string.Format("{0:%h} hours {0:%m} minutes", TimeSpan.FromMinutes(totalBreakMinutes));
                                if (totalBreakMinutes < 60) stats.BreakDuration = Math.Round(totalBreakMinutes) + " mins";
                            }
                        }
                        var dateToCompare = availability.FirstOrDefault()?.CheckoutTime ?? DateTime.Now;
                        int checkinHour = availability.FirstOrDefault().CheckinTime.Hour;
                        int checkinMins = availability.FirstOrDefault().CheckinTime.Minute;
                        var checkinTimeOrignal = availability.FirstOrDefault().CheckinTime;
                        var uniqueHours = new List<int>();
                        int offset = 0;
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
                            var validTimeSpans = timeSpans.Where(p => p.Minutes >= 6).ToList();
                            var currentSpanMins = timeSpans.Where(p => p.Minutes <= 5).ToList();
                            if (validTimeSpans.Count > 0)
                            {
                                var activeDuration = validTimeSpans.Count != 1 || indexer == 0 || indexer == (uniqueHours.Count - 1) || currentSpanMins.Count > 0 ? 60 - validTimeSpans.Select(p => p.Minutes).Sum() : validTimeSpans.Select(p => p.Minutes).Sum();

                                var inActiveDuration = validTimeSpans.Count != 1 || indexer == 0 || indexer == (uniqueHours.Count - 1) || currentSpanMins.Count > 0 ? Math.Abs(validTimeSpans.Select(p => p.Minutes).Sum() - (checkinTimeOffset ?? 0) - (currentTimeOffset ?? 0)) : Math.Abs(60 - (activeDuration + (currentTimeOffset ?? 0)));
                                int minInActivity = 0;
                                int maxInActivity = 0;
                                if (validTimeSpans.Count == 1) minInActivity = 0;
                                else minInActivity = validTimeSpans.Count != 1 || indexer == 0 || currentSpanMins.Count > 0 || indexer == (uniqueHours.Count - 1) ? (validTimeSpans.Min(p => p.Minutes) <= inActiveDuration ? validTimeSpans.Min(p => p.Minutes) : inActiveDuration) : 0;
                                if (indexer == 0 && checkinTimeOffset > 0 && validTimeSpans.Count != 1)
                                {
                                    var span = validTimeSpans.OrderByDescending(p => p.Minutes).Skip(1).Take(1);
                                    maxInActivity = validTimeSpans.Count != 1 || currentSpanMins.Count > 0 || indexer == 0 || indexer == (uniqueHours.Count - 1) ? span.Max(p => p.Minutes) : inActiveDuration;
                                }
                                else maxInActivity = validTimeSpans.Count != 1 || currentSpanMins.Count > 0 || indexer == 0 || indexer == (uniqueHours.Count - 1) ? (validTimeSpans.Max(p => p.Minutes) - (checkinTimeOffset ?? 0) - (currentTimeOffset ?? 0)) : inActiveDuration;
                                if (minInActivity > maxInActivity)
                                {
                                    int temp = 0;
                                    temp = maxInActivity;
                                    maxInActivity = minInActivity;
                                    minInActivity = temp;
                                };
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
                        if (stats.TotalActiveTimeInMinutes >= 60) stats.TotalActiveTime = string.Format("{0:%h}h {0:%m}m", totalActiveTime);
                        if (totalInActiveMins >= 60) stats.TotalInActiveTime = string.Format("{0:%h}h {0:%m}m", totalInActiveTime);
                        if (stats.TotalActiveTimeInMinutes < 60) stats.TotalActiveTime = string.Format("{0:%m} mins", totalActiveTime);
                        if (totalInActiveMins < 60) stats.TotalInActiveTime = string.Format("{0:%m} mins", totalInActiveTime);

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
                return data;
            }
            catch (Exception)
            {
                throw new UserFriendlyException("User is not on-board to application");
            }
            
        }

        public async Task<List<UserAvailablitiy>> GetUserAvailabilityList(string username, DateTime? date)
        {
            var dateToCompare = date ?? DateTime.Now;
            return await _userAvailablitiyRepository.GetAll().Where(p => p.UserNTLogin == username && p.AvailabilityTime.Day == dateToCompare.Day && p.AvailabilityTime.Month == dateToCompare.Month && p.AvailabilityTime.Year == dateToCompare.Year).ToListAsync();
        }
        //private async Task<bool> VerifyAccess(string teamMember)
        //{
        //    var user =      
        //}
        public async Task<List<KeyValueDto>> GetDepartmentList()
        {
            var record = new List<KeyValueDto>();
            var uniqueDepartment = await _activityUserRepository.GetAll()
                                    .Select(p => p.DepartmentName)
                                    .Distinct()
                                    .ToListAsync();
            uniqueDepartment.ForEach(p => record.Add(new KeyValueDto() { id = p, text = p }));
            return record;
        }
        public async Task<List<KeyValueDto>> GetTeamList()
        {
            var record = new List<KeyValueDto>();
            var uniqueTeam = await _activityUserRepository.GetAll()
                                    .Select(p => p.TeamName)
                                    .Distinct()
                                    .ToListAsync();
            uniqueTeam.ForEach(p => record.Add(new KeyValueDto() { id = p, text = p }));
            return record;
        }
        public async Task<List<KeyValueDto>> GetManagerList()
        {
            var users = new List<KeyValueDto>();
            var uniqueManagers = await _activityUserRepository.GetAll()
                                    .Select(p => p.ManagerOrSupervisorName)
                                    .Distinct()
                                    .ToListAsync();
            uniqueManagers.ForEach(p => users.Add(new KeyValueDto() { id = p, text = p }));
            return users;
        }
        public async Task<List<UserDetailDto>> GetUserList()
        {
            var users = new List<UserDetailDto>();
            users.Add(new UserDetailDto() { EmpId = "", id = "", text = "Select an option" });
            users.AddRange(await _activityUserRepository.GetAll()
                            .Select(p =>
                                        new UserDetailDto
                                        {
                                            text = p.UserName + " - " + p.EmployeeId,
                                            EmpId = p.EmployeeId,
                                            id = p.UserName
                                        })
                            .ToListAsync());
            return users;
        }
        public async Task<ExecutiveStatDto> GetExternalStats(string token)
        {
            string remoteUrl = iConfig.GetSection("Authentication").GetSection("External:StatsApiUrl").Value;
            string authKey = iConfig.GetSection("Authentication").GetSection("External:AuthKey").Value;
            var client = new RestClient(remoteUrl);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AlwaysMultipartFormData = true;
            request.AddParameter("key", authKey);
            //request.AddParameter("log_key", token);
            request.AddParameter("log_key", token);
            IRestResponse response = await client.ExecuteAsync(request);
            var ExecutiveStatDto = JsonConvert.DeserializeObject<ExecutiveStat>(response.Content);
            var result = ObjectMapper.Map<ExecutiveStatDto>(ExecutiveStatDto);
            return result;
        }
    }
}

