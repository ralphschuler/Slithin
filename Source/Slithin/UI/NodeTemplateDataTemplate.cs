﻿using Avalonia.Controls;
using Avalonia.Controls.Templates;
using NodeEditor.Model;
using NodeEditor.ViewModels;

namespace Slithin.UI;

public class NodeTemplateDataTemplate : IDataTemplate
{

    public IControl Build(object data)
    {
        if(data is INodeTemplate template)
        {
            var data1 = template.Build(0, 0);
            var ret = new ViewLocator().Build(data1);
            ret.SetValue(Control.MaxWidthProperty, 150);
            ret.SetValue(Control.MaxHeightProperty, 250);
            ret.DataContext = ((NodeViewModel)data1).Content;

            return ret;
        }

        return null;
    }

    public bool Match(object data)
    {
        return data is INodeTemplate;
    }
}