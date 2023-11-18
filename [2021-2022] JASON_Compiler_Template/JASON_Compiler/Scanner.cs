using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    Else, ElseIf, EndIf, If, Integer, Float, String, Read, Then, While, Write, Repeat,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    Idenifier, Constant
}
namespace JASON_Compiler
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
            ReservedWords.Add("int", Token_Class.Integer);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);

            Operators.Add(".", Token_Class.Dot);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("!", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);



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

                if (CurrentChar >= 'A' && CurrentChar <= 'z') //if you read a character
                {
                    CurrentLexeme = CurrentChar.ToString(); // Start with the current character

                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];

                        if ((CurrentChar >= 'A' && CurrentChar <= 'z'))
                        {
                            CurrentLexeme += CurrentChar; // Append the character to the lexeme
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
                   for(j = i + 1; j < SourceCode.Length; j++)
                   {
                        if ((CurrentChar >= '0' && CurrentChar <= '9'))
                        {
                            CurrentLexeme += CurrentChar.ToString();
                        }
                   }
                   i = j - 1;
                   FindTokenClass(CurrentLexeme);
                }
                else if(CurrentChar == '{')
                {
                   while(CurrentChar != '}')
                   {
                        j++;
                        continue;
                   }
                    i = j - 1;
                }
                else
                {
                    FindTokenClass(CurrentChar.ToString());
                }
            }
            
            JASON_Compiler.TokenStream = Tokens;
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
                return;
            }

            //Is it an identifier?
            if(isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
                return;
            }

            //Is it a Constant?
            if (isConstant(Lex))
            {
                Tok.token_type = Token_Class.Constant;
                Tokens.Add(Tok);
                return;
            }

            //Is it an operator?
            if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
                return;
            }

            //Is it an undefined?
            Errors.Error_List.Add(Lex);
        }

    

        bool isIdentifier(string lex)
        {
            bool isValid=true;
            // Check if the lex is an identifier or not.
            Regex re = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*$", RegexOptions.Compiled);

            if (!re.IsMatch(lex))
            {
                isValid = false;
            }
            return isValid;
        }
        bool isConstant(string lex)
        {
            bool isValid = true;
            // Check if the lex is a constant (Number) or not.
            Regex intRegex = new Regex(@"^\d+$", RegexOptions.Compiled);
            Regex floatRegex = new Regex(@"^\d+\.\d+$", RegexOptions.Compiled);

            if (!intRegex.IsMatch(lex) || !floatRegex.IsMatch(lex))
            {
                isValid = false;
            }
            return isValid;
        }
    }
}
