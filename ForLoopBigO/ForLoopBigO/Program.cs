// See https://aka.ms/new-console-template for more information
using Microsoft.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ForLoopBigO;


// Parse the code into a syntax tree


string code = @"
    void MyMethod()
    {
        for (int i = 0; i < 10; i++)
        {
i/=2;

for (int i = 0; i < 10; i++)
        {
i/=2;
           
        }            
        }
    }
";

SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();

LoopAnalyzer analyzer = new LoopAnalyzer();
analyzer.Visit(root);

