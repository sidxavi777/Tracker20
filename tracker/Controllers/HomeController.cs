using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Models.ViewModel;
using Utilities;
using Database;
using Database.Data.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Mvc.Rendering;
using Humanizer;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Claims;
using System.Net.NetworkInformation;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using System.Net.Sockets;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Drawing.Printing;

namespace tracker.Controllers
{
    [Authorize]
	
	public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITracker _tracker;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ITracker tracker, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _tracker = tracker;
            _db = db;
            _userManager = userManager;
        }

		public async Task<bool> IsAllowedToView()
		{
			// Get the current user
			var user = await _userManager.GetUserAsync(User);

			return user != null && user.isAuthorized;
		}


		public List<SelectListItem> GetAuthorizedUsers()
        {
            var authorizedUserNames = _userManager.Users
                .Where(u => u.isAuthorized)
                .Select(u => new SelectListItem { Value = u.Name, Text = u.Name })
                .ToList();

            return authorizedUserNames;
        }

		public async Task<IActionResult> Index(TableColumnsVM? filter, int? Page)
		{
			try
			{
                if (await IsAllowedToView())
                {
                    int count = 0;
                    List<TableColumnsVm> tables;

                    var ticket = _db.Tracker.Include(u => u.CommentOfaTicket).ToList();

                    if (filter?.filterByName != null && filter.IsChecked == false)
                    {
                        tables = (await _tracker.GetAllAsync(u => u.AssignedTo == filter.filterByName)).ToList();
                    }
                    else if (filter?.filterByName != null && filter.IsChecked == true)
                    {
                        tables = (await _tracker.GetAllAsync(u => u.AssignedTo == filter.filterByName && u.Status != Status.Closed)).ToList();
                    }
                    else if (filter?.IsChecked == true && filter.filterByName == null)
                    {
                        tables = (await _tracker.GetAllAsync(u => u.Status != Status.Closed)).ToList();
                    }
                    else
                    {
                        tables = (await _tracker.GetAllAsync()).ToList();
                    }

                    tables.ForEach(item => item.RowCount = ++count);

                    // Paginate the list
                    int pageSize = 15;
                    var paginatedList = PaginatedList<TableColumnsVm>.Create(tables, Page ?? 1, pageSize);

                    var listOfColumn = new TableColumnsVM
                    {
                        Tracker = paginatedList,
                        filterByName = null,
                        userNames = GetAuthorizedUsers().Select(u => new SelectListItem
                        {
                            Text = u.Text,
                            Value = u.Value
                        }).ToList(),
                        HasPreviousPage = paginatedList.HasPerviousPage,
                        HasNextPage = paginatedList.HasNextPage,
                        PageIndex = paginatedList.PageIndex,
                        TotalPages = paginatedList.Totalpage

                    };

                    return View(listOfColumn);
                }
                else
                {
                    return View("UnAuth");
                }
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred in Index action.");
				return View("Error");
			}
		}


		public async Task<IActionResult> Create()
        {

            try
            {
                if (await IsAllowedToView())
                {
                    var listOfColumn = new TableColumnsVM
                    {
                        TrackerIndividual = new TableColumnsVm(),
                        filterByName = null,
                        userNames = GetAuthorizedUsers().Select(u => new SelectListItem
                        {
                            Text = u.Text,
                            Value = u.Value
                        }).ToList(),
                    };

                    return View(listOfColumn);
                }
                else
                {
                    return View("UnAuth");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Create action.");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(TableColumnsVM ColumnNames)
        {
            try
            {
                if (await IsAllowedToView())
                {
                    var user = await _userManager.GetUserAsync(User);

                    ColumnNames.TrackerIndividual.CreatedBy = user.Name;
                    ColumnNames.TrackerIndividual.CreatedOn = DateOnly.FromDateTime(DateTime.Now);
                    if (ColumnNames.TrackerIndividual.NewEta != null)
                    {
                        var eta = new List<DateOnly?>();
                        eta.Add(ColumnNames.TrackerIndividual.NewEta);
                        ColumnNames.TrackerIndividual.Eta = eta;
                    }
                    if (ModelState.IsValid)
                    {
                        await _tracker.AddAsync(ColumnNames.TrackerIndividual);
                        await _tracker.SaveAsync();
                        TempData["success"] = "Ticket created successfully";
                    }
                    else
                    {
                        TempData["success"] = "Input data is not valid";
                    }
                }
				else
				{
					return View("UnAuth");
				}
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Create action.");
                TempData["success"] = "Something went wrong";
                return View("Error");
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int? id)
        {

            try
            {
                if (await IsAllowedToView())
                {
                    if (id == null)
                    {
                        return View("Error");
                    }
                    var listOfColumn = new TableColumnsVM
                    {
                        TrackerIndividual = await _tracker.GetAsync(u => u.TableColumnsId == id),
                        filterByName = null,
                        userNames = GetAuthorizedUsers().Select(u => new SelectListItem
                        {
                            Text = u.Text,
                            Value = u.Value
                        }).ToList(),
                    };
                    return View(listOfColumn);
                }
				else
				{
					return View("UnAuth");
				}
			}

			
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Edit action.");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TableColumnsVM columns)
        {
            try
            {
                if (await IsAllowedToView())
                {
                    if (ModelState.IsValid)
                    {
                        if (columns.TrackerIndividual.Status.ToString() == SD.StatusCompleted)
                        {
                            columns.TrackerIndividual.CompletedDate = DateOnly.FromDateTime(DateTime.Now);
                        }
                        if (columns.TrackerIndividual.Eta == null)
                        {
                            columns.TrackerIndividual.Eta = new List<DateOnly?>();
                        }
                        if (columns.TrackerIndividual.NewEta != null)
                        {
                            columns.TrackerIndividual.Eta.Add(columns.TrackerIndividual.NewEta);

                        }
                        await _tracker.UpdateAsync(columns.TrackerIndividual);
                        await _tracker.SaveAsync();
                        TempData["success"] = "Updated successfully";
                        return RedirectToAction("Index", "Home");
                    }

                    return View("Error");
                }
				else
				{
					return View("UnAuth");
				}
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Edit action.");
                TempData["success"] = "Something went wrong";
                return View("Error");
            }
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

        [HttpPost]
        public async Task<IActionResult> Comment(int TicketNumAjax, string commentAjax)
        {
            try
            {
                if (await IsAllowedToView())
                {
                    var user = await _userManager.GetUserAsync(User);
                    Comment data = new Comment
                    {
                        Name = user.Name,
                        Description = commentAjax,
                        Time = DateTime.Now,
                        TicketNumber = TicketNumAjax
                    };
                    _db.comment.Add(data);
                    await _db.SaveChangesAsync();
                    int newCommentId = data.Id;
                    var displayName = user.Name;
                    var timeHumanised = DateTime.Now.Humanize();
                    return Ok(new { id = data.TicketNumber, name = displayName, desc = data.Description, time = timeHumanised, cId = newCommentId, tId = TicketNumAjax });
                }
                else
                {
					return View("UnAuth");
				}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Comment action.");
                return BadRequest("An error occurred.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int Sid, int S)
        {
            if (await IsAllowedToView())
            {
                var ticket = await _tracker.GetAsync(u => u.TableColumnsId == Sid);
                Status status = (Status)S;
                ticket.Status = status;
                if (ticket.Status.ToString() == "Closed")
                {
                    ticket.CompletedDate = DateOnly.FromDateTime(DateTime.Now);
                }
                else
                {
                    ticket.CompletedDate = null;
                }
                await _db.SaveChangesAsync();
                return Json(new { success = true, date = ticket.CompletedDate.ToString(), id = Sid });
            }
            else
            {
				return View("UnAuth");
			}
        }

        [Authorize(Roles = SD.Admin)]
        public async Task<IActionResult> Manage()
        {
            try
            {
                if (await IsAllowedToView())
                {
                    var allUsersInfo = await _userManager.Users
                .Select(user => new ApplicationUser { Id = user.Id, Email = user.Email, isAuthorized = user.isAuthorized })
                .ToListAsync();

                    foreach (var x in allUsersInfo)
                    {
                        var user = await _userManager.FindByIdAsync(x.Id);

                        if (user != null)
                        {
                            // Explicitly convert IList<string> to List<string>
                            x.Roles = (await _userManager.GetRolesAsync(user)).ToList();
                        }
                    }
                    return View(allUsersInfo);
                }
                else
                {
					return View("UnAuth");
				}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Manage action.");
                return View("Error");
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (await IsAllowedToView())
            {
                var row = await _tracker.GetAsync(u => u.TableColumnsId == id);
                if (row != null)
                {
                    _tracker.Delete(row);
                    await _tracker.SaveAsync();

                    return Json(new { success = true });
                }
                return NotFound();
            }
            else
            {
				return View("UnAuth");
			}
        }



        [HttpPost]
        public async Task<IActionResult> Manage(List<ApplicationUser> selectedUsers)
        {
            try
            {
                if (await IsAllowedToView())
                {
                    foreach (var user in selectedUsers)
                    {
                        var existingUser = await _userManager.FindByIdAsync(user.Id);
                        if (existingUser != null)
                        {
                            existingUser.isAuthorized = user.isAuthorized;

                            var existingRoles = await _userManager.GetRolesAsync(existingUser);
                            await _userManager.RemoveFromRolesAsync(existingUser, existingRoles.ToArray());

                            if (user.Roles != null && user.Roles.Contains("Admin"))
                            {
                                await _userManager.AddToRoleAsync(existingUser, "Admin");
                            }
                            else
                            {
                                await _userManager.AddToRoleAsync(existingUser, "Member");
                            }

                            await _userManager.UpdateAsync(existingUser);
                        }
                    }
                    return RedirectToAction("Index");
                }
				else
				{
					return View("UnAuth");
				}
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Manage action.");
                return View("Error");
            }
        }

        [Authorize(Roles = SD.Admin)]
        public async Task<IActionResult> Chart()
        {
            if (await IsAllowedToView())
            {
                return View();
            }
            else
            {
				return View("UnAuth");
			}
        }

        [HttpPost]

        public async Task<IActionResult> CommentEdit(int id, int ticketId, string newDescription)
        {
            if (await IsAllowedToView())
            {
                var ticketWithSpecificComment = _db.Tracker
                .Include(u => u.CommentOfaTicket)
                .FirstOrDefault(t => t.TableColumnsId == ticketId);



                if (ticketWithSpecificComment != null)
                {
                    var specificComment = ticketWithSpecificComment.CommentOfaTicket.FirstOrDefault(c => c.Id == id);

                    if (specificComment != null)
                    {
                        specificComment.Description = newDescription;
                        await _db.SaveChangesAsync();
                        return Json(new { success = true, message = "Comment updated successfully", id = specificComment.Id, desc = specificComment.Description.ToString(), ticketId = ticketId });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Comment not found for the given ticket" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Ticket not found" });
                }
            }
            else
            {
				return View("UnAuth");
			}
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteComment(int id, int tId)
        {
            if (await IsAllowedToView())
            {
                var ticketWithSpecificComment = _db.Tracker
                .Include(u => u.CommentOfaTicket)
                .FirstOrDefault(t => t.TableColumnsId == tId);

                if (ticketWithSpecificComment != null)
                {
                    var specificComment = ticketWithSpecificComment.CommentOfaTicket.FirstOrDefault(c => c.Id == id);

                    if (specificComment != null)
                    {
                        _db.comment.Remove(specificComment);
						await _db.SaveChangesAsync();

					}
					return Json(new { success = true, message = "Comment deleted successfully", ticketId = tId, cId = id });
                }
                else
                {
                    return Json(new { success = false, message = "Comment not found for the given ticket" });
                }

            }
			else
			{
				return View("UnAuth");
			}
		}


        [Authorize(Roles = SD.Admin)]
        [HttpPost]
        public async Task<IActionResult> ChartData()
        {
            if (await IsAllowedToView())
            {
                List<TableColumnsVm> AllTickets = ((List<TableColumnsVm>)await _tracker.GetAllAsync()).ToList();

                var inProgress = AllTickets.Where(t => t.Status.ToString() == Enum.GetName(typeof(Status), Status.InProgress)).ToList();
                var onHolded = AllTickets.Where(t => t.Status.ToString() == Enum.GetName(typeof(Status), Status.Hold)).ToList();

                var createdPastWeek = AllTickets
                .Where(t => t.CreatedOn >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7)))
                .ToList();


                var createdPastMonth = AllTickets
                    .Where(t => t.CreatedOn >= DateOnly.FromDateTime(DateTime.Now.AddMonths(-1)))
                    .ToList();

                var closedPastWeek = AllTickets
                    .Where(t => t.CompletedDate >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7)))
                    .ToList();

                var closedPastMonth = AllTickets
                    .Where(t => t.CompletedDate >= DateOnly.FromDateTime(DateTime.Now.AddMonths(-1)))
                    .ToList();

                var overallClosed = AllTickets
                   .Where(t => t.Status.ToString() == SD.StatusCompleted)
                   .ToList();

                var overallNew = AllTickets
                   .Where(t => t.Status.ToString() == SD.StatusNew)
                   .ToList();

                Chart counts = new Chart
                {
                    InProgres = inProgress.Count(),
                    onHold = onHolded.Count(),
                    createdPastWeek = createdPastWeek.Count(),
                    createdPastMonth = createdPastMonth.Count(),
                    closedPastWeek = closedPastWeek.Count(),
                    closedPastMonth = closedPastMonth.Count(),
                    overallClosedCount = overallClosed.Count(),
                    overAllNew = overallNew.Count()

                };
                ChartDataResponseVM week = new ChartDataResponseVM
                {
                    Data = new int[2] { counts.createdPastWeek, counts.closedPastWeek },
                    Labels = new string[2] { "Created " + counts.createdPastWeek, "Closed " + counts.closedPastWeek }
                };
                ChartDataResponseVM month = new ChartDataResponseVM
                {
                    Data = new int[2] { counts.createdPastMonth, counts.closedPastMonth },
                    Labels = new string[2] { "Created " + counts.createdPastMonth, "Closed " + counts.closedPastMonth }
                };
                ChartDataResponseVM overAll = new ChartDataResponseVM
                {
                    Data = new int[4] { counts.InProgres, counts.onHold, counts.overallClosedCount, counts.overAllNew },
                    Labels = new string[4] { "In progress " + counts.InProgres, "on hold " + counts.onHold, "Closed " + counts.overallClosedCount, "New " + counts.overAllNew }
                };

                List<ChartDataResponseVM> output = new List<ChartDataResponseVM>();
                output.Add(week);
                output.Add(month);
                output.Add(overAll);
                return Ok(output);
            }
            else
            {
					return View("UnAuth");
			}

        }
    }
}
