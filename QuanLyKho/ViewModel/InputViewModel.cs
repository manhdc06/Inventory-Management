// InputViewModel.cs
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
    public class InputViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<InputInfo> List { get; set; }
        public ObservableCollection<Object> Objects { get; set; }
        public ObservableCollection<Input> Inputs { get; set; }
        public ObservableCollection<Unit> Units { get; set; }

        private InputInfo _selectedItem;
        public InputInfo SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                if (value != null)
                {
                    SelectedObject = value.Object;
                    SelectedInput = value.Input;
                    Count = value.Count ?? 0;
                    InputPrice = (decimal)(value.InputPrice ?? 0);
                    OutputPrice = (decimal)(value.OutputPrice ?? 0);
                    Status = value.Status ?? "";
                    SelectedDate = value.Input?.DateInPut.Value.Date;
                    SelectedUnit = value.Object?.Unit;
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
            }
        }

        private Unit _selectedUnit;
        public Unit SelectedUnit
        {
            get => _selectedUnit;
            set { _selectedUnit = value; OnPropertyChanged(); }
        }

        private Input _selectedInput;
        public Input SelectedInput { get => _selectedInput; set { _selectedInput = value; OnPropertyChanged(); } }

        private DateTime? _selectedDate;
        public DateTime? SelectedDate { get => _selectedDate; set { _selectedDate = value; OnPropertyChanged(); } }

        private int _count;
        public int Count
        {
            get => _count;
            set { _count = value; OnPropertyChanged(); }
        }

        private decimal _inputPrice;
        public decimal InputPrice
        {
            get => _inputPrice;
            set { _inputPrice = value; OnPropertyChanged(); }
        }

        private decimal _outputPrice;
        public decimal OutputPrice
        {
            get => _outputPrice;
            set { _outputPrice = value; OnPropertyChanged(); }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }

        public InputViewModel()
        {
            List = new ObservableCollection<InputInfo>(DataProvider.Ins.DB.InputInfoes.Include(i => i.Object.Unit).ToList());
            Objects = new ObservableCollection<Object>(DataProvider.Ins.DB.Objects.Include(o => o.Unit).ToList());
            Units = new ObservableCollection<Unit>(DataProvider.Ins.DB.Units.ToList());
            Inputs = new ObservableCollection<Input>(DataProvider.Ins.DB.Inputs.ToList());

            SelectedDate = DateTime.Today;

            AddCommand = new RelayCommand<object>(_ => true, _ => AddItem());
            DeleteCommand = new RelayCommand<object>(_ => SelectedItem != null, _ => DeleteItem());
            EditCommand = new RelayCommand<object>(_ => SelectedItem != null, _ => SaveEditedItem());
        }

        private void AddItem()
        {
            if (SelectedObject == null)
            {
                MessageBox.Show("Vui lòng chọn vật tư.");
                return;
            }

            if (SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày nhập.");
                return;
            }

            var dateInput = SelectedDate.Value.Date + DateTime.Now.TimeOfDay;

            SelectedInput = Inputs.FirstOrDefault(i => i.DateInPut == dateInput);
            if (SelectedInput == null)
            {
                var newInput = new Input
                {
                    Id = Guid.NewGuid().ToString(),
                    DateInPut = dateInput
                };
                DataProvider.Ins.DB.Inputs.Add(newInput);
                DataProvider.Ins.DB.SaveChanges();
                Inputs.Add(newInput);
                SelectedInput = newInput;
            }

            var newItem = new InputInfo
            {
                Id = Guid.NewGuid().ToString(),
                IdObject = SelectedObject.Id,
                IdInput = SelectedInput.Id,
                Object = SelectedObject,
                Input = SelectedInput,
                Count = Count,
                InputPrice = (double?)InputPrice,
                OutputPrice = (double?)OutputPrice,
                Status = Status
            };

            DataProvider.Ins.DB.InputInfoes.Add(newItem);
            DataProvider.Ins.DB.SaveChanges();
            List.Add(newItem);
            MessageBox.Show("Thêm thành công!");
            ClearFields();
        }

        private void SaveEditedItem()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Không có mục nào được chọn để sửa.");
                return;
            }

            var item = DataProvider.Ins.DB.InputInfoes.FirstOrDefault(i => i.Id == SelectedItem.Id);
            if (item != null)
            {
                item.IdObject = SelectedObject?.Id;
                item.IdInput = SelectedItem.IdInput;
                item.Count = Count;
                item.InputPrice = (double?)InputPrice;
                item.OutputPrice = (double?)OutputPrice;
                item.Status = Status;

                DataProvider.Ins.DB.SaveChanges();

                MessageBox.Show("Sửa thành công!");
                RefreshList();
                ClearFields();
            }
        }

        private void DeleteItem()
        {
            if (SelectedItem == null) return;

            var item = DataProvider.Ins.DB.InputInfoes.FirstOrDefault(i => i.Id == SelectedItem.Id);
            if (item != null)
            {
                DataProvider.Ins.DB.InputInfoes.Remove(item);
                DataProvider.Ins.DB.SaveChanges();
                List.Remove(SelectedItem);
                ClearFields();
            }
        }

        private void ClearFields()
        {
            SelectedObject = null;
            SelectedInput = null;
            SelectedDate = null;
            Count = 0;
            InputPrice = 0;
            OutputPrice = 0;
            Status = "";
            SelectedItem = null;
        }

        private void RefreshList()
        {
            List = new ObservableCollection<InputInfo>(DataProvider.Ins.DB.InputInfoes.Include(i => i.Object.Unit).ToList());
            OnPropertyChanged(nameof(List));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
