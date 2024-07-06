
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using AgentObserverWeb.Models;

namespace AgentObserverWeb.Controllers
{
    public class ActivityController : ApiController
    {
        private ApplicationDbContext db;
        private string authKey = "";
        public ActivityController()
        {
            authKey = WebConfigurationManager.AppSettings["authKey"];
            db = new ApplicationDbContext();
        }
        [HttpGet]
        [ActionName("GetUserAvailability")]
        public IHttpActionResult GetUserAvailability(string empId, string key)
        {
            if (ValidateAuthKey(key))
            {
                var usernTlogin = GetUserNtLoginById(empId);
                if (string.IsNullOrEmpty(usernTlogin))
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("user not found", System.Text.Encoding.UTF8, "text/plain"),
                        StatusCode = HttpStatusCode.NotFound
                    };
                    return Ok(response);
                }
                var record = db.UserAvailablitiy.FirstOrDefault(p => p.UserNTLogin == usernTlogin && p.AvailabilityTime.Day == DateTime.Now.Day && p.AvailabilityTime.Month == DateTime.Now.Month && p.AvailabilityTime.Year == DateTime.Now.Year);
                return Ok(record);
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Invalid Token", System.Text.Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return Ok(response);
            }
        }
        public IHttpActionResult MarkUserStatus(UserStatusInputDto input)
        {
            if (ValidateAuthKey(input.Key))
            {
                try
                {
                    var usernTlogin = GetUserNtLoginById(input.EmpId);
                    if (!string.IsNullOrEmpty(usernTlogin))
                    {
                        var record = db.UserAvailablitiy.FirstOrDefault(p => p.UserNTLogin == usernTlogin && p.AvailabilityTime.Day == input.RecordDateTime.Day && p.AvailabilityTime.Month == input.RecordDateTime.Month && p.AvailabilityTime.Year == input.RecordDateTime.Year);
                        if (input.InForCheckin)
                        {
                            if (record == null)
                            {
                                db.UserAvailablitiy.Add(new UserAvailablitiy()
                                {
                                    AvailabilityTime = input.RecordDateTime,
                                    CheckinTime = input.RecordDateTime,
                                    IsCheckedIn = true,
                                    IsCheckedOut = false,
                                    UserNTLogin = usernTlogin,
                                    BreakStartTime = null,
                                    BreakEndTime = null,
                                    IsBreak = null
                                });
                                db.SaveChanges();
                                var response = new HttpResponseMessage(HttpStatusCode.OK)
                                {
                                    Content = new StringContent("checked-in successfully", System.Text.Encoding.UTF8, "text/plain"),
                                    StatusCode = HttpStatusCode.OK
                                };
                                return Ok(response);
                            }
                            else
                            {
                                var response = new HttpResponseMessage(HttpStatusCode.OK)
                                {
                                    Content = new StringContent("already checked-in", System.Text.Encoding.UTF8, "text/plain"),
                                    StatusCode = HttpStatusCode.OK
                                };
                                return Ok(response);
                            }
                        }
                        else
                        {
                            if (record == null)
                            {
                                var response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                                {
                                    Content = new StringContent("User is not checked-in. Please checkin to mark checkout", System.Text.Encoding.UTF8, "text/plain"),
                                    StatusCode = HttpStatusCode.Forbidden
                                };
                                return Ok(response);
                            }
                            if (record != null && record.CheckoutTime == null)
                            {
                                record.IsCheckedOut = true;
                                record.CheckoutTime = input.RecordDateTime;
                                db.UserAvailablitiy.AddOrUpdate(record);
                                db.SaveChanges();
                                var response = new HttpResponseMessage(HttpStatusCode.OK)
                                {
                                    Content = new StringContent("checked-out successfully", System.Text.Encoding.UTF8, "text/plain"),
                                    StatusCode = HttpStatusCode.OK
                                };
                                return Ok(response);
                            }
                            else
                            {
                                var response = new HttpResponseMessage(HttpStatusCode.OK)
                                {
                                    Content = new StringContent("already checked-out", System.Text.Encoding.UTF8, "text/plain"),
                                    StatusCode = HttpStatusCode.OK
                                };
                                return Ok(response);
                            }
                        }
                    }
                    else
                    {
                        var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                        {
                            Content = new StringContent("User NtLogin/EmployeeId not found", System.Text.Encoding.UTF8, "text/plain"),
                            StatusCode = HttpStatusCode.InternalServerError
                        };
                        return Ok(response);
                    }
                }
                catch (Exception ex)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(ex.Message, System.Text.Encoding.UTF8, "text/plain"),
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                    return Ok(response);
                }
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Invalid Token", System.Text.Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return Ok(response);
            }
        }
        public IHttpActionResult MarkCheckin(string empId, string key)
        {
            if (ValidateAuthKey(key))
            { 
                try
                {
                    var usernTlogin = GetUserNtLoginById(empId);
                    if (!string.IsNullOrEmpty(usernTlogin))
                    {
                        var record = db.UserAvailablitiy.FirstOrDefault(p => p.UserNTLogin == usernTlogin && p.AvailabilityTime.Day == DateTime.Now.Day && p.AvailabilityTime.Month == DateTime.Now.Month && p.AvailabilityTime.Year == DateTime.Now.Year);
                        if (record == null)
                        {
                            db.UserAvailablitiy.Add(new UserAvailablitiy()
                            {
                                AvailabilityTime = DateTime.Now,
                                CheckinTime = DateTime.Now,
                                IsCheckedIn = true,
                                IsCheckedOut = false,
                                UserNTLogin = usernTlogin,
                                BreakStartTime = null,
                                BreakEndTime = null,
                                IsBreak = null
                            });
                            db.SaveChanges();
                            var response = new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = new StringContent("checked-in successfully", System.Text.Encoding.UTF8, "text/plain"),
                                StatusCode = HttpStatusCode.OK
                            };
                            return Ok(response);
                        }
                        else
                        {
                            var response = new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = new StringContent("already checked-in", System.Text.Encoding.UTF8, "text/plain"),
                                StatusCode = HttpStatusCode.OK
                            };
                            return Ok(response);
                        }
                    }
                    else
                    {
                        var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                        {
                            Content = new StringContent("User not found", System.Text.Encoding.UTF8, "text/plain"),
                            StatusCode = HttpStatusCode.NotFound
                        };
                        return Ok(response);
                    }
                }
                catch (Exception ex)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(ex.Message, System.Text.Encoding.UTF8, "text/plain"),
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                    return Ok(response);
                }
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Invalid Token", System.Text.Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return Ok(response);
            }
        }
        public string GetUserNtLoginById(string empId)
        {
            var userNtLogin = db.ActivityUser.FirstOrDefault(p => p.EmployeeId == empId);
            if (userNtLogin != null) return userNtLogin.UserName;
            else return "";
        }
        public bool ValidateAuthKey(string key)
        {
            if (!string.IsNullOrEmpty(key) && key == authKey) return true;
            else return false;
        }
        public IHttpActionResult MarkCheckout(string empId, string key)
        {
            if (ValidateAuthKey(key))
            {
                try
                {
                    var usernTlogin = GetUserNtLoginById(empId);
                    var record = db.UserAvailablitiy.FirstOrDefault(p => p.UserNTLogin == usernTlogin && p.AvailabilityTime.Day == DateTime.Now.Day && p.AvailabilityTime.Month == DateTime.Now.Month && p.AvailabilityTime.Year == DateTime.Now.Year);
                    if (record.CheckoutTime == null)
                    {
                        record.IsCheckedOut = true;
                        record.CheckoutTime = DateTime.Now;
                        db.UserAvailablitiy.AddOrUpdate(record);
                        db.SaveChanges();
                        var response = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent("checked-out successfully", System.Text.Encoding.UTF8, "text/plain"),
                            StatusCode = HttpStatusCode.OK
                        };
                        return Ok(response);
                    }
                    else
                    {
                        var response = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent("already checked-out", System.Text.Encoding.UTF8, "text/plain"),
                            StatusCode = HttpStatusCode.OK
                        };
                        return Ok(response);
                    }
                }
                catch (Exception ex)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(ex.Message, System.Text.Encoding.UTF8, "text/plain"),
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                    return Ok(response);
                }
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Invalid Token", System.Text.Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return Ok(response);
            }
        }
        public IHttpActionResult MarkBreak(string empId, string key)
        {
            if (ValidateAuthKey(key))
            {
                var usernTlogin = GetUserNtLoginById(empId);
                var record = db.UserAvailablitiy.FirstOrDefault(p => p.UserNTLogin == usernTlogin && p.AvailabilityTime.Day == DateTime.Now.Day && p.AvailabilityTime.Month == DateTime.Now.Month && p.AvailabilityTime.Year == DateTime.Now.Year);
                if (record != null)
                {
                    var currentTime = DateTime.Now;
                    if (AddBreak(usernTlogin, currentTime))
                    {
                        record.BreakEndTime = null;
                        record.IsBreak = true;
                        record.BreakStartTime = currentTime;
                        db.UserAvailablitiy.AddOrUpdate(record);
                        db.SaveChanges();
                    }
                }
                else
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("User is not checkedin", System.Text.Encoding.UTF8, "text/plain"),
                        StatusCode = HttpStatusCode.Forbidden
                    };
                    return Ok(response);
                }
                return Ok();
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Invalid Token", System.Text.Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return Ok(response);
            }

        }
        public IHttpActionResult EndBreak(string empId, string key)
        {
            if (ValidateAuthKey(key))
            {
                var usernTlogin = GetUserNtLoginById(empId);
                var record = db.UserAvailablitiy.FirstOrDefault(p => p.UserNTLogin == usernTlogin && p.AvailabilityTime.Day == DateTime.Now.Day && p.AvailabilityTime.Month == DateTime.Now.Month && p.AvailabilityTime.Year == DateTime.Now.Year);
                if (record != null)
                {
                    var userBreak = db.UserBreaks.OrderByDescending(p => p.CreatedDateTime).FirstOrDefault(p => p.BreakEndTime == null && p.UserNTLogin == usernTlogin);
                    if (userBreak != null)
                    {
                        userBreak.BreakEndTime = DateTime.Now;
                        userBreak.IsBreak = false;
                        db.UserBreaks.AddOrUpdate(userBreak);
                        db.SaveChanges();
                    }
                    else
                    {
                        var response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                        {
                            Content = new StringContent("User is not on break", System.Text.Encoding.UTF8, "text/plain"),
                            StatusCode = HttpStatusCode.Forbidden
                        };
                        return Ok(response);
                    }
                }
                else
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("User is not checkedin", System.Text.Encoding.UTF8, "text/plain"),
                        StatusCode = HttpStatusCode.Forbidden
                    };
                    return Ok(response);
                }
                return Ok();
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Invalid Token", System.Text.Encoding.UTF8, "text/plain"),
                    StatusCode = HttpStatusCode.Unauthorized
                };
                return Ok(response);
            }
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
    }
}