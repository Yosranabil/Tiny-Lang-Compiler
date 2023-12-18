using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TINY_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();
        
        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public  Node root;
        
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }

        Node Program()
        {
            Node program = new Node("Program");

            program.Children.Add(Function_Statements());
            program.Children.Add(Main_Function());

            MessageBox.Show("Success");

            return program;
        }

        Node Main_Function()
        {
            Node main_function = new Node("MainFunction");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;

            // check the main function structure
            if (token_class_type == Token_Class.T_Integer || token_class_type == Token_Class.T_Float || token_class_type == Token_Class.T_String)
            {
                main_function.Children.Add(Datatype());
                main_function.Children.Add(match(Token_Class.T_Main));
                main_function.Children.Add(match(Token_Class.T_LParanthesis));
                main_function.Children.Add(match(Token_Class.T_RParanthesis));
                main_function.Children.Add(Function_Body());
            }
            
            return main_function;
        }

        Node Datatype()
        {
            Node datatype = new Node("Datatype");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;

            // check the datatype structure
            if (token_class_type == Token_Class.T_Integer)
            {
                datatype.Children.Add(match(Token_Class.T_Integer));
            }
            else if (token_class_type == Token_Class.T_Float)
            {
                datatype.Children.Add(match(Token_Class.T_Float));
            }
            else if (token_class_type == Token_Class.T_String)
            {
                datatype.Children.Add(match(Token_Class.T_String));
            }

            return datatype;
        }

        Node Function_Statements()
        {
            Node function_statements = new Node("FunctionStatements");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            // check the function statements sructure
            if (TokenStream[InputPointer].token_type == Token_Class.T_Integer || TokenStream[InputPointer].token_type == Token_Class.T_Float || TokenStream[InputPointer].token_type == Token_Class.T_String)
            {
                function_statements.Children.Add(Function_Statement());
                function_statements.Children.Add(Function_Statements_Dash());
            }

            return function_statements;
        }
        Node Function_Statement()
        {
            Node function_statement = new Node("FunctionStatement");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;
            Token_Class token_class_next_type = TokenStream[InputPointer + 1].token_type;

            // check the function statement sructure
            if ((token_class_type == Token_Class.T_Integer && token_class_next_type != Token_Class.T_Main) || (token_class_type == Token_Class.T_Float && token_class_next_type != Token_Class.T_Main) || (token_class_type == Token_Class.T_String && token_class_next_type != Token_Class.T_Main))
            {
                function_statement.Children.Add(Function_Decl());
                function_statement.Children.Add(Function_Body());

                return function_statement;
            }
            else
            {
                return null;
            }
        }
        Node Function_Statements_Dash()
        {
            Node function_statments_dash = new Node("FunctionStatementsDash");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;
            Token_Class token_class_next_type = TokenStream[InputPointer + 1].token_type;

            // check the function statements dash sructure
            if ((token_class_type == Token_Class.T_Integer && token_class_next_type != Token_Class.T_Main) || (token_class_type == Token_Class.T_Float && token_class_next_type != Token_Class.T_Main) || (token_class_type == Token_Class.T_String && token_class_next_type != Token_Class.T_Main))
            {
                function_statments_dash.Children.Add(Function_Statement());
                function_statments_dash.Children.Add(Function_Statements_Dash());

                return function_statments_dash;
            }

            return null;
        }

        Node Function_Decl()
        {
            Node function_decl = new Node("FunctionDecl");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            // check the function declaration structure
            if (TokenStream[InputPointer].token_type == Token_Class.T_Integer || TokenStream[InputPointer].token_type == Token_Class.T_Float || TokenStream[InputPointer].token_type == Token_Class.T_String)
            {
                function_decl.Children.Add(Datatype());
                function_decl.Children.Add(match(Token_Class.T_Identifier));
                function_decl.Children.Add(match(Token_Class.T_LParanthesis));
                function_decl.Children.Add(Parameters());
                function_decl.Children.Add(match(Token_Class.T_RParanthesis));
            }

            return function_decl;
        }

        Node Function_Body()
        {
            Node function_body = new Node("FunctionBody");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            // check the function body structure
            if (TokenStream[InputPointer].token_type == Token_Class.T_LCurlBracket)
            {
                function_body.Children.Add(match(Token_Class.T_LCurlBracket));
                function_body.Children.Add(Statements());
                function_body.Children.Add(Return_Statement());
                function_body.Children.Add(match(Token_Class.T_RCurlBracket));
            }

            return function_body;
        }

        Node Function_Call()
        {
            Node function_call = new Node("FunctionCall");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;

            // check the function call structure
            if(token_class_type == Token_Class.T_Identifier)
            {
                function_call.Children.Add(match(Token_Class.T_Identifier));
                function_call.Children.Add(match(Token_Class.T_LParanthesis));
                if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier)
                {
                    function_call.Children.Add(Identifier_List());
                }             
                function_call.Children.Add(match(Token_Class.T_RParanthesis));
            }

            return function_call;
        }

        Node Expression()
        {
            Node expression = new Node("Expression");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;
            Token_Class token_class_next_type = TokenStream[InputPointer + 1].token_type;

            // check the expression structure
            if (token_class_type == Token_Class.T_String_Literal)
            {
                expression.Children.Add(match(Token_Class.T_String_Literal));
            }
            else if(token_class_type == Token_Class.T_LParanthesis || token_class_type == Token_Class.T_Float_Literal || token_class_type == Token_Class.T_Number || (token_class_type == Token_Class.T_Identifier && token_class_next_type == Token_Class.T_LParanthesis) || token_class_type == Token_Class.T_Identifier)
            {
                expression.Children.Add(Equation());
            }
            else if(token_class_type == Token_Class.T_Number || token_class_type == Token_Class.T_Float_Literal || (token_class_type == Token_Class.T_Identifier && token_class_next_type == Token_Class.T_LParanthesis) || token_class_type == Token_Class.T_Identifier)
            {
                expression.Children.Add(Term());
            }

            return expression;
        }

        Node Term()
        {
            Node term = new Node("Term");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;
            Token_Class token_class_next_type = TokenStream[InputPointer + 1].token_type;

            // check the term structure
            if (token_class_type == Token_Class.T_Number)
            {
                term.Children.Add(match(Token_Class.T_Number));
            }
            else if(token_class_type == Token_Class.T_Float_Literal)
            {
                term.Children.Add(match(Token_Class.T_Float_Literal));
            }
            else if(token_class_type == Token_Class.T_Identifier && token_class_next_type == Token_Class.T_LParanthesis)
            {
                term.Children.Add(Function_Call());
            }
            else if(token_class_type == Token_Class.T_Identifier)
            {
                term.Children.Add(match(Token_Class.T_Identifier));
            }

            return term;
        }

        Node Identifier_List()
        {
            Node identifier_list = new Node("IdentifierList");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            // check the identifier list structure
            if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier)
            {
                identifier_list.Children.Add(Id());
                identifier_list.Children.Add(Identifier_List_Dash());
            }
            return identifier_list;
        }

        Node Identifier_List_Dash()
        {
            Node identifier_list_dash = new Node("IdentifierListDash");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            // check the identifier list dash structure
            if (TokenStream[InputPointer].token_type == Token_Class.T_Comma)
            {
                identifier_list_dash.Children.Add(match(Token_Class.T_Comma));
                identifier_list_dash.Children.Add(Id());
                identifier_list_dash.Children.Add(Identifier_List_Dash());

                return identifier_list_dash;
            }

            return null;
        }

        Node Id()
        {
            Node id = new Node("Id");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            // check the id structure
            if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier)
            {
                id.Children.Add(match(Token_Class.T_Identifier));
                id.Children.Add(Id_Dash());
            }
            return id;
        }

        Node Id_Dash()
        {
            Node id_dash = new Node("IdDash");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            // check the id dash structure
            if (TokenStream[InputPointer].token_type == Token_Class.T_AssignmentOp)
            {
                id_dash.Children.Add(match(Token_Class.T_AssignmentOp));
                id_dash.Children.Add(Expression());

                return id_dash;
            }

            return null;
        }

        Node Statement()
        {
            Node statement = new Node("Statement");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;
            Token_Class token_class_next_type = TokenStream[InputPointer + 1].token_type;

            // check the statement structure
            if (token_class_type == Token_Class.T_Write)
            {
                statement.Children.Add(Write_Statement());
            }
            else if (token_class_type == Token_Class.T_Read)
            {
                statement.Children.Add(Read_Statement());
            }
            else if (token_class_type == Token_Class.T_Identifier && token_class_next_type == Token_Class.T_AssignmentOp)
            {
                statement.Children.Add(Assignment_Statement());
            }
            else if (token_class_type == Token_Class.T_Integer || token_class_type == Token_Class.T_Float || token_class_type == Token_Class.T_String)
            {
                statement.Children.Add(Declaration_Statement());

            }
            else if (token_class_type == Token_Class.T_If)
            {
                statement.Children.Add(If_Statement());

            }
            else if (token_class_type == Token_Class.T_Repeat)
            {
                statement.Children.Add(Repeat_Statement());

            }
            else if (token_class_type == Token_Class.T_Identifier && token_class_next_type == Token_Class.T_LParanthesis)
            {
                statement.Children.Add(Function_Call());
            }

            return statement;
        }

        Node Assignment_Statement()
        {
            Node assignment_statement = new Node("AssignmentStatement");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;

            // check the assignment statement structure
            if (token_class_type == Token_Class.T_Identifier)
            {
                assignment_statement.Children.Add(match(Token_Class.T_Identifier));
                assignment_statement.Children.Add(match(Token_Class.T_AssignmentOp));
                assignment_statement.Children.Add(Expression());
                assignment_statement.Children.Add(match(Token_Class.T_Semicolon));
            }

            return assignment_statement;
        }

        Node Declaration_Statement()
        {
            Node declaration_statement = new Node("DeclarationStatement");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;

            // check the declaration statement structure
            if(token_class_type == Token_Class.T_Integer || token_class_type == Token_Class.T_Float || token_class_type == Token_Class.T_String)
            {
                declaration_statement.Children.Add(Datatype());
                declaration_statement.Children.Add(Identifier_List());
                declaration_statement.Children.Add(match(Token_Class.T_Semicolon));
            }

            return declaration_statement;
        }

        Node If_Statement()
        {
            Node if_statement = new Node("IfStatement");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;

            // check the if statement structure
            if (token_class_type == Token_Class.T_If)
            {
                if_statement.Children.Add(match(Token_Class.T_If));
                if_statement.Children.Add(Condition_Statement());
                if_statement.Children.Add(match(Token_Class.T_Then));
                if_statement.Children.Add(Statements());
                if_statement.Children.Add(Ret_Statement());
                if_statement.Children.Add(Else_If_Statement());
                if_statement.Children.Add(Else_Statement());
                if_statement.Children.Add(match(Token_Class.T_End));
            }

            return if_statement;
        }

        Node Else_If_Statement()
        {
            Node else_if_statement = new Node("ElseIfStatement");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_Class = TokenStream[InputPointer].token_type;

            // check the else if statement structure
            if (token_Class == Token_Class.T_ElseIf)
            {
                else_if_statement.Children.Add(match(Token_Class.T_ElseIf));
                else_if_statement.Children.Add(Condition_Statement());
                else_if_statement.Children.Add(match(Token_Class.T_Then));
                else_if_statement.Children.Add(Statements());
                else_if_statement.Children.Add(Ret_Statement());
                else_if_statement.Children.Add(Else_Statement());

                return else_if_statement;
            }
            else
            {
                return null;
            }
        }

        Node Else_Statement()
        {
            Node else_statement = new Node("ElseStatement");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_Class = TokenStream[InputPointer].token_type;

            // check the else statement structure
            if (token_Class == Token_Class.T_Else)
            {
                else_statement.Children.Add(match(Token_Class.T_Else));
                else_statement.Children.Add(Statements());

                return else_statement;
            }
            else
            {
                return null;
            }
        }

        Node Repeat_Statement()
        {
            Node repeat_statement = new Node("RepeatStatement");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;

            // check the repeat statement structure
            if (token_class_type == Token_Class.T_Repeat)
            {
                repeat_statement.Children.Add(match(Token_Class.T_Repeat));
                repeat_statement.Children.Add(Statements());
                repeat_statement.Children.Add(match(Token_Class.T_Until));
                repeat_statement.Children.Add(Condition_Statement());
            }

            return repeat_statement;
        }

        Node Statements()
        {
            Node statements = new Node("Statements");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;
            Token_Class token_class_next_type = TokenStream[InputPointer + 1].token_type;

            // check the statements structure
            if (token_class_type == Token_Class.T_Read || token_class_type == Token_Class.T_Write || token_class_type == Token_Class.T_Repeat ||
                token_class_next_type == Token_Class.T_AssignmentOp || 
                token_class_type == Token_Class.T_Integer || token_class_type == Token_Class.T_Float || token_class_type == Token_Class.T_String || 
                token_class_type == Token_Class.T_If || 
                (token_class_type == Token_Class.T_Identifier && token_class_next_type == Token_Class.T_LParanthesis))
            {
                statements.Children.Add(Statement());
                statements.Children.Add(Statements_Dash());
            }

            return statements;
        }

        Node Statements_Dash()
        {
            Node statements = new Node("StatementsDash");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;
            Token_Class token_class_next_type = TokenStream[InputPointer + 1].token_type;

            // check the statements dash structure
            if (token_class_type == Token_Class.T_Read || token_class_type == Token_Class.T_Write || token_class_type == Token_Class.T_Repeat ||
                token_class_next_type == Token_Class.T_AssignmentOp ||
                token_class_type == Token_Class.T_Integer || token_class_type == Token_Class.T_Float || token_class_type == Token_Class.T_String ||
                token_class_type == Token_Class.T_If ||
                (token_class_type == Token_Class.T_Identifier && token_class_next_type == Token_Class.T_LParanthesis))
            {
                statements.Children.Add(Statement());
                statements.Children.Add(Statements_Dash());

                return statements;
            }

            return null;
        }

        Node Write_Statement()
        {
            Node write_statement = new Node("WriteStatement");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;


            // check the write statement structure
            if (TokenStream[InputPointer].token_type == Token_Class.T_Write)
            {
                write_statement.Children.Add(match(Token_Class.T_Write));
                write_statement.Children.Add(Write_Statement_Dash());
            }

            return write_statement;
        }

        Node Write_Statement_Dash()
        {
            Node write_statement_dash = new Node("WriteStatementDash");
            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;
            Token_Class token_class_next_type = TokenStream[InputPointer + 1].token_type;

            // check the write statement dash structure
            if (token_class_type == Token_Class.T_String_Literal || 
                token_class_type == Token_Class.T_LParanthesis || 
                token_class_type == Token_Class.T_Number || token_class_type == Token_Class.T_Float_Literal ||
                (token_class_type == Token_Class.T_Identifier && token_class_next_type == Token_Class.T_LParanthesis) ||
                token_class_type == Token_Class.T_Identifier)
            {
                write_statement_dash.Children.Add(Expression());
                write_statement_dash.Children.Add(match(Token_Class.T_Semicolon));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_Endl)
            {
                write_statement_dash.Children.Add(match(Token_Class.T_Endl));
                write_statement_dash.Children.Add(match(Token_Class.T_Semicolon));
            }

            return write_statement_dash;
        }

        Node Read_Statement()
        {
            Node read_statement = new Node("ReadStatement");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;


            // check the read statement structure
            if (TokenStream[InputPointer].token_type == Token_Class.T_Read)
            {
                read_statement.Children.Add(match(Token_Class.T_Read));
                read_statement.Children.Add(match(Token_Class.T_Identifier));
                read_statement.Children.Add(match(Token_Class.T_Semicolon));
            }

            return read_statement;
        }

        Node Ret_Statement()
        {
            Node ret_statement = new Node("RetStatement");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;


            // check the ret statement structure
            if (TokenStream[InputPointer].token_type == Token_Class.T_Return)
            {
                ret_statement.Children.Add(Return_Statement());
                return ret_statement;
            }

            return null;  
        }

        Node Return_Statement()
        {
            Node return_statement = new Node("ReturnStatement");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;


            // check the return statement structure
            if (TokenStream[InputPointer].token_type == Token_Class.T_Return)
            {
                return_statement.Children.Add(match(Token_Class.T_Return));
                return_statement.Children.Add(Expression());
                return_statement.Children.Add(match(Token_Class.T_Semicolon));
            }

            return return_statement;
        }

        Node Condition_Statement()
        {
            Node condition_statement = new Node("ConditionStatement");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;


            // check the condition statement structure
            if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier)
            {
                condition_statement.Children.Add(Condition());
                condition_statement.Children.Add(Condition_Statement_Dash());
            }

            return condition_statement;
        }

        Node Condition_Statement_Dash()
        {
            Node condition_statement_dash = new Node("ConditionStatementDash");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;

            // check the condition statement dash  structure
            if (token_class_type == Token_Class.T_OrOp || token_class_type == Token_Class.T_AndOp)
            {
                condition_statement_dash.Children.Add(BooleanOps());
                condition_statement_dash.Children.Add(Condition());
                condition_statement_dash.Children.Add(Condition_Statement_Dash());

                return condition_statement_dash;
            }
            return null;

        }

        Node Condition()
        {
            Node condition = new Node("Condition");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            // check the condition  structure
            if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier)
            {
                condition.Children.Add(match(Token_Class.T_Identifier));
                condition.Children.Add(ConditionOps());
                condition.Children.Add(Term());
            }

            return condition;
        }

        Node Equation()
        {
            Node equation = new Node("Equation");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            // check the equation structure
            if (TokenStream[InputPointer].token_type == Token_Class.T_LParanthesis)
            {
                equation.Children.Add(match(Token_Class.T_LParanthesis));
                equation.Children.Add(Equation());

                equation.Children.Add(match(Token_Class.T_RParanthesis));
                equation.Children.Add(Equation_Dash());
            }
            else
            {
                equation.Children.Add(Term());
                equation.Children.Add(Equation_Dash());
            }

            return equation;
        }

        Node Equation_Dash()
        {
            Node equation_dash = new Node("EquationDash");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            // check the equation dash structure
            if (TokenStream[InputPointer].token_type == Token_Class.T_PlusOp || TokenStream[InputPointer].token_type == Token_Class.T_MinusOp || TokenStream[InputPointer].token_type == Token_Class.T_MultiplyOp || TokenStream[InputPointer].token_type == Token_Class.T_DivideOp)
            {
                equation_dash.Children.Add(ArithmeticOps());
                equation_dash.Children.Add(Equation());
                equation_dash.Children.Add(Equation_Dash());

                return equation_dash;
            }

            return null;
        }
        Node ArithmeticOps()
        {
            Node arithmetic_ops = new Node("ArithmeticOps");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;

            // check the arithmetic operations structure
            if (token_class_type == Token_Class.T_PlusOp || token_class_type == Token_Class.T_MinusOp || token_class_type == Token_Class.T_MultiplyOp || token_class_type == Token_Class.T_DivideOp)
            {
                arithmetic_ops.Children.Add(match(TokenStream[InputPointer].token_type));
            }

            return arithmetic_ops;
        }

        Node ConditionOps()
        {
            Node condition_ops = new Node("ConditionOps");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;

            // check the condition operations structure
            if (token_class_type == Token_Class.T_NotEqualOp || token_class_type == Token_Class.T_EqualOp || token_class_type == Token_Class.T_GreaterThanOp || token_class_type == Token_Class.T_LessThanOp)
            {
                condition_ops.Children.Add(match(TokenStream[InputPointer].token_type));
            }

            return condition_ops;
        }

        Node BooleanOps()
        {
            Node boolean_ops = new Node("BooleanOps");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;

            Token_Class token_class_type = TokenStream[InputPointer].token_type;

            // check the boolean operations structure
            if (token_class_type == Token_Class.T_OrOp || token_class_type == Token_Class.T_AndOp)
            {
                boolean_ops.Children.Add(match(TokenStream[InputPointer].token_type));
            }

            return boolean_ops;
        }

        Node Parameter()
        {
            Node parameter = new Node("Parameter");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;


            // check the parameter structure
            if (TokenStream[InputPointer].token_type == Token_Class.T_Integer || TokenStream[InputPointer].token_type == Token_Class.T_Float || TokenStream[InputPointer].token_type == Token_Class.T_String)
            {
                parameter.Children.Add(Datatype());
                parameter.Children.Add(match(Token_Class.T_Identifier));

                return parameter;
            }

            return null;
        }

        Node Parameters()
        {
            Node parameters = new Node("Parameters");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;


            // check the parameters structure
            if (TokenStream[InputPointer].token_type == Token_Class.T_Integer || TokenStream[InputPointer].token_type == Token_Class.T_Float || TokenStream[InputPointer].token_type == Token_Class.T_String)
            {
                parameters.Children.Add(Parameter());
                parameters.Children.Add(Parameters_Dash());
            }

            return parameters;
        }

        Node Parameters_Dash()
        {
            Node parameters_dash = new Node("ParametersDash");

            if (!(InputPointer < TokenStream.Count - 1))
                return null;


            // check the parameters dash structure
            if (TokenStream[InputPointer].token_type == Token_Class.T_Comma)
            {
                parameters_dash.Children.Add(match(Token_Class.T_Comma));
                parameters_dash.Children.Add(Parameter());
                parameters_dash.Children.Add(Parameters_Dash());

                return parameters_dash;
            }

            return null;
        }

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found at " + TokenStream[InputPointer].lex.ToString() + "\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString()  + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
