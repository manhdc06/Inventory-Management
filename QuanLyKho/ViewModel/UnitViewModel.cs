using QuanLyKho.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyKho.ViewModel
{
    public class UnitViewModel : BaseViewModel
    {
        private ObservableCollection<Unit> _List;
        public ObservableCollection<Unit> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private Unit _SelectedItem;
        public Unit SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    DisplayName = SelectedItem.DisplayName;
                }
            }
        }

        private string _DisplayName;
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }

        private bool _IsEditable;
        public bool IsEditable { get => _IsEditable; set { _IsEditable = value; OnPropertyChanged(); } }

        private bool _IsSaveEnabled;
        public bool IsSaveEnabled { get => _IsSaveEnabled; set { _IsSaveEnabled = value; OnPropertyChanged(); } }

        private bool _IsControlsEnabled;
        public bool IsControlsEnabled { get => _IsControlsEnabled; set { _IsControlsEnabled = value; OnPropertyChanged(); } }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        private bool isAddNew;

        public UnitViewModel()
        {
            IsEditable = false;
            IsSaveEnabled = false;
            IsControlsEnabled = true;

            LoadUnits();

            AddCommand = new RelayCommand<object>((p) => true, (p) => AddUnit());
            EditCommand = new RelayCommand<object>((p) => SelectedItem != null, (p) => EditUnit());
            DeleteCommand = new RelayCommand<object>((p) => SelectedItem != null, (p) => DeleteUnit());
            SaveCommand = new RelayCommand<object>((p) => IsSaveEnabled && !string.IsNullOrWhiteSpace(DisplayName), (p) => SaveUnit());
        }

        private void LoadUnits()
        {
            List = new ObservableCollection<Unit>(DataProvider.Ins.DB.Units.ToList());
        }

        private void AddUnit()
        {
            isAddNew = true;
            DisplayName = "";

            IsEditable = true;
            IsSaveEnabled = true;
            IsControlsEnabled = false;
        }

        private void EditUnit()
        {
            if (SelectedItem == null) return;

            isAddNew = false;

            IsEditable = true;
            IsSaveEnabled = true;
            IsControlsEnabled = false;
        }

        private void DeleteUnit()
        {
            if (SelectedItem == null) return;

            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa đơn vị đo này không?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    int used = DataProvider.Ins.DB.Objects.Count(o => o.IdUnit == SelectedItem.Id);
                    if (used > 0)
                    {
                        MessageBox.Show("Không thể xóa đơn vị đo vì đã được sử dụng trong danh sách vật tư!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    DataProvider.Ins.DB.Units.Remove(SelectedItem);
                    DataProvider.Ins.DB.SaveChanges();

                    LoadUnits();
                    ClearSelection();

                    MessageBox.Show("Đã xóa đơn vị đo!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveUnit()
        {
            string trimmedName = DisplayName?.Trim();
            if (string.IsNullOrEmpty(trimmedName))
            {
                MessageBox.Show("Vui lòng nhập tên đơn vị hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (isAddNew)
                {
                    bool exists = DataProvider.Ins.DB.Units.Any(u => u.DisplayName.ToLower() == trimmedName.ToLower());
                    if (exists)
                    {
                        MessageBox.Show("Đơn vị đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var newUnit = new Unit() { DisplayName = trimmedName };
                    DataProvider.Ins.DB.Units.Add(newUnit);
                    DataProvider.Ins.DB.SaveChanges();
                    LoadUnits();
                    SelectedItem = newUnit;
                    MessageBox.Show("Thêm thành công!", "Thông báo");
                }
                else
                {
                    var unit = DataProvider.Ins.DB.Units.FirstOrDefault(x => x.Id == SelectedItem.Id);
                    if (unit != null)
                    {
                        bool exists = DataProvider.Ins.DB.Units.Any(u => u.DisplayName.ToLower() == trimmedName.ToLower() && u.Id != unit.Id);
                        if (exists)
                        {
                            MessageBox.Show("Tên đơn vị đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        unit.DisplayName = trimmedName;
                        DataProvider.Ins.DB.SaveChanges();
                        LoadUnits();
                        SelectedItem = unit;
                        MessageBox.Show("Cập nhật thành công!", "Thông báo");
                    }
                }

                IsEditable = false;
                IsSaveEnabled = false;
                IsControlsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearSelection()
        {
            SelectedItem = null;
            DisplayName = "";
        }
    }
}
