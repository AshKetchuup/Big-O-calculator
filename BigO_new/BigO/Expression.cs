using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;


namespace BigO
{
    class Expression
    {
      

        string code = @"
    void MyMethod()
    {
        for (int i = 1; i < n; i++)
        {
i=i/2;            
        }

int z = 0;

while(z < n)
{
z+= 3
}

}

for(int i =0;i<n; i*=2)
{
i++;
}
";

        Complexity Complexity { get; set; }

        Dictionary<string, string> oppositeOperation = new Dictionary<string, string>
        {
            { "+", "-" },
      
            { "-", "+" },
            { "*", "/" },
            { "/", "*" },
            { "^", "log" } // You can add other operators as needed
        };
        Dictionary<string, string> repeatedOperation = new Dictionary<string, string>
        {
            { "+", "*" },
            
            { "-", "/" },
            { "*", "^" },
            { "/", "/^" },
            
        };




        private List<string> operationDominance = new List<string>
        {
        "-+", "*/", "^", "log"

        };


        

        //Constructor for loop statements  
        public Expression(string initial, string condition, string iteration, string modification = "")
        {


            try
            {

                //Objective Expression.1.2
                //Check if the /= or *= iterators are not used when the intial is 0 


                
                if (initial == "0" && (FindOperation(iteration) == "*" || FindOperation(iteration) == "/") && !(FindOperation(modification) == "+" || FindOperation(modification) == "-"))
                {
                    throw new Exception("If you are multiplying or dividing 0 it will stay the same and hence an infinite loop.");
                }
                //check this for both the iteration and modification
                // since iteration cannot be null
               else if (initial == "0" && (FindOperation(modification) == "*" || FindOperation(modification) == "/") && !(FindOperation(iteration) == "+" || FindOperation(iteration) == "-"))
                {
                    throw new Exception("If you are multiplying or dividing 0 it will stay the same and hence an infinite loop.");
                }



                // finds the more dominant operation between the modification and the iteration
                iteration = (modification != "") ? findHigherOperation(modification, iteration) : iteration;


                //MessageBox.Show(initial);
                //MessageBox.Show(condition);
                //MessageBox.Show(iteration);

                // Checks whether the loop conditions are correct and returns a bool
                if (CheckLoopConditions(ref initial, ref condition, iteration))
                {

                Complexity comp =  LoopToEquation(initial, condition, iteration);
                    comp.Display();
                }


                else
                {
                    throw new ArgumentException("Your loop conditions are not right");
                }
            }

            catch (Exception ex)
            {                
                MessageBox.Show(ex.ToString());
            }



           


        }


        private bool CheckLoopConditions(ref string initial, ref string condition, string iteration)
        {


            //Objective Expression.1.1 Checking the loop conditions so that they are right

            bool check = false;
            if (condition.Contains(">"))
            {
                condition = condition.Substring(condition.IndexOf(">") + 1);


                if (condition.Contains("="))
                {
                    condition = condition.Substring(condition.IndexOf("=") + 1);

                }
                
                // Check if the loop will actually decrement
                // we have to swap the initial and condition if it is decrementing
                // And replace the iteration with the opposite operation because going the opposite way
                
                if(!(FindOperation(iteration)== "*" || FindOperation(iteration) == "+"))
                {
                    string temp = initial;
                    initial = condition;
                    condition = temp;
                    iteration= oppositeOperation[FindOperation(iteration)];
                    check = true;
                }
            }
            if (condition.Contains("<"))
            {
                condition = condition.Substring(condition.IndexOf("<") + 1);
                
                if (condition.Contains("="))
                {
                    condition = condition.Substring(condition.IndexOf("=") + 1);

                }
                // Check if the loop will actually increment
                if ((FindOperation(iteration) == "*" || FindOperation(iteration) == "+"))
                {
                    check = true;
                }
            }
            return check;
        }
        private Complexity LoopToEquation(string initial, string condition, string iteration)
        {

            //Objective Expression.1.3
            //Check if the condition is constant 
            //where the initial and the final are numbers
            if (int.TryParse(condition, out int result) && int.TryParse(initial.Substring(initial.IndexOf("=") + 1), out int innit))
            {
                MessageBox.Show("O(1)");
                return new Complexity("1");
            }
         
            //Make it log base the power thing
            string equation = $"{initial}";
            if (iteration.Contains("*") )
            {
                equation += "*2^k";
            }
            else if (iteration.Contains("/"))
            {

                equation += "/2^k";
            }
            else
            {
                equation += repeatedOperation[FindOperation(iteration)] + "k";
            }



            //First tokenizes the equation that is created
            //Then it converts the tokenized list into a postfixformat
            // then rearranges it
                return new Complexity($"{Rearrange(DjisktraShuntingYard(Tokenize(equation)))}");
        }

