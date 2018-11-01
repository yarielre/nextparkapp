using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inside.WebApi.ViewModels
{
    public class ChangePasswordViewModel
    {
        public int Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
