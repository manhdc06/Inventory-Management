   M S B A M L   `   `   `  ����D  @QuanLyKho, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:QuanLyKho	QuanLyKho  5   	   ?$clr-namespace:QuanLyKho.UserControlAQuanLyKho.UserControlA  5          QuanLyKho.SuplierWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:QuanLyKho     5   	   3uc$clr-namespace:QuanLyKho.UserControlA     5      U QMaterialDesignThemes.Wpf, Version=2.3.1.953, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5	   	   -   5          �Name$  window��5         MaterialDesignBody#���   5
   	   $��Regular"�5   	   $��13(�5   	   &   System.Windows.Media.TextOptions   TextFormattingMode$ Ideal=�5   	      TextRenderingMode$
 Auto=�5   	     MaterialDesignPaper#���  5   	     MaterialDesignFont#���  5   	    � 
ResizeMode$ NoResize=�5   	    � WindowStyle$
 None=�5   	    � WindowStartupLocation$ CenterScreen=�5   	    � Title$ Nhà cung cấp��5      $	��500��6"   $	��700��6/   .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   6,   �� 5      6   .+�5   
   � $��0��6   .+�5      )   #QuanLyKho.UserControlA.ControlBarUC  
 � Tag 6   �� *Title+	 �� ElementName$	 window��6I   5      � 5    
   $��1��6    5!      �� 5"      $
��auto �6    60   �� 5#      $
��auto �6    60   �� 5$      6"   .+�5(      #  MaterialDesignThemes.Wpf.Card  $��0��6"   $��5q�6/   .��5)      � $��0 0 0 10q�6   .+�5*      �� $��5 5 15 5q�6;   )  #MaterialDesignThemes.Wpf.HintAssist
   Hint$
 Tên đơn vị đo��6M    % !MaterialDesignFloatingHintTextBox#��[ 6|   ��6   �� *DisplayName+6|   �� 5+      $��5 5 15 5q�68   $
 Địa chỉ��6J   #��[ 6s   ��6   �� *	Address+6s   �� 5,      $��5 5 15 5q�66   $
 Điện thoại��6H   #��[ 6t   ��6   �� *Phone+6t   �� 5-      $��5 5 15 5q�66   $
 Email��6H   #��[ 6o   ��6   �� *Email+6o   �� 5.      $��5 5 15 5q�69   $
 Thông tin thêm��6K   #��[ 6{   ��6   �� *
MoreInfo+6{   (  "System.Windows.Controls.DatePicker  5/      $��Center=�6"   $	��100��6b   $��	5 5 15 -7q�6o   $
 Ngày hợp tác��6�    ( $MaterialDesignFloatingHintDatePicker#��[ 6�      SelectedDate 6=   �� *ContracDate+6�   50      51        52      $��1��6"   $��5q�6/   .��53      � .+�54      � .+�55      �� $��Thêm��6@   $��5q�6O   $	��100��6Z   ��6!   �� *
AddCommand+6h   �� 56      $
��Xoá��6C   $��5q�6Q   $	��100��6\   ��6!   �� *DeleteCommand+6j   �� 57      $��Sửa��6A   $��5q�6O   $	��100��6Z   ��6!   �� *EditCommand+6h   58      59      5:        5;      $��2��6"   $��5q�6/   $��Stretch=�6:   .��5<      �� $
��Auto=�6   .��5=      �� ��6   �� *List+ �� SelectedItem 6<   �� *SelectedItem+ �� Mode$ OneWayToSource=� �� View 5>      �� 5?      .P�5@   "   �� $��id��61    �� DisplayMemberBinding 6=   �� *Id+6c   �� 5A   "   $��Tên đơn vị đo��61    6H   �� *DisplayName+6w   �� 5B   "   $��Địa chỉ��61    6B   �� *	Address+6m   �� 5C   "   $��Điện thoại��61    6E   �� *Phone+6n   �� 5D   "   $��Email��61    6@   �� *Email+6i   �� 5E   "   $��Thông tin khác��61    6I   �� *
MoreInfo+6u   �� 5F   "   $��Ngày hợp tác��61    6G   �� *ContracDate+6v   5G      5I      5J      5K      5L      5M      5N      