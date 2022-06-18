using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess
{
    public static class DataSource
    {
        private static IList<Publisher> _pulishers { get; set; }
        private static IList<Book> _books { get; set; }
        private static IList<Author> _authors { get; set; }
        private static IList<User> _users { get; set; }
        private static IList<Role> _roles { get; set; }

        public static IList<User> GetUsers()
        {
            if (_users == null)
            {
                _users = new List<User>()
                {
                    new User()
                    {
                        EmailAddress = "longpnhse150499@fpt.edu.vn",
                        Password = "123",
                        Source = null,
                        FirstName = "Long",
                        MiddleName = "Ngoc Hoang",
                        LastName = "Phan",
                        RoleId = 1,
                        PubId = null,
                        HireDate = null
                    },
                    new User()
                    {
                        EmailAddress = "longpnhse150499@fpt.edu.vn",
                        Password = "123",
                        Source = null,
                        FirstName = "Long",
                        MiddleName = null,
                        LastName = "Phan",
                        RoleId = 1,
                        PubId = null,
                        HireDate = null
                    }
                };
            }

            return _users;
        }

        private static IList<Role> GetRoles()
        {
            if (_roles == null)
            {
                _roles = new List<Role>()
                {
                    new Role()
                    {
                        RoleDesc = "Admin"
                    },

                    new Role()
                    {
                        RoleDesc = "Publisher"
                    },
                    new Role()
                    {
                        RoleDesc = "User"
                    }
                };
            }

            return _roles;
        }

        private static IList<Publisher> GetPublishers()
        {
            if (_pulishers == null)
            {
                _pulishers = new List<Publisher>()
                {
                    new Publisher()
                    {
                        PublisherName = "Long",
                        City = "HCM",
                        Country = "Viet Nam"
                    },
                    new Publisher()
                    {
                        PublisherName = "Pearson",
                        City = "London",
                        Country = "England"
                    }
                };
            }
            return _pulishers;
        }

        private static IList<Author> GetAuthors()
        {
            if (_authors == null)
            {
                _authors = new List<Author>()
                {
                    new Author {
                        LastName = "Newton",
                        FirstName = "Beau",
                        Phone = "(783) 628-1033",
                        Address = "Ap #715-3420 Eu St.",
                        City = "Galway",
                        Zip = 451618,
                        EmailAddress = "dui.lectus.rutrum@icloud.ca"
                    },
                    new Author {
                        LastName = "Baldwin",
                        FirstName = "Angela",
                        Phone = "(302) 177-2378",
                        Address = "570-3372 Purus St.",
                        City = "Pereira",
                        Zip = 23858,
                        EmailAddress = "amet@protonmail.ca"
                    },
                    new Author {
                        LastName = "Hunt",
                        FirstName = "Brenna",
                        Phone = "(768) 699-5335",
                        Address = "P.O. Box 973, 2469 Cras Avenue",
                        City = "Mirpur",
                        Zip = 446163,
                        EmailAddress = "integer.eu.lacus@google.ca"
                    },
                    new Author {
                        LastName = "Mcgowan",
                        FirstName = "Diana",
                        Phone = "1-424-372-2387",
                        Address = "788-1869 Nunc Av.",
                        City = "Hawera",
                        Zip = 4133,
                        EmailAddress = "lorem.lorem.luctus@outlook.com"
                    },
                    new Author {
                        LastName = "Atkinson",
                        FirstName = "Amos",
                        Phone = "1-714-657-0902",
                        Address = "959-6209 Sapien Rd.",
                        City = "Huntly",
                        Zip = 12093-168,
                        EmailAddress = "purus@outlook.couk"
                    }
                };
            }
            return _authors;
        }

        private static IList<Book> GetBooks()
        {
            if (_books == null)
            {
                _books = new List<Book>()
                {
                    new Book()
                    {
                        Title = "IT ENDS WITH US",
                        Type = "Fiction",
                        PubId = 1,
                        Price = 200000,
                        Advance = 50000,
                        Royalty = 0,
                        YtdSales = 18000000,
                        Notes = "",
                        PublishedDate = DateTime.Now.Date
                    },
                    new Book()
                    {
                        Title = "ATOMIC HABITS",
                        Type = "Miscellaneous",
                        PubId = 2,
                        Price = 225000,
                        Advance = 120000,
                        Royalty = 80000,
                        YtdSales = 25760000,
                        Notes = "",
                        PublishedDate = DateTime.Now.Date
                    },
                    new Book()
                    {
                        Title = "Wonder",
                        Type = "Children",
                        PubId = 1,
                        Price = 70000,
                        Advance = 300000,
                        Royalty = 20000,
                        YtdSales = 11521000,
                        Notes = "",
                        PublishedDate = DateTime.Now.Date
                    },
                    new Book()
                    {
                        Title = "HAPPY-GO-LUCKY",
                        Type = "Nonfiction",
                        PubId = 1,
                        Price = 360000,
                        Advance = 5000000,
                        Royalty = 180000,
                        YtdSales = 36458000,
                        Notes = "",
                        PublishedDate = DateTime.Now.Date
                    },
                    new Book()
                    {
                        Title = "KILLING THE KILLERS",
                        Type = "Nonfiction",
                        PubId = 2,
                        Price = 360000,
                        Advance = 5000000,
                        Royalty = 180000,
                        YtdSales = 36458000,
                        Notes = "",
                        PublishedDate = DateTime.Now.Date
                    },
                };
            }
            return _books;
        }

        public static void MigrateData(ApplicationDbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new NullReferenceException();
            }

            if (dbContext.Roles.Count() == 0)
            {
                foreach (var role in GetRoles())
                {
                    dbContext.Roles.Add(role);
                }
                dbContext.SaveChanges();
            }

            if (dbContext.Users.Count() == 0)
            {
                foreach (var user in GetUsers())
                {
                    dbContext.Users.Add(user);
                }
                dbContext.SaveChanges();
            }

            if (dbContext.Publishers.Count() == 0)
            {
                foreach (var publisher in GetPublishers())
                {
                    dbContext.Publishers.Add(publisher);
                }
                dbContext.SaveChanges();
            }

            if (dbContext.Authors.Count() == 0)
            {
                foreach (var author in GetAuthors())
                {
                    dbContext.Authors.Add(author);
                }
                dbContext.SaveChanges();
            }

            if (dbContext.Books.Count() == 0)
            {
                foreach (var book in GetBooks())
                {
                    dbContext.Books.Add(book);
                }
                dbContext.SaveChanges();
            }
        }

    }
}
