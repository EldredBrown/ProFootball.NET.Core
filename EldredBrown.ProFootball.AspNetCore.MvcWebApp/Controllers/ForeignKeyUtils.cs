using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    public class ForeignKeyUtils
    {
        public static bool ForeignKeyConstraintConflictExistsOnCreate(string message)
        {
            return message.StartsWith("The INSERT statement conflicted with the FOREIGN KEY constraint");
        }

        public static bool ForeignKeyConstraintConflictExistsOnEdit(string message)
        {
            return message.StartsWith("The UPDATE statement conflicted with the FOREIGN KEY constraint");
        }

        public static void AddModelErrorForForeignKeyConstraintConflict(string errMsgIntro, string excMessage,
            ModelStateDictionary modelState)
        {
            var errMsgBody = "Conflict with a FOREIGN KEY constraint on";
            var foreignKeyConstraint = excMessage.Split('"')[1];
            var key = foreignKeyConstraint.Split('_')[3];
            modelState.AddModelError(string.Empty, $"{errMsgIntro} {errMsgBody} {key}.");
        }
    }
}
