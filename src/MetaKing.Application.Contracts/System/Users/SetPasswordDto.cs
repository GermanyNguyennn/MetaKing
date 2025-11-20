using System;
using System.Collections.Generic;
using System.Text;

namespace MetaKing.System.Users
{
    public class SetPasswordDto
    {
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
