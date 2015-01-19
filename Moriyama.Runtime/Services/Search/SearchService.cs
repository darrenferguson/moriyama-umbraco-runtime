using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using log4net;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Moriyama.Runtime.Interfaces;
using Moriyama.Runtime.Models;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;
using Lucene.Net.Highlight;

namespace Moriyama.Runtime.Services.Search
{
    public class SearchService : ISearchService
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly object _lock;

        private readonly RAMDirectory _directory;
        private IndexWriter _writer;

        private static SearchService _instance;

        private SearchService()
        {
            _lock = new object();
            _directory = new RAMDirectory();
            _writer = new IndexWriter(_directory, new StandardAnalyzer(Version.LUCENE_29), IndexWriter.MaxFieldLength.UNLIMITED);
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
                value = StripHtml(value);

                d.Add(new Field(property.Key, value, Field.Store.YES, Field.Index.ANALYZED));
            }

            return d;
        }

        protected string StripHtml(string htmlString)
        {
            const string pattern = @"<(.|\n)*?>";
            return Regex.Replace(htmlString, pattern, string.Empty);
        }

        // For debugging!

        //public void FlushToDisc()
        //{
        //    var dir = new DirectoryInfo(@"C:\temp\index");
        //    foreach (var file in dir.GetFiles())
        //    {
        //        file.Delete();
        //    }

        //    var targetDir = FSDirectory.Open(dir);

        //    Directory.Copy(_directory, targetDir, false);
        //}

        public void Index(RuntimeContentModel model)
        {
            var doc = GetLuceneDocument(model);

            lock (_lock)
            {

                try
                {
                    _writer.DeleteDocuments(new Term("Url", doc.Get("Url")));
                    _writer.Commit();

                    _writer.AddDocument(doc);
                    _writer.Commit();

                    if (Logger.IsDebugEnabled)
                    {
                        Logger.Debug("Indexing: " + model.Name + " for search.");
                        // FlushToDisc();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
                finally
                {
                    // _writer.Optimize();
                }
            }
        }


        private string GeneratePreviewText(Query q, string text)
        {
            var scorer = new QueryScorer(q);
            var formatter = new SimpleHTMLFormatter("<em>", "</em>");

            var highlighter = new Highlighter(formatter, scorer);

            highlighter.SetTextFragmenter(new SimpleFragmenter(250));

            var stream = new StandardAnalyzer(Version.LUCENE_29).TokenStream("bodyText", new StringReader(text));
            return highlighter.GetBestFragments(stream, text, 3, "...");
        }

        public void Delete(string url)
        {
            using (var writer = new IndexWriter(_directory, new StandardAnalyzer(Version.LUCENE_29), true))
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

        public IEnumerable<SearchResultModel> Search(string searchTerm)
        {
            var results = new List<SearchResultModel>();
            var indexSearcher = new IndexSearcher(_directory);

            try
            {
                var queryParser = new QueryParser("bodyText", new StandardAnalyzer());

                var query = queryParser.Parse(searchTerm);
                var hits = indexSearcher.Search(query);
                var numHits = hits.Length();

                for (var i = 0; i < numHits; ++i)
                {
                    var doc = hits.Doc(i);
                    var previewText = GeneratePreviewText(query, doc.Get("bodyText"));

                    results.Add(new SearchResultModel
                    {
                        Url = doc.Get("Url"),
                        PreviewText = previewText
                       
                    });
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
