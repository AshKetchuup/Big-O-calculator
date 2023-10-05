using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ForLoopBigO
{
     class LoopAnalyzer: CSharpSyntaxWalker
    {

   

   
        public void AnalyzeCode(string code)
        {
            

            // Parse the code into a syntax tree
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();

            // Analyze the syntax tree
            Visit(root);

           
        }


        public override void VisitForStatement(ForStatementSyntax node)
        {
           

            // Extract the initialized statement, condition, and iterator
            VariableDeclarationSyntax initialization = node.Declaration; 
            ExpressionSyntax condition = node.Condition;
            SeparatedSyntaxList<ExpressionSyntax> iterator = node.Incrementors;

            // Output the extracted components
            Console.WriteLine("For Loop:");
            Console.WriteLine("  Initialization: " + initialization);
            Console.WriteLine("  Condition: " + condition);
            Console.WriteLine("  Iterator: " + string.Join(", ", iterator));
            Console.WriteLine();

            var loopVariable = initialization.Variables.First();

            foreach (StatementSyntax statement in node.Statement.DescendantNodes().OfType<StatementSyntax>())
            {
                if (statement is ExpressionStatementSyntax expressionStatement &&
                expressionStatement.Expression is AssignmentExpressionSyntax assignmentExpression &&
                assignmentExpression.Left is IdentifierNameSyntax identifierName &&
                identifierName.Identifier.ValueText == loopVariable.Identifier.ValueText)
                {
                    Console.WriteLine("    Modification: " + assignmentExpression.OperatorToken);
                }
            }




            base.VisitForStatement(node);

        }




        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            base.VisitWhileStatement(node);
        }

    }
}
