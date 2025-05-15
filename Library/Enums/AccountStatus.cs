namespace TruckDispatcherApi.Library
{
    /// <summary>
    /// Status to manage user account by rules for last login date,
    /// sending warnings (email and notifications). Only for Admin purposes.
    /// </summary>
    public enum AccountStatus
    {
        None,

        /// <summary>
        /// Registered user
        /// </summary>
        ActiveUser,

        /// <summary>
        /// User's last login date is more then 1 year ago and admin has sent warning about deleting account in 7 days.
        /// Account must be deleted in 1 year + 7 days after last login date.
        /// </summary>
        InactiveUserToRemove,

        /// <summary>
        /// User's last login date is more then 1 year ago and admin has sent warning about deleting account in 7 days.
        /// Account must be deleted in 1 year + 7 days after last login date.
        /// </summary>
        Warned
    }
}
