using QuanLyKho.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuanLyKho.ViewModel
{
    public class SuplierViewModel : BaseViewModel
    {

        DataProvider dataProvider = DataProvider.Ins;

        private QuanLyKhoThucAnEntities3 context;

        private ObservableCollection<Suplier> _list;
        public ObservableCollection<Suplier> List
        {
            get { return _list; }
            set { _list = value; OnPropertyChanged(); }
        }

        private Suplier _selectedItem = new Suplier();
        public Suplier SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }

        public SuplierViewModel()
        {
            context = DataProvider.Ins.DB;
            LoadSuppliers();

            AddCommand = new RelayCommand<object>((p) => true, AddSupplier);
            DeleteCommand = new RelayCommand<object>((p) => SelectedItem != null, DeleteSupplier);
            EditCommand = new RelayCommand<object>((p) => SelectedItem != null, EditSupplier);

        }


        private void LoadSuppliers()
        {
            List = new ObservableCollection<Suplier>(context.Supliers.ToList());
        }

        private void AddSupplier(object parameter)
        {
            if (SelectedItem == null)
            {
                Console.WriteLine("SelectedItem is null!");
                return;
            }
            // Kiểm tra nếu SelectedItem hợp lệ
            if (SelectedItem != null)
            {
                // Tạo một đối tượng mới dựa trên thông tin nhập vào
                var newSupplier = new Suplier
                {
                    DisplayName = SelectedItem.DisplayName,
                    Address = SelectedItem.Address,
                    Phone = SelectedItem.Phone,
                    Email = SelectedItem.Email,
                    MoreInfo = SelectedItem.MoreInfo,
                    ContractDate = SelectedItem.ContractDate // Đảm bảo viết đúng thuộc tính
                };

                // Thêm vào cơ sở dữ liệu
                context.Supliers.Add(newSupplier);
                context.SaveChanges();

                // Cập nhật danh sách hiển thị
                LoadSuppliers();

                // Xóa SelectedItem để tránh lỗi nhập dữ liệu lặp
                SelectedItem = null;
            }
        }


        private void DeleteSupplier(object parameter)
        {
            if (SelectedItem != null)
            {
                var supplierToDelete = context.Supliers.Find(SelectedItem.Id);
                if (supplierToDelete != null)
                {
                    context.Supliers.Remove(supplierToDelete);
                    context.SaveChanges();
                    LoadSuppliers();
                }
            }
        }


        private void EditSupplier(object parameter)
        {
            if (SelectedItem != null)
            {
                var supplierToEdit = context.Supliers.Find(SelectedItem.Id);
                if (supplierToEdit != null)
                {
                    supplierToEdit.DisplayName = SelectedItem.DisplayName;
                    supplierToEdit.Address = SelectedItem.Address;
                    supplierToEdit.Phone = SelectedItem.Phone;
                    supplierToEdit.Email = SelectedItem.Email;
                    supplierToEdit.MoreInfo = SelectedItem.MoreInfo;
                    supplierToEdit.ContractDate = SelectedItem.ContractDate;

                    context.SaveChanges();
                    LoadSuppliers();
                }
            }
        }

    }
}
