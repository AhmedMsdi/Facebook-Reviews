using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Atreemo.Models
{
    public class User : Person
    {
        [Key]
        public int UserID { get; set; }
        [DisplayName("Last name")]
        public string LastName { get; set; }
        //[Required] //since we don't use it in Update anymore
        [DataType(DataType.Password)]
        [StringLength(128, ErrorMessage = "{0} must be between {2} and {1} characters long.", MinimumLength = 5)]
        public string Password { get; set; }
        //[Required]
        [DisplayName("Confirm password")]
        [StringLength(128, ErrorMessage = "{0} must be between {2} and {1} characters long.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        [System.Web.Mvc.Remote("doesUserNameExist", "Account", HttpMethod = "POST", ErrorMessage = "Username already exists. Please enter a different username.")]
        public string Username { get; set; }
        [DisplayName("Display name")]
        [Required]
        public string UserFullName { get; set; }
        public string Comment { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public bool? IsApproved { get; set; }
        public bool IsActiveDirectory { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? IsOnLine { get; set; }
        [DisplayName("Locked account")]
        public bool? IsLockedOut { get; set; }
        public DateTime? LastLockedOutDate { get; set; }
        public int? FailedPasswordAttemptCount { get; set; }
        public DateTime? FailedPasswordAttemptWindowStart { get; set; }
        public int? FailedPasswordAnswerAttemptCount { get; set; }
        public DateTime? FailedPasswordAnswerAttemptWindowStart { get; set; }
        [DisplayName("Environment")]
        public int DefaultEnv { get; set; }
        [DisplayName("Preferred language")]
        public int PreferedLanguageID { get; set; }
        [DisplayName("Supervised by")]
        public List<int> ApproverUserID { get; set; }
        [DisplayName("Supervised by")]
        public string ApproverUserName { get; set; }
        [DisplayName("Is final approver")]
        public bool IsFinalApprover { get; set; }

        [DisplayName("Event supervised by")]
        public int? EventSupervisedBy { get; set; }
        [DisplayName("Is event final approver")]
        public bool? IsEventFinalApprover { get; set; }

        public string SelectedGroups { get; set; }
        public string SelectedVenues { get; set; }

        [DisplayName("Away from")]
        [DataType(DataType.Date)]
        public DateTime? AwayFrom { get; set; }
        [DisplayName("Back on")]
        [DataType(DataType.Date)]
        public DateTime? AwayTo { get; set; }
        public int EscalatedTo { get; set; }
        public bool CanCloseFeedback { get; set; }
        [DisplayName("Copy to")]
        public int? ForwardTo { get; set; }
        [DisplayName("Employee ID")]
        public string ClientID { get; set; }
        [DisplayName("Number of logins")]
        public int NumberOfLogins { get; set; }
        [DisplayName("First login date")]
        public DateTime? FirstLogin { get; set; }
        public int? PageID { get; set; }
        public string PageURL { get; set; }
        public string PageActionName { get; set; }
        public string PageControllerName { get; set; }
        public DateTime? PageLastModifiedOn { get; set; }

        public int? RemainingDaysBeforeExpiry { get; set; }
        public bool NeverExpires { get; set; }

        [DisplayName("Days to expire")]
        public int? DaysToExpire { get; set; }

        [DisplayName("Password last changed on")]
        public string LastPasswordChangedDateString { get; set; }
        public bool CanUpdateEmail { get; set; }

        public bool? UserIsGroup { get; set; }
        [DisplayName("Groups")]
        public string UserGroups { get; set; }
        [DisplayName("Venues")]
        public string UserVenues { get; set; }
        [DisplayName("Feedback notifications")]
        public bool FeedbackNotifications { get; set; }
        [DisplayName("Enquiry notifications")]
        public bool EnquiryNotifications { get; set; }
        [DisplayName("Surveys notifications")]
        public bool SurveysNotifications { get; set; }
        [DisplayName("SLP notifications")]
        public bool SLPNotifications { get; set; }
        public string Areas { get; set; }
        public bool? HasSite { get; set; }
        public int UsedAsTo { get; set; }
        public int SendFeedbackNotificationAfterReply { get; set; }
        public User()
        {

        }

        public User(int UserID, string Title, string Username, string UserFullName, string Email)
        {
            this.UserID = UserID;
            this._Title = Title;
            this.Username = Username;
            this.UserFullName = UserFullName;
            this.Email = Email;
        }

        public User(int UserID, string UserFullName, bool? UserIsGroup, string Username, string FirstName, string LastName, string Email)
        {
            this.UserID = UserID;
            this.UserFullName = UserFullName;
            this.UserIsGroup = UserIsGroup;
            this.Username = Username;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
        }
    }
}