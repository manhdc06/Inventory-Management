using QuanLyKho.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuanLyKho.ViewModel
{
    public class UserViewModel : BaseViewModel
    {
        private ObservableCollection<User> _List;
        public ObservableCollection<User> List
        {
            get => _List;
            set { _List = value; OnPropertyChanged(); }
        }

        private ObservableCollection<UsersRole> _Role;
        public ObservableCollection<UsersRole> Role
        {
            get => _Role;
            set { _Role = value; OnPropertyChanged(); }
        }

        private User _SelectedItem;
        public User SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    UserName = SelectedItem.UserName;
                    DisplayName = SelectedItem.DisplayName;
                    SelectedRole = SelectedItem.UsersRole;
                }
            }
        }
        private string _Password;
        public string Password
        {
            get => _Password;
            set { _Password = value; OnPropertyChanged(); }
        }
        private string _UserName;
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }

        private string _DisplayName;
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }

        private UsersRole _SelectedRole;
        public UsersRole SelectedRole { get => _SelectedRole; set { _SelectedRole = value; OnPropertyChanged(); } }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }

        public UserViewModel()
        {
            // Load data from the database
            List = new ObservableCollection<User>(DataProvider.Ins.DB.Users);
            Role = new ObservableCollection<UsersRole>(DataProvider.Ins.DB.UsersRoles); // Load roles

            // Add User Command
            AddCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(DisplayName) || SelectedRole == null)
                    return false;

                var displayList = DataProvider.Ins.DB.Users.Where(x => x.UserName == UserName);
                return !displayList.Any();
            }, (p) =>
            {
                var user = new User()
                {
                    UserName = UserName,
                    DisplayName = DisplayName,
                    IdRole = SelectedRole.Id,
                    Password = "123456"
                };

                DataProvider.Ins.DB.Users.Add(user);
                DataProvider.Ins.DB.SaveChanges();
                List.Add(user);

                MessageBox.Show("Thêm người dùng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // Reset form
                UserName = "";
                DisplayName = "";
                SelectedRole = null;
            });


            // Edit User Command
            EditCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(DisplayName) || SelectedItem == null || SelectedRole == null)
                    return false;

                var displayList = DataProvider.Ins.DB.Users.Where(x => x.UserName == UserName && x.Id != SelectedItem.Id);
                return !displayList.Any();
            }, (p) =>
            {
                var user = DataProvider.Ins.DB.Users.SingleOrDefault(x => x.Id == SelectedItem.Id);
                if (user != null)
                {
                    user.UserName = UserName;
                    user.DisplayName = DisplayName;
                    user.IdRole = SelectedRole.Id;
                    DataProvider.Ins.DB.SaveChanges();

                    List = new ObservableCollection<User>(DataProvider.Ins.DB.Users); // Refresh list

                    MessageBox.Show("Sửa người dùng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Reset form
                UserName = "";
                DisplayName = "";
                SelectedRole = null;
                SelectedItem = null;
            });


            // Delete User Command
            DeleteCommand = new RelayCommand<object>((p) =>
            {
                return SelectedItem != null;
            }, (p) =>
            {
                var result = MessageBox.Show("Bạn có chắc muốn xoá người dùng này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    DataProvider.Ins.DB.Users.Remove(SelectedItem);
                    DataProvider.Ins.DB.SaveChanges();
                    List.Remove(SelectedItem);

                    MessageBox.Show("Xoá người dùng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Reset form
                    UserName = "";
                    DisplayName = "";
                    SelectedRole = null;
                    SelectedItem = null;
                }
            });



            // Change Password Command
            ChangePasswordCommand = new RelayCommand<object>((p) =>
            {
                return SelectedItem != null && !string.IsNullOrEmpty(Password);
            }, (p) =>
            {
                var user = DataProvider.Ins.DB.Users.SingleOrDefault(x => x.Id == SelectedItem.Id);
                if (user != null)
                {
                    string encodedPassword = LoginViewModel.MD5Hash(Password); // mã hóa giống khi đăng nhập
                    user.Password = encodedPassword;
                    DataProvider.Ins.DB.SaveChanges();

                    MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Reset password field
                Password = "";
            });
        }
    }
}
