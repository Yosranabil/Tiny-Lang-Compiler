using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    Else_KW, ElseIf_KW, Endl_KW, If_KW, Integer_KW, Float_KW, String_KW, Read_KW, Then_KW, Write_KW, Repeat_KW, Until_KW, Return_KW, Main_KW, End_KW,
    AndOp, OrOp, Dot, Semicolon, Comma, LParanthesis, RParanthesis, LCurlBracket, RCurlBracket, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp, AssignmentOp,
    Idenifier, Number, String, Float, Comment
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
            ReservedWords.Add("if", Token_Class.If_KW);
            ReservedWords.Add("elseif", Token_Class.ElseIf_KW);
            ReservedWords.Add("else", Token_Class.Else_KW);
            ReservedWords.Add("int", Token_Class.Integer_KW);
            ReservedWords.Add("float", Token_Class.Float_KW);
            ReservedWords.Add("string", Token_Class.String_KW);
            ReservedWords.Add("read", Token_Class.Read_KW);
            ReservedWords.Add("write", Token_Class.Write_KW);
            ReservedWords.Add("repeat", Token_Class.Repeat_KW);
            ReservedWords.Add("until", Token_Class.Until_KW);
            ReservedWords.Add("then", Token_Class.Then_KW);
            ReservedWords.Add("return", Token_Class.Return_KW);
            ReservedWords.Add("endl", Token_Class.Endl_KW);
            ReservedWords.Add("main", Token_Class.Main_KW);
            ReservedWords.Add("end", Token_Class.End_KW);

            Operators.Add(".", Token_Class.Dot);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add("{", Token_Class.LCurlBracket);
            Operators.Add("}", Token_Class.RCurlBracket);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add(":=", Token_Class.AssignmentOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);
        }

        public void StartScanning(string SourceCode)
    {
            bool expectingSemicolon = false;

            for (int i=0; i<SourceCode.Length;i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

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
                            i = j - 1;
                            break;
                        }
                    }
                    FindTokenClass(CurrentLexeme);
                }


                else if (CurrentChar >= '0' && CurrentChar <= '9')//if you read a number
                {
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        if (CurrentChar == '=' || CurrentChar == '+' || CurrentChar == '-' || CurrentChar == '/' ||
                            CurrentChar == '*' || CurrentChar == '<' || CurrentChar == '>' || CurrentChar == '&' ||
                            CurrentChar == ':' || CurrentChar == ';' || CurrentChar == ',' || CurrentChar == ')' ||
                            CurrentChar == ' ' || CurrentChar == '\n' || CurrentChar == '\r')
                        {
                            i = j - 1; break;
                        }
                        CurrentLexeme += CurrentChar.ToString();
                    }
                    FindTokenClass(CurrentLexeme);
                }
                else if (CurrentChar == '/' && SourceCode[j + 1] == '*')//if you read a comment
                {
                    bool isValidComment = true;

                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();

                        if (CurrentChar == '/')
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
                        Errors.Error_List.Add($"Wrong Comment Format in {CurrentLexeme}");
                    }
                }
                else if (CurrentChar == '&' || CurrentChar == '|')//if you read a boolean operator
                {
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();

                        if ((CurrentLexeme == "&&" && SourceCode[j + 1] != '&') || (CurrentLexeme == "||" && SourceCode[j + 1] != '|'))
                        {
                            FindTokenClass(CurrentLexeme);
                            break;
                        }
                    }
                    i = j;
                }
                else if(CurrentChar == ':')//if you read an assign operator
                {
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();

                        if (CurrentChar == '=')
                        {
                            FindTokenClass(CurrentLexeme);
                            break;
                        }
                    }
                    i = j + 1;
                }
                else if(CurrentChar == '\"')//if you read a string
                {
                    bool isValidString = true;

                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();

                        if (CurrentChar == '\"')
                        {

                            break;
                        }
                    }
                    if (j == SourceCode.Length)
                    {
                        isValidString = false;
                    }

                    if (isValidString)
                    {
                        FindTokenClass(CurrentLexeme);
                        i = j + 1;
                    }
                    else
                    {
                        Errors.Error_List.Add($"Wrong string Format in {CurrentLexeme}");
                    }
                }
                else if(CurrentChar == '.')//if you read float
                {
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();

                        if ((CurrentChar >= '0' && CurrentChar <= '9'))
                        {
                            CurrentLexeme += CurrentChar.ToString();
                        }
                        else
                        {
                            i = j - 1; break;
                        }
                    }
                    FindTokenClass(CurrentLexeme);
                }
                else if (CurrentChar == '<') // if you read less than operator
                {
                    if (SourceCode[j + 1] == '>')
                    {
                        CurrentLexeme += SourceCode[j + 1];
                        i = j + 1;
                    }

                    FindTokenClass(CurrentLexeme);
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
            else if(isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
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
                Tok.token_type = Token_Class.Float;
                Tokens.Add(Tok);
            }

            //Is it an number?
            else if (isNumber(Lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
            }

            //Is it a string?
            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.String;
                Tokens.Add(Tok);
            }

            //Is it an undefined?
            else
            {
                Errors.Error_List.Add($"Couldn't find token class for {Lex}");
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
            if(!regexNumber.IsMatch(lex))
            {
                isValid= false;
            }
            return isValid;
        }

        bool isFloat(string lex)
        {
            // Check if the lex is a float or not.
            bool isValid = true; 
            Regex regexFloat = new Regex(@"^[0-9]+(\.[0-9]+)$", RegexOptions.Compiled);
            if(!regexFloat.IsMatch(lex))
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
