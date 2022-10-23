using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpStart
{
    public class BulkEntitiesModel
    {
        public List<Entity> CreateList { get; set; }
        public List<Entity> UpdateList { get; set; }
        public List<Entity> DeleteList { get; set; }

        public BulkEntitiesModel()
        {
            this.CreateList = new List<Entity>();
            this.UpdateList = new List<Entity>();
            this.DeleteList = new List<Entity>();
        }
    }
}
