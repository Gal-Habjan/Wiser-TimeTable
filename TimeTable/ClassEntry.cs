using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTable
{
    public class FirebaseResponse
    {
        public string Key { get; set; }
        public object Object { get; set; }
    }
    public class RootObject
    {
        public string Hash { get; set; }
        public List<ClassEntry> Classes { get; set; }
    }
    public class ClassEntry
    {
        public string Dan { get; set; }
        public string Datum { get; set; }
        public string Ura { get; set; }
        public string Prostor { get; set; }
        public string Opis { get; set; }
        public string Skupina { get; set; }
        public string Izvajalec { get; set; }
        public bool isFirst { get; set; }
        public bool hasOverlap { get; set; }
    }
}
