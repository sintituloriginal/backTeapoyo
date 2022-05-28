using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;


public class trackDinamico
{
    public trackDinamico(string name,Type type)
    {
        this.FieldName = name;
        this.FieldType = type;
    }
    public string FieldName { get; set; }
    public Type FieldType { get; set; }
}

public class generic
{
    public string key { get; set; }
    public string value { get; set; }
}