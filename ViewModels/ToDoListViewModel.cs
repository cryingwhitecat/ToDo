using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Models;
using Todo.Models.EF;

namespace Todo.ViewModels
{
    /// <summary>
    /// ViewModel for the /account/list page
    /// </summary>
    public class ToDoListViewModel
    {
        public List<ToDoItem> ToDoItems { get; set; }
        public User IdentityUser { get; set; }
    }
}
