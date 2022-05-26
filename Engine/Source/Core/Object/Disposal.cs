using System;

namespace Infinity
{
    public class Disposal : IDisposable
    {
        public bool IsDisposed => m_IsDisposed;

        private bool m_IsDisposed = false;

        ~Disposal()
        {
            Finalizer();
        }

        protected virtual void Release() { }

        private void Finalizer()
        {
            if (!m_IsDisposed)
            {
                Release();
            }

            m_IsDisposed = true;
        }

        public void Dispose()
        {
            Finalizer();
            GC.SuppressFinalize(this);
        }
    }
}