        public  Complexity getComplexity()
        {
            return this.Complexity;
        }




        /// <summary>
        /// Helper Functions
        /// </summary>
     

        public string findHigherOperation(string x, string y)
        {
            string opX = FindOperation(x);
            string opY = FindOperation(y);
            int xFirst = 0;
            int yFirst = 0;
            for (int i = 0; i < operationDominance.Count; i++)
            {
                if (operationDominance[i].Contains(opX))
                {
                    xFirst = i;
                }
                if (operationDominance[i].Contains(opY))
                {
                    yFirst = i;
                }
            }
            return xFirst > yFirst ? x : y;
        }
        private List<string> Tokenize(string list)
        {
            string pattern = @"[\+\-\^\/\*]";

            Regex regex = new Regex(pattern);
            List<string> tokenList = new List<string>();
            string token = "";
            for(int i = 0; i < list.Length; i++)
            {
                if(regex.IsMatch(list[i].ToString()))
                {
                    tokenList.Add(token);   
                    tokenList.Add(list[i].ToString());
                    token = "";
                }
                else
                {
                    token+=list[i].ToString();
                }
                
            
            }
            //adds the last token 
            tokenList.Add(token);

            return tokenList;
        }

        /// <summary>
        /// Rearranges the given postfix expression to get the expression in terms of n
        /// </summary>
        /// <param name="tokenList"></param>
        /// <returns></returns>
        private string Rearrange(List<string> tokenList)
        {
            string equation = @"n";
            while(tokenList.Count > 0)
            {
                if (tokenList[tokenList.Count - 1] == "^")
                    equation = "log(" + equation + ")";

                else if (tokenList.Count == 1)
                {
                    //equation += "=k";
                    break;
                }
                else
                {
                    //if(oppositeOperation[
                    //    tokenList[tokenList.Count - 1]] != "/" && oppositeOperation[
                    //    tokenList[tokenList.Count - 1]] != "*")
                    equation = equation +
                        oppositeOperation[
                        tokenList[tokenList.Count - 1]] + tokenList[0];

                }
                tokenList.RemoveAt(0);
                tokenList.RemoveAt((tokenList.Count - 1));
            }

            //Removing the / or * 
            if (equation.Contains("/"))
                equation = equation.Remove(equation.IndexOf("/"));

            else if (equation.Contains("*"))
                equation = equation.Remove(equation.IndexOf("*"));

            else if (equation.Contains("log"))
                equation += ")"; //add the remaining bracket that was removed



            return equation;    
        }


        /// <summary>
        /// Converts the given tokenList from infix to postfix 
        /// </summary>
        /// <param name="tokenList"></param>
        /// <returns></returns>
        private List<string> DjisktraShuntingYard(List<string> tokenList)
        {

            Regex reg = new Regex("^[a-zA-Z]+$");
            Stack<string> operatorStack = new Stack<string>();
            Queue<string> outputQueue = new Queue<string>();
            int i =0;
            while( i<tokenList.Count)
            {
                if (int.TryParse(tokenList[i], out int x))
                {
                    outputQueue.Enqueue(tokenList[i]);
                    
                }


                //Check if its a variable
                //Using regex
               else if(reg.IsMatch
                    (tokenList[i]))                
                {
                    outputQueue.Enqueue((string)tokenList[i]);
                }
                if (FindOperation(tokenList[i]) != "")
                {
                    while (operatorStack.Count>0
                        && operatorStack.Peek()==findHigherOperation(operatorStack.Peek(),tokenList[i]))
                        // operators on top of the stack with greater precedence
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    //Adding the actual operator onto the OPERATOR STACK
                    operatorStack.Push(tokenList[i]);   
                }
                i++;
            }
            
            //while there are operators on the stack pop them out onto the output queue
            while(operatorStack.Count>0)
                outputQueue.Enqueue(operatorStack.Pop());

            return outputQueue.ToList();

        }

        /// <summary>
        /// Finds if there is an operation
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public string FindOperation(string op)
        {
            // Regular expression pattern to match the specified mathematical operators.
            string pattern = @"[\+\-\^\/\*]";

            Regex regex = new Regex(pattern);
            Match match = regex.Match(op);
            return match.Success ? match.Value : "";
        }


    }
}
