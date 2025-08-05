using System.Collections.Generic;

namespace WebRestaurant.Models
{
    public class MenuIndexViewModel
    {
        public List<MenuItem> MenuItems { get; set; }
        public List<string> Categories { get; set; }
        public string SelectedCategory { get; set; }
        public string SortOrder { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
