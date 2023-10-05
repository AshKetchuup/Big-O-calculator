using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ForLoopBigO
{
      class Expression
    {

      private  List<string> RHS;
        private List<string> LHS;

      

        string  FindOperation(string op) // use regex for this 
            // return whether it is +,-,/ or *
        {
            // Regular expression pattern to match the specified mathematical operators.
            string pattern = @"[\+\-\^\/\*]";

            // Use Regex.IsMatch to check if the input string contains any matches for the pattern.

            return Regex.Match(pattern, op).Value;
        
        }
                
        


    }
}
