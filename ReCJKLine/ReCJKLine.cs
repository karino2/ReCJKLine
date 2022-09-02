namespace ReCJKLine;
using System;

public class ReCJKLine
{

  public struct ShortcutKey
  {
    ConsoleKey Key;
    bool Shift;
    bool Alt;
    bool Ctrl;

    public ShortcutKey(ConsoleKey key, bool shift, bool alt, bool ctrl)
    {
      Key = key;
      Shift = shift;
      Alt = alt;
      Ctrl = ctrl;
    }

    public ShortcutKey(ConsoleKeyInfo kinfo) : this(kinfo.Key, (kinfo.Modifiers&ConsoleModifiers.Shift) != 0, (kinfo.Modifiers&ConsoleModifiers.Alt) != 0, (kinfo.Modifiers&ConsoleModifiers.Control) != 0)
    {      
    }
  }

  List<char> _chars = new List<char>();
  List<int> _widths = new List<int>();
  int _pos = 0;

  Dictionary<ShortcutKey, Action<ConsoleKeyInfo>> _keyMap;

  public ReCJKLine()
  {
    _keyMap =new Dictionary<ShortcutKey, Action<ConsoleKeyInfo>>();
    _keyMap[new ShortcutKey(ConsoleKey.LeftArrow, false, false, false  )] = MoveLeft;
    _keyMap[new ShortcutKey(ConsoleKey.RightArrow, false, false, false )] = MoveRight;
    _keyMap[new ShortcutKey(ConsoleKey.Backspace, false, false, false )] = Backspace;
    _keyMap[new ShortcutKey(ConsoleKey.B, false, false, true )] = MoveLeft;
    _keyMap[new ShortcutKey(ConsoleKey.F, false, false, true )] = MoveRight;
    _keyMap[new ShortcutKey(ConsoleKey.H, false, false, true )] = Backspace;
    _keyMap[new ShortcutKey(ConsoleKey.D, false, false, true )] = Delete;
    _keyMap[new ShortcutKey(ConsoleKey.K, false, false, true )] = DeleteAfter;
  }

  void DebugPrint(string msg)
  {
    // System.Console.Error.WriteLine(msg);
  }

  void DebugPrint(string fmt, object? arg1, object? arg2)
  {
    // System.Console.Error.WriteLine(fmt, arg1, arg2);
  }

  void MoveLeft(ConsoleKeyInfo _) {
    if (_pos != 0)
    {
      int size = _widths[_pos-1];
      System.Console.SetCursorPosition(Console.CursorLeft-size, Console.CursorTop);
      _pos--;
    }
    else
    {
      Console.Beep();
    }
  }

  void Backspace(ConsoleKeyInfo _) {
    if (_pos != 0)
    {
      int size = _widths[_pos-1];
      System.Console.SetCursorPosition(Console.CursorLeft-size, Console.CursorTop);
      ClearAfter(_pos-1);
      _widths.RemoveAt(_pos-1);
      _chars.RemoveAt(_pos-1);
      _pos--;
      PrintAfter(_pos);
    }
    else
    {
      Console.Beep();
    }
  }

  void Delete(ConsoleKeyInfo _) {
    if (_pos != _chars.Count)
    {
      ClearAfter(_pos);
      _widths.RemoveAt(_pos);
      _chars.RemoveAt(_pos);
      PrintAfter(_pos);
    }
    else
    {
      Console.Beep();
    }
  }

  void DeleteAfter(ConsoleKeyInfo _) {
    if (_pos != _chars.Count)
    {
      ClearAfter(_pos);
      int delcount = _widths.Count - _pos;
      _widths.RemoveRange(_pos, delcount);
      _chars.RemoveRange(_pos, delcount);
    }
    else
    {
      Console.Beep();
    }
  }


  void MoveRight(ConsoleKeyInfo _) {
    if (_pos == _widths.Count)
    {
      Console.Beep();
    }
    else
    {
      int size = _widths[_pos];
      System.Console.SetCursorPosition(Console.CursorLeft+size, Console.CursorTop);
      _pos++;
    }
  }

  void WriteChar(ConsoleKeyInfo kinfo) {
    int width = IsFullWidth(kinfo.KeyChar) ? 2 : 1;
    System.Console.Write(kinfo.KeyChar);
    if (_pos == _chars.Count)
    {
      _chars.Add(kinfo.KeyChar);
      _widths.Add(width);
    }
    else
    {
      _chars.Insert(_pos, kinfo.KeyChar);
      _widths.Insert(_pos, width);
      PrintAfter(_pos+1);
    }    
    _pos++;
  }

  void PrintAfter(int pos)
  {
    var curpos = Console.GetCursorPosition();
    for(int i = pos; i < _chars.Count; i++)
    {
      Console.Write(_chars[i]);
    }
    Console.SetCursorPosition(curpos.Left, curpos.Top);    
  }

  // Move cursor to pos before call this function.
  void ClearAfter(int pos)
  {
    var curpos = Console.GetCursorPosition();
    for(int i = pos; i < _chars.Count; i++)
    {
      for(int j = 0; j < _widths[i]; j++)
        Console.Write(' ');
    }
    Console.SetCursorPosition(curpos.Left, curpos.Top);
  }

  Action<ConsoleKeyInfo> LookupHandler(ConsoleKeyInfo kinfo)
  {
    return _keyMap.GetValueOrDefault(new ShortcutKey(kinfo), WriteChar);
  }


  bool IsFullWidth(char ch)
  {
    return (ch >= '\u3000' && ch  <= '\u9FFF');
  }

  public string ReadLine(string prompt)
  {
    Console.Write(prompt);
    while(true)
    {
      var keyinfo = Console.ReadKey(true);
      DebugPrint("Key={0}, KeyChar={1}.", keyinfo.Key, (int)keyinfo.KeyChar);
      if (keyinfo.KeyChar >= '\u3000' && keyinfo.KeyChar  <= '\u9FFF')
        DebugPrint("全角！");

      if (keyinfo.Key == ConsoleKey.Enter)
      {
        Console.WriteLine();
        return new String(_chars.ToArray());
      }

      var handler = LookupHandler(keyinfo);
      handler.Invoke(keyinfo);
    }
  }

  public static void Main()
  {
    var recjk = new ReCJKLine();

    var res = recjk.ReadLine("test>");
    Console.WriteLine("result: {0}", res);
    // Console.WriteLine("Hello, World!");
  }
}
