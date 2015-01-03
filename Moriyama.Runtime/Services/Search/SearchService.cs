using System;
using System.Reflection;
using log4net;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Moriyama.Runtime.Interfaces;
using Moriyama.Runtime.Models;

namespace Moriyama.Runtime.Services.Search
{
    public class SearchService : ISearchService
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly object _lock;

        private readonly RAMDirectory _directory;

        private static SearchService _instance;

        private SearchService()
        {
           
            _lock = new object();
            _directory = new RAMDirectory();
        }

        public static SearchService Instance
        {
            get { return _instance ?? (_instance = new SearchService()); }
        }

        private Document GetLuceneDocument(RuntimeContentModel content)
        {
            var d = new  Document();

            d.Add(new Field("Url", content.Url, Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("Name", content.Name, Field.Store.YES, Field.Index.ANALYZED));
            
            return d;
        }
       
        public void Index(RuntimeContentModel model)
        {
            var doc = GetLuceneDocument(model);

            lock (_lock)
            {
                using (var writer = new IndexWriter(_directory, new StandardAnalyzer(), true))
                {
                    try
                    {
                        writer.DeleteDocuments(new Term("Url", doc.Get("Url")));
                        writer.Commit();

                        writer.AddDocument(doc);
                        writer.Commit();
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex);
                    }
                    finally
                    {
                        writer.Optimize();
                        writer.Dispose();
                    }
                }
            }
        }
    }
}
