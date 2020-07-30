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
            if(userid == null && !User.Identity.IsAuthenticated) // if no user is currently logged in 
            {
                
                return RedirectToAction("index", "home"); // return to home
            }
            //else try to obtain a user model
            try
            {
                User user = null;
                if (User.Identity.IsAuthenticated)
                    user = await _userManager.FindByNameAsync(User.Identity.Name);
                else
                    user = await _userManager.FindByIdAsync(userid);
                var todos = new List<ToDoItem>();
                using (var context = new ToDoListDBContext())
                {
                    todos = context.ToDoItem.Where(item => item.UserId == user.Id).ToList();
                }
                var viewModel = new ToDoListViewModel() { IdentityUser = user, ToDoItems = todos };
                return View(viewModel);
            }
            catch(Exception e)
            {
                return RedirectToAction("error", "home", new { error = e.Message });
            }
        }
        /// <summary>
        /// Login Action(if user is already authenticated, redirects to tasks list)
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Logout Action
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// This method handles user authentication 
        /// </summary>
        /// <param name="viewModel">submitted form</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)// if the form is ok
                {
                    var result = await _signInManager.PasswordSignInAsync(viewModel.Username,
                        viewModel.Password, viewModel.RememberMe, false); // try logging in with a password
                    if (result.Succeeded) // if all went well, redirect to items list
                    {
                        var user = await _userManager.FindByNameAsync(viewModel.Username);
                        var redirectParams = new { userid = user.Id };
                        return RedirectToAction("list", "account", redirectParams);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Login Failed."); // else show an error message
                    }
                }
            }
            catch (Exception e) // if something terrible happened
                                //return an error page
            {
                return RedirectToAction("error", "home", new { error = e.Message });
            }
            return View(viewModel);
        }
        /// <summary>
        /// This method handles user registration.
        /// </summary>
        /// <param name="viewModel">submitted form</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid) // if the form is valid
            {
                User user = new User { UserName = viewModel.Username }; //create a new user model and try registering it
                try
                {
                    var result = await _userManager.CreateAsync(user, viewModel.Password);
                    if (result.Succeeded) // if all went well, let the new user create his todo items
                    {
                        await _signInManager.SignInAsync(user, false);
                        var usr = await _userManager.FindByNameAsync(viewModel.Username);
                        return RedirectToAction("list", "account", new { userid = usr.Id });
                    }
                    else//else tell him what went wrong
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                catch (Exception e) //if something crashed, return an error page
                {
                    return RedirectToAction("error", "home", new { error = e.Message });
                }

            }
            return View(viewModel);
        }
        //--------------CRUD METHODS-------------------//
        [HttpPost]
        public async Task<IActionResult> Create(ToDoItemViewModel toDoItem)
        {
            if (ModelState.IsValid) // if the form is valid
            {
                try // try creating a new item
                {
                    using (var context = new ToDoListDBContext())
                    {

                        var newItem = new ToDoItem() // create new item from our ViewModel
                        { 
                            Finished = false,
                            Title = toDoItem.Title,
                            UserId = toDoItem.UserId 
                        };
                        //add new item to the database
                        context.ToDoItem.Add(newItem);
                        await context.SaveChangesAsync();
                    }
                }
                catch(Exception e) // if something went wrong 
                {
                    return RedirectToAction("error", "home", new { error = e.Message }); // return error page
                }
            }
            return RedirectToAction("list","account", new { userid = toDoItem.UserId });
        }
        [HttpPost]
        public IActionResult Delete(int itemId)
        {
            try
            {
                using (var context = new ToDoListDBContext())
                {
                    var item = context.ToDoItem.Where(item => item.ItemId == itemId).First();
                    context.ToDoItem.Remove(item);
                    context.SaveChanges();
                }
                return Json(new { success = true });
            }
            catch (Exception e) // if something went wrong 
            {
                return RedirectToAction("error", "home", new { error = e.Message }); // return error page
            }
        }
        [HttpPost]
        public IActionResult Edit(ToDoItemViewModel viewModel)
        {
            var id = viewModel.ItemId;
            try
            {
                using (var context = new ToDoListDBContext()) 
                {
                    var entity = context.ToDoItem.Where(x => x.ItemId == id).First();
                    if (entity == null)                                     //if there's no item with such id
                        throw new NullReferenceException("Item not found"); // throw an error
                                                                            //else - update the item and refresh the list
                    entity.Title = viewModel.Title;
                    entity.Finished = viewModel.Finished;
                    context.ToDoItem.Update(entity);
                    context.SaveChanges();
                }
                return RedirectToAction("list", "account", new { userid = viewModel.UserId });
            }
            catch (Exception e) // if something went wrong 
            {
                return RedirectToAction("error", "home", new { error = e.Message }); // return error page
            }
        }
    }
}
