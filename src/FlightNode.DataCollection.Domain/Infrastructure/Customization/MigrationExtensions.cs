using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightNode.DataCollection.Infrastructure.Customization
{
    public static class MigrationExtensions
    {
        public static void CreateView(this DbMigration migration, string viewName, string viewQueryString)
        {

            ((IDbMigration)migration)
              .AddOperation(new CreateViewOperation(viewName,
                 viewQueryString));
        }

        public static void DropView(this DbMigration migration, string viewName)
        {
            ((IDbMigration) migration).AddOperation(new DropViewOperation(viewName));
        }
    }
}
