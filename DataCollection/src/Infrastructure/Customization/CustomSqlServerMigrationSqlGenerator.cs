using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Utilities;
using System.Data.Entity.SqlServer;

namespace FlightNode.DataCollection.Infrastructure.Customization
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class CustomSqlServerMigrationSqlGenerator : SqlServerMigrationSqlGenerator
    {
        protected override void Generate(MigrationOperation migrationOperation)
        {
            var createViewOperation = migrationOperation as CreateViewOperation;
            if (createViewOperation != null)
            {
                using (IndentedTextWriter writer = Writer())
                {
                    writer.WriteLine("CREATE VIEW {0} AS {1} ; ",
                                      createViewOperation.ViewName,
                                      createViewOperation.ViewString);
                    Statement(writer);
                }

                return;
            }

            var dropViewOperation = migrationOperation as DropViewOperation;
            if (dropViewOperation != null)
            {
                using (IndentedTextWriter writer = Writer())
                {
                    writer.WriteLine("DROP VIEW {0};", dropViewOperation.ViewName);
                    Statement(writer);
                }
            }
        }
    }
}
