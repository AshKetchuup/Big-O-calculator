using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;


namespace BigO
{
    class Expression
    {
        string code = @"
for (int i = 1; i < n; i++)
        { 
for (int i = 1; i < n; i++)
        { }

for (int i = 1; i < n; i++)
        { for (int i = 1; i < n; i++)
        { }}
}


for (int i = 1; i < n; i++)
        { 
for (int i = 1; i < n; i*=2)
        { }

for (int i = 1; i < n; i*=2)
        { for (int i = 1; i < n; i++)
        { }}
}


for (int i = 1; i < n; i*=2)
        { 
for (int i = 1; i < n; i*=2)
        { }

for (int i = 1; i < n; i*=2)
        { for (int i = 1; i < n; i*=2)
        { }}
}
";
        internal Complexity theActualComplexity { get; set; }

       private Dictionary<string, string> oppositeOperation = new Dictionary<string, string>
        {
            { "+", "-" },

            { "-", "+" },
            { "*", "/" },
            { "/", "*" },
            { "^", "log" } // You can add other operators as needed
        };
       private Dictionary<string, string> repeatedOperation = new Dictionary<string, string>
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
        internal Expression(string initial, string condition, string iteration, string modification = "")
        {


            try
            {

                //Objective Expression.2
                //Check if the /= or *= iterators are not used when the intial is 0 


                if (modification != "")
                {
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


                    //Objective Expression 3
                    //Checks them to ensure that the modification is not the opposite of the iteration
                    if (oppositeOperation[FindOperation(iteration)] == FindOperation(modification) || oppositeOperation[FindOperation(modification)] == FindOperation(iteration))
                    {
                        throw new Exception("Your operations are the opposites of each other. Get better");
                    }

                }



                // finds the more dominant operation between the modification and the iteration
                iteration = (modification != "") ? findHigherOperation(modification, iteration) : FindOperation(iteration);

                // Checks whether the loop conditions are correct and returns a bool
                if (CheckLoopConditions(ref initial, ref condition, iteration))
                {

                    theActualComplexity = LoopToEquation(initial, condition, iteration);

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


        //Constructor strictly for propagating complexities
        internal Expression(string complexity)
        {
            theActualComplexity = new Complexity(EvaluatePostFix(DijsktraShuntingYard(Tokenize(complexity))));

        }
        internal bool CheckLoopConditions(ref string initial, ref string condition, string iteration)
        {


            //Objective Expression.1 Checking the loop conditions so that they are right

            bool check = false;
            if (condition.Contains(">"))
            {

                // checks the condition and extracts the final condition
                condition = condition.Substring(condition.IndexOf(">") + 1);
                if (condition.Contains("="))
                {
                    condition = condition.Substring(condition.IndexOf("=") + 1);

                }

                //Checking the int variables to see if they would actually run
                /*
                 for (int x = 9 ;x>10 ;x-- ){} this would give an infinite loop but without an exception returns O(n)
                  for (int x = 10 ;x>9 ;x*=2 ){}
                 */
                if (int.TryParse(condition, out int result) && int.TryParse(initial.Substring(initial.IndexOf("=") + 1), out int start))
                {
                    if (iteration == "-" || iteration == "/" && start <= result)
                    {

                        return false;
                    }
                    if (iteration == "*" || iteration == "+" && start >= result)
                    {

                        return false;
                    }
                }



                // Check if the loop will actually decrement
                // we have to swap the initial and condition if it is decrementing
                // And replace the iteration with the opposite operation because going the opposite way
                if (!(FindOperation(iteration) == "*" || FindOperation(iteration) == "+"))
                {
                    string temp = initial;
                    initial = condition;
                    condition = temp;
                    iteration = oppositeOperation[FindOperation(iteration)];
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


                //Checking the int variables to see if they would actually run
                /*
                 for (int x = 10 ;x>9 ;x++ ){} this would give an infinite loop but without an exception returns O(n)
                 */
                if (int.TryParse(condition, out int result) && int.TryParse(initial.Substring(initial.IndexOf("=") + 1), out int start))
                {
                    if (iteration == "-" || iteration == "/" && start <= result)
                        return false;
                    
                    if (iteration == "*" || iteration == "+" && start >= result)
                        return false;
                    
                }

                // Check if the loop will actually increment
                if ((FindOperation(iteration) == "*" || FindOperation(iteration) == "+"))
                {
                    check = true;
                }
            }

            return check;
        }
        internal Complexity LoopToEquation(string initial, string condition, string iteration)
        {

            //Objective Expression.4
            //Check if the condition is constant 
            //where the initial and the final are numbers
            if (int.TryParse(condition, out int result) && int.TryParse(initial.Substring(initial.IndexOf("=") + 1), out int start))
                return new Complexity("1");


            //Make it log base the power thing
            string equation = $"{initial}";
            if (iteration.Contains("*"))
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
            return new Complexity($"{Rearrange(DijsktraShuntingYard(Tokenize(equation)))}");
        }

        /// <summary>
        /// Helper Functions
        /// </summary>


        internal string findHigherOperation(string x, string y)
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


        //implement checking for log as a tokenzer in this 
        internal static List<string> Tokenize(string list)
        {
            string pattern = @"[\+\-\^\/\*]";

            Regex regex = new Regex(pattern);
            List<string> tokenList = new List<string>();
            string token = "";
            for (int i = 0; i < list.Length; i++)
            {


                // adds it if it's an operator
                if (regex.IsMatch(list[i].ToString()))
                {
                    if(token != "")  tokenList.Add(token);
                    tokenList.Add(list[i].ToString());
                    token = "";
                }

                else if (list[i] == '(' || list[i] == ')')
                {
                    if(token != "")
                    {
                        tokenList.Add(token.ToString());
                        token = "";
                    }
                    tokenList.Add(list[i].ToString());
                }
                else
                {
                    token += list[i].ToString();
                }


            }
            //adds the last token 
           if( token != "") tokenList.Add(token);
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
            while (tokenList.Count > 0)
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

            if (equation.Contains("*"))
                equation = equation.Remove(equation.IndexOf("*"));

            if (equation.Contains("log"))
                equation += ")"; //add the remaining bracket that was removed



            return equation;
        }


        /// <summary>
        /// Converts the given tokenList from infix to postfix 
        /// </summary>
        /// <param name="tokenList"></param>
        /// <returns></returns>
        private List<string> DijsktraShuntingYard(List<string> tokenList)
        {

            Regex reg = new Regex("^[a-zA-Z]+$");

            string pattern = @"[\+\-\^\/\*]";

            Regex regex = new Regex(pattern);
            Stack<string> operatorStack = new Stack<string>();
            Queue<string> outputQueue = new Queue<string>();
            int i = 0;
            while (i < tokenList.Count) // while there are are tokens to be read
            {
                if (int.TryParse(tokenList[i], out int x))
                {
                    outputQueue.Enqueue(tokenList[i]);
                }


                //Check if its a variable
                //Using regex
                else if (reg.IsMatch
                     (tokenList[i]))
                {
                    outputQueue.Enqueue((string)tokenList[i]);
                }


                // check if there's a log
                else if(tokenList[i].Contains("log"))
                {
                    outputQueue.Enqueue((string)tokenList[i]);
                }

                // if there is an operator
                if (FindOperation(tokenList[i]) != "")
                {
                    while (operatorStack.Count > 0
                        && operatorStack.Peek() == findHigherOperation(operatorStack.Peek(), tokenList[i]) && operatorStack.Peek() != "(")
                    // operators on top of the stack with greater precedence
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    //Adding the actual operator onto the OPERATOR STACK
                    operatorStack.Push(tokenList[i]);
                }

                // if it's a left bracket push onto stack
                if(tokenList[i] == "(")
                    operatorStack.Push(tokenList[i]);   

                if(tokenList[i] == ")")
                {
                    while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }
                    // POP THE leftbracket
                    operatorStack.Pop();

                }


                i++;
            }

            //while there are operators on the stack pop them out onto the output queue
            while (operatorStack.Count > 0)
                outputQueue.Enqueue(operatorStack.Pop());

            return outputQueue.ToList();

        }

        /// <summary>
        /// Finds if there is an operation
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        private string FindOperation(string op)
        {
            // Regular expression pattern to match the specified mathematical operators.
            string pattern = @"[\+\-\^\/\*]";

            Regex regex = new Regex(pattern);
            Match match = regex.Match(op);
            return match.Success ? match.Value : "";
        }

        //Functions for additional design.


        /// <summary>
        /// Evaluates a postfix function
        /// </summary>
        /// <param name="tokenlist"></param>
        /// <returns></returns>
        private string EvaluatePostFix(List<string> tokenlist)
        {
            Regex reg = new Regex("^[a-zA-Z]+$");
            Stack<string> stack = new Stack<string>();
           
           // needs to be repeated for each log 
          
            while(tokenlist.Contains("log"))
            {
                int index = tokenlist.IndexOf("log");
                tokenlist[index]= "log("+tokenlist[index+1]+")";
                tokenlist.RemoveAt(index+1);
            }



            foreach (string token in tokenlist)
            {
                //if its an operation
                if (FindOperation(token) != "")
                {
                    Complexity one = new Complexity(stack.Pop());

                    // stack empty exception invalid operation 
                    Complexity two = new Complexity(stack.Pop());

                    if (FindOperation(token) == "*")
                        stack.Push(Multiply(one, two));
                    if (FindOperation(token) == "+")
                        stack.Push(Add(one, two));

                    
                }

                else
                {
                    stack.Push(token);
                }
            }

            return stack.Pop();
        }

        // multiply the complexities and combine the things
        private string Multiply(Complexity a, Complexity b)
        {
            if (a.time == b.time)
            {

                if (a.time == TimeType.Poly // if they are polynomial time complexity and if they both contain the same letter variable they depend upon

                    && a.complexity.Contains("n") && b.complexity.Contains("n")) // check if they have the same variable 

                {
                    int powerA = a.complexity.Contains("^") ? int.Parse(a.complexity.Substring(a.complexity.IndexOf("^") + 1)) : 1;
                    int powerB = b.complexity.Contains("^") ? int.Parse(b.complexity.Substring(b.complexity.IndexOf("^") + 1)) : 1;

                    return $"n^{powerA + powerB}";
                }

                //for logarthmic time compleixty
                if(a.time == TimeType.Log)
                {                    
                    int powerA = a.complexity.Contains("^") ? int.Parse(a.complexity.Split(new char[] { '^' })[1]) : 1; // this gives the power of A
                    int powerB = b.complexity.Contains("^") ? int.Parse(b.complexity.Split(new char[] { '^' })?[1]) : 1;
                    return $"log(n)^{powerB + powerA}";
                }
            }

            else
            {

                if(a.time == TimeType.LinearArithmetic && b.time==TimeType.Poly || b.time == TimeType.LinearArithmetic && a.time == TimeType.Poly)
                {
                    int powerA = a.complexity.Contains("^") ? int.Parse(a.complexity.Substring(a.complexity.IndexOf("^") + 1)) : 1;
                    int powerB = b.complexity.Contains("^") ? int.Parse(b.complexity.Substring(b.complexity.IndexOf("^") + 1)) : 1;
                    return $"n^{powerA + powerB}log(n)";
                }
           
                if (a.time == TimeType.Poly && b.time == TimeType.Log || b.time == TimeType.Poly && a.time == TimeType.Log)
                    return b.time == TimeType.Poly ? b.complexity + a.complexity: a.complexity+b.complexity ;

                if (a.time == TimeType.LinearArithmetic && b.time == TimeType.Log )
                {
                    int powerA = a.complexity.Contains("^") ? int.Parse(a.complexity.Substring(a.complexity.IndexOf("^") + 1)) : 1;
                    int powerB = b.complexity.Contains("^") ? int.Parse(b.complexity.Split(new char[] { '^' })[1]) : 1;
                    return $"n^{powerA}log(n)^{powerB + 1}";
                }
                if (a.time == TimeType.Log && b.time == TimeType.LinearArithmetic)
                {
                    int powerA = a.complexity.Contains("^") ? int.Parse(a.complexity.Substring(a.complexity.IndexOf("^") + 1)) : 1;
                    int powerB = b.complexity.Contains("^") ? int.Parse(b.complexity.Split(new char[] { '^' })[1]) : 1;
                    return $"n^{powerA}log(n)^{powerB + 1}";
                }
            }

            return "";
        }


        /// <summary>
        /// Finds the higher time complexity for a given function since Big O finds the most dominant term 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private string Add(Complexity a, Complexity b)
        {
            if (a.time == b.time)
            {
                if (a.time == TimeType.Poly // if they are polynomial time complexity and if they both contain the same letter variable they depend upon
                      && a.complexity.Contains("n") && b.complexity.Contains("n")) // check if they have the same variable 
                {
                    int powerA = a.complexity.Contains("^") ? int.Parse(a.complexity.Substring(a.complexity.IndexOf("^") + 1)) : 1;
                    int powerB = b.complexity.Contains("^") ? int.Parse(b.complexity.Substring(b.complexity.IndexOf("^") + 1)) : 1;
                    return powerA > powerB ? a.complexity : b.complexity;
                }

                if (a.time == TimeType.Log)
                {                                    
                    int powerA = a.complexity.Contains("^") ? int.Parse(a.complexity.Split(new char[] { '^' })[1]) : 1; // this gives the power of A
                    int powerB = b.complexity.Contains("^") ? int.Parse(b.complexity.Split(new char[] { '^' })[1]) : 1;
                    return powerA > powerB ? a.complexity : b.complexity;
                }


            }

            else
            {

                //for most large powers of n and powers of log(n), polynomial will always be greater 
                if (a.time == TimeType.Poly && b.time == TimeType.Log)
                    return a.complexity;

                 if (b.time == TimeType.Poly && a.time == TimeType.Log)
                    return b.complexity;
            
                 if(a.time == TimeType.LinearArithmetic && b.time == TimeType.Poly)
                {
                    int powerB = b.complexity.Contains("^") ? int.Parse(b.complexity.Substring(b.complexity.IndexOf("^") + 1)) : 1;               
                   string aString = Regex.Split(a.complexity, "^")[1]; // splits each part of the complexity string
                    aString = aString.Remove(aString.IndexOf("log")); // removes part where it contains log
                         int powerA = a.complexity.Contains("^") ? int.Parse(aString) : 1; // this gives the power of A
                    return powerA > powerB ? a.complexity : b.complexity;
                }
                if (b.time == TimeType.LinearArithmetic && a.time == TimeType.Poly)
                {
                    int powerA = a.complexity.Contains("^") ? int.Parse(a.complexity.Substring(a.complexity.IndexOf("^") + 1)) : 1;
                    string bString = Regex.Split(b.complexity, "^")[1];
                    bString = bString.Remove(bString.IndexOf("log"));
                    int powerB = b.complexity.Contains("^") ? int.Parse(bString) : 1;
                    return powerA > powerB ? a.complexity : b.complexity;
                }

                if (a.time == TimeType.LinearArithmetic && b.time == TimeType.Log)
                {
                    int powerA = a.complexity.Contains("^") ? int.Parse(a.complexity.Substring(a.complexity.IndexOf("^") + 1)) : 1;
                    string bString = Regex.Split(b.complexity, "^")[1];
                    bString = bString.Remove(bString.IndexOf("log"));
                    int powerB = b.complexity.Contains("^") ? int.Parse(bString) : 1;
                    return powerA > powerB || powerA == powerB ? a.complexity : b.complexity;

                }
                if (b.time == TimeType.LinearArithmetic &&  a.time == TimeType.Log)
                {
                    int powerB = b.complexity.Contains("^") ? int.Parse(b.complexity.Substring(b.complexity.IndexOf("^") + 1)) : 1;
                    string aString = Regex.Split(a.complexity, "^")[1]; // splits each part of the complexity string
                    aString = aString.Remove(aString.IndexOf("log")); // removes part where it contains log
                    int powerA = a.complexity.Contains("^") ? int.Parse(aString) : 1; // this gives the power of A

                    return powerA > powerB || powerA == powerB ? a.complexity : b.complexity;
                }

            }

            return "";


        }

    }
}
