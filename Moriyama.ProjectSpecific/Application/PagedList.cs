using System.Linq;

namespace Moriyama.Blog.Project.Application
{
    public class PagedList<T>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="list">The full list of items you would like to paginate</param>
        /// <param name="page">(optional) The current page number</param>
        /// <param name="pageSize">(optional) The size of the page</param>
        public PagedList(IQueryable<T> list, int?page=null, int?pageSize=null)
        {
            _list = list;
            _page = page;
            _pageSize = pageSize;
        }

        private readonly IQueryable<T> _list;
        
        /// <summary>
        /// The paginated result
        /// </summary>
        public IQueryable<T> Items
        {
            get
            {
                return _list == null ? null : _list.Skip((Page - 1) * PageSize).Take(PageSize);
            }
        }

        private int? _page;
        /// <summary>
        ///  The current page.
        /// </summary>
        public int Page
        {
            get {
                return !_page.HasValue ? 1 : _page.Value;
            }
        }

        public int Pages
        {
            get { return (TotalItemCount + PageSize -1 ) / PageSize; }
        }

        private int? _pageSize;
        /// <summary>
        /// The size of the page.
        /// </summary>
        public int PageSize
        {
            get
            {
                if (!_pageSize.HasValue)
                {
                    return _list == null ? 0 : _list.Count();
                }

                return _pageSize.Value;
            }
        }

        /// <summary>
        /// The total number of items in the original list of items.
        /// </summary>
        public int TotalItemCount
        {
            get
            {
                return _list == null ? 0 : _list.Count();
            }
        }
    }
}
