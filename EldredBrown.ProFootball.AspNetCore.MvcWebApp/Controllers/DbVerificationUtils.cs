using System.Linq;
using System.Text;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers
{
    public class DbVerificationUtils
    {
        public const string ErrMsgIntro = "Unable to save changes.";

        public enum SqlOperation
        {
            INSERT,
            UPDATE,
        }

        public static void AddModelErrorForForeignKeyConstraintConflict(
            ModelStateDictionary modelState, string excMessage
        )
        {
            var errMsgBody = "Conflict with a FOREIGN KEY constraint on";
            var foreignKeyConstraint = excMessage.Split('"')[1];
            var key = foreignKeyConstraint.Split('_').Last();
            modelState.AddModelError(string.Empty, $"{ErrMsgIntro} {errMsgBody} {key}.");
        }

        public static void AddModelErrorForStringTooLong(ModelStateDictionary modelState, string key)
        {
            modelState.AddModelError(key, $"{ErrMsgIntro} The entered {key} is too long.");
        }

        public static void AddModelErrorForUniqueKeyConstraintConflict(
            ModelStateDictionary modelState, string excMessage = null
        )
        {
            var errMsg = new StringBuilder($"{ErrMsgIntro} Violation of UNIQUE KEY constraint");
            if (!excMessage.IsNullOrEmpty())
            {
                var uniqueKeyConstraint = excMessage.Split('\'')[1];
                var key = uniqueKeyConstraint.Split('_').Last();
                errMsg.Append($" {key}");
            }
            errMsg.Append('.');
            modelState.AddModelError(string.Empty, errMsg.ToString());
        }

        public static bool ForeignKeyConstraintConflictExists(string sqlOperation, string message)
        {
            return message.StartsWith($"The {sqlOperation} statement conflicted with the FOREIGN KEY");
        }

        public static bool ForeignKeyConstraintConflictExistsOnCreate(string message)
        {
            return message.StartsWith("The INSERT statement conflicted with the FOREIGN KEY");
        }

        public static bool ForeignKeyConstraintConflictExistsOnEdit(string message)
        {
            return message.StartsWith("The UPDATE statement conflicted with the FOREIGN KEY");
        }

        public static string GetColumnNameFromDbUpdateException(DbUpdateException ex)
        {
            return ex.InnerException.Message.Split(", ")[1].Split(' ')[1].Trim('.');
        }

        public static bool StringTooLong(DbUpdateException ex)
        {
            return ex.InnerException.Message.StartsWith("String or binary data would be truncated in table");
        }

        public static bool UniqueKeyConstraintExists(string message)
        {
            return message.StartsWith("Violation of UNIQUE KEY constraint");
        }
    }
}
