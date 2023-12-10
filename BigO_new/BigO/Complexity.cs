using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace BigO
{
   internal enum TimeType {Poly, Log, LinearArithmetic, Constant, Composite }
    class Complexity
    {

      
        internal TimeType time { get; set; }
        internal string complexity { get; set; }
      
        internal Complexity(string complexity)
        {
            this.complexity = complexity;
            this.time = findType(complexity);
        }

        //not fully implemtned
        private TimeType findType(string complexity)
        {
            
            List<string> comp = Expression.Tokenize(complexity);

            if (complexity.Contains("log") && comp.Count(x=> x.Contains("n"))> 1)
                return TimeType.LinearArithmetic;

            

           else if (complexity.Contains("log"))
            {
                return TimeType.Log;
            }
           else if (complexity.Contains("n"))
            {
                return TimeType.Poly;
            }
               return TimeType.Constant;
        }

        internal List<string> Display()
        {
            string temp = "";
            if (complexity.Contains("*")) // to avoid the null reference exception 
            {

                temp = complexity;                
                Expression expression = new Expression(complexity);
                complexity =
                   ConvertToSuperscript(
                    expression.theActualComplexity.complexity);
            }


            if (complexity != "") // exception 
            {
               
                MessageBox.Show($"O({complexity})");          
            }
            else
            {
                complexity = "1";
            }

            return new List<string> { temp!= "" ?  temp + "=>" + complexity: ""  , $"O({complexity})" };
            
        }


        // function for adding the complexities ( will be replaced)
        internal Complexity AddComplexities(List<Complexity> complexity)
        {
            string complex = "";


            for (int i = 0; i < complexity.Count; i++)
            {

                if (complexity[i].complexity != "")
                {
                    complex += complexity[i].complexity;
                    if (i != complexity.Count - 1 && complexity[i + 1].complexity != "")
                        complex += "+";
                }

            }

            if (complex != "" && complex.Contains("+") && complex[0] != '(')
                return new Complexity($"({complex})");
            else
                return new Complexity($"{complex}");

        }

        private string ConvertToSuperscript(string input)
        {
            var superscripts = new Dictionary<char, char>
   {
       {'0', '⁰'},
       {'1', '¹'},
       {'2', '²'},
       {'3', '³'},
       {'4', '⁴'},
       {'5', '⁵'},
       {'6', '⁶'},
       {'7', '⁷'},
       {'8', '⁸'},
       {'9', '⁹'}
      
   };

            var result = new StringBuilder();

            foreach (var c in input)
            {
                if (superscripts.TryGetValue(c, out var superscript) && c != '1')
                {
                    result.Append(superscript);
                }
                else if(c != '^')
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

     

    }
}
