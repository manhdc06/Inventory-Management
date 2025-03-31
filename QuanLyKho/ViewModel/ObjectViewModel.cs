using QuanLyKho.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Object = QuanLyKho.Model.Object;

namespace QuanLyKho.ViewModel
{
    public class ObjectViewModel : BaseViewModel
    {
        private ObservableCollection<Object> _List;
        public ObservableCollection<Object> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<Unit> _Unit;
        public ObservableCollection<Unit> Unit { get => _Unit; set { _Unit = value; OnPropertyChanged(); } }

        private ObservableCollection<Suplier> _Suplier;
        public ObservableCollection<Suplier> Suplier { get => _Suplier; set { _Suplier = value; OnPropertyChanged(); } }

        private Object _SelectedItem;
        public Object SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    DisplayName = SelectedItem.DisplayName;
                    SelectedUnit = SelectedItem.Unit;
                    SelectedSuplier = SelectedItem.Suplier;
                    QRcode = SelectedItem.QRcode;
                    BarCode = SelectedItem.BarCode;
                }
            }
        }

        private string _DisplayName;
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }

        private Unit _SelectedUnit;
        public Unit SelectedUnit { get => _SelectedUnit; set { _SelectedUnit = value; OnPropertyChanged(); } }

        private Suplier _SelectedSuplier;
        public Suplier SelectedSuplier { get => _SelectedSuplier; set { _SelectedSuplier = value; OnPropertyChanged(); } }

        private string _QRcode;
        public string QRcode { get => _QRcode; set { _QRcode = value; OnPropertyChanged(); } }

        private string _BarCode;
        public string BarCode { get => _BarCode; set { _BarCode = value; OnPropertyChanged(); } }

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

        public ObjectViewModel()
        {
            // Initialize properties
            IsEditable = false;
            IsSaveEnabled = false;
            IsControlsEnabled = true;

            // Load data
            LoadObjects();
            LoadUnits();
            LoadSupliers();

            // Initialize commands
            AddCommand = new RelayCommand<object>((p) => { return true; }, (p) => { AddObject(); });
            EditCommand = new RelayCommand<object>((p) => { return SelectedItem != null; }, (p) => { EditObject(); });
            DeleteCommand = new RelayCommand<object>((p) => { return SelectedItem != null; }, (p) => { DeleteObject(); });
            SaveCommand = new RelayCommand<object>((p) => { return IsSaveEnabled && !string.IsNullOrEmpty(DisplayName) && SelectedUnit != null && SelectedSuplier != null; }, (p) => { SaveObject(); });
        }

        private void LoadObjects()
        {
            List = new ObservableCollection<Object>(DataProvider.Ins.DB.Objects);
        }

        private void LoadUnits()
        {
            Unit = new ObservableCollection<Unit>(DataProvider.Ins.DB.Units);
        }

        private void LoadSupliers()
        {
            Suplier = new ObservableCollection<Suplier>(DataProvider.Ins.DB.Supliers);
        }

        private void AddObject()
        {
            isAddNew = true;

            // Clear form fields
            DisplayName = "";
            SelectedUnit = null;
            SelectedSuplier = null;
            QRcode = "";
            BarCode = "";

            // Enable editing and save button
            IsEditable = true;
            IsSaveEnabled = true;
        }

        private void EditObject()
        {
            isAddNew = false;

            // Enable editing and save button
            IsEditable = true;
            IsSaveEnabled = true;
        }

        private void DeleteObject()
        {
            if (SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa vật tư này không?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Check if the object is used in other tables before deleting
                        var inputInfoCheck = DataProvider.Ins.DB.InputInfoes.Where(x => x.IdObject == SelectedItem.Id).Count();
                        var outputInfoCheck = DataProvider.Ins.DB.OutputInfoes.Where(x => x.IdObject == SelectedItem.Id).Count();

                        if (inputInfoCheck > 0 || outputInfoCheck > 0)
                        {
                            MessageBox.Show("Không thể xóa vật tư này vì đã được sử dụng trong phiếu nhập hoặc phiếu xuất!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // Delete from database
                        DataProvider.Ins.DB.Objects.Remove(SelectedItem);
                        DataProvider.Ins.DB.SaveChanges();

                        // Update list
                        List.Remove(SelectedItem);

                        // Clear selection
                        ClearSelection();

                        MessageBox.Show("Xóa vật tư thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Có lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void SaveObject()
        {
            if (isAddNew) // Add new object
            {
                try
                {
                    // Create new object
                    var newObject = new Object()
                    {
                        Id = Guid.NewGuid().ToString(),
                        DisplayName = DisplayName,
                        IdUnit = SelectedUnit.Id,
                        IdSuplier = SelectedSuplier.Id,
                        QRcode = QRcode,
                        BarCode = BarCode,
                        Unit = SelectedUnit,
                        Suplier = SelectedSuplier
                    };

                    // Add to database
                    DataProvider.Ins.DB.Objects.Add(newObject);
                    DataProvider.Ins.DB.SaveChanges();

                    // Add to list
                    List.Add(newObject);

                    // Set as selected item
                    SelectedItem = newObject;

                    MessageBox.Show("Thêm vật tư thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi khi thêm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else // Update existing object
            {
                try
                {
                    // Find and update the object
                    var objectToUpdate = DataProvider.Ins.DB.Objects.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();

                    if (objectToUpdate != null)
                    {
                        objectToUpdate.DisplayName = DisplayName;
                        objectToUpdate.IdUnit = SelectedUnit.Id;
                        objectToUpdate.IdSuplier = SelectedSuplier.Id;
                        objectToUpdate.QRcode = QRcode;
                        objectToUpdate.BarCode = BarCode;

                        // Update navigation properties
                        objectToUpdate.Unit = SelectedUnit;
                        objectToUpdate.Suplier = SelectedSuplier;

                        // Save changes
                        DataProvider.Ins.DB.SaveChanges();

                        // Update in the list
                        LoadObjects();

                        MessageBox.Show("Cập nhật vật tư thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
        }

        private void ClearSelection()
        {
            SelectedItem = null;
            DisplayName = "";
            SelectedUnit = null;
            SelectedSuplier = null;
            QRcode = "";
            BarCode = "";
        }
    }
}