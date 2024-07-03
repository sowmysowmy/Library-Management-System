using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;


class Program
{
    static void Main(string[] args)
    {
        Library library = new Library();

        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("                                   ***********    Welcome to ABC Library   ***********             ");
            Console.WriteLine("1. Librarian Login");
            Console.WriteLine("2. User Login");
            Console.WriteLine("3. Exit");
            Console.Write("Please enter your choice:");

            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    library.LibrarianLogin();
                    break;
                case 2:
                    library.UserLogin();
                    break;
                case 3:
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }

            Console.WriteLine();
        }
    }
}

public class Library
{
    private List<Book> books;
    private string usersJsonFilePath = @"C:\Users\dell\source\repos\ConsoleApp1\ConsoleApp1\users.json";
    private string booksJsonFilePath = @"C:\Users\dell\source\repos\ConsoleApp1\ConsoleApp1\books.json";
    private string borrowJsonFilePath = @"C:\Users\dell\source\repos\ConsoleApp1\ConsoleApp1\borrow.json";

    private string librarianUsername = "admin";
    private string librarianPassword = "123";

    public void LibrarianLogin()
    {
        Console.Write("Enter username: ");
        string username = Console.ReadLine();

        Console.Write("Enter password: ");
        string password = Console.ReadLine();

        if (username == librarianUsername && password == librarianPassword)
        {
            Console.WriteLine("Login successful!");
            LibrarianManagementSystem();
        }
        else
        {
            Console.WriteLine("Invalid username or password.");
        }
    }

    public void UserLogin()
    {
        Console.Write("Enter username: ");
        string username = Console.ReadLine();

        Console.Write("Enter password: ");
        string password = Console.ReadLine();

        List<User> users = ReadUsersFromJson();
        User user = users.Find(u => u.Username == username && u.Password == password);

        if (user != null)
        {
            Console.WriteLine($"Welcome, {user.Username}!");
            UserManagement(user);
        }
        else
        {
            Console.WriteLine("Invalid username or password.");
        }
    }

