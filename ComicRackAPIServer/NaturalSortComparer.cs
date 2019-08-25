using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;

namespace BCR
{

#if USE_NATIVE_STRCMP
  [SuppressUnmanagedCodeSecurity]
  internal static class SafeNativeMethods
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    public static extern int StrCmpLogicalW(string psz1, string psz2);
  }

  public sealed class NaturalStringComparer : IComparer<string>
  {
    private bool isAscending;

    public NaturalStringComparer(bool inAscendingOrder = true)
    {
      this.isAscending = inAscendingOrder;
    }

    public int Compare(string a, string b)
    {
      int retVal = SafeNativeMethods.StrCmpLogicalW(a, b);
      return isAscending ? retVal : -retVal;
    }
  }
#endif


  /// <summary>
  /// http://www.codeproject.com/Articles/22517/Natural-Sort-Comparer?msg=2372465#xx2372465xx
  /// NB: this method does not work correctly if a string contains number parts that are larger then long.MaxValue.....
  /// This comparer is about 10% faster than the NaturalSortComparer below.
  /// However, this comparer doesn't produce the exact same result as NaturalSortComparer, so it contains a bug somewhere.
  /// </summary>
  public class LogicalStringComparer : Comparer<string>
  {
    private bool isAscending;

    public LogicalStringComparer(bool inAscendingOrder = true)
    {
      this.isAscending = inAscendingOrder;
    }

    public override int Compare(string x, string y)
    {
      unsafe
      {
        if (x == y) 
          return 0;

        if (x == null && y != null)
          return isAscending ? -1 : 1;

        if (x != null && y == null)
          return isAscending ? 1 : -1;


        int lastResult = 0;
        char* temp = (char*)IntPtr.Zero;

        fixed (char* curx = x)
        {
          fixed (char* cury = y)
          {
            char* cx = curx;
            char* cy = cury;

            while (true)
            {
              if (*cx == '\0' || *cy == '\0')
                return isAscending ? lastResult : -lastResult;

              if (char.IsLetter(*cx) && char.IsLetter(*cy))
              {
                lastResult = cx->CompareTo(*cy);

                if (lastResult != 0)
                {
                  return isAscending ? lastResult : -lastResult;
                }
                cy++; cx++;
                continue;
              }

              if (char.IsWhiteSpace(*cx))
              {
                cx++;
                continue;
              }

              if (char.IsWhiteSpace(*cy))
              {
                cy++;
                continue;
              }

              long nx = -1, ny = -1;
              string sx="", sy="";
              bool ix = true, iy = true;

              if (char.IsNumber(*cx))
              {
                temp = cx;
                while (char.IsNumber(*cx)) cx++;
                sx = new string(temp, 0, (int)((cx) - temp));
                ix = long.TryParse(sx, out nx);
              }

              if (char.IsNumber(*cy))
              {
                temp = cy;
                while (char.IsNumber(*cy)) cy++;
                sy = new string(temp, 0, (int)((cy) - temp));
                iy = long.TryParse(sy, out ny);
              }
              
              if (nx > -1 && ny > -1)
                lastResult = nx.CompareTo(ny);
              else
              if (nx > -1 && ny == -1)
                lastResult = -1;
              else
              if (nx == -1 && ny > -1)
                lastResult = 1;
              else
              {
                //if (!ix || !iy)
                //  lastResult = sx.CompareTo(sy);
              }

              if (lastResult != 0)
              {
                return isAscending ? lastResult : -lastResult;
              }
            }
          }
        }
      }
    }
  }

  /// <summary>
  /// http://www.codeproject.com/Articles/22517/Natural-Sort-Comparer
  /// NB: this method does not work correctly if a string contains number parts that are larger then long.MaxValue.....
  /// </summary>
  public class NaturalSortComparer : Comparer<string>
  {
    private bool isAscending;

    public NaturalSortComparer(bool inAscendingOrder = true)
    {
      this.isAscending = inAscendingOrder;
    }

    #region Comparer<string> implementation

    public override int Compare(string x, string y)
    {
      if (x == y)
        return 0;

      if (x == null && y != null)
        return isAscending ? -1 : 1;

      if (x != null && y == null)
        return isAscending ? 1 : -1;


      string[] x1, y1;

      if (!table.TryGetValue(x, out x1))
      {
        x1 = Regex.Split(x.Replace(" ", ""), "([0-9]+)");
        table.Add(x, x1);
      }

      if (!table.TryGetValue(y, out y1))
      {
        y1 = Regex.Split(y.Replace(" ", ""), "([0-9]+)");
        table.Add(y, y1);
      }

      int returnVal;

      for (int i = 0; i < x1.Length && i < y1.Length; i++)
      {
        if (x1[i] != y1[i])
        {
          returnVal = PartCompare(x1[i], y1[i]);
          return isAscending ? returnVal : -returnVal;
        }
      }

      if (y1.Length > x1.Length)
      {
        returnVal = 1;
      }
      else if (x1.Length > y1.Length)
      {
        returnVal = -1;
      }
      else
      {
        returnVal = 0;
      }

      return isAscending ? returnVal : -returnVal;
    }
    
    #endregion

    private static int PartCompare(string left, string right)
    {
      long x, y;
      if (!long.TryParse(left, out x))
        return left.CompareTo(right);

      if (!long.TryParse(right, out y))
        return left.CompareTo(right);

      return x.CompareTo(y);
    }

    private Dictionary<string, string[]> table = new Dictionary<string, string[]>();
  }
}
