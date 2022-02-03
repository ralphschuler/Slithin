﻿using Slithin.Scripting.Core;
using Slithin.Scripting.Parsing.AST;
using Slithin.Scripting.Parsing.AST.Statements;

namespace Slithin.Scripting.Parsing;

public partial class Parser : BaseParser<SyntaxNode, Lexer, Parser>
{
    public Parser(SourceDocument document, List<Token> tokens, List<Message> messages) : base(document, tokens, messages)
    {
    }

    protected override SyntaxNode Start()
    {
        var cu = new CompilationUnit();
        while (Current.Type != (TokenType.EOF))
        {
            var keyword = Current;

            if (keyword.Type == TokenType.Remember)
            {
                cu.Body.Body.Add(ParseRemember());
            }
            else if (keyword.Type == TokenType.Set)
            {
                cu.Body.Body.Add(ParseVariableAssignment());
            }
            else
            {
                cu.Body.Body.Add(ParseExpressionStatement());
            }
        }

        cu.Messages = Messages;

        return cu;
    }

    private SyntaxNode ParseExpressionStatement()
    {
        var expr = ParseExpression();

        Match(TokenType.Dot);

        return new ExpressionStatement(expr);
    }

    private SyntaxNode ParseRemember()
    {
        NextToken();

        var value = ParseExpression();

        Match(TokenType.As);

        var name = (NameExpression)ParseIdentifierList();

        Match(TokenType.Dot);

        return new RememberStatement(name.Name, value);
    }

    private SyntaxNode ParseVariableAssignment()
    {
        NextToken();

        //set x to 12.
        var nameToken = NextToken();

        Match(TokenType.To);

        var value = ParseExpression();

        Match(TokenType.Dot);

        return new AssignmentStatement(nameToken, value);
    }
}
