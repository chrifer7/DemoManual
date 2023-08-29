using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoManual.Model
{
    public class Book
    {
        // Properties
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public string Isbn { get; set; }

        // Constructors
        public Book()
        {
            // Default constructor
        }

        public Book(string title, string author, int year, string isbn)
        {
            Title = title;
            Author = author;
            Year = year;
            Isbn = isbn;
        }

        // Methods
        public void DisplayBookInfo()
        {
            Console.WriteLine("Title: " + Title);
            Console.WriteLine("Author: " + Author);
            Console.WriteLine("Year: " + Year);
            Console.WriteLine("ISBN: " + Isbn);
        }
    }

}
