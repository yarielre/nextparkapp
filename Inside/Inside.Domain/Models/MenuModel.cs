using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Inside.Xamarin.Models
{
   public class MenuModel
    {
        #region Properties
        public string Icon { get; set; }
        public string Title { get; set; }
        public string PageName { get; set; }
        #endregion

        #region Commands
        public ICommand OnTabActionCommand { get; set; }
        #endregion
    }
}
