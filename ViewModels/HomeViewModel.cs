using System.Collections.Generic;
using StreamApp.Models;

namespace StreamApp.ViewModels
{
    public class HomeViewModel
    {
        public List<Movie> Movies { get; set; } = new List<Movie>();
        public List<Series> Series { get; set; } = new List<Series>();
    }
}
