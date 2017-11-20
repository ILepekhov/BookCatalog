using FileBooksSource;
using GUI.Client.Enums;
using Microsoft.Win32;
using Shared.Binding;
using Shared.Catalog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace GUI.Client.ViewModel
{
    public sealed class BookCatalogViewModel : BaseBinding
    {
        #region Fields

        private SectionViewModel _selectedSection;

        private CatalogViewType _selectedViewType;

        #endregion

        #region Properties

        public ObservableCollection<SectionViewModel> Sections { get; }

        public SectionViewModel SelectedSection
        {
            get { return _selectedSection; }
            set { SetValue(ref _selectedSection, value); }
        }

        public bool CatalogAsList
        {
            get { return _selectedViewType == CatalogViewType.List; }
            set
            {
                _selectedViewType = value
                    ? CatalogViewType.List
                    : CatalogViewType.Shopcase;

                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand LoadCatalogFromFileCmd { get; }

        #endregion

        #region .ctor

        public BookCatalogViewModel()
        {
            Sections = new ObservableCollection<SectionViewModel>();

            LoadCatalogFromFileCmd = new DelegateCommand(ExecLoadCatalogFromFileCmd, _ => true);

            InitSections();

            SelectedSection = Sections.FirstOrDefault();
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
                    var source = new XmlFileBooksSource(openDialog.FileName);

                    var books = source.GetBooks();

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
    }
}
