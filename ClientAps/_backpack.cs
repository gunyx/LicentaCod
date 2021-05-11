using System;
using System.Collections.Generic;
using System.Text;

namespace ClientAps
{
    class _backpack
    {
        
        private Dictionary<_package, bool> pachete;

        public Dictionary<_package, bool> get_list() => this.pachete;

        public void add_package(_package new_package)
        {
            pachete.Add(new_package, false);
        }

        public int get_nr_elements() => this.pachete.Count;

        public int get_packege(int position) => 0;
    }
}
