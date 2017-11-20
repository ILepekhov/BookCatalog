using FileBooksSource;
using Microsoft.Win32;
using Shared.Binding;
using Shared.Catalog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace GUI.Editor.ViewModel
{
    public sealed class BookEditorViewModel : BaseBinding
    {
        #region Fields

        private Book _selectedBook;

        #endregion

        #region Properties

        public ObservableCollection<SectionViewModel> Sections { get; }

        public Book SelectedBook
        {
            get { return _selectedBook; }
            set { SetValue(ref _selectedBook, value); }
        }

        #endregion

        #region Commands

        public ICommand SelectImageCmd { get; }

        public ICommand ChangeSelectedItemCmd { get; }

        public ICommand LoadCatalogFromFileCmd { get; }

        public ICommand SaveCatalogToFileCmd { get; }

        #endregion

        #region .ctor

        public BookEditorViewModel()
        {
            Sections = new ObservableCollection<SectionViewModel>();
            InitSections();

            SelectImageCmd = new DelegateCommand(ExecSelectImageCommand, CanSelectImageCmd);
            ChangeSelectedItemCmd = new DelegateCommand(ExecChangeSelectedItemCmd, _ => true);
            LoadCatalogFromFileCmd = new DelegateCommand(ExecLoadCatalogFromFileCmd, _ => true);
            SaveCatalogToFileCmd = new DelegateCommand(ExecSaveCatalogToFileCmd, CanExecSaveCatalogToFileCmd);
        }

        #endregion

        #region Helpers

        private void InitSections()
        {
            foreach (var item in Enum.GetValues(typeof(SectionType)))
            {
                Sections.Add(new SectionViewModel((SectionType)item));
            }
        }

        private void ClearSectionsContent()
        {
            foreach (var section in Sections)
            {
                section.Books.Clear();
            }
        }

        private void DistributeBooksToSections(List<Book> books)
        {
            var tempBooksDictionary = new Dictionary<SectionType, List<Book>>();

            foreach (var book in books)
            {
                if (tempBooksDictionary.ContainsKey(book.Section))
                {
                    tempBooksDictionary[book.Section].Add(book);
                }
                else
                {
                    tempBooksDictionary.Add(book.Section, new List<Book> { book });
                }
            }

            foreach (var sectionType in tempBooksDictionary.Keys)
            {
                var section = Sections.FirstOrDefault(s => s.SectionType == sectionType);
                if (section != null)
                {
                    section.AddBooksRange(tempBooksDictionary[sectionType]);
                }
            }
        }

        #endregion

        #region SelectImageCmd

        private bool CanSelectImageCmd(object parameter)
        {
            return SelectedBook != null;
        }

        private void ExecSelectImageCommand(object parameter)
        {
            var imagePath = GetImagePath();
            if (!string.IsNullOrEmpty(imagePath))
            {
                using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    SelectedBook.Image = new byte[(int)fs.Length];
                    fs.Read(SelectedBook.Image, 0, (int)fs.Length);
                }
            }

            OnPropertyChanged(nameof(SelectedBook));
        }

        private string GetImagePath()
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Файлы изображений|*.jpg;*.png;*.bmp";

            if (openDialog.ShowDialog().Value)
            {
                return openDialog.FileName;
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion

        #region ChangeSelectedItemCmd

        private void ExecChangeSelectedItemCmd(object parameter)
        {
            SelectedBook = parameter as Book;
        }

        #endregion

        #region LoadCatalogFromFileCmd

        private void ExecLoadCatalogFromFileCmd(object parameter)
        {
            OpenFileDialog openDialog = new OpenFileDialog()
            {
                Filter = "Каталог книг (*.xlm)|*.xml",
            };

            if (openDialog.ShowDialog().Value)
            {
                ClearSectionsContent();

                try
                {
                    var books = BookCatalogXmlSerializerHelper.Deserialize(openDialog.FileName);

                    if (books.Any())
                    {
                        DistributeBooksToSections(books);
                    }
                }
                catch
                {
                    //toDo: сделать вывод сообщения об ошибке
                }
            }
        }

        #endregion

        #region SaveCatalogToFileCmd

        private bool CanExecSaveCatalogToFileCmd(object parameter)
        {
            return Sections.SelectMany(s => s.Books).Count() > 0;
        }

        private void ExecSaveCatalogToFileCmd(object parameter)
        {
            SaveFileDialog saveDialog = new SaveFileDialog()
            {
                Filter = "Каталог книг (*.xlm)|*.xml",
            };

            if (saveDialog.ShowDialog().Value)
            {
                try
                {
                    var books = Sections.SelectMany(s => s.Books).ToList();

                    BookCatalogXmlSerializerHelper.Serialize(books, saveDialog.FileName);
                }
                catch
                {
                    // toDo: сделать вывод сообщения об ошибке
                }
            }
        }

        #endregion
    }
}
