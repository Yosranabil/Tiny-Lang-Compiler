using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    Else, ElseIf, Endl, If, IntegerDT, FloatDT, StringDT, Read, Then, Write, Repeat, Until, Return, Main, End,
    AndOp, OrOp, Dot, Semicolon, Comma, LParanthesis, RParanthesis, LCurlBracket, RCurlBracket, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp, AssignmentOp,
    Idenifier, Number, String, Comment
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
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("elseif", Token_Class.ElseIf);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("int", Token_Class.IntegerDT);
            ReservedWords.Add("float", Token_Class.FloatDT);
            ReservedWords.Add("string", Token_Class.StringDT);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("main", Token_Class.Main);
            ReservedWords.Add("end", Token_Class.End);

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
            for(int i=0; i<SourceCode.Length;i++)
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


                else if (CurrentChar >= '0' && CurrentChar <= '9')
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
                else if (CurrentChar == '/' && SourceCode[j + 1] == '*')
                {
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();

                        if (CurrentChar == '/')
                        {
                            FindTokenClass(CurrentLexeme);
                            break;
                        }
                    }
                    i = j + 1;
                }
                else if (CurrentChar == '&' || CurrentChar == '|')
                {
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();

                        if ((CurrentChar == '&' && SourceCode[j + 1] == '&') || (CurrentChar == '|' && SourceCode[j + 1] == '|'))
                        {
                            FindTokenClass(CurrentLexeme);
                            break;
                        }
                    }
                    i = j + 1;
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
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;

            //Is it a reserved word?
            if(ReservedWords.ContainsKey(Lex))
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

            //Is it an integer?
            else if(isInt(Lex))
            {
                Tok.token_type = Token_Class.IntegerDT;
                Tokens.Add(Tok);
            }

            //Is it an float?
            else if (isFloat(Lex))
            {
                Tok.token_type = Token_Class.FloatDT;
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

            //Is it a comment?
            else if (isComment(Lex))
            {
                Tok.token_type = Token_Class.Comment;
                Tokens.Add(Tok);
            }

            //Is it an undefined?
            else
            {
                Errors.Error_List.Add(Lex);
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

        bool isInt(string lex)
        {
            // Check if the lex is an integer or not.
            bool isValid = true; 
            Regex regexInt = new Regex(@"^[0-9]+$", RegexOptions.Compiled);
            if(!regexInt.IsMatch(lex))
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
            Regex regexString = new Regex("\"(^\")\"", RegexOptions.Compiled);
            if(!regexString.IsMatch(lex))
            {
                isValid = false;
            }
            return isValid;

        }

        bool isComment(string lex)
        {
            bool isValid = true;
            Regex regexComment = new Regex("\\/\\*[\\s\\S]*?\\*\\/", RegexOptions.Compiled);
            if (!regexComment.IsMatch(lex))
            {
                isValid = false;
            }
            return isValid;
        }
    }
}
