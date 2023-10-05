using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace BigO
{
    class LoopAnalyzer : CSharpSyntaxWalker
    {
        //https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp?view=roslyn-dotnet-4.6.0
        CompilationUnitSyntax root;
        public void AnalyzeCode(string code)
        {


            // Parse the code into a syntax tree
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
             root = syntaxTree.GetCompilationUnitRoot();

            // Analyze the syntax tree
            Visit(root);


        }


        public override void VisitForStatement(ForStatementSyntax node)
        {


            // Extract the initialized statement, condition, and iterator
            VariableDeclarationSyntax initialization = node.Declaration;
            ExpressionSyntax condition = node.Condition;
            SeparatedSyntaxList<ExpressionSyntax> iterator = node.Incrementors;
            string modification = "";


            var loopVariable = initialization.Variables.First();

            foreach (StatementSyntax statement in node.Statement.DescendantNodes().OfType<StatementSyntax>())
            {
                if (statement is ExpressionStatementSyntax expressionStatement &&
                expressionStatement.Expression is AssignmentExpressionSyntax assignmentExpression &&
                assignmentExpression.Left is IdentifierNameSyntax identifierName &&
                identifierName.Identifier.ValueText == loopVariable.Identifier.ValueText)
                {
                    modification
                         = assignmentExpression.ToString();
                }
            }


            Expression expression = new Expression(initialization.ToString(), condition.ToString(), iterator.ToString(), modification);



            base.VisitForStatement(node);

        }

        /// <summary>
        ///  take into account propagation of timecomplexity 
        /// </summary>
        public void DepthFirstSearch(CSharpSyntaxNode node)
        {
            



        }

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {

            // Extract the initialized statement, condition, and iterator
            string initialization = "";
            ExpressionSyntax condition = node.Condition;
            IdentifierNameSyntax loopVariable = null;
            string modification = "";

            if (node.Condition is BinaryExpressionSyntax binaryCondition)
            {
                // Check if the left side of the binary condition is an identifier
                if (binaryCondition.Left is IdentifierNameSyntax leftIdentifier)
                {
                    loopVariable = leftIdentifier;
                }
                // Check if the right side of the binary condition is an identifier
                else if (binaryCondition.Right is IdentifierNameSyntax rightIdentifier)
                {
                    loopVariable = rightIdentifier;
                }
            }





            //Traverses the node's previous nodes and finds whether there is an initalization in there
            //gave up finding the initialization
          









            //Same code as before but finds the first modification 
            foreach (StatementSyntax statement in node.Statement.DescendantNodes().OfType<StatementSyntax>())
                {
                    if (statement is ExpressionStatementSyntax expressionStatement &&
                    expressionStatement.Expression is AssignmentExpressionSyntax assignmentExpression &&
                    assignmentExpression.Left is IdentifierNameSyntax identifierName &&
                    identifierName.Identifier.ValueText == loopVariable.Identifier.ValueText)
                    {
                        modification
                             = assignmentExpression.ToString();
                        break;// get outta the loop once its found
                    }
                }

            
          


            Expression expression = new Expression(initialization.ToString(), condition.ToString(), modification.ToString());
            base.VisitWhileStatement(node);
        }

       
    }
}
