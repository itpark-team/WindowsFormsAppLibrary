using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAppLibrary.Models;

namespace WindowsFormsAppLibrary
{
    public partial class FormMain : Form
    {
        private LibraryEntities libraryEntities;

        private void UpdateDataGridViewBooks(List<Book> books)
        {
            dataGridViewBooks.Rows.Clear();

            foreach (Book book in books)
            {
                dataGridViewBooks.Rows.Add(
                    book.Id,
                    book.Name,
                    book.Price,
                    book.Author.FirstName + " " + book.Author.LastName);
            }
        }

        private void UpdateComboBoxAuthors()
        {
            comboBoxFilteredAuthors.Items.Clear();

            List<Author> authors = libraryEntities.Authors.ToList();

            comboBoxFilteredAuthors.Items.Add("Все");

            foreach (Author author in authors)
            {
                comboBoxFilteredAuthors.Items.Add(author.FirstName + " " + author.LastName);
            }

            comboBoxFilteredAuthors.SelectedIndex = 0;
        }

        private void UpdateComboBoxBookAuthor()
        {
            comboBoxBookAuthor.Items.Clear();

            List<Author> authors = libraryEntities.Authors.ToList();

            foreach (Author author in authors)
            {
                comboBoxBookAuthor.Items.Add(author.FirstName + " " + author.LastName);
            }

            comboBoxBookAuthor.SelectedIndex = -1;
        }

        private void ClearBookFields()
        {
            textBoxBookId.Clear();
            textBoxBookName.Clear();
            textBoxBookPrice.Clear();
            comboBoxBookAuthor.SelectedIndex = -1;
        }

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            libraryEntities = new LibraryEntities();

            List<Book> books = libraryEntities.Books.ToList();
            UpdateDataGridViewBooks(books);
            UpdateComboBoxAuthors();
            UpdateComboBoxBookAuthor();
        }

        private void buttonChooseBooksByAuthor_Click(object sender, EventArgs e)
        {
            int selectedIndex = comboBoxFilteredAuthors.SelectedIndex;

            if(selectedIndex == 0)
            {
                List<Book> books = libraryEntities.Books.ToList();
                UpdateDataGridViewBooks(books);
            }
            else
            {
                List<Author> authors = libraryEntities.Authors.ToList();
                int selectedIdAuthor = authors[selectedIndex - 1].Id;

                List<Book> books = libraryEntities.Books.Where(book => book.IdAuthor == selectedIdAuthor).ToList();

                UpdateDataGridViewBooks(books);
            }
        }

        private void buttonClearFields_Click(object sender, EventArgs e)
        {
            ClearBookFields();
        }

        private void buttonAddBook_Click(object sender, EventArgs e)
        {
            string name = textBoxBookName.Text;
            int price = int.Parse(textBoxBookPrice.Text);

            int selectedIndex = comboBoxBookAuthor.SelectedIndex;
            List<Author> authors = libraryEntities.Authors.ToList();
            int idAuthor = authors[selectedIndex].Id;

            Book book = new Book()
            {
                Name = name,
                Price = price,
                IdAuthor = idAuthor
            };

            libraryEntities.Books.Add(book);
            libraryEntities.SaveChanges();

            List<Book> books = libraryEntities.Books.ToList();
            UpdateDataGridViewBooks(books);

            comboBoxFilteredAuthors.SelectedIndex = 0;

            ClearBookFields();
        }

        private void dataGridViewBooks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewBooks.SelectedRows.Count == 0)
            {
                return;
            }

            int selectedIdBook = int.Parse(dataGridViewBooks.SelectedRows[0].Cells[0].Value.ToString());

           Book selectedBook = libraryEntities.Books.First(book => book.Id == selectedIdBook);


            textBoxBookId.Text = selectedBook.Id.ToString();
            textBoxBookName.Text = selectedBook.Name;
            textBoxBookPrice.Text = selectedBook.Price.ToString();

            List<Author> authors = libraryEntities.Authors.ToList();
            int selectedIndexAuthor = authors.FindIndex(author => author.Id == selectedBook.Author.Id);

            comboBoxBookAuthor.SelectedIndex = selectedIndexAuthor;

        }

        private void buttonDeleteBook_Click(object sender, EventArgs e)
        {
            int idBook = int.Parse(textBoxBookId.Text);

            Book selectedBook = libraryEntities.Books.First(book => book.Id == idBook);

            libraryEntities.Books.Remove(selectedBook);
            libraryEntities.SaveChanges();

            List<Book> books = libraryEntities.Books.ToList();
            UpdateDataGridViewBooks(books);

            comboBoxFilteredAuthors.SelectedIndex = 0;

            ClearBookFields();
        }
    }
}
