using System.Web.Profile;
using System.Web.Security;

namespace CustomTypes.Objects
{
    public class UserProfile : ProfileBase
    {
        public static UserProfile GetUserProfile(string username)
        {
            var userProfile      = Create(username) as UserProfile;
            userProfile.UserName = username;
            return userProfile;
        }

        public static UserProfile GetUserProfile()
        {
            return Create(Membership.GetUser().UserName) as UserProfile;
        }

        static public UserProfile NewUser
        {
            get
            {
                return (UserProfile)
                       (Create(Membership.GetUser(System.Web.HttpContext.Current.Session["Username"]).UserName));
            }
        }

        static public UserProfile LoggedUser
        {
            get
            {
                return (UserProfile)
                       (Create(Membership.GetUser().UserName));
            }
        }

        public string UserID        { get; set; }

        public new string UserName  { get; set; }

        public bool IsLocked        { get; set; }

        public string FirstName
        {
            get { return ((string)(base["FirstName"])); }
            set { base["FirstName"] = value; Save(); }
        }

        public int DepartementID
        {
            get { return ((int)(base["DepartementID"])); }
            set { base["DepartementID"] = value; Save(); }
        }

        public int? SupervisorDepartementID
        {
            get { return ((int?)(base["SupervisorDepartementID"])); }
            set { base["SupervisorDepartementID"] = value; Save(); }
        }

        public string LastName
        {
            get { return ((string)(base["LastName"])); }
            set { base["LastName"] = value; Save(); }
        }
        
        public bool SalesViewByPass
        {
            get { return ((bool)(base["SalesViewByPass"])); }
            set { base["SalesViewByPass"] = value; Save(); }
        }
    }
}
