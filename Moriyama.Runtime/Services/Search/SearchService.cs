using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Moriyama.Runtime.Extension;
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

        private RuntimeContentModel GetContentModel(Document content)
        {
            var r = new RuntimeContentModel();

           

            return r;
        }

        private Document GetLuceneDocument(RuntimeContentModel content)
        {
            var d = new Document();

            d.Add(new Field("Url", content.Url, Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("Name", content.Name, Field.Store.YES, Field.Index.ANALYZED));
            
            d.Add(new Field("CreateDate", content.CreateDate.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("UpdateDate", content.UpdateDate.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            
            d.Add(new Field("Type", content.Type, Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("CreatorName", content.CreatorName, Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("WriterName", content.WriterName, Field.Store.YES, Field.Index.NOT_ANALYZED));

            d.Add(new Field("RelativeUrl", content.RelativeUrl, Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("Template", content.Template, Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("SortOrder", content.SortOrder.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("Level", content.Level.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

            foreach (var property in content.Content)
            {
                var value = property.Value.ToString();
                d.Add(new Field(property.Key, value, Field.Store.YES, Field.Index.ANALYZED));
            }

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

        public void Delete(string url)
        {
            using (var writer = new IndexWriter(_directory, new StandardAnalyzer(), true))
            {
                try
                {
                    writer.DeleteDocuments(new Term("Url", url));
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

        public IEnumerable<string> Search(string searchTerm)
        {
            var results = new List<string>();
            var indexSearcher = new IndexSearcher(_directory);

            try
            {
                var queryParser = new QueryParser("bodyText", new StandardAnalyzer());

                Query query = queryParser.Parse(searchTerm);
                var hits = indexSearcher.Search(query);
                var numHits = hits.Length();

                for (var i = 0; i < numHits; ++i)
                {
                    var doc = hits.Doc(i);
                    results.Add(doc.Get("Url"));
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
            }
            finally
            {
                indexSearcher.Close();
            }

            return results;
        }
    }
}
