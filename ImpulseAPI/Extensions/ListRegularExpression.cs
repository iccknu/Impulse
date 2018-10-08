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
            IEnumerable<string> numbers = (IEnumerable<string>)obj;
            var regex = new Regex(Pattern);
            foreach (string number in numbers)
            {
                if (!regex.IsMatch(number))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
