using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace BigO
{


     class LoopAnalyzer 
    {
        //https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp?view=roslyn-dotnet-4.6.0
        CompilationUnitSyntax root;
        SyntaxTree syntaxTree;
        public void AnalyzeCode(string code)
        {
            

            // Convert it into a syntax tree
            syntaxTree = CSharpSyntaxTree.ParseText(code);
            //Then get the compilation root (or the root node) 
            root = syntaxTree.GetCompilationUnitRoot();

            //Before embarking on building my own traveral algorithm I used the inbuilt Visit(); function  

            PostOrderTraversal(root).Display();
           

        }

        public Complexity PostOrderTraversal(CSharpSyntaxNode node)
        {
            if (node == null)
                return new Complexity("");


            List<Complexity> list = new List<Complexity>();
            if (node.ChildNodes().Any())
            {
                foreach (CSharpSyntaxNode child in node.ChildNodes())
                {
                    list.Add(PostOrderTraversal(child));
                }
            }


            // if something is returned
            if (returnComp(node).getComplexity() != "")
            {
                if (returnComp(node).AddComplexities(list).getComplexity() != "")
                    return new Complexity(returnComp(node).AddComplexities(list).getComplexity() + "*" + returnComp(node).getComplexity());

                else
                    return new Complexity(returnComp(node).getComplexity());

            }

            //if nothing is returned at all
            if (returnComp(node).getComplexity() == "")
            {
                if (returnComp(node).AddComplexities(list).getComplexity() != "")
                    return new Complexity(returnComp(node).AddComplexities(list).getComplexity());

                else
                    return new Complexity("");
            }

            else
            {
                return new Complexity("1");
            }


        }
  

       
        
        private Complexity returnComp(CSharpSyntaxNode node)
        {  
            switch (node)
            {
                case WhileStatementSyntax whileLoop:
                    return (VisitWhileStatement(whileLoop).getComplexity());
                  

                case ForStatementSyntax forLoop:
              
                   
                    return (VisitForStatement(forLoop).getComplexity());


                case ForEachStatementSyntax foreachLoop:
                return(new Complexity("n"));
            

                // Handle other node types as needed
                default:
                    return(new Complexity(""));
                 
            }

        }
      public new Expression VisitForStatement(ForStatementSyntax node)
        {
            // detect i++ !

            // Extract the initialized statement, condition, and iterator
            string initialization = "";
            ExpressionSyntax condition = node.Condition;
            SeparatedSyntaxList<ExpressionSyntax> iterator = node.Incrementors;
            string modification = "";
            IdentifierNameSyntax loopVariable = null;


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
            // Traverse the previous nodes to find the initialization
            // Extract the loop variable name from the condition


            foreach (var node2 in root.DescendantNodes())
            {
                if (node2 is LocalDeclarationStatementSyntax localDeclaration)
                {
                    foreach (var variable in localDeclaration.Declaration.Variables)
                    {
                        // Check if the variable is explicitly initialized and matches the loopVariable's identifier
                        if (variable.Initializer != null && variable.Identifier.Text == loopVariable.Identifier.Text)
                        {

                            string localDec = localDeclaration.ToString().Replace(";", "");
                            initialization = localDec.Substring(localDec.IndexOf("=") + 1);

                            break; // Exit the loop
                        }
                    }
                }
            }

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
            

            return new Expression(initialization, condition.ToString(), iterator.ToString(), modification);



        }

        public new Expression VisitWhileStatement(WhileStatementSyntax node)
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
            // Traverse the previous nodes to find the initialization
            // Extract the loop variable name from the condition

            foreach (var node2 in root.DescendantNodes())
            {
                if (node2 is LocalDeclarationStatementSyntax localDeclaration)
                {
                    foreach (var variable in localDeclaration.Declaration.Variables)
                    {
                        // Check if the variable is explicitly initialized and matches the loopVariable's identifier
                        if (variable.Initializer != null && variable.Identifier.Text == loopVariable.Identifier.Text)
                        {

                            string localDec = localDeclaration.ToString().Replace(";","");  
                            initialization = localDec.Substring(localDec.IndexOf("=")+1);
                        
                            break; // Exit the loop
                        }
                    }
                }
            }







            //Same code as before but finds the first modification 
            foreach (StatementSyntax statement in node.Statement.DescendantNodes().OfType<StatementSyntax>())
                {
                   
                if (statement is ExpressionStatementSyntax expressionStatement &&
    expressionStatement.Expression is PostfixUnaryExpressionSyntax postfixIncrement &&
    postfixIncrement.Operand is IdentifierNameSyntax identifierName &&
    identifierName.Identifier.ValueText == loopVariable.Identifier.ValueText)
                {
                    // You've found a postfix increment of the loop variable
                    modification = postfixIncrement.ToString();
                    break; // Exit the loop once it's found
                }

                if (statement is ExpressionStatementSyntax expressionStatement2 &&
                expressionStatement2.Expression is AssignmentExpressionSyntax assignmentExpression &&
                assignmentExpression.Left is IdentifierNameSyntax identifierName2 &&
                identifierName2.Identifier.ValueText == loopVariable.Identifier.ValueText)
                {
                    modification
                         = assignmentExpression.ToString();
                    break;
                }
            }

            
          

           
            Expression expression = new Expression(initialization.ToString(), condition.ToString(), modification.ToString());
            return expression;
            //base.VisitWhileStatement(node);
        }

       
    }
}
