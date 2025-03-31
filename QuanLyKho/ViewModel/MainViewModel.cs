using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using QuanLyKho.Model;

namespace QuanLyKho.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<Inventory> InventoryList { get => _InventoryList; set { _InventoryList = value; OnPropertyChanged(); } }
        private ObservableCollection<Inventory> _InventoryList;

        public bool IsLoaded { get; set; } = false;
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand UnitCommand { get; set; }
        public ICommand SuplierCommand { get; set; }
        public ICommand CustomerCommand { get; set; }
        public ICommand ObjectCommand { get; set; }
        public ICommand UserCommand { get; set; }
        public ICommand InputCommand { get; set; }
        public ICommand OutputCommand { get; set; }

        public MainViewModel()
        {
            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                IsLoaded = true;
                if (p == null)
                    return;
                p.Hide();
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();

                if (loginWindow.DataContext == null)
                    return;
                var LoginVM = loginWindow.DataContext as LoginViewModel;

                if (LoginVM.IsLogin)
                {
                    p.Show();
                    LoadInventoryData();
                }
                else
                {
                    p.Close();
                }

            });

            UnitCommand = new RelayCommand<object>((p) => true, (p) => new UnitWindow().ShowDialog());
            SuplierCommand = new RelayCommand<object>((p) => true, (p) => new SuplierWindow().ShowDialog());
            CustomerCommand = new RelayCommand<object>((p) => true, (p) => new CustomerWindow().ShowDialog());
            ObjectCommand = new RelayCommand<object>((p) => true, (p) => new ObjectWindow().ShowDialog());
            UserCommand = new RelayCommand<object>((p) => true, (p) => new UserWindow().ShowDialog());
            InputCommand = new RelayCommand<object>((p) => true, (p) => new InputWindow().ShowDialog());
            OutputCommand = new RelayCommand<object>((p) => true, (p) => new OutputWindow().ShowDialog());
        }

        void LoadInventoryData()
        {
            InventoryList = new ObservableCollection<Inventory>();
            var objectList = DataProvider.Ins.DB.Objects;

            int i = 1;
            foreach (var item in objectList)
            {
                var inputList = DataProvider.Ins.DB.InputInfoes.Where(p => p.IdObject == item.Id);
                var outputList = DataProvider.Ins.DB.OutputInfoes.Where(p => p.IdObject == item.Id);

                int sumInput = inputList.Any() ? inputList.Sum(p => p.Count ?? 0) : 0;
                int sumOutput = outputList.Any() ? outputList.Sum(p => p.Count ?? 0) : 0;
                int stockQuantity = Math.Max(0, sumInput - sumOutput);

                Inventory inventory = new Inventory
                {
                    STT = i++,
                    Object = item,
                    InputCount = sumInput,
                    OutputCount = sumOutput,
                    Count = stockQuantity
                };

                InventoryList.Add(inventory);
            }

            // Tính tổng
            TotalInput = InventoryList.Sum(x => x.InputCount);
            TotalOutput = InventoryList.Sum(x => x.OutputCount);
            TotalInventory = InventoryList.Sum(x => x.Count);
        }

        // Tổng toàn kho
        private int _TotalInput;
        public int TotalInput
        {
            get => _TotalInput;
            set { _TotalInput = value; OnPropertyChanged(); }
        }

        private int _TotalOutput;
        public int TotalOutput
        {
            get => _TotalOutput;
            set { _TotalOutput = value; OnPropertyChanged(); }
        }

        private int _TotalInventory;
        public int TotalInventory
        {
            get => _TotalInventory;
            set { _TotalInventory = value; OnPropertyChanged(); }
        }

        // Lựa chọn vật tư => xem chi tiết
        private Inventory _SelectedInventory;
        public Inventory SelectedInventory
        {
            get => _SelectedInventory;
            set
            {
                _SelectedInventory = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayedInput));
                OnPropertyChanged(nameof(DisplayedOutput));
                OnPropertyChanged(nameof(DisplayedInventory));
            }
        }

        // Hiển thị tùy theo dòng chọn
        public int DisplayedInput => SelectedInventory != null ? SelectedInventory.InputCount : TotalInput;
        public int DisplayedOutput => SelectedInventory != null ? SelectedInventory.OutputCount : TotalOutput;
        public int DisplayedInventory => SelectedInventory != null ? SelectedInventory.Count : TotalInventory;
    }
}
