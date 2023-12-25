using JASON_Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    T_Else, T_ElseIf, T_Endl, T_If, T_Integer, T_Float, T_String, T_Read, T_Then, T_Write, T_Repeat, T_Until, T_Return, T_Main, T_End,
    T_AndOp, T_OrOp, T_Semicolon, T_Comma, T_LParanthesis, T_RParanthesis, T_LCurlBracket, T_RCurlBracket, T_EqualOp, T_LessThanOp,
    T_GreaterThanOp, T_NotEqualOp, T_PlusOp, T_MinusOp, T_MultiplyOp, T_DivideOp, T_AssignmentOp,
    T_Identifier, T_Number, T_String_Literal, T_Float_Literal,
    T_DataType
}
namespace TINY_Compiler
{
    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("if", Token_Class.T_If);
            ReservedWords.Add("elseif", Token_Class.T_ElseIf);
            ReservedWords.Add("else", Token_Class.T_Else);
            ReservedWords.Add("int", Token_Class.T_Integer);
            ReservedWords.Add("float", Token_Class.T_Float);
            ReservedWords.Add("string", Token_Class.T_String);
            ReservedWords.Add("read", Token_Class.T_Read);
            ReservedWords.Add("write", Token_Class.T_Write);
            ReservedWords.Add("repeat", Token_Class.T_Repeat);
            ReservedWords.Add("until", Token_Class.T_Until);
            ReservedWords.Add("then", Token_Class.T_Then);
            ReservedWords.Add("return", Token_Class.T_Return);
            ReservedWords.Add("endl", Token_Class.T_Endl);
            ReservedWords.Add("main", Token_Class.T_Main);
            ReservedWords.Add("end", Token_Class.T_End);

