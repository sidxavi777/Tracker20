using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;


namespace Models
{
    public class PaginatedList<T>: List<T>
    {
        public int PageIndex { get; set; }
        public int Totalpage { get; set; }



        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.Totalpage = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }


        public bool HasPerviousPage => PageIndex > 1;
        public bool HasNextPage=> PageIndex< Totalpage;

        public static PaginatedList<T> Create (List<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count;
            var items = source.Skip((pageIndex-1)*pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items,count, pageIndex, pageSize);
        }
    }
}
