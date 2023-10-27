using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace BigO
{
     class Complexity
    {
        private string complexity;
            
        public Complexity(string complexity,string timetype = "")
        {
            this.complexity = complexity;
        }


        private Complexity AddComplexities()
        {
            return null;
        }


        private Complexity ReturnHigherOrderComplexity()
        {


            // if the same type
            return null;

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
