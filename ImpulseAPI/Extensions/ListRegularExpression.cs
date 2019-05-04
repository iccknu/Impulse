using Providers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ImpulseAPI.Extensions
{
    public class ListRegularExpression : RegularExpressionAttribute
    {
        public ListRegularExpression(string pattern) : base(pattern) { }

        public override bool IsValid(object obj)
        {
            ICollection<string> emailsOrNumbers = (ICollection<string>)obj;

            if (emailsOrNumbers.Count == 0)
                return false;

            var regex = new Regex(Pattern);
            foreach (string emailsOrNumber in emailsOrNumbers)
            {
                if (!regex.IsMatch(emailsOrNumber) && !EmailProvider.IsValidEmail(emailsOrNumber))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
