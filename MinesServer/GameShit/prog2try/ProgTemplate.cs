using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.prog2try
{
    public class ProgTemplate
    {
        public int id;
        public string name;
        public string data;
        public Dictionary<string,IFunction> programm
        {
            get
            {
                _programm ??= Parse();
                return _programm;
            }
        }
        private Dictionary<string, IFunction> Parse()
        {
            
            return null;
        }
        private Dictionary<string, IFunction> _programm;
    }
}
