using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightNode.DataCollection.Infrastructure.Customization
{
    public class DropViewOperation : MigrationOperation
    {
        public DropViewOperation(string viewName)
            : base(null)
        {
            ViewName = viewName;
        }

        public string ViewName { get; private set; }


        public override bool IsDestructiveChange
        {
            get { return false; }
        }
    }
}
