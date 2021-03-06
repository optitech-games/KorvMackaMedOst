﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KorvMackaMedOst.Data;
using KorvMackaMedOst.Models;
using KorvMackaMedOst.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KorvMackaMedOst.Controllers
{
    public class TodoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TodoController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            string userid = _userManager.GetUserId(User);
            TodoViewModel vm = new TodoViewModel();
            var todoItems = new List<TodoItem>();
            var result=_context.TodoItems.Where(x => x.UserId == userid);
            if(result!=null)
            {
                vm.TodoItems = result.ToList();
            }
                return View(vm);
        }

        [HttpPost]
        public ActionResult AddItem(string content)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(content);
            //}
            TodoItem todoItem = new TodoItem
            {
                Content = content,
                UserId = _userManager.GetUserId(User)

            };
            if (User.Identity.IsAuthenticated)
            {
                
                _context.Add(todoItem);
                _context.SaveChanges();
            }
            else
            {
                TempData["ErrorMessage"] = $"Not added: {content}. Try to login first!";
            }
            return RedirectToAction("Index", "Todo");
        }
     
        public ActionResult UpdateItem(Guid id)
        {
            var userid = _userManager.GetUserId(User);
            if (User.Identity.IsAuthenticated)
            {
                var todoItem = _context.TodoItems.Find(id);
                
                if (todoItem != null && userid==todoItem.UserId)
                {
                    if (todoItem.IsCompleted == false)
                    {
                        todoItem.IsCompleted = true;
                    }
                    else
                    {
                        todoItem.IsCompleted = false;
                    }
                    _context.Update(todoItem);
                    _context.SaveChanges();
                }
                else
                    TempData["ErrorMessage"] = $"Could not update. Something went wrong.";
            }
            else if(!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = $"Could not update. Try to login first!";
            }
            else
                TempData["ErrorMessage"] = $"Could not update. Something went wrong.";
            return RedirectToAction("Index", "Todo");
        }
    }
}
