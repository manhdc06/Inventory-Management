﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyKho.ViewModel
{
    public class ControlBarViewModel : BaseViewModel
    {
        #region commands
        public ICommand CloseWindowCommand { get; set; }
        public ICommand MaximizedWindowCommand { get; set; }
        public ICommand MinimizedWindowCommand { get; set; }

        public ICommand MouseMoveWindowCommand { get; set; }
        #endregion
        public ControlBarViewModel() 
        {
           CloseWindowCommand = new RelayCommand<UserControl>((p) => { return true; }, (p) => {
               FrameworkElement window = GetWindowParent(p);
               var w = window as Window;
               if (w != null)
               {
                   w.Close();
               }
           });
            MaximizedWindowCommand = new RelayCommand<UserControl>((p) => { return true; }, (p) => {
                FrameworkElement window = GetWindowParent(p);
                var w = window as Window;
                if (w != null)
                {
                    if(w.WindowState != WindowState.Maximized)
                        w.WindowState=WindowState.Maximized;
                    else w.WindowState=WindowState.Normal;
                }
            });
            MinimizedWindowCommand = new RelayCommand<UserControl>((p) => { return true; }, (p) => {
                FrameworkElement window = GetWindowParent(p);
                var w = window as Window;
                if (w != null)
                {
                    if (w.WindowState != WindowState.Minimized)
                        w.WindowState = WindowState.Minimized;
                    else w.WindowState = WindowState.Maximized;
                }
            });
            MouseMoveWindowCommand = new RelayCommand<UserControl>((p) => { return true; }, (p) => {
                FrameworkElement window = GetWindowParent(p);
                var w = window as Window;
                if (w != null)
                {
                    w.DragMove();
                }
            });
        }
        FrameworkElement GetWindowParent(UserControl p)
        {
            FrameworkElement parent = p;
            while (parent.Parent != null) { 
            parent =parent.Parent as FrameworkElement;
            }
            return parent;
        }
    }
}
