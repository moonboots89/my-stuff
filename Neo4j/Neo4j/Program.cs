using System;
using System.Linq;
using Neo4jClient;
using Neo4jClient.Transactions;

namespace Neo4j
{
    class Program
    {
        static void Main(string[] args)
        {
            //var client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "root");
            //client.Connect();

            var client = (ITransactionalGraphClient)new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "root");
            client.Connect();
            using (var tx = client.BeginTransaction())
            {
                client.Cypher.Merge("(tx:Tx {Value:'Test'})").OnCreate().Set("tx = {tx}").WithParam("tx", new Tx { Value = "Test" }).ExecuteWithoutResults();
                client.Cypher.Merge("(tx:Tx {Value:'Test7'})").OnCreate().Set("tx = {tx}").WithParam("tx", new Tx { Value = "Test7" }).ExecuteWithoutResults();
                var languages = Enumerable.Range(0, 4).Select(x => new Language { Id = x });
                foreach (Language language in languages)
                {
                    client.Cypher.Merge("(language:Language { Id: {id} })").OnCreate().Set("language.Id = {id}").WithParam("id", language.Id).ExecuteWithoutResults();
                    client.Cypher
                        .Match("(tx1:Tx)", "(language1:Language)")
                        .Where((Tx tx1) => tx1.Value == "Test")
                        .AndWhere((Language language1) => language1.Id == language.Id)
                        .Merge("(tx1)-[:HAS_LANGUAGE]->(language1)")
                        .ExecuteWithoutResults();
                }

                //transactionResults = client.Cypher.Match("(n)").Return().Results);
                tx.Commit();
            }

            //var newUser = new User { Id = 600, Name = "newName", Age = 28};
            //client.Cypher
            //    .Merge("(user:User { Id: {id} })")
            //    //.OnCreate()
            //    .Set("user.Name = {name}")
            //    //.Set("user = {myUser}")
            //    .WithParams(new
            //    {
            //        id = newUser.Id,
            //        name = newUser.Name,
            //        myUser = newUser
            //    })
            //    .ExecuteWithoutResults();

            //var results = client.Cypher
            //    .Match("(user:User)")
            //    .Return(user => user.As<User>())
            //    .Results;
        }
    }

    public class Language
    {
        public int Id { get; set; }
    }

    public class Tx
    {
        public string Value { get; set; }
    }

    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
    }
}
