using FileBooksSource;
using Microsoft.Win32;
using Shared.Binding;
using Shared.Catalog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using WcfClientBooksSource;
using loc = Shared.Localization.Properties.Resources;

namespace GUI.Editor.ViewModel
{
    public sealed class BookEditorViewModel : BaseBinding
    {
        #region Fields

        private Book _selectedBook;

        private bool _fileSourceChecked;
        private bool _serviceSourceChecked;

        private bool _loadInProgress;

        #endregion

        #region Properties

        public ObservableCollection<SectionViewModel> Sections { get; }

        public Book SelectedBook
        {
            get { return _selectedBook; }
            set { SetValue(ref _selectedBook, value); }
        }

        public bool FileSourceChecked
        {
            get { return _fileSourceChecked; }
            set { SetValue(ref _fileSourceChecked, value); }
        }

        public bool ServiceSourceChecked
        {
            get { return _serviceSourceChecked; }
            set { SetValue(ref _serviceSourceChecked, value); }
        }

        public string ServiceAddress { get; set; }
        public uint ServicePort { get; set; }

        /// <summary>
        /// Флаг, управляющий отображением индикатора загрузки
        /// </summary>
        public bool LoadInProgress
        {
            get { return _loadInProgress; }
            set { SetValue(ref _loadInProgress, value); }
        }

        #endregion

        #region Commands

        public ICommand SelectImageCmd { get; }

        public ICommand ChangeSelectedItemCmd { get; }

        public ICommand LoadCatalogCmd { get; }

        public ICommand SaveCatalogCmd { get; }

        #endregion

        #region .ctor

        public BookEditorViewModel()
        {
            Sections = new ObservableCollection<SectionViewModel>();
            InitSections();

            SelectImageCmd = new DelegateCommand(ExecSelectImageCommand, CanSelectImageCmd);
            ChangeSelectedItemCmd = new DelegateCommand(ExecChangeSelectedItemCmd, _ => true);
            LoadCatalogCmd = new DelegateCommand(ExecLoadCatalogCmd, _ => true);
            SaveCatalogCmd = new DelegateCommand(ExecSaveCatalogCmd, CanExecSaveCatalogCmd);

            FileSourceChecked = true;

            ServiceAddress = "localhost";
            ServicePort = 1220;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Сформированть коллекцию разделов (пустых)
        /// </summary>
        private void InitSections()
        {
            foreach (var item in Enum.GetValues(typeof(SectionType)))
            {
                Sections.Add(new SectionViewModel((SectionType)item));
            }
        }

        /// <summary>
        /// Очистка коллекции разделов
        /// </summary>
        private void ClearSectionsContent()
        {
            foreach (var section in Sections)
            {
                section.Books.Clear();
            }
        }

        /// <summary>
        /// Распределить список книг по разделам
        /// </summary>
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

        /// <summary>
        /// Установка видимости индикатора загрузки в GUI-потоке
        /// </summary>
        private void SetLoadingState(bool isLoading)
        {
            App.Current.Dispatcher.Invoke(() => LoadInProgress = isLoading);
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

        #region LoadCatalogCmd

        private void ExecLoadCatalogCmd(object parameter)
        {
            if (FileSourceChecked)
                LoadCatalogFromFile();

            if (ServiceSourceChecked)
                LoadCatalogFromService();
        }

        private void LoadCatalogFromFile()
        {
            OpenFileDialog openDialog = new OpenFileDialog()
            {
                Filter = $"{loc.BookCatalog} (*.xlm)|*.xml",
            };

            if (openDialog.ShowDialog().Value)
            {
                ClearSectionsContent();

                SetLoadingState(true);

                ThreadPool.QueueUserWorkItem(_ =>
                {
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
                    finally
                    {
                        SetLoadingState(false);
                    }
                });
            }
        }

        private void LoadCatalogFromService()
        {
            ClearSectionsContent();

            SetLoadingState(true);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                var source = new WcfClient(ServiceAddress, ServicePort);

                try
                {
                    var books = source.GetBooks();

                    if (books == null)
                    {
                        MessageBox.Show(loc.CouldNotConnectToService, loc.Error, MessageBoxButton.OK);

                        SetLoadingState(false);
                        return;
                    }

                    if (books.Any())
                    {
                        DistributeBooksToSections(books);
                    }
                }
                catch
                {
                    //toDo: сделать вывод сообщения об ошибке
                }
                finally
                {
                    SetLoadingState(false);
                }
            });
        }

        #endregion

        #region SaveCatalogCmd

        private bool CanExecSaveCatalogCmd(object parameter)
        {
            return Sections.SelectMany(s => s.Books).Count() > 0;
        }

        private void ExecSaveCatalogCmd(object parameter)
        {
            var books = Sections.SelectMany(s => s.Books).ToList();

            if (FileSourceChecked)
                SaveCatalogToFile(books);

            if (ServiceSourceChecked)
                SaveCatalogOverService(books);
        }

        private void SaveCatalogToFile(List<Book> books)
        {
            SaveFileDialog saveDialog = new SaveFileDialog()
            {
                Filter = $"{loc.BookCatalog} (*.xlm)|*.xml",
            };

            if (saveDialog.ShowDialog().Value)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    SetLoadingState(true);

                    try
                    {
                        BookCatalogXmlSerializerHelper.Serialize(books, saveDialog.FileName);
                    }
                    catch
                    {
                        // toDo: сделать вывод сообщения об ошибке
                    }
                    finally
                    {
                        SetLoadingState(false);
                    }
                });
            }
        }

        private void SaveCatalogOverService(List<Book> books)
        {
            var source = new WcfClient(ServiceAddress, ServicePort);

            SetLoadingState(true);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    if (!source.SaveBooks(books))
                    {
                        MessageBox.Show(loc.CouldNotConnectToService, loc.Error, MessageBoxButton.OK);
                    }
                }
                catch
                {
                    //toDo: сделать вывод сообщения об ошибке
                }
                finally
                {
                    SetLoadingState(false);
                }
            });
        }

        #endregion
    }
}
