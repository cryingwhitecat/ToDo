using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Todo.Models;
using Todo.Models.EF;
using Todo.ViewModels;
using ToDoItem = Todo.Models.EF.ToDoItem;

namespace Todo.Controllers
{
    /// <summary>
    /// Controller responsible for handling user authentification and showing all relative ToDo Items
    /// </summary>
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        /// <summary>
        /// Lists all ToDO items associated with the user with a specific id
        /// </summary>
        /// <param name="userid">User id in AspNetUsers table</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> List(string userid)
        {
            if(userid == null && !User.Identity.IsAuthenticated)
            {
                
                return RedirectToAction("index", "home");
            }
            User user = null;
            if (User.Identity.IsAuthenticated)
                 user = await _userManager.FindByNameAsync(User.Identity.Name);
            else
                user = await _userManager.FindByIdAsync(userid);
            var todos = new List<ToDoItem>();
            using(var context = new ToDoListDBContext())
            {
                todos = context.ToDoItem.Where(item => item.UserId == user.Id).ToList();
            }
            var viewModel = new ToDoListViewModel() { IdentityUser = user, ToDoItems = todos };
            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                return RedirectToAction("list", "account", new { userid = currentUser.Id });
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(viewModel.Username,
                    viewModel.Password, viewModel.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(viewModel.Username);
                    var redirectParams = new { userid = user.Id};
                    return RedirectToAction("list", "account",redirectParams);
                }
                else
                {
                    ModelState.AddModelError("", "Login Failed.");
                }
            }
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                User user = new User { UserName = viewModel.Username };
                var result = await _userManager.CreateAsync(user, viewModel.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    var usr = await _userManager.FindByNameAsync(viewModel.Username);
                    return RedirectToAction("list", "account",new { userid = usr.Id });
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ToDoItemViewModel toDoItem)
        {
            if (ModelState.IsValid)
            {
                using(var context = new ToDoListDBContext())
                {
                    var newItem = new ToDoItem() { Finished = false, Title = toDoItem.Title, UserId = toDoItem.UserId };
                    context.ToDoItem.Add(newItem);
                    await context.SaveChangesAsync();
                }
            }
            return RedirectToAction("list","account", new { userid = toDoItem.UserId });
        }
        [HttpPost]
        public IActionResult Delete(int itemId)
        {
            using (var context = new ToDoListDBContext())
            {
                var item = context.ToDoItem.Where(item => item.ItemId == itemId).First();
                context.ToDoItem.Remove(item);
                context.SaveChanges();
            }
            return Json(new { success = true });
        }
        [HttpPost]
        public IActionResult Edit(ToDoItemViewModel viewModel)
        {
            var id = viewModel.ItemId;
             
            using (var context = new ToDoListDBContext())
            {
                var entity = context.ToDoItem.Where(x => x.ItemId == id).First();
                entity.Title = viewModel.Title;
                entity.Finished = viewModel.Finished;
                context.ToDoItem.Update(entity);
                context.SaveChanges();
            }
            return RedirectToAction("list", "account", new { userid = viewModel.UserId });
        }
    }
}
