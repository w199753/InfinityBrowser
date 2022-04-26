using System;

namespace InfinityEngine
{
    [Serializable]
    public class Object : Disposal
    {
        public string name;

        public Object()
        {
            name = null;
        }

        public Object(string name)
        {
            this.name = name;
        }

        protected override void Release() 
        {
            base.Release();
        }
    }
}
