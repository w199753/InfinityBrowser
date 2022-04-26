using System;

namespace InfinityEngine
{
    public class Disposal : IDisposable
    {
        private bool IsDisposed = false;

        public Disposal()
        {
            
        }

        ~Disposal()
        {
            Finalizer();
        }

        protected virtual void Release() { }

        private void Finalizer()
        {
            if (!IsDisposed)
            {
                Release();
            }

            IsDisposed = true;
        }

        public void Dispose()
        {
            Finalizer();
            GC.SuppressFinalize(this);
        }
    }
}
