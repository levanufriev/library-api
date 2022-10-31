﻿using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        public List<Book> Books { get; set; }
    }
}