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
                    DisplayName = SelectedItem.Displayname;
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
            // Initialize properties
            IsEditable = false;
            IsSaveEnabled = false;
            IsControlsEnabled = true;

            // Load data
            LoadUnits();

            // Initialize commands
            AddCommand = new RelayCommand<object>((p) => { return true; }, (p) => { AddUnit(); });
            EditCommand = new RelayCommand<object>((p) => { return SelectedItem != null; }, (p) => { EditUnit(); });
            DeleteCommand = new RelayCommand<object>((p) => { return SelectedItem != null; }, (p) => { DeleteUnit(); });
            SaveCommand = new RelayCommand<object>((p) => { return IsSaveEnabled && !string.IsNullOrEmpty(DisplayName); }, (p) => { SaveUnit(); });
        }

        private void LoadUnits()
        {
            List = new ObservableCollection<Unit>(DataProvider.Ins.DB.Units);
        }

        private void AddUnit()
        {
            isAddNew = true;

            // Clear form fields
            DisplayName = "";

            // Enable editing and save button
            IsEditable = true;
            IsSaveEnabled = true;
            IsControlsEnabled = false;
        }

        private void EditUnit()
        {
            isAddNew = false;

            // Enable editing and save button
            IsEditable = true;
            IsSaveEnabled = true;
            IsControlsEnabled = false;
        }

        private void DeleteUnit()
        {
            if (SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa đơn vị đo này không?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Check if the unit is used in Objects table
                        var objectCheck = DataProvider.Ins.DB.Objects.Where(x => x.IdUnit == SelectedItem.Id).Count();

                        if (objectCheck > 0)
                        {
                            MessageBox.Show("Không thể xóa đơn vị đo này vì đã được sử dụng trong danh sách vật tư!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // Delete from database
                        DataProvider.Ins.DB.Units.Remove(SelectedItem);
                        DataProvider.Ins.DB.SaveChanges();

                        // Update list
                        List.Remove(SelectedItem);

                        // Clear selection
                        ClearSelection();

                        MessageBox.Show("Xóa đơn vị đo thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Có lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void SaveUnit()
        {
            if (isAddNew) // Add new unit
            {
                try
                {
                    // Check if unit with same name already exists
                    var existingUnit = DataProvider.Ins.DB.Units.FirstOrDefault(u => u.Displayname.ToLower() == DisplayName.ToLower());
                    if (existingUnit != null)
                    {
                        MessageBox.Show("Đơn vị đo này đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Create new unit
                    var newUnit = new Unit()
                    {
                        Displayname = DisplayName
                    };

                    // Add to database
                    DataProvider.Ins.DB.Units.Add(newUnit);
                    DataProvider.Ins.DB.SaveChanges();

                    // Add to list
                    List.Add(newUnit);

                    // Set as selected item
                    SelectedItem = newUnit;

                    MessageBox.Show("Thêm đơn vị đo thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi khi thêm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else // Update existing unit
            {
                try
                {
                    // Check if another unit with same name already exists
                    var existingUnit = DataProvider.Ins.DB.Units.FirstOrDefault(u =>
                        u.Displayname.ToLower() == DisplayName.ToLower() &&
                        u.Id != SelectedItem.Id);

                    if (existingUnit != null)
                    {
                        MessageBox.Show("Đơn vị đo này đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Find and update the unit
                    var unitToUpdate = DataProvider.Ins.DB.Units.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();

                    if (unitToUpdate != null)
                    {
                        unitToUpdate.Displayname = DisplayName;

                        // Save changes
                        DataProvider.Ins.DB.SaveChanges();

                        // Update in the list
                        LoadUnits();

                        MessageBox.Show("Cập nhật đơn vị đo thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi khi cập nhật: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Reset state
            IsEditable = false;
            IsSaveEnabled = false;
            IsControlsEnabled = true;
        }

        private void ClearSelection()
        {
            SelectedItem = null;
            DisplayName = "";
        }
    }
}