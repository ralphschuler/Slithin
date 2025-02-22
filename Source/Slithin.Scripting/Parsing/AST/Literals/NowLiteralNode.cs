﻿using Slithin.Scripting.Parsing.AST;
namespace Slithin.Scripting.Parsing.AST.Literals;

public class NowLiteralNode : LiteralNode
{
    public NowLiteralNode(object value) : base(value)
    {
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
