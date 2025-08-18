using System;
using System.Collections.Generic;

namespace Library_Management.Models
{
    public class DashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public int UniqueGenres { get; set; }
        public List<BookListViewModel> RecentBooks { get; set; } = new();
        public List<GenreStatistic> GenreDistribution { get; set; } = new();
        public List<BookListViewModel> LowStockBooks { get; set; } = new();
        public List<BookListViewModel> OutOfStockBooks { get; set; } = new();
        
        public int BorrowedCopies => TotalCopies - AvailableCopies;
        public double AvailabilityPercentage => TotalCopies > 0 ? Math.Round((double)AvailableCopies / TotalCopies * 100, 1) : 0;
    }

    public class GenreStatistic
    {
        public string Genre { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
