using Linq2Rest.Parser;
using Nancy;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

/// <summary>
/// This is a modified version of Nancy.OData
/// https://github.com/adamhathcock/Nancy.OData
/// 
/// It adds natural sorting on the results, because Linq2Rest/OData doesn't support custom orderby functions.
/// 
/// </summary>
/// 
namespace BCR
{
  public static class ODataExtensions
  {
    private const string ODATA_URI_KEY = "OData_Uri";

    private static NameValueCollection ParseUriOptions(NancyContext context)
    {
      object item;
      if (context.Items.TryGetValue(ODATA_URI_KEY, out item))
      {
        return item as NameValueCollection;
      }
      NameValueCollection nv = new NameValueCollection();
      context.Items.Add(ODATA_URI_KEY, nv);
      var queryString = context.Request.Url.Query;
      if (string.IsNullOrWhiteSpace(queryString))
      {
        return nv;
      }
      if (!queryString.StartsWith("?"))
      {
        throw new InvalidOperationException("Invalid OData query string " + queryString);
      }
      var parameters = queryString.Substring(1).Split('&', '=');
      if (parameters.Length % 2 != 0)
      {
        throw new InvalidOperationException("Invalid OData query string " + queryString);
      }
      for (int i = 0; i < parameters.Length; i += 2)
      {
        nv.Add(parameters[i], Uri.UnescapeDataString(parameters[i + 1]));
      }
      return nv;
    }

    public static string GetReflectedPropertyValueAsString(this object subject, string field)
    {
      object reflectedValue = subject.GetType().GetProperty(field).GetValue(subject, null);
      return reflectedValue != null ? reflectedValue.ToString() : "";
    }

    public static object GetReflectedPropertyValue(this object subject, string field)
    {
      return subject.GetType().GetProperty(field).GetValue(subject, null);
    }

    public static bool ReflectedPropertyIsString(this object subject, string field)
    {
      return subject.GetType().GetProperty(field).GetType() == typeof(bool);
    }

    /// <summary>
    /// Returns a filtered and ordered result set.
    /// Uses natural sort.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context"></param>
    /// <param name="modelItems">The objects to filter</param>
    /// <param name="totalCount">The total number of items in the resultset before paging.</param>
    /// <returns></returns>
    public static IEnumerable<object> ApplyODataUriFilter<T>(this NancyContext context, IEnumerable<T> modelItems, ref int totalCount)
    {
      NameValueCollection nv = ParseUriOptions(context);

      // Perform sorting ourselves, because linq2rest doesn't allow custom comparers.
      // In order to use a custom sort function, we should first retrieve all filtered objects without paging (i.e. remove $skip and $top).
      // Then sort the objects and lastly apply the paging and select.

      NameValueCollection pagingNV = new NameValueCollection();
      if (null != nv.Get("$skip"))
      {
        pagingNV.Add("$skip", nv.Get("$skip"));
        nv.Remove("$skip");
      }

      if (null != nv.Get("$top"))
      {
        pagingNV.Add("$top", nv.Get("$top"));
        nv.Remove("$top");
      }

      // Remove select as well, as we need to be able to cast the objects to T type after sorting.
      if (null != nv.Get("$select"))
      {
        pagingNV.Add("$select", nv.Get("$select"));
        nv.Remove("$select");
      }

      string sortOption = nv.Get("$orderby");
      nv.Remove("$orderby");


      // Do a query that returns all records using the supplied filter parameters, so we can sort them ourselves.
      // As we removed the $select filter, the returned objects are still of type T.
      var parser = new ParameterParser<T>();
      var filter = parser.Parse(nv);
      var objects = filter.Filter(modelItems).Cast<T>();
      totalCount = objects.Count();

      // Now sort
      if (null != sortOption)
      {
        char[] delimiterChars = { ',' };
        string[] orderby = sortOption.Split(delimiterChars);
        string[] field = new string[orderby.Length];
        bool[] ascending = new bool[orderby.Length];
        char[] delimiterSpace = { ' ' };

        for (int i = 0; i < orderby.Length; i++)
        {
          string[] terms = orderby[i].Split(delimiterSpace);
          field[i] = terms[0];
          ascending[i] = true;
          if (terms.Count() == 2)
            ascending[i] = terms[1] != "desc";
        }

        // TODO: generalize the comparer selection.
        if (typeof(T) == typeof(Comic))
        {
          if (orderby.Count() == 1)
          {
            objects = objects.OrderBy(item => item as Comic, new ComicComparer(field[0], ascending[0]));
          }
          else
          if (orderby.Count() == 2)
          {
            objects = objects.OrderBy(item => item as Comic, new ComicComparer(field[0], ascending[0]))
                             .ThenBy(item => item as Comic, new ComicComparer(field[1], ascending[1]));
          }
          else
          if (orderby.Count() >= 3)
          {
            objects = objects.OrderBy(item => item as Comic, new ComicComparer(field[0], ascending[0]))
                             .ThenBy(item => item as Comic, new ComicComparer(field[1], ascending[1]))
                             .ThenBy(item => item as Comic, new ComicComparer(field[2], ascending[2]));
          }
        }
        else
        {
          // Uhg, use natural sort on every field.....
          // This must change someday.
          if (orderby.Count() == 1)
          {
            objects = objects.OrderBy(item => item.GetReflectedPropertyValueAsString(field[0]), new NaturalSortComparer(ascending[0]));
          }
          else
          if (orderby.Count() == 2)
          {
            objects = objects.OrderBy(item => item.GetReflectedPropertyValueAsString(field[0]), new NaturalSortComparer(ascending[0]))
                              .ThenBy(item => item.GetReflectedPropertyValueAsString(field[1]), new NaturalSortComparer(ascending[1]));
          }
          else
          if (orderby.Count() >= 3)
          {
            objects = objects.OrderBy(item => item.GetReflectedPropertyValueAsString(field[0]), new NaturalSortComparer(ascending[0]))
                              .ThenBy(item => item.GetReflectedPropertyValueAsString(field[1]), new NaturalSortComparer(ascending[1]))
                              .ThenBy(item => item.GetReflectedPropertyValueAsString(field[2]), new NaturalSortComparer(ascending[2]));
          }
        }

      }

      // Now limit the resultset by applying the paging and select options.
      var parser2 = new ParameterParser<T>();
      var filter2 = parser2.Parse(pagingNV);
      var objects2 = filter2.Filter(objects.Cast<T>());
      return objects2;

    }

    public static Response AsOData<T>(this IResponseFormatter formatter, IEnumerable<T> modelItems, HttpStatusCode code = HttpStatusCode.OK)
    {
      // BCR only supports json, no need to supply and check the $format every time....
      int totalCount = 0;
      return formatter.AsJson(formatter.Context.ApplyODataUriFilter(modelItems, ref totalCount), code);
    }
  }
}
