﻿namespace QuanLyKho.Model
{
    public class DataProvider
    {
        private static DataProvider _ins;
        public static DataProvider Ins 
        {
            get 
            {
                if (_ins == null)
                    _ins = new DataProvider();
                return _ins;
            }
            set { _ins = value; }
        }
        public QuanLyKhoThucAnEntities3 DB { get; set; }
        private DataProvider()
        {
            DB = new QuanLyKhoThucAnEntities3();
        }            
    }
}
