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
    class Complexity
    {
         string complexity { get; set; }

        public Complexity(string complexity, string timetype = "")
        {
            this.complexity = complexity;
        }

        public string getComplexity()
        {
            return this.complexity;
        }
  

        public Complexity AddComplexities(List<Complexity> complexity)
        {
            string complex = "";


            for (int i = 0; i < complexity.Count; i++)
            {

                if (complexity[i].complexity != "" )
                {
                   complex += complexity[i].complexity;
                    if (i != complexity.Count - 1 && complexity[i + 1].complexity != "")
                        complex += "+";
                }
              
            }

           if(complex != "" && complex.Contains("+"))
                return new Complexity($"({complex})");
            else
                return new Complexity($"{complex}");

        }


        private Complexity ReturnHigherOrderComplexity(List<Complexity> complexities)
        {


            // if the same type
            if (complexities.Distinct().Skip(1).Any())
                return complexities[0];

            else
            {

                return null;
            }

        }

        public void Display()
        {
            MessageBox.Show($"O({complexity})");
        }

        private Complexity MultiplyComplexity()
        {
            return null;
        }

    }
}
