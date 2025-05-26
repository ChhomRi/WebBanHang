using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHang.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { set; get; }
        public DateTime Birthday { set; get; }

    }
}
