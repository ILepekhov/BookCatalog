using FileBooksSource;
using GUI.Client.Enums;
using Microsoft.Win32;
using Shared.Binding;
using Shared.Catalog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using WcfClientBooksSource;
using loc = Shared.Localization.Properties.Resources;

namespace GUI.Client.ViewModel
{
    public sealed class BookCatalogViewModel : BaseBinding
    {
        #region Fields

        private SectionViewModel _selectedSection;

        private CatalogViewType _selectedViewType;

        private bool _loadInProgress;

        #endregion

        #region Properties

        /// <summary>
        /// Список разделов
        /// </summary>
        public ObservableCollection<SectionViewModel> Sections { get; }

        /// <summary>
        /// Выбранный раздел
        /// </summary>
        public SectionViewModel SelectedSection
        {
            get { return _selectedSection; }
            set { SetValue(ref _selectedSection, value); }
        }

        /// <summary>
        /// Флаг типа представления: true - список, false - витрина
        /// </summary>
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

        /// <summary>
        /// Настройки подключения к Wcf-службе
        /// </summary>
        public ConnectionSettings ConnectionSettings { get; }

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

        public ICommand LoadCatalogFromFileCmd { get; }

        public ICommand LoadCatalogFromServiceCmd { get; }

        public ICommand ShowConnectionSettingsCmd { get; }

        public ICommand OpenHyperlinkCmd { get; }

        #endregion

        #region .ctor

        public BookCatalogViewModel()
        {
            Sections = new ObservableCollection<SectionViewModel>();

            LoadCatalogFromFileCmd = new DelegateCommand(ExecLoadCatalogFromFileCmd, _ => true);
            LoadCatalogFromServiceCmd = new DelegateCommand(ExecLoadCatalogFromServiceCmd, _ => true);
            ShowConnectionSettingsCmd = new DelegateCommand(ExecShowConnectionSettingsCmd, _ => true);
            OpenHyperlinkCmd = new DelegateCommand(ExecOpenHyperlinkCmd, CanExecOpenHyperlinkCmd);

            InitSections();

            SelectedSection = Sections.FirstOrDefault();

            ConnectionSettings = new ConnectionSettings("localhost", 1220);
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
        /// <param name="books"></param>
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

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    SetLoadingState(true);

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
                    finally
                    {
                        SetLoadingState(false);
                    }
                });
            }
        }

        #endregion

        #region LoadCatalogFromServiceCmd

        private void ExecLoadCatalogFromServiceCmd(object parameter)
        {
            ClearSectionsContent();

            ThreadPool.QueueUserWorkItem(_ =>
            {
                SetLoadingState(true);

                var source = new WcfClient(ConnectionSettings.Address, ConnectionSettings.Port);

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

        #region ShowConnectionsSettingsCmd

        private void ExecShowConnectionSettingsCmd(object parameter)
        {
            new ConnectionSettingsView(ConnectionSettings).ShowDialog();
        }

        #endregion

        #region OpenHyperlinkCmd

        private bool CanExecOpenHyperlinkCmd(object parameter)
        {
            return !string.IsNullOrEmpty(parameter as string);
        }

        private void ExecOpenHyperlinkCmd(object parameter)
        {
            var uri = parameter as string;

            Process.Start(new ProcessStartInfo(uri));
        }

        #endregion
    }
}