            Operators.Add(";", Token_Class.T_Semicolon);
            Operators.Add("{", Token_Class.T_LCurlBracket);
            Operators.Add("}", Token_Class.T_RCurlBracket);
            Operators.Add(",", Token_Class.T_Comma);
            Operators.Add("(", Token_Class.T_LParanthesis);
            Operators.Add(")", Token_Class.T_RParanthesis);
            Operators.Add("=", Token_Class.T_EqualOp);
            Operators.Add(":=", Token_Class.T_AssignmentOp);
            Operators.Add("<", Token_Class.T_LessThanOp);
            Operators.Add(">", Token_Class.T_GreaterThanOp);
            Operators.Add("<>", Token_Class.T_NotEqualOp);
            Operators.Add("+", Token_Class.T_PlusOp);
            Operators.Add("-", Token_Class.T_MinusOp);
            Operators.Add("*", Token_Class.T_MultiplyOp);
            Operators.Add("/", Token_Class.T_DivideOp);
            Operators.Add("&&", Token_Class.T_AndOp);
            Operators.Add("||", Token_Class.T_OrOp);
        }

        public void StartScanning(string SourceCode)
        {
            Errors.Error_List.Clear();
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                //---------------------------------------------------------------------------------------------------------------//
                ///////////////////////////////////// For Reserved Words & Identifiers ///////////////////////////////////////////
                //--------------------------------------------------------------------------------------------------------------//
                if (CurrentChar >= 'A' && CurrentChar <= 'Z' || CurrentChar >= 'a' && CurrentChar <= 'z') //if you read a character
                {

                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];

                        if ((CurrentChar >= 'A' && CurrentChar <= 'z') || (CurrentChar >= '0' && CurrentChar <= '9'))
                        {
                            CurrentLexeme += CurrentChar;
                        }
                        else
                        {
                            break;
                        }
                    }
                    i = j - 1;
                    FindTokenClass(CurrentLexeme);
                }

                //---------------------------------------------------------------------------------------------------------------//
                /////////////////////////////////////// For Numbers(Integers & Float) ////////////////////////////////////////////
                //--------------------------------------------------------------------------------------------------------------//
                else if (CurrentChar >= '0' && CurrentChar <= '9')//if you read a number
                {
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        if ((CurrentChar >= '0' && CurrentChar <= '9') || CurrentChar == '.')
                        {
                            CurrentLexeme += CurrentChar.ToString();
                        }
                        else
                        {
                            break;
                        }
                    }
                    i = j - 1;
                    FindTokenClass(CurrentLexeme);
                }

                //---------------------------------------------------------------------------------------------------------------//
                /////////////////////////////////////////////// For Comments /////////////////////////////////////////////////////
                //--------------------------------------------------------------------------------------------------------------//
                else if (CurrentChar == '/' && j + 1 < SourceCode.Length && SourceCode[j + 1] == '*')//if you read a comment
                {
                    bool isValidComment = true;

                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();

                        if (CurrentChar == '/' && SourceCode[j - 1] == '*')
                        {
                            break;
                        }
                    }
                    if (j == SourceCode.Length)
                    {
                        isValidComment = false;
                    }

                    if (isValidComment)
                    {
                        i = j + 1;
                    }
                    else
                    {
                        Errors.Error_List.Add($"Unterminated Comment {CurrentLexeme}");
                        return;
                    }
                }
                //---------------------------------------------------------------------------------------------------------------//
                /////////////////////////////////////////////// For Operators /////////////////////////////////////////////////////
                //--------------------------------------------------------------------------------------------------------------//
                else if (CurrentChar == '&')//if you read a && operator
                {
                    if (j + 1 < SourceCode.Length && SourceCode[j + 1] == '&')
                    {
                        CurrentLexeme += SourceCode[j + 1];
                        i = j + 1;
                    }
                    FindTokenClass(CurrentLexeme);
                }
                else if (CurrentChar == '|')//if you read a || operator
                {
                    if (j + 1 < SourceCode.Length && SourceCode[j + 1] == '|')
                    {
                        CurrentLexeme += SourceCode[j + 1];
                        i = j + 1;
                    }

                    FindTokenClass(CurrentLexeme);
                }
                else if (CurrentChar == ':')//if you read an assign operator
                {
                    if (j + 1 < SourceCode.Length && SourceCode[j + 1] == '=')
                    {
                        CurrentLexeme += SourceCode[j + 1];
                        i = j + 1;
                    }

                    FindTokenClass(CurrentLexeme);
                }
                else if (CurrentChar == '<') // if you read <>, < operators
                {
                    if (j + 1 < SourceCode.Length && SourceCode[j + 1] == '>')
                    {
                        CurrentLexeme += SourceCode[j + 1];
                        i = j + 1;
                    }

                    FindTokenClass(CurrentLexeme);
                }

                //---------------------------------------------------------------------------------------------------------------//
                /////////////////////////////////////////////// For Strings ///////////////////////////////////////////////////////
                //--------------------------------------------------------------------------------------------------------------//
                else if (CurrentChar == '\"') // if you read a string
                {
                    bool isValidString = true;

                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();

                        if (CurrentChar == '\"')
                        {
                            isValidString = true;
                            break;
                        }

                        // Check if we reached the end of the line
                        else if (CurrentChar == '\n' || CurrentChar == '\r')
                        {
                            isValidString = false;
                            break;
                        }
                    }

                    if (!isValidString)
                    {
                        Errors.Error_List.Add($"Wrong string format in {CurrentLexeme}. Missing closing double quote.");
                        return;
                    }
                    else
                    {
                        FindTokenClass(CurrentLexeme);
                    }

                    i = j;
                }

                else
                {
                    FindTokenClass(CurrentChar.ToString());
                }
            }

            TINY_Compiler.TokenStream = Tokens;
        }

        void FindTokenClass(string Lex)
        {
            Token Tok = new Token();
            Tok.lex = Lex;

            //Is it a Null or Blank Space?
            if (string.IsNullOrWhiteSpace(Lex))
            {
                return;
            }

            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }

            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.T_Identifier;
                Tokens.Add(Tok);
            }

            //Is it an operator?
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }


            //Is it an float?
            else if (isFloat(Lex))
            {
                Tok.token_type = Token_Class.T_Float_Literal;
                Tokens.Add(Tok);
            }

            //Is it an number?
            else if (isNumber(Lex))
            {
                Tok.token_type = Token_Class.T_Number;
                Tokens.Add(Tok);
            }

            //Is it a string?
            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.T_String_Literal;
                Tokens.Add(Tok);
            }

            //Is it an undefined?
            else
            {
                Errors.Error_List.Add($"Couldn't find token class for {Lex} \n");
            }
        }

        bool isIdentifier(string lex)
        {
            // Check if the lex is an identifier or not.
            bool isValid = true;
            Regex regexIdentifier = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*$", RegexOptions.Compiled);

            if (!regexIdentifier.IsMatch(lex))
            {
                isValid = false;
            }
            return isValid;
        }

        bool isNumber(string lex)
        {
            // Check if the lex is a number or not.
            bool isValid = true;
            Regex regexNumber = new Regex(@"^[0-9]+(\.[0-9]+)?$", RegexOptions.Compiled);
            if (!regexNumber.IsMatch(lex))
            {
                isValid = false;
            }
            return isValid;
        }

        bool isFloat(string lex)
        {
            // Check if the lex is a float or not.
            bool isValid = true;
            Regex regexFloat = new Regex(@"^[0-9]+(\.[0-9]+)$", RegexOptions.Compiled);
            if (!regexFloat.IsMatch(lex))
            {
                isValid = false;
            }
            return isValid;
        }

        bool isString(string lex)
        {
            // Check if the lex is a string or not.
            bool isValid = true;
            Regex regexString = new Regex("\"(.*?)\"", RegexOptions.Compiled);
            if (!regexString.IsMatch(lex))
            {
                isValid = false;
            }
            return isValid;
        }

    }
}
