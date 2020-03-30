using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskMate.Models
{
    public class TagModel
    {
        public int ID { get; set; }
        public String Name { get; set; }

        public TagModel(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}
