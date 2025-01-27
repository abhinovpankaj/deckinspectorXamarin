﻿using SQLite;

namespace Mobile.Code.Models
{
    public class VisualProjectLocationPhoto : BindingModel
    {

        [PrimaryKey]
        public string Id { get; set; }
        public string VisualLocationId { get; set; }

        public string CreatedOn { get; set; }

        public bool IsOriginal { get; set; }
        public bool IsActive { get; set; }
        public string UserId { get; set; }
        public bool IsDelete { get; set; }
        public string ImageName { get; set; }
        public string ImageDescription { get; set; }
        public int SeqNo { get; set; }
        public string Username { get; set; }
        private string _mg;
        public string ImageUrl
        {
            get { return _mg; }
            set { _mg = value; OnPropertyChanged("ImageUrl"); }
        }
        public bool InvasiveImage { get; set; }

        
        
    }
}
