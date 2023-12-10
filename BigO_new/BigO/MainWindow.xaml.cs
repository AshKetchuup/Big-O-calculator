using System;
using System.Windows;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace BigO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string Description { get { return DescriptionText.Text; } set { DescriptionText.Text = value; } }
        private string Complexity { get { return timeComplexity.Text; } set { timeComplexity.Text = value; } }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           string code = CodeInput.Text.ToString();


            if(CheckCodeValidity(code) && code != "")
            {
                
                LoopAnalyzer analyzer = new LoopAnalyzer();
                // returns the description of the function 
                List<string> list = analyzer.AnalyzeCode(code);
                Description = list[0];
                Complexity = list[1];   
                
            }
            if( code == "")
            {
                Description = "You have not entered any code. Please enter code."; 
            }

        }

        private bool CheckCodeValidity(string code)
        {   
            //Converting the code into syntaxtree
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

            // It gets the syntax tree's diagnostics
            var diagnostics = syntaxTree.GetDiagnostics();

            // Check if there are any syntax errors
            string mistakes = "";
            foreach (var diagnostic in diagnostics)
            {
                if (diagnostic.Severity == DiagnosticSeverity.Error) //adds it to the mistakes string  
                   mistakes += diagnostic.GetMessage().ToString()+ Environment.NewLine;
            }

            if(mistakes.Length> 0) 
            {
                Description = mistakes;
                Complexity = "";
                return false;
            }
            else
            {
                return true;
            }
        }

        private void DescriptionText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void TimeComplexity_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
