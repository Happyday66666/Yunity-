using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Yunity.Areas.Identity.Data;

// Add profile data for application users by adding properties to the YunityUser class
public class YunityUser : IdentityUser
{
    //新增欄位到ASPUser

    public string? Role { get; set; }
}

