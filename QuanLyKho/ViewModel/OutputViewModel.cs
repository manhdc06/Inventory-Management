using QuanLyKho.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Object = QuanLyKho.Model.Object;

namespace QuanLyKho.ViewModel
{
    public class OutputViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<OutputInfo> List { get; set; }
        public ObservableCollection<Customer> Customers { get; set; }
        public ObservableCollection<Object> Objects { get; set; }
        public ObservableCollection<Output> Outputs { get; set; }
        public ObservableCollection<Unit> Units { get; set; }

        private OutputInfo _selectedItem;
        public OutputInfo SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                if (value != null)
                {
                    SelectedObject = value.Object;
                    SelectedCustomer = value.Customer;
                    SelectedUnit = value.Object?.Unit;
                    DateInput = value.Output?.DateInPut;
                    Count = value.Count ?? 0;
                    OutputPrice = value.OutputPrice ?? 0;
                    Status = value.Status ?? "";
                }
            }
        }

        private Object _selectedObject;
        public Object SelectedObject
        {
            get => _selectedObject;
            set
            {
                _selectedObject = value;
                OnPropertyChanged();
                SelectedUnit = value?.Unit;

                // Tự động lấy giá xuất gần nhất
                if (_selectedObject != null)
                {
                    var latestInput = DataProvider.Ins.DB.InputInfoes
                        .Include(i => i.Input)
                        .Where(i => i.IdObject == _selectedObject.Id && i.OutputPrice != null)
                        .OrderByDescending(i => i.Input.DateInPut)
                        .FirstOrDefault();

                    if (latestInput?.OutputPrice != null)
                    {
                        OutputPrice = latestInput.OutputPrice.Value;
                    }
                }
            }
        }

        private Unit _selectedUnit;
        public Unit SelectedUnit { get => _selectedUnit; set { _selectedUnit = value; OnPropertyChanged(); } }
        private Customer _selectedCustomer;
        public Customer SelectedCustomer { get => _selectedCustomer; set { _selectedCustomer = value; OnPropertyChanged(); } }

        private DateTime? _dateInput;
        public DateTime? DateInput { get => _dateInput; set { _dateInput = value; OnPropertyChanged(); } }

        private int _count;
        public int Count { get => _count; set { _count = value; OnPropertyChanged(); } }

        private double _outputPrice;
        public double OutputPrice { get => _outputPrice; set { _outputPrice = value; OnPropertyChanged(); } }

        private string _status;
        public string Status { get => _status; set { _status = value; OnPropertyChanged(); } }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }

        public OutputViewModel()
        {
            List = new ObservableCollection<OutputInfo>(DataProvider.Ins.DB.OutputInfoes.Include(i => i.Object.Unit).Include(i => i.Customer).Include(i => i.Output).ToList());
            Customers = new ObservableCollection<Customer>(DataProvider.Ins.DB.Customers.ToList());
            Objects = new ObservableCollection<Object>(DataProvider.Ins.DB.Objects.Include(o => o.Unit).ToList());
            Outputs = new ObservableCollection<Output>(DataProvider.Ins.DB.Outputs.ToList());
            Units = new ObservableCollection<Unit>(DataProvider.Ins.DB.Units.ToList());

            AddCommand = new RelayCommand<object>(_ => true, _ => AddItem());
            DeleteCommand = new RelayCommand<object>(_ => SelectedItem != null, _ => DeleteItem());
            EditCommand = new RelayCommand<object>(_ => SelectedItem != null, _ => EditItem());
        }

        private void AddItem()
        {
            if (!ValidateInputs()) return;

            var totalInput = DataProvider.Ins.DB.InputInfoes
      .Where(x => x.IdObject == SelectedObject.Id)
      .Select(x => x.Count ?? 0)
      .DefaultIfEmpty(0)
      .Sum();

            var totalOutput = DataProvider.Ins.DB.OutputInfoes
                .Where(x => x.IdObject == SelectedObject.Id)
                .Select(x => x.Count ?? 0)
                .DefaultIfEmpty(0)
                .Sum();

            int available = totalInput - totalOutput;

            if (Count > available)
            {
                MessageBox.Show($"Không đủ hàng trong kho! Hiện tại còn {available}.");
                return;
            }

            var output = Outputs.FirstOrDefault(o => o.DateInPut == DateInput.Value);
            if (output == null)
            {
                output = new Output
                {
                    Id = Guid.NewGuid().ToString(),
                    DateInPut = DateInput.Value
                };
                DataProvider.Ins.DB.Outputs.Add(output);
                DataProvider.Ins.DB.SaveChanges();
                Outputs.Add(output);
            }

            var newItem = new OutputInfo
            {
                Id = Guid.NewGuid().ToString(),
                IdObject = SelectedObject.Id,
                idOutputInfo = output.Id,
                IdCustomer = SelectedCustomer.Id,
                Count = Count,
                OutputPrice = OutputPrice,
                Status = Status,
                Object = SelectedObject,
                Output = output,
                Customer = SelectedCustomer
            };

            DataProvider.Ins.DB.OutputInfoes.Add(newItem);
            DataProvider.Ins.DB.SaveChanges();
            List.Add(newItem);
            MessageBox.Show("Thêm thành công!");
            ClearFields();
        }

        private void EditItem()
        {
            if (SelectedItem == null) return;

            var item = DataProvider.Ins.DB.OutputInfoes.FirstOrDefault(x => x.Id == SelectedItem.Id);
            if (item != null)
            {
                var totalInput = DataProvider.Ins.DB.InputInfoes
      .Where(x => x.IdObject == SelectedObject.Id)
      .Select(x => x.Count ?? 0)
      .DefaultIfEmpty(0)
      .Sum();

                var totalOutput = DataProvider.Ins.DB.OutputInfoes
                    .Where(x => x.IdObject == SelectedObject.Id && x.Id != item.Id)
                    .Select(x => x.Count ?? 0)
                    .DefaultIfEmpty(0)
                    .Sum();


                int available = totalInput - totalOutput;

                if (Count > available)
                {
                    MessageBox.Show($"Không đủ hàng để sửa! Hiện còn {available}.");
                    return;
                }

                item.IdObject = SelectedObject?.Id;
                item.Object = SelectedObject;
                item.IdCustomer = SelectedCustomer?.Id ?? 0;
                item.Customer = SelectedCustomer;
                item.Count = Count;
                item.OutputPrice = OutputPrice;
                item.Status = Status;

                DataProvider.Ins.DB.SaveChanges();
                List = new ObservableCollection<OutputInfo>(DataProvider.Ins.DB.OutputInfoes.Include(i => i.Object.Unit).Include(i => i.Customer).Include(i => i.Output).ToList());
                OnPropertyChanged(nameof(List));
                ClearFields();
                MessageBox.Show("Sửa thành công!");
            }
        }

        private void DeleteItem()
        {
            if (SelectedItem == null) return;

            DataProvider.Ins.DB.OutputInfoes.Remove(SelectedItem);
            DataProvider.Ins.DB.SaveChanges();
            List.Remove(SelectedItem);
            ClearFields();
        }

        private bool ValidateInputs()
        {
            if (SelectedObject == null)
            {
                MessageBox.Show("Bạn chưa chọn vật tư!");
                return false;
            }
            if (SelectedCustomer == null)
            {
                MessageBox.Show("Bạn chưa chọn khách hàng!");
                return false;
            }
            if (DateInput == null)
            {
                MessageBox.Show("Bạn chưa chọn ngày xuất!");
                return false;
            }
            if (Count <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0!");
                return false;
            }
            if (OutputPrice <= 0)
            {
                MessageBox.Show("Giá xuất phải lớn hơn 0!");
                return false;
            }
            if (string.IsNullOrWhiteSpace(Status))
            {
                MessageBox.Show("Vui lòng nhập trạng thái!");
                return false;
            }
            return true;
        }

        private void ClearFields()
        {
            SelectedItem = null;
            SelectedObject = null;
            SelectedCustomer = null;
            SelectedUnit = null;
            DateInput = null;
            Count = 0;
            OutputPrice = 0;
            Status = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
