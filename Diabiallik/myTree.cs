using System;
using System.Collections.Generic;
using System.Text;

namespace Diabiallik
{
    class myTree
    {
        public List<dynamic>
            fullRoundMove = new List<dynamic>(360), // All moves * possible passing ball moves
            treeRoude = new List<dynamic>(10); // tree for saving best way... i hope 
        public int treeDeep = 0;
        public myTree deeperMove;
        public myTree()
        {
        }
    }
}
