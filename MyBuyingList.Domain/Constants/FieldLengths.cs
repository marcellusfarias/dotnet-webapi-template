using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBuyingList.Domain.Constants;

// follow pattern: [ENTITY]_[PROPERTY]_{MESSAGE}
public static class FieldLengths
{
    // User
    public static int USER_USERNAME_MIN_LENGTH = 3;
    public static int USER_USERNAME_MAX_LENGTH = 32;
    public static int USER_EMAIL_MAX_LENGTH = 254;
    public static int USER_PASSWORD_MAX_LENGTH = 72;

    // Buying List
    public static int BUYINGLIST_NAME_MAX_LENGTH = 64;

    // Group
    public static int GROUP_NAME_MAX_LENGTH = 64;

}
