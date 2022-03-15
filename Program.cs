using Neo4j.Driver;
using System;
using System.Linq;

namespace Neo4jExportForm
{
    class Program
    {
        public class Neo4jExportExample : IDisposable
        {
            private bool _disposed = false;
            private readonly IDriver _driver;

            ~Neo4jExportExample() => Dispose(false);

            public Neo4jExportExample(string uri, string user, string password)
            {
                _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
            }

            public void ExportAsCSV()
            {
                using (var session = _driver.Session())
                {
                    var greeting = session.WriteTransaction(tx =>
                    {
                        var result = tx.Run("CALL apoc.export.csv.query" +
                            "('MATCH(u: User) -[r]->(m)" +
                            "RETURN u.Name as user, m.Name as employees LIMIT 10'," +
                            " 'results13.csv', {})");
                        return result;
                    });
                }
            }

            public void ExportAsJSON()
            {
                using (var session = _driver.Session())
                {
                    var greeting = session.WriteTransaction(tx =>
                    {
                        // CALL apoc.export.json.all('all.json',{useTypes:true})
                        var result = tx.Run("CALL apoc.export.json.all('CompleteJson.json',{useTypes:true})");
                        return result;
                    });
                }
            }

            public void ExportAsJSONWithQuery()
            {
                using (var session = _driver.Session())
                {
                    var results = session.WriteTransaction(tx =>
                    {
                        // CALL apoc.export.json.all('all.json',{useTypes:true})
                        var result = tx.Run("MATCH(u: User) -[r]->(m: User)" +
                            "WITH collect(u) as a, collect(r) as b CALL apoc.export.json.data(a, b, 'New.json', null)" +
                            "YIELD data RETURN data");
                        return result;
                    });
                    Console.WriteLine(results);
                }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (_disposed)
                    return;

                if (disposing)
                {
                    _driver?.Dispose();
                }

                _disposed = true;
            }

            public static void Main()
            {
                using (var neo4jexport = new Neo4jExportExample("bolt://localhost:7687", "neo4j", "9652jano."))
                {
                    neo4jexport.ExportAsCSV();
                    neo4jexport.ExportAsJSON();
                    neo4jexport.ExportAsJSONWithQuery();
                }
            }
        }
    }
}
