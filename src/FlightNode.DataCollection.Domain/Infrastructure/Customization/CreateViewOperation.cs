using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightNode.DataCollection.Infrastructure.Customization
{
    public class CreateViewOperation : MigrationOperation
    {
        public CreateViewOperation(string viewName, string viewQueryString)
            : base(null)
        {
            ViewName = viewName;
            ViewString = viewQueryString;
        }
        
        public string ViewName { get; private set; }
        
        public string ViewString { get; private set; }

        public override bool IsDestructiveChange
        {
            get { return false; }
        }
    }
}
