// CustomerViewModel.cs
using QuanLyKho.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace QuanLyKho.ViewModel
{
    public class CustomerViewModel : BaseViewModel
    {
        private QuanLyKhoThucAnEntities3 context;

        private ObservableCollection<Customer> _list;
        public ObservableCollection<Customer> List
        {
            get => _list;
            set { _list = value; OnPropertyChanged(); }
        }

        private Customer _selectedItem = new Customer();
        public Customer SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }

        public CustomerViewModel()
        {
            context = DataProvider.Ins.DB;
            LoadCustomers();

            AddCommand = new RelayCommand<object>(_ => true, AddCustomer);
            DeleteCommand = new RelayCommand<object>(_ => SelectedItem != null, DeleteCustomer);
            EditCommand = new RelayCommand<object>(_ => SelectedItem != null, EditCustomer);
        }

        private void LoadCustomers()
        {
            List = new ObservableCollection<Customer>(context.Customers.ToList());
        }

        private void AddCustomer(object parameter)
        {
            if (SelectedItem != null)
            {
                var newCustomer = new Customer
                {
                    DisplayName = SelectedItem.DisplayName,
                    Address = SelectedItem.Address,
                    Phone = SelectedItem.Phone,
                    Email = SelectedItem.Email,
                    MoreInfo = SelectedItem.MoreInfo,
                    ContractDate = SelectedItem.ContractDate
                };

                context.Customers.Add(newCustomer);
                context.SaveChanges();
                LoadCustomers();
                SelectedItem = new Customer();
            }
        }

        private void DeleteCustomer(object parameter)
        {
            var customerToDelete = context.Customers.Find(SelectedItem?.Id);
            if (customerToDelete != null)
            {
                context.Customers.Remove(customerToDelete);
                context.SaveChanges();
                LoadCustomers();
                SelectedItem = new Customer();
            }
        }

        private void EditCustomer(object parameter)
        {
            var customerToEdit = context.Customers.Find(SelectedItem?.Id);
            if (customerToEdit != null)
            {
                customerToEdit.DisplayName = SelectedItem.DisplayName;
                customerToEdit.Address = SelectedItem.Address;
                customerToEdit.Phone = SelectedItem.Phone;
                customerToEdit.Email = SelectedItem.Email;
                customerToEdit.MoreInfo = SelectedItem.MoreInfo;
                customerToEdit.ContractDate = SelectedItem.ContractDate;

                context.SaveChanges();
                LoadCustomers();
                SelectedItem = new Customer();
            }
        }
    }
}