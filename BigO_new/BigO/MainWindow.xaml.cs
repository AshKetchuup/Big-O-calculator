using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
namespace BigO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           string code = CodeInput.Text.ToString();

            if(CheckCodeValidity(code))
            {
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
                CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();
                LoopAnalyzer analyzer = new LoopAnalyzer();
                analyzer.AnalyzeCode(code);
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
                if (diagnostic.Severity == DiagnosticSeverity.Error)
                {
                   mistakes += diagnostic.GetMessage().ToString()+ Environment.NewLine;
                    
                    
                }
            }

            if(mistakes.Length> 0) 
            {
                MessageBox.Show(mistakes);
                return false;
            }
            else
            {
                return true;
            }
        }


        private void GenerateGraph(Complexity complexity)
        {

        }


        public void DisplayComplexity(string str)
        {
            complexity.Text = str;

        }

        private void TimeComplexity_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
