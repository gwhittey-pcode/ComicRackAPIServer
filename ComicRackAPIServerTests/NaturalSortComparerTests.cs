using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BCR.Test
{
  [TestClass()]
  public class NaturalSortComparerTests
  {
    [TestMethod()]
    public void NaturalSortComparerPerformanceTest()
    {
      // Performance test 
      bool ascending = true;
      bool output = false;
      int size = 1000000;
      
      TimeSpan naturalsorttime;
#if USE_NATIVE_STRCMP
      TimeSpan NaturalStringComparerTime;
#endif
      TimeSpan logicalsorttime;
      TimeSpan normalsorttime;
      Random rnd = new Random(DateTime.Now.Millisecond);

      List<string> testItems = new List<string>(size);
      for (int i = 0; i < size; i++)
      {
        if (i % 2 == 0)
          testItems.Add(rnd.Next(100).ToString() + ((char)('A' + rnd.Next(25))) + rnd.Next(100).ToString());
        else
          testItems.Add(((char)('A' + rnd.Next(25))) + rnd.Next(100).ToString());
      }
      List<string> testitems2 = new List<string>(testItems);
      List<string> testitems3 = new List<string>(testItems);
#if USE_NATIVE_STRCMP
      List<string> testitems4 = new List<string>(testItems);
#endif

      DateTime start = DateTime.Now;
      NaturalSortComparer comparer = new NaturalSortComparer(ascending);
      testItems.Sort(comparer);

      DateTime stop = DateTime.Now;

      naturalsorttime = stop - start;

      start = DateTime.Now;
      LogicalStringComparer logicalcomparer = new LogicalStringComparer(ascending);
      testitems2.Sort(logicalcomparer);
      stop = DateTime.Now;

      logicalsorttime = stop - start;

      start = DateTime.Now;
      testitems3.Sort();
      stop = DateTime.Now;

      normalsorttime = stop - start;
      
#if USE_NATIVE_STRCMP
      start = DateTime.Now;
      testitems4.Sort(new NaturalStringComparer(ascending));
      stop = DateTime.Now;

      NaturalStringComparerTime = stop - start;
#endif

      
      
      if (output)
      {
        Debug.WriteLine("Natural Comparison: ");
        foreach (string _item in testItems)
        {
          Debug.WriteLine(_item);
        }

        Debug.WriteLine("Logical Comparison: ");
        foreach (string _item in testitems2)
        {
          Debug.WriteLine(_item);
        }

        Debug.WriteLine("Normal Comparison");
        foreach (string _item in testitems3)
        {
          Debug.WriteLine(_item);
        }
      }
      
      Console.WriteLine("Elapsed time for natural sort: " + naturalsorttime);
      Console.WriteLine("Elapsed time for logical sort: " + logicalsorttime);
#if USE_NATIVE_STRCMP
      Console.WriteLine("Elapsed time for native natural sort: " + NaturalStringComparerTime);
#endif
      Console.WriteLine("Elapsed time for normal sort : " + normalsorttime);

      for (int i = 0; i < testItems.Count; i++)
      {
        Assert.AreEqual(testItems[i], testitems2[i]);
#if USE_NATIVE_STRCMP
        Assert.AreEqual(testItems[i], testitems4[i]);
#endif
      }

    }

    
  }
}
