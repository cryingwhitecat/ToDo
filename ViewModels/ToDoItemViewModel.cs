using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.ViewModels
{
    public class ToDoItemViewModel
    {
        public int ItemId { get; set; }
        public string UserId { get; set; }
        [Required(ErrorMessage ="Title can't be empty")]
        public string Title { get; set; }
        [Display(Name ="Status")]
        public bool Finished { get; set; }
    }
}