    private void LibrarianManagementSystem()
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\nLibrarian Management System");
            Console.WriteLine("1. Book Management");
            Console.WriteLine("2. User Management");
            Console.WriteLine("3. Borrow History");
            Console.WriteLine("4. Exit");
            Console.Write("Please enter your choice: ");

            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    LoadBooksFromJson();
                    BookManagement();
                    break;
                case 2:
                    User();
                    break;
                case 3:
                    ViewBorrowHistory();
                    break;
                case 4:
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid");
                    break;
            }
        }
    }

    private List<User> ReadUsersFromJson()
    {
        if (File.Exists(usersJsonFilePath))
        {
            string json = File.ReadAllText(usersJsonFilePath);
            return JsonConvert.DeserializeObject<List<User>>(json);
        }
        else
        {
            return new List<User>();
        }
    }

    private void User()
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\nUser Management");
            Console.WriteLine("1. Add User");
            Console.WriteLine("2. Delete User");
            Console.WriteLine("3. Exit User Management");
            Console.Write("Please enter your choice: ");

            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    AddUser();
                    break;
                case 2:
                    DeleteUser();
                    break;
                case 3:
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private void AddUser()
    {
        Console.Clear();
        Console.WriteLine("Add a new user");
        var user = new User();
        Console.Write("Username: ");
        user.Username = Console.ReadLine();
        Console.Write("Password: ");
        user.Password = Console.ReadLine();

        AddUserByLibrarian(user);

        Console.WriteLine("User added successfully!");
        Console.ReadKey();
    }

    private void DeleteUser()
    {
        Console.Clear();
        Console.WriteLine("Delete a user");
        Console.Write("Enter the username of the user to delete: ");
        string username = Console.ReadLine();
        var users = ReadUsersFromJson();
        var user = users.Find(u => u.Username == username);
        if (user != null)
        {
            users.Remove(user);
            SaveUsersToJson(users);
            Console.WriteLine("User deleted successfully!");
        }
        else
        {
            Console.WriteLine("User not found.");
        }
        Console.ReadKey();
    }

    public void AddUserByLibrarian(User user)
    {
        var users = ReadUsersFromJson();
        users.Add(user);
        SaveUsersToJson(users);
    }


    private void UserManagement(User user)
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\nUser Management");
            Console.WriteLine("1. View Available Books");
            Console.WriteLine("2. Borrow a Book");
            Console.WriteLine("3. View Borrowed Books");
            Console.WriteLine("4. Return a Book");
            Console.WriteLine("5. Exit User Management");
            Console.Write("Please enter your choice: ");

            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    ViewAvailableBooks();
                    break;
                case 2:
                    BorrowBook(user);
                    break;
                case 3:
                    ViewBorrowedBooks(user);
                    break;
                case 4:
                    ReturnBook(user);
                    break;
                case 5:
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private void ViewAvailableBooks()
    {
        LoadBooksFromJson();
        Console.Clear();
        if (books == null || books.Count == 0)
        {
            Console.WriteLine("No books available.");
        }
        else
        {
            Console.WriteLine("List of available books:");
            int index = 1;
            foreach (var book in books)
            {
                Console.WriteLine($"{index}. {book.Title} by {book.Author} ({book.Year})");
                index++;
            }
        }
        Console.ReadKey();
    }

    private void BorrowBook(User user)
    {
        LoadBooksFromJson();
        Console.Clear();
        Console.WriteLine("Borrow a book");
        Console.Write("Enter the name of the book you want to borrow: ");
        string name = Console.ReadLine();
        var book = books.Find(b => b.Title == name);
        if (book != null)
        {
            DateTime dueDate = DateTime.Today.AddDays(7);

            if (user.BorrowedBooks.Any(b => b.Book.Title == name))
            {
                Console.WriteLine("You have already borrowed this book.");
            }
            else
            {
                user.BorrowedBooks.Add(new BorrowedBook { Book = book, DueDate = dueDate });
                SaveUsersToJson(ReadUsersFromJson());
                SaveBorrowToJson(user);
                Console.WriteLine("Book borrowed successfully!");
            }
        }
        else
        {
            Console.WriteLine("Book not found.");
        }
        Console.ReadKey();
    }


    private void ViewBorrowedBooks(User user)
    {
        Console.Clear();
        if (user.BorrowedBooks.Count == 0)
        {
            Console.WriteLine("You haven't borrowed any book");
        }
        else
        {
            Console.WriteLine("List of borrowed books:");
            foreach (var borrowedBook in user.BorrowedBooks)
            {
                Console.WriteLine($"{borrowedBook.Book.Title} by {borrowedBook.Book.Author} (Due Date: {borrowedBook.DueDate.ToShortDateString()})");
            }
        }
        Console.ReadKey();
    }

    private void ReturnBook(User user)
    {
        Console.Clear();
        Console.WriteLine("Return a book");
        Console.Write("Enter the title of the book you want to return: ");
        string title = Console.ReadLine();
        var borrowedBook = user.BorrowedBooks.Find(b => b.Book.Title == title);
        if (borrowedBook != null)
        {
            user.BorrowedBooks.Remove(borrowedBook);
            SaveUsersToJson(ReadUsersFromJson());
            RemoveBorrowedBook(borrowedBook);
            Console.WriteLine("Book returned successfully!");
        }
        else
        {
            Console.WriteLine("You haven't borrowed this book.");
        }
        Console.ReadKey();
    }

    private void SaveUsersToJson(List<User> users)
    {
        string json = JsonConvert.SerializeObject(users);
        File.WriteAllText(usersJsonFilePath, json);
    }

    private void SaveBorrowToJson(User user)
    {
        List<BorrowedBook> borrowedBooks = user.BorrowedBooks;
        string json = JsonConvert.SerializeObject(borrowedBooks);
        File.WriteAllText(borrowJsonFilePath, json);
    }

    private void LoadBooksFromJson()
    {
        if (File.Exists(booksJsonFilePath))
        {
            string json = File.ReadAllText(booksJsonFilePath);
            books = JsonConvert.DeserializeObject<List<Book>>(json);
        }
        else
        {
            books = new List<Book>();
        }
    }

    private void RemoveBorrowedBook(BorrowedBook borrowedBook)
    {
        if (File.Exists(borrowJsonFilePath))
        {
            string json = File.ReadAllText(borrowJsonFilePath);
            List<BorrowedBook> borrowedBooks = JsonConvert.DeserializeObject<List<BorrowedBook>>(json);
            borrowedBooks.Remove(borrowedBook);
            json = JsonConvert.SerializeObject(borrowedBooks);
            File.WriteAllText(borrowJsonFilePath, json);
        }
    }

    private void BookManagement()
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\nBook Management");
            Console.WriteLine("1. Add Book");
            Console.WriteLine("2. View All Books");
            Console.WriteLine("3. Delete Book");
            Console.WriteLine("4. Search Book by Name"); 
            Console.WriteLine("5. Exit Book Management");
            Console.Write("Please enter your choice: ");

            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    AddBook();
                    break;
                case 2:
                    ViewAllBooks();
                    break;
                case 3:
                    DeleteBook();
                    break;
                case 4:
                    SearchBookByName(); 
                    break;
                case 5:
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private void ViewAllBooks()
    {
        Console.Clear();
        if (books == null)
        {
            Console.WriteLine("No books available.");
            return;
        }

        Console.WriteLine("List of all books");
        int index = 1;
        foreach (var book in books)
        {
            Console.WriteLine($"{index}. {book.Title} by {book.Author} ({book.Year})");
            index++;
        }
        Console.ReadKey();
    }

    private void AddBook()
    {
        Console.Clear();
        Console.WriteLine("Add a new book");
        var book = new Book();
        Console.Write("Title: ");
        book.Title = Console.ReadLine();
        Console.Write("Author: ");
        book.Author = Console.ReadLine();
        Console.Write("Year: ");
        book.Year = int.Parse(Console.ReadLine());

        AddBookByLibrarian(book);

        Console.WriteLine("Book added successfully!");
        Console.ReadKey();
    }

    public void AddBookByLibrarian(Book book)
    {
        if (books == null)
        {
            books = new List<Book>();
        }
        books.Add(book);
        SaveBooksToJson(books);
    }

    private void DeleteBook()
    {
        Console.Clear();
        Console.WriteLine("Delete a book");
        Console.Write("Enter the title of the book to delete: ");
        string title = Console.ReadLine();
        var book = books.Find(b => b.Title == title);
        if (book != null)
        {
            books.Remove(book);
            SaveBooksToJson(books);
            Console.WriteLine("Book deleted successfully!");
        }
        else
        {
            Console.WriteLine("Book not found.");
        }
        Console.ReadKey();
    }

    private void SearchBookByName() 
    {
        Console.Clear();
        Console.WriteLine("Search Book by Name");
        Console.Write("Enter the name of the book: ");
        string name = Console.ReadLine();
        var foundBooks = books.Where(b => b.Title.ToLower().Contains(name.ToLower())).ToList();
        if (foundBooks.Count > 0)
        {
            Console.WriteLine("Found Books:");
            for (int i = 0; i < foundBooks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {foundBooks[i].Title} by {foundBooks[i].Author} ({foundBooks[i].Year})");
            }
        }
        else
        {
            Console.WriteLine("No book found with the given name.");
        }
        Console.ReadKey();
    }

    private void SaveBooksToJson(List<Book> books)
    {
        string json = JsonConvert.SerializeObject(books);
        File.WriteAllText(booksJsonFilePath, json);
    }

    private void ViewBorrowHistory()
    {
        Console.Clear();
        Console.WriteLine("View Borrow History");

        if (File.Exists(borrowJsonFilePath))
        {
            string json = File.ReadAllText(borrowJsonFilePath);
            var borrowedBooks = JsonConvert.DeserializeObject<List<BorrowedBook>>(json);

            if (borrowedBooks != null && borrowedBooks.Count > 0)
            {
                Console.WriteLine("List of borrowed books:");
                foreach (var borrowedBook in borrowedBooks)
                {
                    Console.WriteLine($"Book: {borrowedBook.Book.Title} (Due Date: {borrowedBook.DueDate.ToShortDateString()})");
                }
            }
            else
            {
                Console.WriteLine("No borrow history available.");
            }
        }
        else
        {
            Console.WriteLine("No borrow history available.");
        }

        Console.ReadKey();
    }
}

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int Year { get; set; }
}

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public List<BorrowedBook> BorrowedBooks { get; set; } = new List<BorrowedBook>();
}

public class BorrowedBook
{
    public Book Book { get; set; }
    public DateTime DueDate { get; set; }
}
