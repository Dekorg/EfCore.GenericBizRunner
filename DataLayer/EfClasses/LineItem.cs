﻿// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfClasses
{
    public class LineItem : IValidatableObject 
    {
        public int LineItemId { get; private set; }

        [Range(1,5, ErrorMessage =                      
            "This order is over the limit of 5 books.")] 
        public byte LineNum { get; internal set; }

        public short NumBooks { get; private set; }

        /// <summary>
        /// This holds a copy of the book price. We do this in case the price of the book changes,
        /// e.g. if the price was discounted in the future the order is still correct.
        /// </summary>
        public decimal BookPrice { get; private set; }

        // relationships

        public int OrderId { get; private set; }
        public int BookId { get; private set; }

        public Book ChosenBook { get; private set; }


        public LineItem(short numBooks, Book chosenBook)
        {
            NumBooks = numBooks;
            ChosenBook = chosenBook ?? throw new ArgumentNullException(nameof(chosenBook));
            BookPrice = chosenBook.ActualPrice;
        }

        /// <summary>
        /// Used by EF Core
        /// </summary>
        private LineItem() { }


        /// <summary>
        /// Extra validation rules: shows how IValidatableObject can add to the validation of a class
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        IEnumerable<ValidationResult> IValidatableObject.Validate 
            (ValidationContext validationContext)                 
        {
            var currContext = 
                validationContext.GetService(typeof(DbContext));

            if (ChosenBook.ActualPrice < 0)                      
                yield return new ValidationResult($"Sorry, the book '{ChosenBook.Title}' is not for sale."); 

            if (NumBooks > 100)
                yield return new ValidationResult("If you want to order a 100 or more books"+       
                        " please phone us on 01234-5678-90",              
                    new[] { nameof(NumBooks) });  
        }
    }

}