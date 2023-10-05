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
        for (int i = 0; i < 10; i++)
        {
i/=2;            
        }
int z = 0;
while(z < n)
{
z+= 3
}

}
";



        Dictionary<string, string> parameters = new Dictionary<string, string> {  };

        Dictionary<string, string> values = new Dictionary<string, string>
        { };



        private List<string> dominanceHierarchy = new List<string>();

        private List<string> operationDominance = new List<string>
        {
        "-+", "*/", "^", "log"

        };



        //Constructor for loops 
        public Expression(string initial, string condition, string iteration, string modification = "")
        {

            
            if (condition.Contains(">"))
            {
                condition = condition.Substring(condition.IndexOf(">") + 1);
                
                // Check if the loop will actually decrement

            }
            if (condition.Contains("<"))
            {
                condition = condition.Substring(condition.IndexOf("<") + 1);
                // Check if the loop will actually increment

            }
            if (condition.Contains("="))
            {
                condition = condition.Substring(condition.IndexOf("=") + 1);

            }

            if (modification != "")
            {
                iteration = findHigherOperation(modification, iteration);
            }

            //Objective Expression 1.2
            //Check if the /= or *= iterators are not used when the intial is 0 
            if(initial.Substring(initial.IndexOf("="))=="0")
            {

            }

            MessageBox.Show(initial);
            MessageBox.Show(condition);
            MessageBox.Show(iteration);
        }


        private Complexity LoopToEquation(string initial, string condition, string iteration)
        {
            
            //Objective Expression 1.3
            //Check if the condition is constant 
            //where the initial and the final are numbers
            if (int.TryParse(condition, out int result) && int.TryParse(initial.Substring(initial.IndexOf("=") + 1), out int innit))
                return new Complexity("1");

            string equation;

            return new Complexity($"{Rearrange(DjisktraShuntingYard(Tokenize(equation)))}");
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
            return regex.Split(list).ToList<string>();
        }

        private string Rearrange(List<string> tokenList)
        {
            string equation = @"n";
            while(tokenList.Count > 0)
            {
                if (tokenList[tokenList.Count] == "^")               
                    equation = "log(" + equation + ")";
                    
                
                else
                {
                    equation = equation + tokenList[tokenList.Count - 1]+ tokenList[0];
                    
                }
                tokenList.RemoveAt(0);
                tokenList.RemoveAt((tokenList.Count - 1));
            }

            return equation;    
        }

        private List<string> DjisktraShuntingYard(List<string> tokenList)
        {
            Stack<string> operatorStack = new Stack<string>();
            Queue<string> outputQueue = new Queue<string>();
            int i =0;
            while( i<tokenList.Count)
            {
                if (int.TryParse(tokenList[i], out int x))
                {
                    outputQueue.Enqueue(tokenList[0]);
                    
                }

                if (FindOperation(tokenList[i]) != "")
                {
                    while (operatorStack.Peek()==findHigherOperation(operatorStack.Peek(),tokenList[i]))
                        // operators on top of the stack with greater precedence
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }
                }
                i++;
            }
            return outputQueue.ToList();

        }

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
