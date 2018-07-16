using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for IPProcessStatus
/// </summary>
public class IPProcessStatus
{
    private static Hashtable _Status = new Hashtable();

    public static object GetValue(string SessionID)
    {
        return _Status[SessionID];
    }
    
    public static void Add(string SessionID, object Value)
    {
        _Status.Add(SessionID, Value);
    }

    public static void Update(string SessionID, object Value)
    {
        _Status[SessionID] = Value;
    }

    public static void Remove(string SessionID)
    {
        _Status.Remove(SessionID);
    }

    public static bool Contains(string SessionID)
    {
        return _Status.Contains(SessionID);
    }
}
