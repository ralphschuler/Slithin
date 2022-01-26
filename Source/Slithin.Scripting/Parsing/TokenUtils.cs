﻿namespace Slithin.Scripting.Parsing;

public static class TokenUtils
{
    public static int GetBinaryOperatorPrecedence(this TokenType kind)
    {
        return kind switch
        {
            TokenType.Star or TokenType.Slash or TokenType.Colon => 5,
            TokenType.Plus or TokenType.Minus => 4,
            _ => 0,
        };
    }

    public static TokenType GetTokenType(string name)
    {
        return name switch
        {
            "as" => TokenType.As,
            "at" => TokenType.At,
            "not" => TokenType.Not,
            "negate" => TokenType.Minus,
            "remember" => TokenType.Remember,
            _ => TokenType.Identifier,
        };
    }

    public static int GetUnaryOperatorPrecedence(this TokenType kind)
    {
        return kind switch
        {
            TokenType.Minus or TokenType.Not => 6,
            _ => 0,
        };
    }
}
