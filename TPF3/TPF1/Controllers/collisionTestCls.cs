using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace TPF1
{
    class collisionTestCls
    {
        public collisionTestCls()
        {}

        public bool testCollisionBullet(BoundingSphere obj1, BoundingSphere obj2)
        {
            if (obj1.Intersects(obj2) || obj2.Intersects(obj1))
                return true;
            else
                return false;
        }
        
    }
}
